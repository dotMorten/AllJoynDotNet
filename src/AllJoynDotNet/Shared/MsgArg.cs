using System;
using System.Linq;
using System.Runtime.InteropServices;

namespace AllJoynDotNet
{
    public partial class MsgArg : AllJoynWrapper
    {
        static MsgArg()
        {
            Init.Initialize();
        }

        private IntPtr _bytePtr;

        public MsgArg() : base(alljoyn_msgarg_create())
        {

        }

        public MsgArg(string value) : this()
        {
            Set(value);
        }

        protected override void Dispose(bool disposing)
        {
            if (!IsDisposed)
            {
                if (_bytePtr != IntPtr.Zero)
                {
                    Marshal.FreeCoTaskMem(_bytePtr);
                    _bytePtr = IntPtr.Zero;
                }
                //alljoyn_msgarg_destroy(Handle);
            }
            base.Dispose(disposing);
        }

        private void Set(object value)
        {
            UIntPtr numArgs = (UIntPtr)1;
            string signature = "";
#if DEBUG //we don't cache the value in debug to provoke potential bugs. TODO: Remove later
            _value = null;
            _valueInitialized = false;
#else
            _value = value;
            _valueInitialized = true;
#endif

            if (_bytePtr != IntPtr.Zero)
            {
                Marshal.FreeCoTaskMem(_bytePtr);
                _bytePtr = IntPtr.Zero;
            }

            /*
            ALLJOYN_ARRAY            = 'a',    ///< AllJoyn array container type
            ALLJOYN_DICT_ENTRY       = 'e',    ///< AllJoyn dictionary or map container type - an array of key-value pairs
            ALLJOYN_SIGNATURE        = 'g',    ///< AllJoyn signature basic type
            ALLJOYN_HANDLE           = 'h',    ///< AllJoyn socket handle basic type
            ALLJOYN_STRUCT           = 'r',    ///< AllJoyn struct container type
            */

            if (value.GetType() == typeof(string))
            {
                signature = "s";
                _bytePtr = Marshal.StringToCoTaskMemAnsi((string)value);
                alljoyn_msgarg_set(Handle, signature, __arglist(_bytePtr));
            }
            else if (value.GetType() == typeof(bool))
            {
                signature = "b";
                int newValue = ((bool)value ? 1 : 0);
                alljoyn_msgarg_set(Handle, signature, __arglist(newValue));
            }
            else if (value.GetType() == typeof(double))
            {
                signature = "d";
                alljoyn_msgarg_set(Handle, signature, __arglist((double)value));
            }
            else if (value.GetType() == typeof(int))
            {
                signature = "i";
                alljoyn_msgarg_set(Handle, signature, __arglist((int)value));
            }
            else if (value.GetType() == typeof(uint))
            {
                signature = "u";
                alljoyn_msgarg_set(Handle, signature, __arglist((uint)value));
            }
            else if (value.GetType() == typeof(short))
            {
                signature = "n";
                alljoyn_msgarg_set(Handle, signature, __arglist((short)value));
            }
            else if (value.GetType() == typeof(ushort))
            {
                signature = "q";
                alljoyn_msgarg_set(Handle, signature, __arglist((ushort)value));
            }
            else if (value.GetType() == typeof(long))
            {
                signature = "x";
                alljoyn_msgarg_set(Handle, signature, __arglist((long)value));
            }
            else if (value.GetType() == typeof(ulong))
            {
                signature = "t";
                alljoyn_msgarg_set(Handle, signature, __arglist((ulong)value));
            }
            else if (value.GetType() == typeof(byte))
            {
                signature = "y";
                alljoyn_msgarg_set(Handle, signature, __arglist((byte)value));
            }
            else if (value is Array)
            {
                SetArrayValue((Array)value);
            }
            else if (value is System.Collections.IDictionary)
            {
                throw new NotImplementedException("TODO");
            }
            else
            {
                if(value is float)
                    throw new NotSupportedException($"Float not supported. Please use 'double'");
                else if (value is System.Collections.IList)
                    throw new NotSupportedException($"Lists not supported. Please use arrays");
                else
                    throw new NotSupportedException($"Unsupported value type {value.GetType().FullName}");
            }
        }

        private void SetArrayValue(Array elements)
        {
            var eType = elements.GetType().GetElementType();
            if (eType == typeof(byte))
            {
                var arr = (byte[])elements;
                alljoyn_msgarg_set_uint8_array(Handle, (UIntPtr)arr.Length, arr);
            }
            else if (eType == typeof(bool))
            {
                var arr = (bool[])elements;
                alljoyn_msgarg_set_bool_array(Handle, (UIntPtr)arr.Length, arr.Select(t => t.ToQccBool()).ToArray());
            }
            else if (eType == typeof(short))
            {
                var arr = (short[])elements;
                alljoyn_msgarg_set_int16_array(Handle, (UIntPtr)arr.Length, arr);
            }
            else if (eType == typeof(ushort))
            {
                var arr = (ushort[])elements;
                alljoyn_msgarg_set_uint16_array(Handle, (UIntPtr)arr.Length, arr);
            }
            else if (eType == typeof(int))
            {
                var arr = (int[])elements;
                alljoyn_msgarg_set_int32_array(Handle, (UIntPtr)arr.Length, arr);
            }
            else if (eType == typeof(uint))
            {
                var arr = (uint[])elements;
                alljoyn_msgarg_set_uint32_array(Handle, (UIntPtr)arr.Length, arr);
            }
            else if (eType == typeof(long))
            {
                var arr = (long[])elements;
                alljoyn_msgarg_set_int64_array(Handle, (UIntPtr)arr.Length, arr);
            }
            else if (eType == typeof(ulong))
            {
                var arr = (ulong[])elements;
                alljoyn_msgarg_set_uint64_array(Handle, (UIntPtr)arr.Length, arr);
            }
            else
                throw new NotImplementedException();
        }

        public void Clear()
        {
            alljoyn_msgarg_clear(Handle);
        }

        private object _value;
        private bool _valueInitialized;
        private object _thisLock = new object();
        public object Value
        {
            get
            {
                return System.Threading.LazyInitializer.EnsureInitialized(ref _value, ref _valueInitialized, ref _thisLock, () =>
                {
                    return TypeConversionHelpers.GetValueFromVariant(this, this.Signature);
                });
            }
            set
            {
                Set(value);
            }
        }

        public string Signature
        {
            get
            {
                UIntPtr size = (UIntPtr)16;
                System.Text.StringBuilder sb = new System.Text.StringBuilder(256);
                var resultLength = (UInt64)alljoyn_msgarg_signature(Handle, sb, size + 1);
                return sb.ToString();
            }
        }
    }
}