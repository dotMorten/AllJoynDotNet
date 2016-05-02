using AllJoynDotNet;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TestApp.Shared
{
    //Port of sample '\alljoyn\alljoyn_c\samples\about\about_client.cc'
    class AboutServiceTest : ISample
    {
        private BusAttachment bus;
        
        public void Start()
        {
            // Create the bus attachment
            bus = new BusAttachment("AboutServiceTest", true);

            bus.Start();
            Log.WriteLine("BusAttachment started");
            bus.Connect();
            Log.WriteLine("BusAttachment connect succeeded. BusName: " + bus.UniqueName);

            bus.AboutAnnounced += Result_AboutAnnounced;

            string[] interfaces = new[] { "INTERFACE_NAME" };
            bus.WhoImplementsInterfaces(interfaces);
            Log.WriteLine("WhoImplements called.");
            Log.LogBreak();
        }
        public void Stop()
        {
            bus.AboutAnnounced -= Result_AboutAnnounced;
            Log.WriteLine("AboutListener disposed");

            bus.Stop();
            Log.WriteLine("BusAttachment stopped");
            bus.Join();
            Log.WriteLine("Join complete");
            bus.Dispose();
            Log.WriteLine("BusAttachment disposed");
        }

        private static void Listener_SessionLost(object sender, EventArgs e)
        {
            Log.WriteLine("Session Lost");
        }

        private static void Result_AboutAnnounced(object sender, AboutListener.AboutAnnouncedEventArgs e)
        {
            Log.WriteLine("Announce signal discovered");
            Log.WriteLine($"\tFrom bus :{e.BusName}");
            Log.WriteLine($"\tAbout version {e.Version}");
            Log.WriteLine($"\tSessionPort {e.Port}");
            Log.WriteLine($"\tObjectDescription:");
            var paths = e.ObjectDescription.GetPaths().ToArray();
            foreach(var path in paths)
            {
                Log.WriteLine($"\t\t{path}");
                var interfaces = e.ObjectDescription.GetInterfaces(path);
                foreach(var i in interfaces)
                {
                    Log.WriteLine($"\t\t\t" + i);
                }
            }
            Log.WriteLine($"\tAboutData:");
            var fields = e.AboutData.GetFields();
            foreach (var field in fields)
            {
                var f = e.AboutData.GetField(field);
                Log.WriteLine($"\t\t{field}");
            }
        }

            //        typedef struct my_about_listener_t
            //        {
            //            alljoyn_sessionlistener sessionlistener;
            //            alljoyn_aboutlistener aboutlistener;
            //        }
            //        my_about_listener;

        }
}
