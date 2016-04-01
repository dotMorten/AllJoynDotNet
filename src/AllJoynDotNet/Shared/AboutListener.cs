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
                about_listener_announced = alljoyn_about_announced_delegate
            };
            var handle = alljoyn_aboutlistener_create(callback, IntPtr.Zero);
            SetHandle(handle);
        }
        
        private void alljoyn_about_announced_delegate(IntPtr context, string busName, UInt16 version, UInt16 port, IntPtr objectDescriptionArg, IntPtr aboutDataArg)
        {
            AboutAnnounced?.Invoke(this, new AboutAnnouncedEventArgs(
                busName, version, port, objectDescriptionArg, aboutDataArg));
        }

        public event EventHandler<AboutAnnouncedEventArgs> AboutAnnounced;

        public sealed class AboutAnnouncedEventArgs : EventArgs
        {
            internal AboutAnnouncedEventArgs(string busName, UInt16 version, UInt16 port, IntPtr objectDescriptionArg, IntPtr aboutDataArg)
            {
                BusName = busName;
                Version = version;
                Port = port;
                IntPtr handle = AboutObjectDescription.alljoyn_aboutobjectdescription_create();
                var status = AboutObjectDescription.alljoyn_aboutobjectdescription_createfrommsgarg(handle, objectDescriptionArg);
                if (status != 0)
                    throw new AllJoynException(status);
                ObjectDescription = new AboutObjectDescription(handle);
                handle = AboutData.alljoyn_aboutdata_create_full(aboutDataArg, "en");
                AboutData = new AboutData(handle);
            }

            public string BusName { get; }
            public UInt16 Version { get; }
            public UInt16 Port { get; }
            public AboutObjectDescription ObjectDescription { get; }
            public AboutData AboutData { get; }
        }
    }
}