using AllJoynDotNet;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices;
using System.Runtime.InteropServices.WindowsRuntime;
using TestApp.Shared;
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
        ISample currentSample;
        public MainPage()
        {
            this.InitializeComponent();
            Log.OnMessage += Log_OnMessage;
            Log.WriteLine($"AllJoyn Library Version: {version.VersionString} ({version.Version})\nAllJoyn BuildInfo:{version.BuildInfo}");
            //currentSample = new CreateInterfaceTest();
            currentSample = new AboutServiceTest();
            currentSample.Start();
        }

        private void Log_OnMessage(object sender, string e)
        {
            var _ = Dispatcher.RunAsync(Windows.UI.Core.CoreDispatcherPriority.Normal, () =>
            {
                logger.Text += e;
            });
        }
    }
}
