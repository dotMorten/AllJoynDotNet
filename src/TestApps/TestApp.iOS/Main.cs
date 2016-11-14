using TestApp.Shared;
using UIKit;

namespace TestApp.iOS
{
    public class Application
    {
        // This is the main entry point of the application.
        static void Main(string[] args)
        {
            // if you want to use a different Application Delegate class from "AppDelegate"
            // you can specify it here.
            UIApplication.Main(args, null, "AppDelegate");

            Log.OnMessage += Log_OnMessage;

            var sample = new GetLibraryInfo();            
            sample.Start();
        }

        private static void Log_OnMessage(object sender, string e)
        {
        }
    }
}