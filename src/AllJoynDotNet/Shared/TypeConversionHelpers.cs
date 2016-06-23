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
                case AllJoynTypeIds.Byte:
                    {
                        IntPtr valOut;
                        var status = MsgArg.alljoyn_msgarg_get(argument.Handle, "y", __arglist(out valOut));
                        return (byte)valOut;
                    }
                case AllJoynTypeIds.Boolean:
                    return GetVariantArg<bool>(argument, "b");
                case AllJoynTypeIds.Int16:
                    return GetVariantArg<Int16>(argument, "n");
                case AllJoynTypeIds.UInt16:
                    return GetUInt16Arg(argument, "q");
                case AllJoynTypeIds.Int32:
                    return GetVariantArg<Int32>(argument, "i");
                case AllJoynTypeIds.UInt32:
                    return GetVariantArg<UInt32>(argument, "u");
                case AllJoynTypeIds.Int64:
                    return GetVariantArg<Int64>(argument, "x");
                case AllJoynTypeIds.UInt64:
                    return GetVariantArg<UInt64>(argument, "t");
                case AllJoynTypeIds.Double:
                    return GetVariantArg<double>(argument, "d");
                case AllJoynTypeIds.String:
                case AllJoynTypeIds.DbusObjectPath:
                    return GetVariantArg_String(argument, "s");
                case AllJoynTypeIds.StructBegin:
                    return GetVariantStructureArg(argument);
                case AllJoynTypeIds.Array:
                    if (signature.Length < 2)
                    {
                        throw new ArgumentException("ER_BUS_BAD_SIGNATURE");
                    }
                    if (signature[1] == AllJoynTypeIds.DictionaryBegin)
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
            var status = MsgArg.alljoyn_msgarg_get(argument.Handle, signature, __arglist(ref value));
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
            if (signature[0] != AllJoynTypeIds.Array)
            {
                throw new ArgumentException("not an array");
            }
            if (IsArrayOfPrimitives(signature))
            {
                switch (signature[1])
                {
                    case AllJoynTypeIds.Int16:
                        return GetPrimitiveArrayMessageArg<short>(argument, signature, Marshal.Copy);
                    case AllJoynTypeIds.Int32:
                        return GetPrimitiveArrayMessageArg<Int32>(argument, signature, Marshal.Copy);
                    case AllJoynTypeIds.Int64:
                        return GetPrimitiveArrayMessageArg<Int64>(argument, signature, Marshal.Copy);
                    case AllJoynTypeIds.Double:
                        return GetPrimitiveArrayMessageArg<double>(argument, signature, Marshal.Copy);
                    case AllJoynTypeIds.Byte:
                        return GetPrimitiveArrayMessageArg<byte>(argument, signature, Marshal.Copy);
                        //return GetByteArrayMessageArg(argument, signature);
                    case AllJoynTypeIds.UInt16:
                        return GetPrimitiveArrayMessageArg<ushort>(argument, signature, MarshalHelpers.Copy);
                    case AllJoynTypeIds.UInt32:
                        return GetPrimitiveArrayMessageArg<uint>(argument, signature, MarshalHelpers.Copy);
                    case AllJoynTypeIds.UInt64:
                        return GetPrimitiveArrayMessageArg<ulong>(argument, signature, MarshalHelpers.Copy);
                    case AllJoynTypeIds.Boolean:
                    default:
                        throw new Exception("TODO");
                }
            }

            if(signature == AllJoynTypeIds.StringArray)
                return GetStringArrayMessageArg(argument, signature);
            throw new NotSupportedException($"ArrayType '{signature}' not implemented");
        }

        private static string[] GetStringArrayMessageArg(MsgArg argument, string signature)
        {
            UIntPtr las;
            IntPtr as_arg;
            var status = MsgArg.alljoyn_msgarg_get(argument.Handle, AllJoynTypeIds.StringArray, __arglist(out las, out as_arg));
            AllJoynException.CheckStatus(status);
            string[] result = new string[(int)las];
            IntPtr val;
            for (int i = 0; i < (int)las; i++)
            {
                status = MsgArg.alljoyn_msgarg_get(
                    MsgArg.alljoyn_msgarg_array_element(
                        as_arg, (UIntPtr)i), AllJoynTypeIds.String.ToString(), __arglist(out val));
                AllJoynException.CheckStatus(status);
                result[i] = Marshal.PtrToStringAnsi(val);
            }
            MsgArg.alljoyn_msgarg_destroy(as_arg);
           // MsgArg.alljoyn_msgarg_stabilize(argument.Handle);
            return result;
        }
        /*private static byte[] GetByteArrayMessageArg(MsgArg argument, string signature)
        {
            UIntPtr las;
            IntPtr as_arg;
            var status = MsgArg.alljoyn_msgarg_get(argument.Handle, AllJoynTypeIds.ByteArray, __arglist(out las, out as_arg));
            AllJoynException.CheckStatus(status);
            byte[] result = new byte[(int)las];
            IntPtr val;
            for (int i = 0; i < (int)las; i++)
            {
                var a = MsgArg.alljoyn_msgarg_array_element(as_arg, (UIntPtr)i);
                status = MsgArg.alljoyn_msgarg_get(a, AllJoynTypeIds.Byte.ToString(), __arglist(out val));
                AllJoynException.CheckStatus(status);
                result[i] = Marshal.ReadByte(val);
            }
            MsgArg.alljoyn_msgarg_destroy(as_arg);
            // MsgArg.alljoyn_msgarg_stabilize(argument.Handle);
            return result;
        }*/

        private static T[] GetPrimitiveArrayMessageArg<T>(MsgArg argument, string signature, Action<IntPtr, T[], int, int> copyAction)
        {
            UIntPtr size = UIntPtr.Zero;
            IntPtr valuePtr;
            var status = MsgArg.alljoyn_msgarg_get(argument.Handle, signature, __arglist(out size, out valuePtr));
            AllJoynException.CheckStatus(status);
            T[] elements = new T[(int)size];
            copyAction(valuePtr, elements, 0, (int)size);
            //Marshal.FreeCoTaskMem(valuePtr);
            return elements;
        }

        static bool IsArrayOfPrimitives(string signature)
        {
            return (signature.Length == 2) &&
                (signature[1] != AllJoynTypeIds.String) &&
                (signature[1] != AllJoynTypeIds.Variant) &&
                (signature[1] != AllJoynTypeIds.DbusObjectPath);
        }
    }
}
