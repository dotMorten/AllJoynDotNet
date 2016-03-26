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
        public static string VersionString
        {
            get
            {
                Init.Initialize();
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
                Init.Initialize();
                IntPtr p = alljoyn_getbuildinfo();
                return Marshal.PtrToStringAnsi(p);
            }
        }

        /// <summary>
        /// Gives the version of AllJoyn Library as a <see cref="System.Version"/> type
        /// </summary>
        public static Version Version
        {
            get
            {
                Init.Initialize();
                UInt32 v = alljoyn_getnumericversion();                
                var major = (int)((v >> 24) & 0xff);
                var minor = (int)((v >> 16) & 0xff);
                var build = (int)((v >> 8) & 0xff);
                var rev = (int)(v & 0xff);
                return new Version(major, minor, build, rev);
            }
        }
    }
}