
    using System;

namespace AllJoynDotNet
{
    public abstract class AllJoynWrapper : IDisposable
    {
        private IntPtr _handle;
        private bool isHandleSet;
        internal AllJoynWrapper(IntPtr handle) {
            _handle = handle;
            isHandleSet = IntPtr.Zero != handle;

#if DEBUG
            //Force some GC to ensure we don't crash in the disposers
            GC.Collect();
            GC.WaitForPendingFinalizers();
#endif
        }

        //Should ONLY be called from a constructor that relies on creating+pinning callback listeners:
        internal void SetHandle(IntPtr handle)
        {
            if (isHandleSet)
                throw new InvalidOperationException();
            _handle = handle;
            isHandleSet = IntPtr.Zero != handle;
        }
        internal IntPtr Handle { get { return _handle; } }

        static AllJoynWrapper()
        {
            Init.Initialize();
        }

        ~AllJoynWrapper()
        {
            Dispose(false);
        }

        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        protected virtual void Dispose(bool disposing)
		{ 
			IsDisposed = true;
            _handle = IntPtr.Zero;
		} 
        protected bool IsDisposed { get; private set; }
    }
}