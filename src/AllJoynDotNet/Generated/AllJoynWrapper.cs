
    using System;

namespace AllJoynDotNet
{
    public abstract class AllJoynWrapper : IDisposable
    {
        private IntPtr _handle;

        internal AllJoynWrapper(IntPtr handle) { _handle = handle; }

        //Should ONLY be called from a constructor:
        internal void SetHandle(IntPtr handle)
        {
            _handle = handle;
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
		} 
        protected bool IsDisposed { get; private set; }
    }
}