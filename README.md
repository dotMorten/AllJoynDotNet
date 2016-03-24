# AllJoynDotNet
A .NET wrapper for the AllJoyn C Library

##### Note: This is very much a work in progress!

Features:
  - Code generator that auto-generates the Interop from the AllJoyn C-API, including full code-summaries (make sure you pull the submodule for this to work).
  - Library for Windows Universal, Windows Desktop, Xamarin-Android, and Xamarin-iOS.

What's not working:
  - Besides limited creation of bus attachments, no classes has been done for the rest yet.
  - Generated interop code is mostly untested, but so far seems to work in most places.
  - Xamarin-Android currently doesn't work at all (can't find the library).
  - Xamarin-iOS hasn't been started yet (help wanted).

#### FAQ
**Windows 10 UWP already AllJoyn. Do I need this?**
   - The managed interface is far too limited, and the provided code-generation tools can't create proper certifiable services. You still need C++ to create a proper client. When running under UWP, not AllJoyn native library is needed, since the one provided with Windows 10 is used.

**Why?**
   - Because I don't like to write too much code in C++. I'm faster in C#. Also building AllJoyn clients often mean running on mobile, so we need a cross platform way like Xamarin  

##### A code sample:
```csharp
// Create the bus attachment
var bus = new BusAttachment("ServiceTest", true);
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
```
