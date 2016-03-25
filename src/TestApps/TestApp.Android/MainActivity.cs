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
    [Activity(Label = "AllJoynDotNet - TestApp", MainLauncher = true, Icon = "@drawable/icon")]
    public class MainActivity : Activity
    {
        int count = 1;
        BusAttachment bus;

        protected override void OnCreate(Bundle bundle)
        {
            base.OnCreate(bundle);

            //Uncomment this to test if the lib is loading correctly:
            //Java.Lang.Runtime.GetRuntime().LoadLibrary("alljoyn_c");

            // Set our view from the "main" layout resource
            SetContentView(Resource.Layout.Main);

            // Get our button from the layout resource,
            // and attach an event to it
            Button button = FindViewById<Button>(Resource.Id.MyButton);
            TextView status = FindViewById<TextView>(Resource.Id.StatusTextView);
            status.Text = $"AllJoyn Library Version: {version.VersionString} ({version.Version})\nAllJoyn BuildInfo:{version.BuildInfo}\n";
            button.Click += delegate {
                try {
                    button.Enabled = false;
                    bus = new BusAttachment("ServiceTest", true);
                    status.Text += "\nBus created";
                    bus.Start();
                    status.Text += "\nBus started";
                    bus.Connect();
                    status.Text += "\nBus connected. ID:" + bus.UniqueName;

                    string interfaceName = "org.test.a1234.AnnounceHandlerTest";
                    string interfaceQcc = "<node>" +
                                            $"<interface name='{interfaceName}'>" +
                                            "  <method name='Foo'>" +
                                            "  </method>" +
                                            "</interface>" +
                                            "</node>";
                    bus.CreateInterfacesFromXml(interfaceQcc);
                    var name = bus.GetInterface(interfaceName);
                    bus.Stop();
                    status.Text += "\nBus stopped";

                    bus.Join();
                    status.Text += "\nBus join complete";
                    bus.Dispose();
                    status.Text += "\nBus disposed";
                    button.Text = "Success";
                }
                catch(AllJoynException ex)
                {
                    status.Text += "\nALLJOYN ERROR : " + ex.Message + "\n" + ex.StackTrace +
                    "\nAllJoynError: 0x" + ex.AllJoynErrorCode.ToString("x4") + " " + ex.AllJoynError
                    + "\n\t" + ex.AllJoynComment;
                }
                catch (System.Exception ex)
                {
                    status.Text += "\nERROR : " + ex.Message + "\n" + ex.StackTrace;
                    if (ex.InnerException != null)
                        status.Text += "\n\tInner Exception : " + ex.InnerException.Message;
                }
                button.Enabled = true;

            };
        }
    }
}

