/* #if WINDOWS_UWP
using System;
using Windows.Devices.AllJoyn;

namespace AllJoynDotNet
{
    public partial class BusAttachment
    {
        private readonly AllJoynBusAttachment _bus;
        // https://allseenalliance.org/docs/api/c/_bus_attachment_8h.html
        public BusAttachment(AllJoynBusAttachment bus) : this(GetPointer(bus))
        {
            _bus = bus;
        }

        public BusAttachment() : this(new AllJoynBusAttachment()) { }

        public AllJoynBusAttachment AllJoynBusAttachment
        {
            get
            {
                return _bus;
            }
        }

        private static IntPtr GetPointer(Windows.Devices.AllJoyn.AllJoynBusAttachment bus)
        {
            return System.Runtime.InteropServices.Marshal.GetIUnknownForObject(bus);
        }        
    }
}
#endif */