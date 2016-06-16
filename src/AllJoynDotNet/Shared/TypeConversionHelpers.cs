using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;

namespace AllJoynDotNet
{
    internal static class TypeConversionHelpers
    {
        public static object GetValueFromVariant(MsgArg argument, string signature)
        {
            switch (signature[0])
            {
                case 'y':
                    return GetVariantArg<byte>(argument, "y");
                case 'b':
                    return GetVariantArg<bool>(argument, "b");
                case 'n':
                    return GetVariantArg<Int16>(argument, "n");
                case 'q':
                    return GetUInt16Arg(argument, "q");
                case 'i':
                    return GetVariantArg<Int32>(argument, "i");
                case 'u':
                    return GetVariantArg<UInt32>(argument, "u");
                case 'x':
                    return GetVariantArg<Int64>(argument, "x");
                case 't':
                    return GetVariantArg<UInt64>(argument, "t");
                case 'd':
                    return GetVariantArg<double>(argument, "d");
                case 's':
                case 'o':
                    return GetVariantArg_String(argument, "s");
                case '(':
                    return GetVariantStructureArg(argument);
                case 'a':
                    if (signature.Length < 2)
                    {
                        throw new ArgumentException("ER_BUS_BAD_SIGNATURE");
                    }
                    if (signature[1] == '{')
                    {
                        if (signature.Length < 3)
                        {
                            throw new ArgumentException("ER_BUS_BAD_SIGNATURE");
                        }
                        throw new NotImplementedException("TODO");
                        //return GetMapFromVariant(argument, variantSignature[2], value);
                    }
                    else
                    {
                        return GetAllJoynMessageArgArray(argument, signature);
                    }
            }
            throw new ArgumentException("ER_BUS_BAD_SIGNATURE");
        }

        private static object GetVariantStructureArg(MsgArg argument)
        {
            throw new NotImplementedException("TODO");
        }

        private static UInt16 GetUInt16Arg(MsgArg argument, string signature)
        {
            UInt16 value;
            var status = MsgArg.alljoyn_msgarg_get(argument.Handle, signature, __arglist(out value));
            AllJoynException.CheckStatus(status);
            return value;
        }

        private static T GetVariantArg<T>(MsgArg argument, string signature)
        {
            T value = default(T);
            var status = MsgArg.alljoyn_msgarg_get(argument.Handle, signature, __arglist(out value));
            AllJoynException.CheckStatus(status);
            return value;
        }

        private static string GetVariantArg_String(MsgArg argument, string signature)
        {
            IntPtr strOut;
            var status = MsgArg.alljoyn_msgarg_get(argument.Handle, signature, __arglist(out strOut));
            AllJoynException.CheckStatus(status);
            var value = System.Runtime.InteropServices.Marshal.PtrToStringAnsi(strOut);
            return value;
        }
        public static object GetAllJoynMessageArgArray(MsgArg argument, string signature)
        {
            if (signature[0] != 'a')
            {
                throw new ArgumentException("not an array");
            }
            if (IsArrayOfPrimitives(signature))
            {
                switch (signature[1])
                {
                    case 'y':
                        return GetPrimitiveArrayMessageArg<byte>(argument, signature, Marshal.Copy);
                    case 'n':
                        return GetPrimitiveArrayMessageArg<short>(argument, signature, Marshal.Copy);
                    case 'd':
                        return GetPrimitiveArrayMessageArg<double>(argument, signature, Marshal.Copy);
                    case 'i':
                        return GetPrimitiveArrayMessageArg<Int32>(argument, signature, Marshal.Copy);
                    case 'x':
                        return GetPrimitiveArrayMessageArg<Int64>(argument, signature, Marshal.Copy);
                    //char
                    //float
                    default:
                        throw new Exception("TODO");
                }
            }
            if(signature == "as")
                return GetStringArrayMessageArg(argument, signature);
            throw new NotSupportedException($"ArrayType '{signature}' not implemented");
        }

        private static string[] GetStringArrayMessageArg(MsgArg argument, string signature)
        {
            UIntPtr las;
            //var as_arg = MsgArg.alljoyn_msgarg_create();
            IntPtr as_arg;
            var status = MsgArg.alljoyn_msgarg_get(argument.Handle, "as", __arglist(out las, out as_arg));
            AllJoynException.CheckStatus(status);
            string[] result = new string[(int)las];
            IntPtr val;
            for (int i = 0; i < (int)las; i++)
            {
                status = MsgArg.alljoyn_msgarg_get(
                    MsgArg.alljoyn_msgarg_array_element(
                        as_arg, (UIntPtr)i), "s", __arglist(out val));
                AllJoynException.CheckStatus(status);
                result[i] = Marshal.PtrToStringAnsi(val);
            }
            MsgArg.alljoyn_msgarg_destroy(as_arg);
            return result;
        }

        private static T[] GetPrimitiveArrayMessageArg<T>(MsgArg argument, string signature, Action<IntPtr, T[], int, int> copyAction)
        {
            UIntPtr size = UIntPtr.Zero;
            IntPtr valuePtr;
            var status = MsgArg.alljoyn_msgarg_get(argument.Handle, signature, __arglist(out size, out valuePtr));
            AllJoynException.CheckStatus(status);
            T[] elements = new T[(int)size];
            copyAction(valuePtr, elements, 0, (int)size);
            return elements;
        }

        static bool IsArrayOfPrimitives(string signature)
        {
            return (signature.Length == 2) && (signature[1] != 's') && (signature[1] != 'v');
        }
    }
}
