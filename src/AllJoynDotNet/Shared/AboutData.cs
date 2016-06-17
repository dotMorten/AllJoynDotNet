using System;
using System.Runtime.InteropServices;

namespace AllJoynDotNet
{
    public partial class AboutData : AllJoynWrapper
    {
        internal AboutData(MsgArg arg, string defaultLanguage) : base(AboutData.alljoyn_aboutdata_create(defaultLanguage))
        {
            var status = AboutData.alljoyn_aboutdata_createfrommsgarg(Handle, arg.Handle, null);
            AllJoynException.CheckStatus(status);
        }

        public AboutData(string defaultLanguage) : base(alljoyn_aboutdata_create(defaultLanguage))
        {
        }

        protected override void Dispose(bool disposing)
        {
            if (!IsDisposed)
            {
                //alljoyn_aboutdata_destroy(Handle);
            }
            base.Dispose(disposing);
        }
        public byte[] AppId
        {
            get
            {
                IntPtr appIdPtr;
                UIntPtr num;
                var status = alljoyn_aboutdata_getappid(Handle, out appIdPtr, out num);
                AllJoynException.CheckStatus(status);
                byte[] appId = new byte[(int)num];
                Marshal.Copy(appIdPtr, appId, 0, appId.Length);
                return appId;
            }
            set
            {
                alljoyn_aboutdata_setappid(Handle, value, (UIntPtr)value.Length);
            }
        }


        public string[] Fields
        {
            get
            {
                return AllJoynNative.GetStringArrayHelper(alljoyn_aboutdata_getfields, Handle);
            }
        }

        public void SetManufacturer(string manufacturer, string language)
        {
            AllJoynException.CheckStatus(alljoyn_aboutdata_setmanufacturer(Handle, manufacturer, language));
        }

        public void SetDescription(string description, string language)
        {
            AllJoynException.CheckStatus(alljoyn_aboutdata_setdescription(Handle, description, language));
        }
        public bool IsValid(string language = null)
        {
            return alljoyn_aboutdata_isvalid(Handle, language);
        }

        public void SetDeviceName(string deviceName, string language)
        {
            alljoyn_aboutdata_setdevicename(Handle, deviceName, language);
        }

        public void SetAppName(string appName, string language)
        {
            alljoyn_aboutdata_setappname(Handle, appName, language);
        }

        public MsgArg GetField(string fieldName, string language = null)
        {
            IntPtr outvalue;
            var status = alljoyn_aboutdata_getfield(Handle, fieldName, out outvalue, language);
            AllJoynException.CheckStatus(status);
            var tmp = new MsgArg(outvalue);
            return tmp;
        }

        public string[] SupportedLanguages
        {
            get
            {
                return AllJoynNative.GetStringArrayHelper(alljoyn_aboutdata_getsupportedlanguages, Handle);
            }
        }

        public string DefaultLanguage
        {
            get
            {
                IntPtr lang;
                var status = alljoyn_aboutdata_getdefaultlanguage(Handle, out lang);
                AllJoynException.CheckStatus(status);
                return Marshal.PtrToStringAnsi(lang);
            }
        }

        public string DeviceId
        {
            get
            {
                IntPtr str;
                AllJoynException.CheckStatus(alljoyn_aboutdata_getdeviceid(Handle, out str));
                return Marshal.PtrToStringAnsi(str);
            }
            set
            {
                AllJoynException.CheckStatus(alljoyn_aboutdata_setdeviceid(Handle, value));
            }
        }
        public string SoftwareVersion
        {
            get
            {
                IntPtr str;
                AllJoynException.CheckStatus(alljoyn_aboutdata_getsoftwareversion(Handle, out str));
                return Marshal.PtrToStringAnsi(str);                
            }
            set
            {
                AllJoynException.CheckStatus(alljoyn_aboutdata_setsoftwareversion(Handle, value));
            }
        }
        public string SupportUrl
        {
            get
            {
                IntPtr str;
                AllJoynException.CheckStatus(alljoyn_aboutdata_getsupporturl(Handle, out str));
                return Marshal.PtrToStringAnsi(str);
            }
            set
            {
                AllJoynException.CheckStatus(alljoyn_aboutdata_setsupporturl(Handle, value));
            }
        }
        public string ModelNumber
        {
            get
            {
                IntPtr str;
                AllJoynException.CheckStatus(alljoyn_aboutdata_getmodelnumber(Handle, out str));
                return Marshal.PtrToStringAnsi(str);
            }
            set
            {
                AllJoynException.CheckStatus(alljoyn_aboutdata_setmodelnumber(Handle, value));
            }
        }
        public string DateOfManufacture
        {
            get
            {
                IntPtr str;
                AllJoynException.CheckStatus(alljoyn_aboutdata_getdateofmanufacture(Handle, out str));
                return Marshal.PtrToStringAnsi(str);
            }
            set
            {
                AllJoynException.CheckStatus(alljoyn_aboutdata_setdateofmanufacture(Handle, value));
            }
        }
        public string HardwareVersion
        {
            get
            {
                IntPtr str;
                AllJoynException.CheckStatus(alljoyn_aboutdata_gethardwareversion(Handle, out str));
                return Marshal.PtrToStringAnsi(str);
            }
            set
            {
                AllJoynException.CheckStatus(alljoyn_aboutdata_sethardwareversion(Handle, value));
            }
        }
        
    }
}