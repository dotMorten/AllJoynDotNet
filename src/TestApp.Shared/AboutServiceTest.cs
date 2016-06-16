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

            //string[] interfaces = new[] { "INTERFACE_NAME" };
            string[] interfaces = new string[] {/* "org.alljoyn.About" */ };
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

        private void Listener_SessionLost(object sender, EventArgs e)
        {
            Log.WriteLine("Session Lost");
        }

        private void Result_AboutAnnounced(object sender, AboutListener.AboutAnnouncedEventArgs e)
        {
            //Log.WriteLine("Announce signal discovered");
            Log.WriteLine($"\tFrom bus :{e.BusName}");
            Log.WriteLine($"\tAbout version {e.Version}");
            Log.WriteLine($"\tSessionPort {e.Port}");
            Log.WriteLine($"\tObjectDescription:");
            foreach (var path in e.ObjectDescription.Paths)
            {
                Log.WriteLine($"\t\t{path}");
                var interfaces = e.ObjectDescription.GetInterfaces(path);
                foreach (var i in interfaces)
                {
                    Log.WriteLine($"\t\t\t" + i);
                }
            }
            Log.WriteLine($"\tAboutData:");
            PrintAboutData(e.AboutData, null, 2);
            //foreach (var field in e.AboutData.Fields)
            //{
            //    var f = e.AboutData.GetField(field);
            //    var v = ArgValueToString(f);
            //    Log.WriteLine($"\t\tKey: {field}\t{v}");
            //}
            Log.WriteLine("*********************************************************************************");

            var session = new Session(TrafficType.Messages, false, Proximity.Any, Transport.Any);
            bus.EnableConcurrentCallbacks();
            var sessionId = bus.JoinSession(e.BusName, e.Port, null, session);
            if (sessionId != 0)
            {
                var aboutProxy = new AboutProxy(bus, e.BusName, sessionId);
                Log.WriteLine("*********************************************************************************");
                Log.WriteLine("AboutProxy.GetObjectDescription:");

                var aod2 = aboutProxy.ObjectDescription;
                foreach(var path in aod2.Paths)
                {
                    Log.WriteLine($"\t\t{path}");
                    foreach(var i in aod2.GetInterfaces(path))
                    {
                        Log.WriteLine($"\t\t\t{i}");
                    }
                }
                Log.WriteLine("*********************************************************************************");
                Log.WriteLine("AboutProxy.GetAboutData: (Default Language)");
                //using 
                var aboutData = aboutProxy.GetAboutData("en");
                {
                    PrintAboutData(aboutData, "en", 1);
                    var langs = aboutData.SupportedLanguages;
                    if (langs.Length > 1)
                    {
                        var dlang = aboutData.DefaultLanguage;
                        foreach (var lang in langs)
                        {
                            if (lang == dlang) continue;
                            using (var data = aboutProxy.GetAboutData(lang))
                            {
                                PrintAboutData(data, lang, 1);
                            }
                        }
                    }
                }
            }
            Log.WriteLine("*********************************************************************************");
        }

        private void PrintAboutData(AboutData aboutData, string language, int tabNum)
        {
            foreach(var field in aboutData.Fields)
            {
                StringBuilder sb = new StringBuilder();
                for (int j = 0; j < tabNum; ++j)
                {
                    sb.Append("\t");
                }
                sb.Append($"Key: {field}");
                var value = aboutData.GetField(field, language);
                sb.Append("\t");
                sb.Append(ArgValueToString(value));
                Log.WriteLine(sb.ToString());
            }
        }
        private static string ArgValueToString(MsgArg arg)
        {
            var v = arg.Value;
            if (v is byte[])
                return string.Join(" ", ((byte[])v).Select(b => b.ToString("x2")));
            if (v is string[])
                return string.Join(" ", (string[])v);
            if (v is string)
                return (string)v;

            return "User defined Value\tSignature: " + arg.Signature;
        }
        //        typedef struct my_about_listener_t
        //        {
        //            alljoyn_sessionlistener sessionlistener;
        //            alljoyn_aboutlistener aboutlistener;
        //        }
        //        my_about_listener;

    }
}
