using System;
using System.Runtime.InteropServices;

namespace AllJoynDotNet
{
    public partial class AboutData : AllJoynWrapper
    {
        protected override void Dispose(bool disposing)
        {
            alljoyn_aboutdata_destroy(Handle);
            base.Dispose(disposing);
        }

        public string[] GetFields()
        {
            return AllJoynNative.GetStringArrayHelper(alljoyn_aboutdata_getfields, Handle);
        }
    }
}