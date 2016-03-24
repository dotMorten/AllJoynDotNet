using System;
using System.Runtime.InteropServices;

namespace AllJoynDotNet
{
    public partial class BusObject
    {
        internal static BusObject Create(IntPtr handle)
        {
            if (handle == IntPtr.Zero) return null;
            return new BusObject(handle);
        }

        protected override void Dispose(bool disposing)
        {
            base.Dispose(disposing);
            alljoyn_busobject_destroy(Handle);
        }

        public void AddInterface(InterfaceDescription iface)
        {
            int result = alljoyn_busobject_addinterface(Handle, iface.Handle);
            if (result != 0)
                throw new AllJoynException(result);
            
        }
        public void AddInterfaceAnnounced(InterfaceDescription iface)
        {
            int result = alljoyn_busobject_addinterface_announced(Handle, iface.Handle);
            if (result != 0)
                throw new AllJoynException(result);
        }
        public void AddMethodHandler(InterfaceDescription iface,
            string name, string signature, string returnSignature,
            string[] argNames, MessageReceiverDelegate handler)
        {
            
            var member = new alljoyn_interfacedescription_member()
            {
                iface = iface.Handle,
                memberType = alljoyn_messagetype.ALLJOYN_MESSAGE_METHOD_CALL, //TODO
                name = name,
                signature = signature,
                returnSignature = returnSignature,
                argNames = string.Join(",", argNames)
            };
            alljoyn_messagereceiver_methodhandler_ptr h = (a, b, c) =>
            {
                var bus = BusAttachment.GetBusAttachment(a);
                //TODO: Recreate bus instance if null
                //if(bus == null)
                //    new BusAttachment()
                handler(bus, b, c);
            };
            //TODO: Pin 'h'
            alljoyn_busobject_addmethodhandler(Handle, member, h, IntPtr.Zero);
        }

        public delegate void MessageReceiverDelegate(BusAttachment bus, object member, object message);


        //alljoyn_busobject_addmethodhandlers
        //alljoyn_busobject_cancelsessionlessmessage
        //alljoyn_busobject_cancelsessionlessmessage_serial
        //alljoyn_busobject_emitpropertieschanged
        //alljoyn_busobject_emitpropertychanged
        //alljoyn_busobject_getannouncedinterfacenames
        //alljoyn_busobject_getbusattachment
        //alljoyn_busobject_getname
        //alljoyn_busobject_getpath
        //alljoyn_busobject_issecure
        //alljoyn_busobject_methodreply_args
        //alljoyn_busobject_methodreply_err
        //alljoyn_busobject_methodreply_status
        //alljoyn_busobject_setannounceflag
        //alljoyn_busobject_signal

    }
}