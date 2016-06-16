using System;
using System.Collections.Generic;
using System.Text;

namespace AllJoynDotNet
{
    public sealed class AllJoynException : Exception
    {
        private readonly string _message;
        internal AllJoynException(QStatus code, string message)
        {
            var error = ErrorCodeLookup.GetError((int)code);
            Source = Constants.DLL_IMPORT_TARGET;
            int codeNumber = (int)code;
            _message = message ?? $"0x{codeNumber:x4} {error.Name}: {error.Comment}";
            AllJoynError = error.Name;
            AllJoynErrorCode = error.Value;
            AllJoynComment = error.Comment;
        }
        internal AllJoynException(QStatus code) : this(code, null)
        {
        }
        public int AllJoynErrorCode { get; }
        public string AllJoynError { get; }
        public string AllJoynComment { get; }

        public override string Message
        {
            get
            {
                return _message;
            }
        }

        internal static void CheckStatus(QStatus status)
        {
            if (status != QStatus.ER_OK)
                throw new AllJoynException(status);
        }
    }
}
