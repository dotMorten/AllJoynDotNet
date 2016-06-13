using System;
using System.Runtime.InteropServices;

namespace AllJoynDotNet
{
    public partial class AboutData : AllJoynWrapper
    {
        internal AboutData(MsgArg arg, string defaultLanguage) : base(AboutData.alljoyn_aboutdata_create(defaultLanguage))
        {
            var status = AboutData.alljoyn_aboutdata_createfrommsgarg(Handle, arg.Handle, null);
            if (status != QStatus.ER_OK)
                throw new AllJoynException(status);
        }

        protected override void Dispose(bool disposing)
        {
            alljoyn_aboutdata_destroy(Handle);
            base.Dispose(disposing);
        }

        public string[] GetFields()
        {
            return AllJoynNative.GetStringArrayHelper(alljoyn_aboutdata_getfields, Handle);
        }
        public object GetField(string fieldName)
        {
            var tmp = new MsgArg();
            var status = alljoyn_aboutdata_getfield(Handle, fieldName, tmp.Handle, null);
            if (status > 0)
                throw new AllJoynException(status);
            return tmp;
        }
    }
}