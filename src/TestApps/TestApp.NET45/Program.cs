using AllJoynDotNet;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TestApp.Shared;

namespace TestApp.NET45
{
    class Program
    {
        static void Main(string[] args)
        {
            Log.OnMessage += Log_OnMessage;
            var samples = Samples.GetSamples().ToArray();
            RunSample(samples);
        }
        private static void RunSample(Sample[] samples)
        {
            Console.Clear();
            Console.WriteLine("*********************************************************************************");
            Console.WriteLine("* Select a sample by entering the sample number                                 *");
            Console.WriteLine("*********************************************************************************");
            for (int i = 0; i < samples.Length; i++)
            {
                Console.WriteLine($"\t{i+1}\t{samples[i].Name}");
            }
            //new GetLibraryInfo().Start();
            //var sample = new Shared.AboutServiceTest();
            //sample.Start();
            var key = Console.ReadKey();
            var sampleId = (int)key.KeyChar - 49;
            if(sampleId>=0 && sampleId<samples.Length)
            {
                var sample = samples[sampleId].CreateSample();
                Console.Clear();
                Console.WriteLine("*********************************************************************************");
                Console.WriteLine($"* {samples[sampleId].Name}");
                Console.WriteLine("*********************************************************************************");
                sample.Start();
                Console.ReadKey();
                sample.Stop();
                RunSample(samples);
            }
        }

        private static void Log_OnMessage(object sender, string e)
        {
            Console.Write(e);
        }
    }
}
