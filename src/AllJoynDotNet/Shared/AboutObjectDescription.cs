using System;
using System.Collections.Generic;
using System.Runtime.InteropServices;

namespace AllJoynDotNet
{
    public partial class AboutObjectDescription : AllJoynWrapper
    {
        internal AboutObjectDescription(MsgArg arg) : base(AboutObjectDescription.alljoyn_aboutobjectdescription_create())
        {
            var status = AboutObjectDescription.alljoyn_aboutobjectdescription_createfrommsgarg(Handle, arg.Handle);
            if (status != QStatus.ER_OK) throw new AllJoynException(status);
        }

        protected override void Dispose(bool disposing)
        {
            alljoyn_aboutobjectdescription_destroy(Handle);
            base.Dispose(disposing);
        }
        public IEnumerable<string> GetPaths()
        {
            return AllJoynNative.GetStringArrayHelper(alljoyn_aboutobjectdescription_getpaths, Handle);
        }
        public IEnumerable<string> GetInterfaces(string path)
        {
            return AllJoynNative.GetStringArrayHelper((a, b, c) => { return alljoyn_aboutobjectdescription_getinterfaces(a, path, b, c); }, Handle);
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