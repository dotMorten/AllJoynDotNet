using AllJoynDotNet;
using System;
using System.Collections.Generic;
using System.Text;

namespace TestApp.Shared
{
    class GetLibraryInfo : ISample
    {
        public void Start()
        {
            Log.WriteLine($"AllJoyn Library Version: {version.VersionString} ({version.Version})");
            Log.WriteLine($"AllJoyn BuildInfo:{version.BuildInfo}");
            Log.WriteLine("******************");
        }

        public void Stop()
        {
        }
    }
}
