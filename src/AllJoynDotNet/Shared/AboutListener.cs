using System;
using System.Diagnostics;
using System.Runtime.InteropServices;

namespace AllJoynDotNet
{
    public partial class AboutListener : AllJoynWrapper
    {
        private alljoyn_aboutlistener_callback callback;
        private AboutListener(alljoyn_aboutlistener_callback callback) : base(IntPtr.Zero)
        {
            this.callback = callback;
        }
        public AboutListener() : base(IntPtr.Zero)
        {
            callback = new alljoyn_aboutlistener_callback()
            {
                about_listener_announced = Marshal.GetFunctionPointerForDelegate<alljoyn_about_announced_ptr>((alljoyn_about_announced_ptr)alljoyn_about_announced_delegate)
            };
            GCHandle gch = GCHandle.Alloc(callback, GCHandleType.Pinned);
            var handle = alljoyn_aboutlistener_create(gch.AddrOfPinnedObject(), IntPtr.Zero);
            SetHandle(handle);
            gch.Free();
        }
        
        private void alljoyn_about_announced_delegate(IntPtr context, string busName, UInt16 version, UInt16 port, IntPtr objectDescriptionArg, IntPtr aboutDataArg)
        {
            AboutAnnounced?.Invoke(this, EventArgs.Empty); // (busName, version);
        }

        public event EventHandler AboutAnnounced;
    }
}