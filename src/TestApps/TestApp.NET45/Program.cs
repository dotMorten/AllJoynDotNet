using AllJoynDotNet;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TestApp.NET45
{
    class Program
    {
        private static BusAttachment bus;
        static void Main(string[] args)
        {
            // Create the bus attachment
            bus = new BusAttachment("ServiceTest", true);
            bus.Start();
            bus.Connect();
            Debug.WriteLine("Bus started successfully. Unique name: " + bus.UniqueName);
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
            Console.WriteLine("Bus started successfully. Unique name: " + bus.UniqueName);
            Console.ReadKey();
        }
    }
}
