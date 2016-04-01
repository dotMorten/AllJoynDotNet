using System;
using System.Runtime.InteropServices;

namespace AllJoynDotNet
{
    public partial class AboutObj : AllJoynWrapper
    {
        protected override void Dispose(bool disposing)
        {
            alljoyn_aboutobj_destroy(Handle);
            base.Dispose(disposing);
        }
    }
}