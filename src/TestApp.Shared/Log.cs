using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Text;

namespace TestApp.Shared
{
    internal static class Log
    {
        public static void WriteLine(string message)
        {
            Debug.WriteLine("*** LOG *** " + message);
            OnMessage?.Invoke(null, message + Environment.NewLine);
        }
        public static event EventHandler<string> OnMessage;
    }
}
