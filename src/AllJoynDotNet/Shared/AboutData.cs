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
    }
}