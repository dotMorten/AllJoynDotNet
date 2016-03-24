using AllJoynDotNet;
using System;
using System.Collections.Generic;
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
            bus = new BusAttachment("ServiceTest", true);
            bus.Start();
            bus.Connect();
            string interfaceName = "org.test.a1234.AnnounceHandlerTest";
            string interfaceQcc = "<node>" +
                                    $"<interface name='{interfaceName}'>" +
                                    "  <method name='Foo'>" +
                                    "  </method>" +
                                    "</interface>" +
                                    "</node>";
            bus.CreateInterfacesFromXml(interfaceQcc);
            var iface = bus.GetInterface(interfaceName);
            var secure = iface.IsSecure;
            var name = iface.Name;
            Console.WriteLine("Bus started successfully. Unique name: " + bus.UniqueName);
            Console.ReadKey();
        }
    }
}
