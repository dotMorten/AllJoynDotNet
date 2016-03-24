using System;
using System.Diagnostics;
using System.Runtime.InteropServices;

namespace AllJoynDotNet
{
    public abstract partial class AboutListener : AllJoynWrapper
    {
        private alljoyn_aboutlistener_callback handler;
        private AboutListener(alljoyn_aboutlistener_callback callback) : base(Create(callback))
        {
            handler = callback;
        }
        public AboutListener() : this(new alljoyn_aboutlistener_callback() { about_listener_announced = alljoyn_about_announced_delegate2 })
        {
            handler.about_listener_announced = alljoyn_about_announced_delegate;
        }
        private static IntPtr Create(alljoyn_aboutlistener_callback callback)
        {
            return alljoyn_aboutlistener_create(callback, IntPtr.Zero);
        }
        private static void alljoyn_about_announced_delegate2(IntPtr context, string busName, UInt16 version, UInt16 port, IntPtr objectDescriptionArg, IntPtr aboutDataArg)
        {
            Debug.WriteLine("alljoyn_about_announced_delegate2");
        }

        private void alljoyn_about_announced_delegate(IntPtr context, string busName, UInt16 version, UInt16 port, IntPtr objectDescriptionArg, IntPtr aboutDataArg)
        {
            OnCallback(busName, version);
        }

        protected abstract void OnCallback(string busName, UInt16 version);
    }
}