using AllJoynDotNet;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TestApp.Shared;

namespace TestApp.NET45
{
    class Program
    { 
       static void Main(string[] args)
       {
            Log.OnMessage += Log_OnMessage;
            new GetLibraryInfo().Start();
            var sample = new Shared.AboutServiceTest();
            sample.Start();
            Console.ReadKey();
            sample.Stop();
            //CreateInterfaceTest.Start(args);
        }

        private static void Log_OnMessage(object sender, string e)
        {
            Console.Write(e);
        }
    }
}
