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
                        bool ok = SetDllDirectory(Environment.Is64BitProcess ? "x64" : "x86");
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
        
		internal static Int32 Shutdown()
        {
            return alljoyn_shutdown();
        }
    }
}