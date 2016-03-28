using AllJoynDotNet;
using System;
using Android.App;
using Android.Content;
using Android.Runtime;
using Android.Views;
using Android.Widget;
using Android.OS;
using TestApp.Shared;

namespace TestApp.Android
{
    [Activity(Label = "AllJoynDotNet - TestApp", MainLauncher = true, Icon = "@drawable/icon")]
    public class MainActivity : Activity
    {
        ISample currentSample;

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

            Log.OnMessage += (s, e) =>
            {
                status.Text += e;
            };
            new GetLibraryInfo().Start();

            button.Click += delegate {
                button.Enabled = false;
                if (currentSample != null)
                    currentSample.Stop();
                try
                {
                    currentSample = new AboutServiceTest();
                    currentSample.Start();
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

