using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PInvokeCodeGenerator
{
    class Program
    {
        static void Main(string[] args)
        {
            string repo = @"..\..\..\..\..\alljoyn\";
            string exportList = repo + @"alljoyn_c\src\Windows\alljoyn_c.def";
            string includeFolder = repo + @"alljoyn_c\inc\alljoyn_c\";
            string outputFolder = @"..\..\..\..\AllJoynDotNet\Generated\";
            //List<string> exportMembers = new List<string>();
            //using (var sr = File.OpenText(exportList))
            //{
            //    if (sr.ReadLine() != "EXPORTS" || sr.ReadLine() != "")
            //        throw new InvalidDataException();
            //    while (!sr.EndOfStream)
            //    {
            //        var line = sr.ReadLine();
            //        if (!string.IsNullOrWhiteSpace(line))
            //            exportMembers.Add(line);
            //    }
            //}
            List<string> generatedFiles = new List<string>();
            foreach (var file in new DirectoryInfo(includeFolder).GetFiles("*.h"))
            {
                //First pass we find all known type definitions before generating code
                var result = DoxygenAnalyzer.Analyze(file.FullName);
                foreach (var item in result.TypeDefinitions)
                {
                    if (item.Doc is DoxygenAnalyzer.MethodInfo)
                    {
                        var csharp = MethodSignatureAnalyzer.CreateTypeDefinition(item.ExportDefinition);

                    }
                }
                foreach (var item in result.Methods)
                {
                    var methodInfo = (item.Doc as DoxygenAnalyzer.MethodInfo);
                    var csharp = MethodSignatureAnalyzer.CreateImport(item.ExportDefinition, methodInfo);

                }
            }
            foreach (var file in new DirectoryInfo(includeFolder).GetFiles("*.h"))
            {
                string generatedFile = file.Name.Replace(".h", ".cs");
                generatedFiles.Add(generatedFile);
                if (File.Exists(outputFolder + generatedFile))
                    File.Delete(outputFolder + generatedFile);
                using (var sw = File.CreateText(outputFolder + generatedFile))
                {
                    //using (StreamWriter sw = new StreamWriter(outfile))
                    {
                        var result = DoxygenAnalyzer.Analyze(file.FullName);
                        sw.WriteLine("// Generated from " + file.Name);
                        sw.Write(HeaderTemplate);
                        foreach (var item in result.TypeDefinitions)
                        {

                            if (item.Doc is DoxygenAnalyzer.MethodInfo)
                            {
                                var csharp = MethodSignatureAnalyzer.CreateTypeDefinition(item.ExportDefinition);
                                if (csharp != null)
                                {
                                    var methodInfo = (item.Doc as DoxygenAnalyzer.MethodInfo);
                                    var doc = methodInfo.XmlDoc;
                                    if (doc != null)
                                        WriteLines(doc, "\t\t/// ", sw);
                                    WriteLines(csharp, "\t\t", sw);
                                    WriteLines(item.ExportDefinition, "\t\t// ", sw);
                                    sw.WriteLine();
                                }
                            }
                        }
                        sw.Write(ClassHeaderTemplate.Replace("{PartialClassDescription}", "//" + result.FileDescription).Replace("{ClassName}", file.Name.Replace(".h", "")));

                        foreach (var item in result.Methods)
                        {
                            var methodInfo = (item.Doc as DoxygenAnalyzer.MethodInfo);
                            if (methodInfo != null)
                            {
                                var doc = methodInfo.XmlDoc;
                                if (doc != null)
                                    WriteLines(doc, "\t\t/// ", sw);
                            }
                            //foreach(var line in item.Doc.Spl)
                            var csharp = MethodSignatureAnalyzer.CreateImport(item.ExportDefinition, methodInfo);
                            WriteLines(csharp, "\t\t", sw);
                            WriteLines(item.ExportDefinition, "\t\t// ", sw);
                            sw.WriteLine("");
                        }

                        sw.Write(FooterTemplate);
                    }
                }

            }
            //using(var sw = File.CreateText(outputFolder + "TypeDefinitions.cs"))
            //{
            //    sw.WriteLine(TypeDefinitionHeader);
            //    foreach(var item in MethodSignatureAnalyzer.AllJoynTypes.OrderBy(t=>t.Key))
            //    {
            //        sw.WriteLine($"\t\tpublic partial class {item.Key} {{ }}");
            //    }
            //    sw.Write("\t}\n}");
            //}
            //generatedFiles.Add("TypeDefinitions.cs");

            File.WriteAllText(outputFolder + "AllJoynWrapper.cs", AllJoynWrapperTemplate);
            generatedFiles.Add("AllJoynWrapper.cs");
            var projectGuid = "6feeac78-a2ec-4eb4-bd87-2863dbfec3de";
            SharedProjectGenerator.Generate(outputFolder, "AllJoynDotNet.GeneratedInterop", projectGuid, generatedFiles);
        }
        private static void WriteLines(string lines, string prefix, StreamWriter sw)
        {
            foreach (var line in lines.Split(new char[] { '\n' }))
            {
                sw.Write(prefix);
                sw.WriteLine(line.Replace("\r", ""));
            }
        }
        private const string TypeDefinitionHeader = @"// Partial type definitions for AllJoyn

//------------------------------------------------------------------------------
// <auto-generated>
//     This code was generated by a tool.
//
//     Changes to this file may cause incorrect behavior and will be lost if
//     the code is regenerated.
// </auto-generated>
//------------------------------------------------------------------------------

namespace AllJoynDotNet
{";

        private const string HeaderTemplate = @"
//------------------------------------------------------------------------------
// <auto-generated>
//     This code was generated by a tool.
//
//     Changes to this file may cause incorrect behavior and will be lost if
//     the code is regenerated.
// </auto-generated>
//------------------------------------------------------------------------------
using System;
using System.Runtime.InteropServices;

namespace AllJoynDotNet
{
";
        private const string ClassHeaderTemplate = @"{PartialClassDescription}
    public partial class {ClassName} : AllJoynWrapper
    {
        internal {ClassName}(IntPtr handle) : base(handle) { }
";

        private const string FooterTemplate = @"
    }
}";

        private const string AllJoynWrapperTemplate = @"
    using System;

namespace AllJoynDotNet
{
    public abstract class AllJoynWrapper : IDisposable
    {
        private IntPtr _handle;
        private bool isHandleSet;
        internal AllJoynWrapper(IntPtr handle) {
            _handle = handle;
            isHandleSet = IntPtr.Zero != handle;
        }

        //Should ONLY be called from a constructor:
        internal void SetHandle(IntPtr handle)
        {
            if (isHandleSet)
                throw new InvalidOperationException();
            _handle = handle;
            isHandleSet = IntPtr.Zero != handle;
        }
        internal IntPtr Handle { get { return _handle; } }

        static AllJoynWrapper()
        {
            Init.Initialize();
        }

        ~AllJoynWrapper()
        {
            Dispose(false);
        }

        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        protected virtual void Dispose(bool disposing)
		{ 
			IsDisposed = true; 
		} 
        protected bool IsDisposed { get; private set; }
    }
}";
    }
}
