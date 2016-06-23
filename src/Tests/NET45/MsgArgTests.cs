using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using AllJoynDotNet;

namespace UnitTests.NET45
{
    [TestClass]
    public class MsgArgTests
    {
        [TestMethod]
        public void SetString()
        {
            MsgArg msg = new MsgArg();
            msg.Value = "ABC";
            Assert.AreEqual("ABC", msg.Value);
            Assert.AreEqual("s", msg.Signature);
        }
        [TestMethod]
        public void SetByte()
        {
            MsgArg msg = new MsgArg();
            msg.Value = (byte)9;
            Assert.AreEqual((byte)9, msg.Value);
            Assert.AreEqual("y", msg.Signature);
        }

        [TestMethod]
        public void SetByteArray()
        {
            MsgArg msg = new MsgArg();
            msg.Value = new byte[] { 1, 2, 3, 4, 5 };
            Assert.AreEqual("ay", msg.Signature);
            var v = msg.Value;
            Assert.IsNotNull(v);
            Assert.IsInstanceOfType(v, typeof(byte[]));
            byte[] arr = (byte[])v;
            Assert.AreEqual(5, arr.Length);
        }
    }
}
