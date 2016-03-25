using System;
using System.Collections.Generic;
using System.IO;
using System.Reflection;
using System.Text;

namespace AllJoynDotNet
{
    internal static class ErrorCodeLookup
    {
        private static object errorcodelock = new object();
        private static Dictionary<int, ErrorStatus> errorcodes;

        internal static ErrorStatus GetError(int code)
        {
            lock(errorcodelock)
            {
                if(errorcodes == null)
                {
                    LoadErrorCodes();
                }
            }
            if (errorcodes.ContainsKey(code))
                return errorcodes[code];
            return new ErrorStatus() { Value = code, Name = "ER_UNKNOWN", Comment = "Unknown Error" };
        }

        private static Stream GetResourceStream(string name)
        {
#if XAMARIN
            Assembly assembly = Assembly.GetExecutingAssembly();
            string[] resources = assembly.GetManifestResourceNames();
            foreach (string resource in resources)
            {
                if (resource.EndsWith(name))
                {
                    return assembly.GetManifestResourceStream(resource);
                }
            }
            return null;
#else
            return typeof(ErrorCodeLookup).GetTypeInfo().Assembly.GetManifestResourceStream("AllJoynDotNet." + name);
#endif

        }

        private static void LoadErrorCodes()
        {
            errorcodes = new Dictionary<int, ErrorStatus>();
            using (var stream = GetResourceStream("Status.xml"))
            {
#if DEBUG       //This shouldn't happen but lets leave it here for debugging
                if (stream == null)
                    throw new InvalidOperationException("Error codes not found");
#endif
                var reader = System.Xml.XmlReader.Create(stream);
                reader.MoveToContent();
                while (reader.ReadToFollowing("status"))
                {
                    var code = ReadCode(reader.ReadSubtree());
                    errorcodes[code.Value] = code;
                }
            }

        }
        private static ErrorStatus ReadCode(System.Xml.XmlReader reader)
        {
            ErrorStatus status = new ErrorStatus();
            reader.MoveToContent();
            while (reader.MoveToNextAttribute())
            {
                switch (reader.Name)
                {
                    case "name":
                        reader.ReadAttributeValue();
                        status.Name = reader.ReadContentAsString(); break;
                    case "value":
                        reader.ReadAttributeValue();
                        var val = reader.ReadContentAsString();
                        status.Value = Convert.ToInt32(val, 16); break;
                    case "comment":
                        reader.ReadAttributeValue();
                        status.Comment = reader.ReadContentAsString(); break;
                    default:
                        reader.ReadAttributeValue(); break;
                }
            }
            return status;
        }
    }

    internal class ErrorStatus
    {
        public int Value { get; set; }
        public string Name { get; set; }
        public string Comment { get; set; }
        public override string ToString()
        {
            return $"0x{Value:x4}: {Name} {Comment}";
        }
    }
}
