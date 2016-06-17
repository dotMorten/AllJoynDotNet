using System;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using System.Text;

namespace AllJoynDotNet
{
    public enum SecurityPolicy
    {
        Inherit = alljoyn_interfacedescription_securitypolicy.AJ_IFC_SECURITY_INHERIT,
        Required = alljoyn_interfacedescription_securitypolicy.AJ_IFC_SECURITY_REQUIRED,
        Off = alljoyn_interfacedescription_securitypolicy.AJ_IFC_SECURITY_OFF
    }

    public partial class InterfaceDescription
    {
        internal static InterfaceDescription Create(IntPtr handle)
        {
            if (handle == IntPtr.Zero) return null;
            return new InterfaceDescription(handle);
        }

        protected override void Dispose(bool disposing)
        {
            base.Dispose(disposing);
            if(Handle != IntPtr.Zero)
            {
                //handle = IntPtr.Zero;
            }
        }

        public string Name
        {
            get
            {
                var p = alljoyn_interfacedescription_getname(Handle);
                return System.Runtime.InteropServices.Marshal.PtrToStringAnsi(p);
            }
        }
        public InterfaceDescriptionMember GetMember(string name)
        {
            alljoyn_interfacedescription_member ptr = new alljoyn_interfacedescription_member();
            alljoyn_interfacedescription_getmember(Handle, name, ref ptr);
            return new InterfaceDescriptionMember(ptr);
        }
        public class InterfaceDescriptionMember
        {
            internal alljoyn_interfacedescription_member Handle { get; }
            internal InterfaceDescriptionMember(alljoyn_interfacedescription_member handle)
            {

                Handle = handle;
            }
            public string Name
            {
                get
                {
                    return Marshal.PtrToStringAnsi(Handle.name);
                }
            }
            public string Signature
            {
                get
                {
                    return Marshal.PtrToStringAnsi(Handle.signature);
                }
            }
            public string ReturnSignature
            {
                get
                {
                    return Marshal.PtrToStringAnsi(Handle.returnSignature);
                }
            }
            public string ArgNames
            {
                get
                {
                    return Marshal.PtrToStringAnsi(Handle.argNames);
                }
            }
        }
        public bool IsSecure
        {
            get
            {
                return alljoyn_interfacedescription_issecure(Handle) == AllJoynNative.QCC_TRUE;
            }
        }
    }
}
