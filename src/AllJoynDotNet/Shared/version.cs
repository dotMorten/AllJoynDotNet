using System;
using System.Runtime.InteropServices;

namespace AllJoynDotNet
{
//
    public partial class version : AllJoynWrapper
    {
        /// <summary>
        /// Gives the version of AllJoyn Library 
        /// </summary>
        public static string Version
        {
            get
            {
                IntPtr p = alljoyn_getversion();
                return Marshal.PtrToStringAnsi(p);
            }
        }

        /// <summary>
        /// Gives build information of AllJoyn Library
        /// </summary>
        public static string BuildInfo
        {
            get
            {
                IntPtr p = alljoyn_getbuildinfo();
                return Marshal.PtrToStringAnsi(p);
            }
        }

        /// <summary>
        /// Gives the version of AllJoyn Library as a single number
        /// </summary>
        public static UInt32 NumericVersion
        {
            get
            {
                return alljoyn_getnumericversion();
            }
        }
    }
}