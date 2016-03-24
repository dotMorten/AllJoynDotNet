using AllJoynDotNet;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices;
using System.Runtime.InteropServices.WindowsRuntime;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Primitives;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Navigation;

// The Blank Page item template is documented at http://go.microsoft.com/fwlink/?LinkId=402352&clcid=0x409

namespace TestApp.UWP
{
    /// <summary>
    /// An empty page that can be used on its own or navigated to within a Frame.
    /// </summary>
    public sealed partial class MainPage : Page
    {
        static BusAttachment g_bus;
        public MainPage()
        {
            this.InitializeComponent();

            //bus = new BusAttachment("BusNameHere", true);
            //bus.Start();
            //bus.Connect();
            //string interfaceName = "org.test.a1234.AnnounceHandlerTest";
            //string interfaceQcc = "<node>" +
            //                        $"<interface name='{interfaceName}'>" +
            //                        "  <method name='Foo'>" +
            //                        "  </method>" +
            //                        "</interface>" +
            //                        "</node>";
            //bus.CreateInterfacesFromXml(interfaceQcc);
            //var name = bus.GetInterface(interfaceName);
            //System.Diagnostics.Debug.WriteLine(bus.UniqueName);

            StartAboutClient();
        }
        
        private void StartAboutClient()
        {
            string INTERFACE_NAME = "com.example.about.feature.interface.sample";
            g_bus = new BusAttachment("AboutServiceTest2", true);
            g_bus.Start();
            g_bus.Connect();
            var iq = g_bus.UniqueName;
            listener = new MyListener();
            g_bus.RegisterAboutListener(listener);
            g_bus.WhoImplementsInterfaces(new[] { INTERFACE_NAME });
        }
        MyListener listener;
        public class MyListener : AboutListener
        {
            protected override void OnCallback(string busName, ushort version)
            {
                System.Diagnostics.Debug.Write($"Callback from {busName} version {version}");
            }
        }
    }
}
