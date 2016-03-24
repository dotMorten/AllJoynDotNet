using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PInvokeCodeGenerator
{
    public static class DoxygenAnalyzer
    {
        public class Method
        {
            public string RawDoc { get; set; }
            public string ExportDefinition { get; set; }
            public DocInfo Doc { get; set; }
        }
        public class AnalyzerResult
        {
            public string FileDescription { get; set; }
            public List<Method> Methods { get; } = new List<Method>();
            public List<Method> TypeDefinitions { get; } = new List<Method>();
        }
        public static AnalyzerResult Analyze(string filename)
        {
            var result = new AnalyzerResult();
            var lines = File.ReadAllLines(filename);
            bool isInsideDoc = false;
            bool isInsideCode = false;
            bool isInsideTypeDef = false;
            int scopeCount = 0;
            StringBuilder doc = null;
            StringBuilder method = new StringBuilder();
            StringBuilder typeDef = new StringBuilder();
            DocInfo docInfo = null;
            for (int i = 0; i < lines.Length; i++)
            {
                var line = lines[i].Trim();
                //if (line.Contains("ALLJOYN_MESSAGE_INVALID     = 0"))
                //    System.Diagnostics.Debugger.Break();
                if (line.StartsWith("/**") && !line.EndsWith("*/") && scopeCount == 0)
                {
                    isInsideDoc = true;
                    doc = new StringBuilder();
                }
                else if (isInsideDoc)
                {
                    if (line.EndsWith("*/"))
                    {
                        isInsideDoc = false;
                        docInfo = AnalyzeDoc(doc.ToString());
                        if (docInfo is FileInfo)
                            result.FileDescription = (docInfo as FileInfo).Description;
                    }
                    else
                        doc.AppendLine(line);
                }
                else if (line.StartsWith("extern ") && !line.StartsWith("extern \"C\""))
                {
                    //Code
                    method = new StringBuilder(line);
                    isInsideCode = true;
                }
                else if (line.StartsWith("typedef"))
                {
                    typeDef = new StringBuilder(line + "\n");
                    isInsideTypeDef = true;
                }
                else if (isInsideCode)
                    method.Append(line);
                else if(isInsideTypeDef)
                {
                    typeDef.AppendLine(line);
                }
                if (isInsideCode && line.EndsWith(";"))
                {
                    isInsideCode = false;
                    isInsideTypeDef = false;
                    result.Methods.Add(new Method() { RawDoc = doc?.ToString(), Doc = docInfo, ExportDefinition = method?.ToString() });
                    doc = null;
                    typeDef = null;
                    docInfo = null;
                    method = null;
                }
                else if (isInsideTypeDef)
                {
                    scopeCount += line.Where(c => c == '{').Count() - line.Where(c => c == '}').Count();
                    if(scopeCount > 0)
                    {

                    }
                    if (line.EndsWith(";") && scopeCount == 0)
                    {
                        isInsideCode = false;
                        isInsideTypeDef = false;
                        result.TypeDefinitions.Add(new Method() { RawDoc = doc?.ToString(), Doc = docInfo, ExportDefinition = typeDef?.ToString() });
                        doc = null;
                        typeDef = null;
                        docInfo = null;
                        method = null;
                    }
                }
                if(isInsideTypeDef)
                {
                }
            }
            return result;
        }

        private static DocInfo AnalyzeDoc(string v)
        {
            List<string> lines = new List<string>(v.Split(new char[] { '\n' }));
            List<string> parameters = new List<string>();
            //Trim off the leading *
            for (int i = 0; i < lines.Count; i++)
            {
                string line = lines[i].Trim();
                while (line.StartsWith("*"))
                    line = line.Substring(1).TrimStart();
                lines[i] = line;
            }
           //Move parameters over
            for (int i = 0; i < lines.Count; i++)
            {
                if (lines[i].StartsWith("@") && !lines[i].StartsWith("@brief"))
                {
                    parameters.AddRange(lines.Skip(i));
                    lines.RemoveRange(i, lines.Count - i);
                }
            }
            //Remove empty start and end lines
            while (lines.Count > 0 && lines[0] == "")
                lines.RemoveAt(0);
            while (lines.Count > 0 && lines[lines.Count - 1] == "")
                lines.RemoveAt(lines.Count - 1);
            if (lines.Count == 0)
                return null;

            //Is this a file descriptor?
            if (parameters.Count > 1 && parameters[0].StartsWith("@file"))
            {
                return new FileInfo() { Description = string.Join("\n\t// ", parameters.Skip(1).ToArray()) };
            }
            else {
                var info = new MethodInfo();
                StringBuilder parameterString = null;
                for (int i = 0; i < parameters.Count; i++)
                {
                    var line = parameters[i].Trim();
                    if (line == "") continue;
                    if (line.StartsWith("@")) //Start of new parameter
                    {
                        ProcessParameter(info, parameterString);
                        parameterString = new StringBuilder(line);
                    }
                    else if (parameterString != null)
                    {
                        parameterString.AppendLine(line);
                    }
                }
                ProcessParameter(info, parameterString);

                //Remove empty start and end lines left from removing parameters
                List<string> paragraphs = new List<string>();
                StringBuilder paragraph = new StringBuilder();
                foreach(var line in lines)
                {
                    if (line.Trim() == "")
                    {
                        if (paragraph.Length > 0)
                            paragraphs.Add(paragraph.ToString());
                        paragraph = new StringBuilder();
                        continue;
                    }
                    else
                        paragraph.AppendLine(line);
                }
                if (paragraph.Length > 0)
                    paragraphs.Add(paragraph.ToString());

                if (paragraphs.Count > 0)
                    info.CodeSummary = paragraphs[0];
                if (paragraphs.Count > 1)
                    info.Remarks.AddRange(paragraphs.Skip(1));
                return info;
            }
        }
        private static void ProcessParameter(MethodInfo info, StringBuilder parameterString)
        {
            if (parameterString == null || parameterString.Length == 0)
                return;
            string value = parameterString.ToString();

            if (value.StartsWith("@return"))
            {
                info.Returns = value.Substring(7).Trim();
            }
            else if (value.StartsWith("@param"))
            {
                var split = value.IndexOf(" ");
                var paramDefinition = value.Substring(0, split);
                var p = value.Substring(split).Trim();
                split = p.IndexOf(" ");
                string orgParamName = split > 0 ? p.Substring(0, split).Trim() : p;
                string paramName = orgParamName;
                string description = split > 0 ? p.Substring(split).Trim() : "";
                paramName = MethodSignatureAnalyzer.SafeParameterName(paramName);
                while (info.Parameters.ContainsKey(paramName))
                    paramName = paramName + "_"; //Sometimes args are duplicated. Rename for now
                bool isIn = paramDefinition.Contains("[in]") || paramDefinition.Contains("[in,out]") || !paramDefinition.Contains("[");
                bool isOut = paramDefinition.Contains("[out]") || paramDefinition.Contains("[in,out]");
                PropertyInfo pi = new PropertyInfo()
                {
                    Name = paramName,
                    OriginalName = orgParamName,
                    Description = description,
                    IsInput = isIn, IsOutput = isOut
                };
                info.Parameters.Add(paramName, pi);
            }
        }

        public abstract class DocInfo
        {

        }
        public class FileInfo : DocInfo
        {
            public string Description { get; set; }
        }
        public class MethodInfo : DocInfo
        {
            public string Returns { get; set; }
            public string CodeSummary { get; set; }
            public List<string> Remarks { get; } = new List<string>();

            public Dictionary<string, PropertyInfo> Parameters { get; } = new Dictionary<string, PropertyInfo>();
            
            public string XmlDoc
            {
                get
                {
                    StringBuilder sb = new StringBuilder();
                    sb.AppendLine("<summary>");
                    sb.Append(CodeSummary);
                    sb.AppendLine("</summary>");
                    if(Remarks.Any())
                    {
                        sb.AppendLine("<remarks>");
                        foreach (var item in Remarks)
                        {
                            sb.AppendLine($"<para>{item}</para>");
                        }
                        sb.AppendLine("</remarks>");
                    }
                    foreach (var item in Parameters)
                    {
                        sb.Append($"<param name=\"{item.Value.Name}\">{item.Value.Description}</param>");
                        if (item.Value.IsInput && item.Value.IsOutput)
                            sb.Append("<!-- in, out -->");
                        else if (item.Value.IsOutput)
                            sb.Append("<!-- out -->");

                        sb.AppendLine();
                    }
                    if (!string.IsNullOrWhiteSpace(Returns))
                        sb.Append($"<returns>{Returns}</returns>");
                    return sb.ToString();
                }
            }
        }
        public class PropertyInfo
        {
            public string Name { get; set; }
            public string OriginalName { get; set; }
            public bool IsInput { get; set; }
            public bool IsOutput { get; set; }
            public string Description { get; set; }
        }
    }
}
