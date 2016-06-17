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
        public BusObject(string path, bool isPlaceHolder) : this(alljoyn_busobject_create(path, isPlaceHolder.ToQccBool(), null, IntPtr.Zero))
        {
        }

        protected override void Dispose(bool disposing)
        {
            base.Dispose(disposing);
            pinnedMethodHandlers = null;
            alljoyn_busobject_destroy(Handle);
        }

        public void AddInterface(InterfaceDescription iface)
        {
            var result = alljoyn_busobject_addinterface(Handle, iface.Handle);
            if (result != 0)
                throw new AllJoynException(result);
            
        }

        public void AddInterfaceAnnounced(InterfaceDescription iface)
        {
            var result = alljoyn_busobject_addinterface_announced(Handle, iface.Handle);
            if (result != 0)
                throw new AllJoynException(result);
        }

        public void SetAnnounceFlag(InterfaceDescription iface, bool announced)
        {
            alljoyn_busobject_setannounceflag(Handle, iface.Handle, announced ? alljoyn_about_announceflag.ANNOUNCED : alljoyn_about_announceflag.UNANNOUNCED);
        }

        private System.Collections.Generic.List<alljoyn_messagereceiver_methodhandler_ptr> pinnedMethodHandlers = new System.Collections.Generic.List<alljoyn_messagereceiver_methodhandler_ptr>();

        public void AddMethodHandler(InterfaceDescription.InterfaceDescriptionMember member, MessageReceiverDelegate handler)
        {
            alljoyn_messagereceiver_methodhandler_ptr h = (bus, member2, message) =>
            {
                handler(
                    IsDisposed ? new BusObject(bus) : this, 
                    new InterfaceDescription.InterfaceDescriptionMember((alljoyn_interfacedescription_member)Marshal.PtrToStructure(member2, typeof(alljoyn_interfacedescription_member))),
                    new Message(message));
            };
            pinnedMethodHandlers.Add(h);

            GCHandle membGch = GCHandle.Alloc(member.Handle, GCHandleType.Pinned);
            MethodEntry entry;
            entry.member = membGch.AddrOfPinnedObject();
            entry.method_handler = Marshal.GetFunctionPointerForDelegate(h);
            GCHandle gch = GCHandle.Alloc(entry, GCHandleType.Pinned);
            
            AllJoynException.CheckStatus(alljoyn_busobject_addmethodhandlers(Handle, gch.AddrOfPinnedObject(), (UIntPtr)1));

            gch.Free();
            membGch.Free();
        }

        [StructLayout(LayoutKind.Sequential)]
        private struct MethodEntry
        {
            public IntPtr member;
            public IntPtr method_handler;
        }
        public void MethodReplyArgs(Message message, MsgArg msgArg, int numArgs)
        {
            alljoyn_busobject_methodreply_args(Handle, message.Handle, msgArg.Handle, (UIntPtr)numArgs);
        }

        public delegate void MessageReceiverDelegate(BusObject bus, InterfaceDescription.InterfaceDescriptionMember member, Message message);
        
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
        //alljoyn_busobject_methodreply_err
        //alljoyn_busobject_methodreply_status
        //alljoyn_busobject_signal

    }
}