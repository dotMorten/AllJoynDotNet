using System;
using System.Runtime.InteropServices;

namespace AllJoynDotNet
{
    //
    public partial class Init
    {
        private static object initLock = new object();
        static bool isInitialized = false;
#if !__ANDROID__ && !NETFX_CORE
        [DllImport("kernel32.dll", CharSet = CharSet.Unicode, SetLastError = true)]
        [return: MarshalAs(UnmanagedType.Bool)]
        private static extern bool SetDllDirectory(string lpPathName);
#endif

        internal static void Initialize()
        {
            lock(initLock)
            {
                if (!isInitialized)
                {
                    try {
#if !__ANDROID__ && !NETFX_CORE
#if NETSTANDARD
                        bool is64bit = IntPtr.Size == 8;
#else
                        bool is64bit = Environment.Is64BitProcess;
#endif
                        bool ok = SetDllDirectory(is64bit ? "x64" : "x86");
#endif
                        var result = alljoyn_init();
                        if (result != 0)
                            throw new AllJoynException(result, "Failed to initialize AllJoyn");
                        isInitialized = true;
                    }
                    catch
                    {
                        throw;
                    }
                }
            }
        }
        
		internal static QStatus Shutdown()
        {
            return alljoyn_shutdown();
        }

        internal static QStatus InitRouter()
        {
            return alljoyn_routerinit();
        }
    }
}