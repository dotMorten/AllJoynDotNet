﻿using System;
using System.Collections.Generic;
using System.Text;

namespace AllJoynDotNet
{
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

        public bool IsSecure
        {
            get
            {
                return alljoyn_interfacedescription_issecure(Handle) == AllJoynNative.QCC_TRUE;
            }
        }
    }
}