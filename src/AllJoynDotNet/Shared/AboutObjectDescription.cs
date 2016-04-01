using System;
using System.Collections.Generic;
using System.Runtime.InteropServices;

namespace AllJoynDotNet
{
    public partial class AboutObjectDescription : AllJoynWrapper
    {
        protected override void Dispose(bool disposing)
        {
            alljoyn_aboutobjectdescription_destroy(Handle);
            base.Dispose(disposing);
        }
        public IEnumerable<string> GetPaths()
        {
            UIntPtr result = alljoyn_aboutobjectdescription_getpaths(Handle, null, UIntPtr.Zero);
            IntPtr[] unsafePaths = new IntPtr[result.ToUInt64()];
            result = alljoyn_aboutobjectdescription_getpaths(Handle, unsafePaths, result);
            string[] paths = new string[result.ToUInt32()];
            for (int i = 0; i < paths.Length; ++i)
            {
                paths[i] = Marshal.PtrToStringAnsi(unsafePaths[i]);
            }
            return paths;
        }
        public IEnumerable<string> GetInterfaces(string path)
        {
            UIntPtr result = alljoyn_aboutobjectdescription_getinterfaces(Handle, path, null, UIntPtr.Zero);
            IntPtr[] unsafeInterfaces = new IntPtr[result.ToUInt32()];
            result = alljoyn_aboutobjectdescription_getinterfaces(Handle, path, unsafeInterfaces, result);
            string[] ifaces = new string[result.ToUInt32()];
            for (int i = 0; i < ifaces.Length; ++i)
            {
                ifaces[i] = Marshal.PtrToStringAnsi(unsafeInterfaces[i]);
            }
            return ifaces;
        }

        public void Clear()
        {
            alljoyn_aboutobjectdescription_clear(Handle);
        }

        public bool HasPath(string path)
        {
            return alljoyn_aboutobjectdescription_haspath(Handle, path);
        }
        public bool HasInterface(string interfaceName)
        {
            return alljoyn_aboutobjectdescription_hasinterface(Handle, interfaceName);
        }
        public bool HasInterfaceAtPath(string path, string interfaceName)
        {
            return alljoyn_aboutobjectdescription_hasinterfaceatpath(Handle, path, interfaceName);
        }
    }
}