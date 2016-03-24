using AllJoynDotNet;
using System;
using Android.App;
using Android.Content;
using Android.Runtime;
using Android.Views;
using Android.Widget;
using Android.OS;

namespace TestApp.Android
{
    [Activity(Label = "TestApp.Android", MainLauncher = true, Icon = "@drawable/icon")]
    public class MainActivity : Activity
    {
        int count = 1;
        BusAttachment bus;

        protected override void OnCreate(Bundle bundle)
        {
            base.OnCreate(bundle);

            // Set our view from the "main" layout resource
            SetContentView(Resource.Layout.Main);

            // Get our button from the layout resource,
            // and attach an event to it
            Button button = FindViewById<Button>(Resource.Id.MyButton);

            button.Click += delegate { button.Text = string.Format("{0} clicks!", count++); };
            //Init.Initialize();
            //bus = new BusAttachment("BusNameHere", true);
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
        }
    }
}

