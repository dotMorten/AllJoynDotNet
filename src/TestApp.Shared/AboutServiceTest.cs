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
        System.Threading.CancellationTokenSource cancelSource;
        public void Start()
        {
            cancelSource = new System.Threading.CancellationTokenSource();
            var _1 = StartService().ContinueWith(t => StopService());
            var _2 = StartClient().ContinueWith(t => StopClient());
#if !XAMARIN && !NETFX_CORE
            Task.WhenAll(_1, _2).Wait();
#endif
            //StartService();
            //StartClient();
        }

        public void Stop()
        {
            cancelSource.Cancel();
            //StopClient();
            //StopService();
        }

#region Service
        private static ushort ASSIGNED_SESSION_PORT = 900;
        private const string INTERFACE_NAME = "com.example.about.feature.interface.sample";
        private BusAttachment serviceBus;
        private SessionPortListener sessionPortListener;
        private BusObject busObject;

        private async Task StartService()
        {
            serviceBus = new BusAttachment("About Service Example", true);
            serviceBus.Start();
            serviceBus.Connect();
            Log.WriteLine($"BusAttachment connect succeeded. BusName {serviceBus.UniqueName}");
            Session sessionOpts = new Session(TrafficType.Messages, false, Proximity.Any, Transport.Any);
            sessionPortListener = new SessionPortListener();
            sessionPortListener.AcceptSessionJoiner += SessionPortListener_AcceptSessionJoiner;
            sessionPortListener.SessionJoined += SessionPortListener_SessionJoined;
            var sessionPort = ASSIGNED_SESSION_PORT;
            serviceBus.BindSessionPort(sessionPort, sessionOpts, sessionPortListener);

            var aboutData = new AboutData("en");
            byte[] appId = { 0x01, 0xB3, 0xBA, 0x14,
                        0x1E, 0x82, 0x11, 0xE4,
                        0x86, 0x51, 0xD1, 0x56,
                        0x1D, 0x5D, 0x46, 0xB0 };
            aboutData.AppId = appId;
            aboutData.SetDeviceName("My Device Name", "en");
            aboutData.DeviceId = "93c06771-c725-48c2-b1ff-6a2a59d445b8";
            aboutData.SetAppName("Application", "en");
            aboutData.SetManufacturer("Manufacturer2", "en");
            aboutData.ModelNumber = "123456";
            aboutData.SetDescription("A poetic description of this application", "en");
            aboutData.DateOfManufacture = "2014-03-24";
            aboutData.SoftwareVersion = "0.1.2";
            aboutData.HardwareVersion = "0.0.1";
            aboutData.SupportUrl = "http://www.example.org";
            /*
              * The default language is automatically added to the `SupportedLanguages`
              * Users don't have to specify the AJSoftwareVersion its automatically added
              * to the AboutData/
              * Adding Spanish Localization values to the AboutData. All strings MUST be
              * UTF-8 encoded.
              */
            aboutData.SetDeviceName("Mi dispositivo Nombre", "es");
            aboutData.SetAppName("aplicación", "es");
            aboutData.SetManufacturer("fabricante", "es");
            aboutData.SetDescription("Una descripción poética de esta aplicación", "es");
            if(!aboutData.IsValid("en"))
            {
                Log.WriteLine("failed to setup about data.");
            }

            string xmlInterface = "<node>\n"                            +
             $"<interface name='{INTERFACE_NAME}'>\n"                   +
             "  <method name='Echo'>\n" +
             "    <arg name='out_arg' type='s' direction='in' />\n" +
             "    <arg name='return_arg' type='s' direction='out' />\n" +
             "  </method>\n" +
             "</interface>\n" +
             "</node>";
            Log.WriteLine(xmlInterface);
            serviceBus.CreateInterfacesFromXml(xmlInterface);
            busObject = create_my_alljoyn_busobject(serviceBus, "/example/path");
            serviceBus.RegisterBusObject(busObject);
            var aboutObj = new AboutObj(serviceBus, false);
            aboutObj.Announce(sessionPort, aboutData);
            Log.WriteLine("AboutObj Announce Succeeded.");
            Log.WriteLine("*********************************************************************************");
            Log.WriteLine("*********************************************************************************");

            while (!cancelSource.IsCancellationRequested)
                await Task.Delay(10);
        }

        private void SessionPortListener_SessionJoined(object sender, SessionPortListener.SessionJoinedEventArgs e)
        {
            Log.WriteLine($"Session Joined SessionId = {e.Joiner} - {e.SessionOptions.SessionId}");
        }

        private void SessionPortListener_AcceptSessionJoiner(object sender, SessionPortListener.AcceptSessionJoinerEventArgs e)
        {
            e.AcceptSession = ASSIGNED_SESSION_PORT == e.SessionPort;
        }

        private void StopService()
        {
            //TODO
            serviceBus.Stop();
            serviceBus.Join();
            serviceBus.Dispose();
        }
        
        private BusObject create_my_alljoyn_busobject(BusAttachment bus, string path)
        {
            var result = new BusObject(path, false);
            var iface = bus.GetInterface(INTERFACE_NAME);
            result.AddInterface(iface);
            result.SetAnnounceFlag(iface, true);

            var member = iface.GetMember("Echo");
            result.AddMethodHandler(member, OnEcho);
            return result;
        }
        private void OnEcho(BusObject busObject, InterfaceDescription.InterfaceDescriptionMember member, Message message)
        {
            string sig = message.Signature;
            Log.WriteLine($"Echo method called with argument: {ArgValueToString(message.GetArgument(0))}");
            busObject.MethodReplyArgs(message, message.GetArgument(0), 1); //reply back with the same message;
        }

#endregion

#region Client
        private BusAttachment clientBus;
        private async Task StartClient()
        { 
            // Create the bus attachment
            clientBus = new BusAttachment("AboutServiceTest", true);

            clientBus.Start();
            Log.WriteLine("BusAttachment started");
            clientBus.Connect();
            Log.WriteLine("BusAttachment connect succeeded. BusName: " + clientBus.UniqueName);

            clientBus.AboutAnnounced += Result_AboutAnnounced;

            string[] interfaces = new[] { INTERFACE_NAME };
            //string[] interfaces = new string[] {/* "org.alljoyn.About" */ };
            clientBus.WhoImplementsInterfaces(interfaces);
            Log.WriteLine("WhoImplements called.");
            Log.LogBreak();

            while (!cancelSource.IsCancellationRequested)
                await Task.Delay(10);
        }
        private void StopClient()
        { 
            clientBus.AboutAnnounced -= Result_AboutAnnounced;
            Log.WriteLine("AboutListener disposed");

            clientBus.Stop();
            Log.WriteLine("BusAttachment stopped");
            clientBus.Join();
            Log.WriteLine("Join complete");
            clientBus.Dispose();
            Log.WriteLine("BusAttachment disposed");
        }

        private void Listener_SessionLost(object sender, EventArgs e)
        {
            Log.WriteLine("Session Lost");
        }

        private async void Result_AboutAnnounced(object sender, AboutListener.AboutAnnouncedEventArgs e)
        {
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
            Log.WriteLine("*********************************************************************************");

            var session = new Session(TrafficType.Messages, false, Proximity.Any, Transport.Any);
            clientBus.EnableConcurrentCallbacks();
            var sessionId = clientBus.JoinSession(e.BusName, e.Port, null, session);
            if (sessionId != 0)
            {
                var aboutProxy = new AboutProxy(clientBus, e.BusName, sessionId);
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
                using (
                var aboutData = aboutProxy.GetAboutData("en"))
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
                Log.WriteLine("*********************************************************************************");
                Log.WriteLine($"AboutProxy.GetVersion {aboutProxy.Version}");
                Log.WriteLine("*********************************************************************************");
                string[] paths = e.ObjectDescription.GetInterfacePaths(INTERFACE_NAME);
                var proxyObject = new ProxyBusObject(clientBus, e.BusName, paths[0], session);
                proxyObject.IntrospectRemoteObject();
                var arg = new MsgArg("ECHO Echo echo...\n");
                //var replyMsg = proxyObject.MethodCall(clientBus, INTERFACE_NAME, "Echo", arg, 1, TimeSpan.FromMilliseconds(25000));
                var replyMsg = await proxyObject.MethodCallAsync(serviceBus, INTERFACE_NAME, "Echo", arg, 1, TimeSpan.FromMilliseconds(25000));
                //
                var msg = replyMsg.GetArgument(0);
                Log.WriteLine($"Echo method reply: {ArgValueToString(msg)}");
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
            return $"User defined Value\tValue: '{v?.ToString()}'\tSignature: '{arg.Signature}'";
        }
#endregion
    }
}
