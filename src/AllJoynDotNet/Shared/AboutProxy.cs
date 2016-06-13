using System;
using System.Runtime.InteropServices;

namespace AllJoynDotNet
{

    public partial class AboutProxy     
	{
        public AboutProxy(BusAttachment bus, string name, Session session) : base(alljoyn_aboutproxy_create(bus.Handle, name, session.Handle))
        {
        }

        protected override void Dispose(bool disposing)
        {
            alljoyn_aboutproxy_destroy(Handle);
            base.Dispose(disposing);
        }

        public AboutObjectDescription GetObjectDescription()
        {
            MsgArg arg = new MsgArg();
            var status = alljoyn_aboutproxy_getobjectdescription(Handle, arg.Handle);
            if (status != QStatus.ER_OK) throw new AllJoynException(status);
            return new AboutObjectDescription(arg);
        }

        public AboutData GetAboutData()
        {
            MsgArg arg = new MsgArg();
            var status = alljoyn_aboutproxy_getaboutdata(Handle, "en", arg.Handle);
            if (status != QStatus.ER_OK) throw new AllJoynException(status);
            return new AboutData(arg, "en");
        }
    }
}