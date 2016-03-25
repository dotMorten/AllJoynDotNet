using System;
using System.Collections.Generic;
using System.Text;

namespace AllJoynDotNet
{
    public sealed class AllJoynException : Exception
    {
        private readonly string _message;
        internal AllJoynException(int code, string message)
        {
            var error = ErrorCodeLookup.GetError(code);
            Source = Constants.DLL_IMPORT_TARGET;
            _message = message ?? $"0x{code:x4} {error.Name}: {error.Comment}";
            AllJoynError = error.Name;
            AllJoynErrorCode = error.Value;
            AllJoynComment = error.Comment;
        }
        internal AllJoynException(int code) : this(code, null)
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
    }
}
