using System;
using System.Collections.Generic;
using System.Text;

namespace AllJoynDotNet
{
    internal static class MarshalHelpers
    {
        internal static unsafe void Copy(IntPtr ptr, ushort[] destination, int startIndex, int length)
        {
            try
            {
                for (int i = 0; i < length; i++)
                {
                    byte* addr = (byte*)ptr + i;
                    if ((unchecked((int)addr) & 0x1) == 0)
                    {
                        // aligned read
                        destination[i + startIndex] = *((UInt16*)addr);
                    }
                    else
                    {
                        // unaligned read
                        UInt16 val;
                        byte* valPtr = (byte*)&val;
                        valPtr[0] = addr[0];
                        valPtr[1] = addr[1];
                        destination[i + startIndex] = val;
                    }
                }
            }
            catch (NullReferenceException)
            {
                // this method is documented to throw AccessViolationException on any AV
                throw; // new AccessViolationException();
            }
        }

        internal static unsafe void Copy(IntPtr ptr, uint[] destination, int startIndex, int length)
        {
            try
            {
                for (int i = 0; i < length; i++)
                {
                    byte* addr = (byte*)ptr + i;
                    if ((unchecked((int)addr) & 0x3) == 0)
                    {
                        // aligned read
                        destination[i + startIndex] = *((UInt32*)addr);
                    }
                    else
                    {
                        // unaligned read
                        UInt32 val;
                        byte* valPtr = (byte*)&val;
                        valPtr[0] = addr[0];
                        valPtr[1] = addr[1];
                        valPtr[2] = addr[2];
                        valPtr[3] = addr[3];
                        destination[i + startIndex] = val;
                    }
                }
            }
            catch (NullReferenceException)
            {
                // this method is documented to throw AccessViolationException on any AV
                throw; // new AccessViolationException();
            }
        }

        internal static unsafe void Copy(IntPtr ptr, ulong[] destination, int startIndex, int length)
        {
            try
            {
                for (int i = 0; i < length; i++)
                {
                    byte* addr = (byte*)ptr + i;
                    if ((unchecked((int)addr) & 0x7) == 0)
                    {
                        // aligned read
                        destination[i + startIndex] = *((UInt64*)addr);
                    }
                    else
                    {
                        // unaligned read
                        UInt64 val;
                        byte* valPtr = (byte*)&val;
                        valPtr[0] = addr[0];
                        valPtr[1] = addr[1];
                        valPtr[2] = addr[2];
                        valPtr[3] = addr[3];
                        valPtr[4] = addr[4];
                        valPtr[5] = addr[5];
                        valPtr[6] = addr[6];
                        valPtr[7] = addr[7];
                        destination[i + startIndex] = val;
                    }
                }
            }
            catch (NullReferenceException)
            {
                // this method is documented to throw AccessViolationException on any AV
                throw; // new AccessViolationException();
            }
        }
    }
}
