using System;
using System.Collections.Generic;
using System.Text;

namespace AllJoynDotNet
{
    public static class AllJoynTypeIds
    {
        public const char Int16 = 'n';
        public const char UInt16 = 'q';
        public const char Int32 = 'i';
        public const char UInt32 = 'u';
        public const char Int64 = 'x';
        public const char UInt64 = 't';
        public const char Double = 'd';
        public const char Byte = 'y';
        public const char String = 's';
        public const char Boolean = 'b';
        public const char Array = 'a';
        public const char StructBegin = '(';
        public const char StructEnd = ')';
        public const char DictionaryBegin = '{';
        public const char DictionaryEnd = '}';
        public const char Variant = 'v';
        public const char DbusObjectPath = 'o';

        public const string ByteArray = "ay";
        public const string StringArray = "as";
    }
}
