using System;
using System.Runtime.InteropServices;

namespace AllJoynDotNet
{
    public partial class SessionListener : AllJoynWrapper
    {
        private alljoyn_sessionlistener_callbacks callbacks;

        public SessionListener() : base(IntPtr.Zero)
        {
            callbacks = new alljoyn_sessionlistener_callbacks()
            {
                session_lost = this.alljoyn_sessionlistener_sessionlost,
                session_member_added = this.alljoyn_sessionlistener_sessionmemberadded,
                session_member_removed = this.alljoyn_sessionlistener_sessionmemberremoved
            };
            var handle = alljoyn_sessionlistener_create(callbacks, IntPtr.Zero);
            base.SetHandle(handle);
        }

        void alljoyn_sessionlistener_sessionlost(IntPtr context, IntPtr sessionId, alljoyn_sessionlostreason reason)
        {
            SessionLost?.Invoke(this, EventArgs.Empty);
        }
        void alljoyn_sessionlistener_sessionmemberadded(IntPtr context, IntPtr sessionId, string uniqueName)
        {
            SessionMemberAdded?.Invoke(this, EventArgs.Empty);
        }
        void alljoyn_sessionlistener_sessionmemberremoved(IntPtr context, IntPtr sessionId, string uniqueName)
        {
            SessionMemberRemoved?.Invoke(this, EventArgs.Empty);
        }

        protected override void Dispose(bool disposing)
        {
            base.Dispose(disposing);
            alljoyn_sessionlistener_destroy(Handle);
        }

        public event EventHandler SessionLost;
        public event EventHandler SessionMemberAdded;
        public event EventHandler SessionMemberRemoved;
    }
}