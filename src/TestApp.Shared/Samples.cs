using System;
using System.Collections.Generic;
using System.Text;

namespace TestApp.Shared
{
    internal static class Samples
    {
        public static IEnumerable<Sample> GetSamples()
        {
            yield return new Sample()
            {
                CreateSample = () => { return new GetLibraryInfo(); },
                Name = "AllJoyn Library Info"
            };
            yield return new Sample() {
                CreateSample = () => { return new AboutServiceTest(); },
                Name = "About Client/Server"
            };
        }
    }
    internal class Sample
    {
        public Func<TestApp.Shared.ISample> CreateSample { get; set; }
        public string Name { get; set; }
    }
}
