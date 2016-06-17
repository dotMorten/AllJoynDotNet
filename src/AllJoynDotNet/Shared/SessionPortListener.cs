using System;
using System.Runtime.InteropServices;

namespace AllJoynDotNet
{
    public partial class SessionPortListener : AllJoynWrapper
    {
        private alljoyn_sessionportlistener_callbacks callbacks;

        public SessionPortListener() : base(IntPtr.Zero)
        {
            callbacks = new alljoyn_sessionportlistener_callbacks()
            {
                accept_session_joiner = this.accept_session_joiner,
                session_joined = this.session_joined
            };
            var handle = alljoyn_sessionportlistener_create(callbacks, IntPtr.Zero);
            base.SetHandle(handle);
        }

        private int accept_session_joiner(IntPtr context, UInt16 sessionPort, string joiner, IntPtr opts)
        {
            var args = new AcceptSessionJoinerEventArgs(sessionPort, joiner, opts);
            AcceptSessionJoiner?.Invoke(this, args);
            return args.AcceptSession.ToQccBool();
        }

        private void session_joined(IntPtr context, UInt16 sessionPort, IntPtr id, string joiner)
        {
            SessionJoined?.Invoke(this, new SessionJoinedEventArgs(sessionPort, joiner, id));
        }

        protected override void Dispose(bool disposing)
        {
            base.Dispose(disposing);
            alljoyn_sessionportlistener_destroy(Handle);
        }

        public event EventHandler<AcceptSessionJoinerEventArgs> AcceptSessionJoiner;
        public event EventHandler<SessionJoinedEventArgs> SessionJoined;

        public class SessionJoinedEventArgs : EventArgs
        {
            private readonly IntPtr _sessionOptsPtr;
            private Session _sessionOpts;
            internal SessionJoinedEventArgs(UInt16 sessionPort, string joiner, IntPtr sessionOpts)
            {
                SessionPort = sessionPort;
                Joiner = joiner;
                _sessionOptsPtr = sessionOpts;
            }
            public UInt16 SessionPort { get; }
            public string Joiner { get; }
            public Session SessionOptions
            {
                get
                {
                    if (_sessionOpts == null && _sessionOptsPtr != IntPtr.Zero)
                        _sessionOpts = new Session(_sessionOptsPtr);
                    return _sessionOpts;
                }
            }
        }
        public sealed class AcceptSessionJoinerEventArgs : SessionJoinedEventArgs
        {
            private readonly IntPtr _sessionOptsPtr;
            private Session _sessionOpts;
            internal AcceptSessionJoinerEventArgs(UInt16 sessionPort, string joiner, IntPtr sessionOpts) : base(sessionPort, joiner, sessionOpts)
            {
            }
            public bool AcceptSession { get; set; } = true;
        }
    }
}