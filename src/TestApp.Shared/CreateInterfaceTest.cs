using AllJoynDotNet;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TestApp.Shared
{
    class CreateInterfaceTest : ISample
    {
        private BusAttachment bus;
        public void Start()
        {
            // Create the bus attachment
            bus = new BusAttachment("ServiceTest", true);
            bus.Start();
            Log.WriteLine("BusAttachment started");
            bus.Connect();
            Log.WriteLine("BusAttachment connect succeeded. BusName: " + bus.UniqueName);
            //Create interface
            string interfaceName = "org.test.a1234.AnnounceHandlerTest";
            string interfaceQcc = "<node>" +
                                    $"<interface name='{interfaceName}'>" +
                                    "  <method name='Foo'>" +
                                    "  </method>" +
                                    "</interface>" +
                                    "</node>";
            bus.CreateInterfacesFromXml(interfaceQcc);
            //Test if the interface is there
            var iface = bus.GetInterface(interfaceName);
            var secure = iface.IsSecure;
            var name = iface.Name;

        }
        public void Stop()
        {
            bus.Stop();
            bus.Join();
            bus.Dispose();
        }
    }
}
