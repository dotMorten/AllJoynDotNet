using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;

namespace AllJoynDotNet
{
    internal class Constants
    {
        internal const string DLL_IMPORT_TARGET =
#if __ANDROID__
            "liballjoyn_c.so";
#elif NETFX_CORE
            "MSAJApi.dll";
#else
            "alljoyn_c.dll";
#endif

    }
    internal static partial class AllJoynNative
    {
        public const Int32 QCC_TRUE = 1;
        public const Int32 QCC_FALSE = 0;
        
        internal static Int32 ToQccBool(this bool flag)
        {
            return flag ? QCC_TRUE : QCC_FALSE;
        }
        internal static string[] GetStringArrayHelper(Func<IntPtr, IntPtr[], UIntPtr, UIntPtr> method,
    IntPtr Handle)
        {
            UIntPtr result = method(Handle, null, UIntPtr.Zero);
            IntPtr[] unsafeStrings = new IntPtr[result.ToUInt64()];
            result = method(Handle, unsafeStrings, result);
            string[] strings = new string[result.ToUInt32()];
            for (int i = 0; i < strings.Length; ++i)
            {
                strings[i] = Marshal.PtrToStringAnsi(unsafeStrings[i]);
            }
            return strings;

        }

    }
}
