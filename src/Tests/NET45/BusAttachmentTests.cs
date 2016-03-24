using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using AllJoynDotNet;

namespace UnitTests.NET45
{
    [TestClass]
    public class BusAttachmentTests
    {
        [TestMethod]
        public void StartStopBusAttachment()
        {
            var bus = new BusAttachment("ServiceTest", true);
            Assert.IsFalse(bus.IsStarted);
            Assert.IsFalse(bus.IsConnected);

            bus.Start();
            Assert.IsTrue(bus.IsStarted);
            Assert.IsFalse(bus.IsConnected);

            bus.Connect();
            Assert.IsTrue(bus.IsConnected);
            Assert.IsFalse(bus.IsStopping);

            bus.Stop();
            Assert.IsTrue(bus.IsStopping);
            Assert.IsTrue(bus.IsStarted);

            bus.Join();
            Assert.IsFalse(bus.IsStarted);
            Assert.IsFalse(bus.IsConnected);

            bus.Dispose();
        }
    }
}
