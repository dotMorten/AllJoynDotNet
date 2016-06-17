using System;
using System.Runtime.InteropServices;
using System.Threading.Tasks;

namespace AllJoynDotNet
{
    public partial class ProxyBusObject
    {
        public ProxyBusObject(BusAttachment bus, string busName, string path, Session sessionID) 
            : base(alljoyn_proxybusobject_create(bus.Handle, busName, path, sessionID.Handle))
        {
        }

        public void IntrospectRemoteObject()
        {
            AllJoynException.CheckStatus(alljoyn_proxybusobject_introspectremoteobject(Handle));
        }

        public Message MethodCall(BusAttachment bus, string ifaceName, string methodName, MsgArg args, 
            uint numArgs, TimeSpan timeout, MessageFlag flags = 0)
        {
                
            if (timeout.Ticks < 0)
                throw new ArgumentOutOfRangeException(nameof(timeout));
            var replyMsg = new Message(bus);
            AllJoynException.CheckStatus(
                alljoyn_proxybusobject_methodcall(Handle, ifaceName, methodName, args.Handle,
                     (UIntPtr)numArgs, replyMsg.Handle, (uint)timeout.TotalMilliseconds, (byte)flags)
            );
            return replyMsg;
        }

        public async Task<Message> MethodCallAsync(BusAttachment bus, string ifaceName, string methodName, MsgArg args, uint numArgs, 
           TimeSpan timeout, MessageFlag flags = 0)
        {
            if (timeout.Ticks < 0)
                throw new ArgumentOutOfRangeException(nameof(timeout));

            var replyMsg = Message.alljoyn_message_create(bus.Handle);
            
            TaskCompletionSource<Message> tcs = new TaskCompletionSource<Message>();
            alljoyn_messagereceiver_replyhandler_ptr replyFunc = (a, b) =>
            {
                tcs.SetResult(new Message(a));
            };
            var handle = GCHandle.Alloc(replyFunc);
            AllJoynException.CheckStatus(
                alljoyn_proxybusobject_methodcallasync(Handle, ifaceName, methodName, replyFunc, args.Handle,
                     (UIntPtr)numArgs, replyMsg, (uint)timeout.TotalMilliseconds, (byte)flags)
            );
            Message message;
            try
            {
                message = await tcs.Task;
            }
            finally
            {
                handle.Free();
            }
            return message;
        }
    }
}