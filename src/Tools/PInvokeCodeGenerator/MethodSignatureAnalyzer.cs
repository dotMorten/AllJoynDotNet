using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PInvokeCodeGenerator
{
    public static class MethodSignatureAnalyzer
    {
        public class TypeInfo
        {
            public string CsName { get; set; }
            public PropertyType Type { get; set; }
            public enum PropertyType
            {
                ValueType,
                StructType,
                ReferenceType,
                Pointer,
                Delegate,
                Enum
            }
        }
        public static void AddKnownType(string cppName, string csname, TypeInfo.PropertyType type)
        {
            if(AllJoynTypes.ContainsKey(cppName))
            {
                if (AllJoynTypes[cppName].Type != type)
                    throw new InvalidCastException();
            }
            else
                AllJoynTypes.Add(cppName, new TypeInfo() { CsName = csname, Type = type });
        }
        public static string CreateTypeDefinition(string cpp)
        {
            var signature = cpp;
            //if (signature.Contains("AJ_CALL * "))
            //    signature = signature.Replace("AJ_CALL * ", "");
            //signature = signature.Replace("const ", "").Replace(";","");
            //var idx1 = signature.IndexOf("(");
            //var idx2 = signature.LastIndexOf("(");
            string def = signature;
            //string param = "";
            //if (idx1 < idx2)
            //{
            //    param = signature.Substring(idx2);
            //    def = signature.Substring(0, idx2);
            //}
            //typedef struct _alljoyn_busattachment_handle* alljoyn_busattachment;
            //typedef void (AJ_CALL * alljoyn_busattachment_joinsessioncb_ptr)(QStatus status, alljoyn_sessionid sessionId, const alljoyn_sessionopts opts, void* context);
            //typedef void (AJ_CALL * alljoyn_busattachment_setlinktimeoutcb_ptr)(QStatus status, uint32_t timeout, void * context);
            var parts = def.Split(new char[] { ' ', ',', '\n' }, StringSplitOptions.RemoveEmptyEntries);
            string type = parts[1]; //void, struct
            if (type == "struct")
            {
                string structname = parts.Last().Replace(";", "").Trim();
                StringBuilder sb = new StringBuilder($"[StructLayout(LayoutKind.Sequential)]\ninternal partial class {structname}\n{{\n");
                if (def.Contains("}"))
                {
                    var bodyStart = def.IndexOf("typedef struct {") + 16;

                    var bodyEnd = def.LastIndexOf("}");
                    string body = def.Substring(bodyStart, bodyEnd - bodyStart);
                    GenerateStructureDefinition(sb, body);
                    foreach (var line in body.Split(new char[] { '\n' }))
                    {
                        sb.AppendLine("\t//" + line.Trim());
                    }
                }
                else
                {
                    AddKnownType(structname, "IntPtr", TypeInfo.PropertyType.Pointer);
                    return null;
                }
                sb.Append("}");
                AddKnownType(structname, structname, TypeInfo.PropertyType.StructType);
                return sb.ToString();
            }
            else if (type == "enum")
            {
                string structname = parts.Last().Replace(";", "").Trim();
                AddKnownType(structname, structname, TypeInfo.PropertyType.Enum);
                StringBuilder sb = new StringBuilder($"internal enum {structname}\n{{\n");
                string enumdef = def.Substring(def.IndexOf("{") + 1);
                enumdef = enumdef.Substring(0, enumdef.LastIndexOf("}"));
                sb.AppendLine(enumdef);
                sb.Append("}");
                return sb.ToString();
            }
            else if (parts[2] == "(AJ_CALL" && parts[3] == "*")
            {
                //Delegate
                string cstype = null;
                if (parts[1].StartsWith("void"))
                    cstype = "void";
                else
                    cstype = CppTypeToDotNetType(type, false, false, false);
                //AddKnownType(type)
                var name = parts[4];
                var endIdx = name.IndexOf("(");
                if (endIdx > 0)
                {
                    parts[4] = name.Substring(endIdx + 1);
                    name = name.Substring(0, endIdx - 1);
                }
                AddKnownType(name, name, TypeInfo.PropertyType.Delegate);
                StringBuilder sb = new StringBuilder($"[UnmanagedFunctionPointer(CallingConvention.Cdecl)]\ninternal delegate {cstype} {name}(");
                int pCount = 0;
                for(int i=4;i<parts.Length - 1;i++)
                {
                    var p = parts[i].Trim();
                    if (p == "const" || p == "") continue;
                    var n = parts[++i].Replace(");","").Trim();
                    var t = CppTypeToDotNetType(p, n.EndsWith("[]"), false, false);
                    if (pCount > 0)
                        sb.Append(", ");
                    sb.Append(t);
                    sb.Append(" ");
                    sb.Append(n);
                    pCount++;
                }
                sb.Append(");");
                return sb.ToString();
                    //typedef QStatus (AJ_CALL * alljoyn_aboutdatalistener_getannouncedaboutdata_ptr)(const void* context, alljoyn_msgarg msgArg);

                    //typedef void (AJ_CALL * alljoyn_about_announced_ptr)(const void* context,
                    //  const char* busName,
                    //  uint16_t version,
                    //  alljoyn_sessionport port,
                    //  const alljoyn_msgarg objectDescriptionArg,
                    //  const alljoyn_msgarg aboutDataArg);
                    // Converts to:
                    //internal delegate void alljoyn_about_announced_ptr(IntPtr context, string busName, UInt16 version, Int32 port, IntPtr objectDescriptionArg, IntPtr aboutDataArg);

            
            }
            else if (type == "void*")
            {
                string name = parts.Last().Replace(";", "").Trim();
                AddKnownType(name, "IntPtr", TypeInfo.PropertyType.Pointer);
                return null; //Will default to Pointer
                //public Delegate 
            }
            else if (parts.Length == 3) //Simple type definition
            {
                var cstype = CppTypeToDotNetType(parts[1], false, false, false);
                AddKnownType(parts[2].Replace(";","").Trim(), cstype, TypeInfo.PropertyType.ValueType);
                return null;
            }

            return "// TODO: " + signature.Replace("\n","\n//");

            //System.Diagnostics.Debugger.Break();
            //return $"public partial struct {name} {{ /* ${param} */ }}";
        }
        //typedef void (AJ_CALL* alljoyn_about_announced_ptr)(const void* context,
        //    const char* busName,
        //    uint16_t version,
        //    alljoyn_sessionport port,
        //    const alljoyn_msgarg objectDescriptionArg,
        //    const alljoyn_msgarg aboutDataArg);

        private static void GenerateStructureDefinition(StringBuilder sb, string body)
        {
            var stripped = System.Text.RegularExpressions.Regex.Replace(body, "/\\*([^*]|[\\r\n]|(\\*+([^*/]|[\\r\\n])))*\\*+/", "");
            foreach (var line in stripped.Split(new char[] { '\n' }))
            {
                var l = line.Replace("const", "").Trim();
                if (l == "") continue;
                if (l.EndsWith(";"))
                    l = l.Remove(l.Length - 1);
                var parts = l.Split(new char[] { ' ' });
                if (parts.Length != 2)
                    Debugger.Break();
                string name = parts[1];
                bool isArray = name.EndsWith("[]");
                if(isArray)
                {
                    name = name.Remove(name.Length - 2);
                }
                var type = CppTypeToDotNetType(parts[0], isArray, false, false);
                if (type.StartsWith("["))
                {
                    var idx = type.IndexOf("]") + 1;
                    type = type.Substring(0, idx) + " public " + type.Substring(idx);
                }
                else
                    type = "public " + type;
                sb.AppendLine($"\t{type} {name};");
               // sb.AppendLine("\t//" + line.Trim());
            }
        }

        public static string CreateImport(string signature, DoxygenAnalyzer.MethodInfo methodInfo)
        {
            var paramindex = signature.IndexOf("(");
            if (paramindex < 0)
                return "//TODO";
            var definition = signature.Substring(0, paramindex).Split(new char[] { }, StringSplitOptions.RemoveEmptyEntries).ToList();
            var parametersString = signature.Substring(paramindex);
            paramindex = parametersString.IndexOf(")");
            parametersString = parametersString.Substring(1, paramindex - 1);
            var parameters = parametersString.Split(new char[] { ',' });
            if (definition[1] != "AJ_API")
                return "// INVALID SIGNATURE";
            if (definition[2] == "const")
                definition.RemoveAt(2);
            string methodName = definition[4];
            string returnType = definition[2];
            //var split = signature.Split("")
            //extern AJ_API alljoyn_busattachment AJ_CALL alljoyn_busattachment_create(const char* applicationName, QCC_BOOL allowRemoteMessages);

            //public static extern IntPtr alljoyn_busattachment_create(
            //[MarshalAs(UnmanagedType.LPStr)]string applicationName,
            //Int32 allowRemoteMessages);

            StringBuilder sb = new StringBuilder("[DllImport(Constants.DLL_IMPORT_TARGET)]\n");
            returnType = CppTypeToDotNetType(returnType, false, false, false);
            if(returnType.StartsWith("["))
            {
                var idx = returnType.IndexOf("]");
                string attribute = returnType.Substring(1, idx - 1);
                returnType = returnType.Substring(idx + 1);
                //Force IntPtr on string return types:
                if (returnType == "string")
                    returnType = "IntPtr";
                else
                    sb.AppendFormat("[return: {0}]\n", attribute);
            }
            sb.Append("internal static extern ");
            sb.Append(returnType);
            sb.Append(" ");
            sb.Append(methodName);
            sb.Append("(");
            if (parameters.Length > 0 && parameters[0] != "void")
                for (int i = 0; i < parameters.Length; i++)
                {
                    var p = parameters[i].Trim();
                    if (p.Length == 0) continue;
                    
                    if (p == "...")
                    {
                        if (i > 0)
                            sb.Remove(sb.Length - 2, 2);
                        sb.Append("/*TODO: '...' type*/");
                    }
                    else
                    {
                        string defaultValue = null;
                        var idx = p.IndexOf(" = ");
                        if (idx > -1)
                        {
                            defaultValue = p.Substring(idx + 3);
                            p = p.Substring(0, idx);
                        }
                        if (p.StartsWith("const "))
                            p = p.Substring(6);
                        idx = p.LastIndexOf(" ");
                        string type = p.Substring(0, idx).Trim();
                        bool isArray = false;
                        string name = p.Substring(idx).Trim();
                        if(name.EndsWith("[]"))
                        {
                            isArray = true;
                            name = name.Substring(0, name.Length - 2);
                        }
                        var propertyDoc = methodInfo?.Parameters.Values.FirstOrDefault(m => m.OriginalName == name);
                        name = propertyDoc?.Name ?? SafeParameterName(name);
                        bool isInput = propertyDoc != null ? propertyDoc.IsInput : true;
                        bool isOutput = propertyDoc != null ? propertyDoc.IsOutput : false;
                        type = CppTypeToDotNetType(type, isArray, isInput, isOutput);

                        sb.Append(type);
                        sb.Append(" ");

                        sb.Append(name);
                        if (defaultValue != null)
                        {
                            if (defaultValue == "false" && type.StartsWith("Int"))
                                defaultValue = "0";
                            sb.Append(" = " + defaultValue);
                        }
                    }
                    if (i < parameters.Length - 1)
                        sb.Append(", ");
                }
            sb.Append(");");
            return sb.ToString();
        }
        public static string SafeParameterName(string name)
        {
            switch(name)
            {
                case "object":
                case "as":
                    return "@" + name;
                default:
                    return name;
            }
        }
        static Dictionary<string, object> UnknownTypes = new Dictionary<string, object>();
        private static string CppTypeToDotNetType(string type, bool isArray, bool isInput, bool isOutput)
        {
            string cstype = null;
            string marshalAs = null;
            if(isOutput)
            {
                if (type == "char**")
                {
                    //type = "char*";
                }
                else if (type.EndsWith("*")) //We should only do this on value types I think
                {
                    type = type.Substring(0, type.Length - 1);
                    //Debugger.Break();
                }
            }
            switch (type)
            {
                case "void":
                    return "void";
                case "char*":
                    cstype = "string";
                    marshalAs = "UnmanagedType.LPStr";
                    break;
                case "char**":
                    if (isOutput)
                    {
                        cstype = "IntPtr[]";
                        marshalAs = "UnmanagedType.LPArray";
                    }
                    else
                    {
                        cstype = "string[]";
                        marshalAs = "UnmanagedType.LPArray, ArraySubType = UnmanagedType.LPStr";
                    }
                    break;
                case "size_t":
                    cstype = "UIntPtr"; break;
                case "size_t*":
                    cstype = "UIntPtr[]"; break;
                case "bool":
                    cstype = "bool"; break;
                case "uint16_t":
                    cstype = "UInt16"; break;
                case "uint16_t*":
                    cstype = "UInt16[]"; break;
                case "uint32_t":
                    cstype = "UInt32"; break;
                case "uint32_t*":
                    cstype = "UInt32[]"; break;
                case "uint64_t":
                    cstype = "UInt64"; break;
                case "uint64_t*":
                    cstype = "UInt64[]"; break;
                case "uint8_t":
                    cstype = "byte"; break;
                case "uint8_t*":
                    cstype = "byte[]"; break;
                //case "uint8_t**":
                //    cstype = "byte[][]"; break;
                case "char":
                    cstype = "sbyte"; break;
                case "void*":
                    cstype = "IntPtr"; break;
                case "int16_t":
                    cstype = "Int16"; break;
                case "int16_t*":
                    cstype = "Int16[]"; break;
                case "int32_t":
                //case "QCC_BOOL":
                //case "QStatus":
                    cstype = "Int32"; break;
                case "int32_t*":
                case "QCC_BOOL*":
                    cstype = "Int32[]"; break;
                case "int64_t":
                    cstype = "Int64"; break;
                case "int64_t*":
                    cstype = "Int64[]"; break;
                case "double":
                    cstype = "double"; break;
                case "double*":
                    cstype = "double[]"; break;
                default:
                    break;
            }
            if(cstype == null)
            //if (type.StartsWith("alljoyn_"))
            {
                //if (type == "alljoyn_busattachment")
                //    Debugger.Break();
                string typename = type;
                //while(typename.EndsWith("*"))
                //    typename = typename.Substring(0, typename.Length - 1) + "[]";
                string baseTypeName = typename.Replace("[]", "").Replace("*","");
                if (AllJoynTypes.ContainsKey(baseTypeName))
                {
                    var t = AllJoynTypes[baseTypeName];
                    if(t.Type == TypeInfo.PropertyType.StructType)
                    {

                    }
                    cstype = t.CsName;
                    //Convert C++ * array to C# array
                    if (!(isInput))
                    {
                        var arrayLength = typename.Where(tn => tn == '*').Count();
                        if (t.Type == TypeInfo.PropertyType.Pointer || t.Type == TypeInfo.PropertyType.StructType)
                            arrayLength--;
                        for (int i = 0; i < arrayLength; i++)
                        {
                            cstype += "[]";
                        }
                    }
                }
            }
            if (cstype == null && !UnknownTypes.ContainsKey(type))
            {
                System.Diagnostics.Debug.WriteLine(type);
                UnknownTypes.Add(type, null);
            }
            if(cstype == null)
            {
                cstype = "IntPtr";
            }

            if (isArray)
            {
                cstype += "[]";
            }
            StringBuilder sb = new StringBuilder();
            if(isOutput && isInput || marshalAs != null)
            {
                sb.Append("[");
                if(marshalAs != null)
                    sb.Append($"MarshalAs({marshalAs})");
                if(isInput && isOutput)
                {
                    if (sb.Length > 1) sb.Append(", ");
                    sb.Append("In, Out");
                }
                if (type == "char**" && isOutput)
                {
                    isOutput = false;
                    if (sb.Length > 1) sb.Append(", ");
                    sb.Append("Out");
                }
                sb.Append("]");
                if (isOutput && !isInput)
                    sb.Append(" out ");
            }
            sb.Append(cstype);
            return sb.ToString();
        }

        public static Dictionary<string, TypeInfo> AllJoynTypes { get; } = new Dictionary<string, TypeInfo>();
    }
}
