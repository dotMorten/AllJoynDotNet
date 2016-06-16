using System;
using System.Runtime.InteropServices;

namespace AllJoynDotNet
{
    public partial class AboutData : AllJoynWrapper
    {
        internal AboutData(MsgArg arg, string defaultLanguage) : base(AboutData.alljoyn_aboutdata_create(defaultLanguage))
        {
            var status = AboutData.alljoyn_aboutdata_createfrommsgarg(Handle, arg.Handle, null);
            AllJoynException.CheckStatus(status);
        }

        protected override void Dispose(bool disposing)
        {
            alljoyn_aboutdata_destroy(Handle);
            base.Dispose(disposing);
        }

        public string[] Fields
        {
            get
            {
                return AllJoynNative.GetStringArrayHelper(alljoyn_aboutdata_getfields, Handle);
            }
        }

        public MsgArg GetField(string fieldName, string language = null)
        {
            IntPtr outvalue;
            var status = alljoyn_aboutdata_getfield(Handle, fieldName, out outvalue, language);
            AllJoynException.CheckStatus(status);
            var tmp = new MsgArg(outvalue);
            return tmp;
        }

        public string[] SupportedLanguages
        {
            get
            {
                return AllJoynNative.GetStringArrayHelper(alljoyn_aboutdata_getsupportedlanguages, Handle);
            }
        }

        public string DefaultLanguage
        {
            get
            {
                IntPtr lang;
                var status = alljoyn_aboutdata_getdefaultlanguage(Handle, out lang);
                AllJoynException.CheckStatus(status);
                return Marshal.PtrToStringAnsi(lang);
            }
        }
    }
}