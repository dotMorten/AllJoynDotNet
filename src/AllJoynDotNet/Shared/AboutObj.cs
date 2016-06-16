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

        public AboutObj(BusAttachment bus, bool announce) : 
            base(alljoyn_aboutobj_create(bus.Handle, announce ? alljoyn_about_announceflag.ANNOUNCED: alljoyn_about_announceflag.UNANNOUNCED))
        { }

// #if WINDOWS_UWP
//         public AboutObj(Windows.Devices.AllJoyn.AllJoynBusAttachment bus, bool announce) :
//             base(alljoyn_aboutobj_create(BusAttachment.GetHandle(bus), announce ? alljoyn_about_announceflag.ANNOUNCED : alljoyn_about_announceflag.UNANNOUNCED))
//         { }
// #endif
        public void Announce(ushort sessionPort, AboutData aboutData)
        {
            alljoyn_aboutobj_announce(Handle, sessionPort, aboutData.Handle);
        }

        public void Unannounce()
        {
            alljoyn_aboutobj_unannounce(Handle);
        }
    }
}