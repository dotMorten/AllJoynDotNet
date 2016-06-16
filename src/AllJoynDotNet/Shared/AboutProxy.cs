using System;
using System.Runtime.InteropServices;

namespace AllJoynDotNet
{

    public partial class AboutProxy     
	{
        public AboutProxy(BusAttachment bus, string name, UInt32 sessionId) : base(alljoyn_aboutproxy_create(bus.Handle, name, sessionId))
        {
        }

        protected override void Dispose(bool disposing)
        {
            alljoyn_aboutproxy_destroy(Handle);
            base.Dispose(disposing);
        }

        public AboutObjectDescription ObjectDescription
        {
            get
            {
                MsgArg arg = new MsgArg();
                var status = alljoyn_aboutproxy_getobjectdescription(Handle, arg.Handle);
                if (status != QStatus.ER_OK) throw new AllJoynException(status);
                return new AboutObjectDescription(arg);
            }
        }

        public AboutData GetAboutData(string language)
        {
            MsgArg arg = new MsgArg();
            var status = alljoyn_aboutproxy_getaboutdata(Handle, language, arg.Handle);
            if (status != QStatus.ER_OK) throw new AllJoynException(status);
            return new AboutData(arg, "en");
        }
    }
}