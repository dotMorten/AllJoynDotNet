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
        public void SetBool()
        {
            MsgArg msg = new MsgArg();
            msg.Value = true;
            Assert.AreEqual(true, msg.Value);
            Assert.AreEqual("b", msg.Signature);
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
        public void SetDouble()
        {
            MsgArg msg = new MsgArg();
            msg.Value = 123.456;
            Assert.AreEqual(123.456, msg.Value);
            Assert.AreEqual("d", msg.Signature);
        }

        [TestMethod]
        public void SetInt32()
        {
            MsgArg msg = new MsgArg();
            msg.Value = (Int32)12345;
            Assert.AreEqual(12345, msg.Value);
            Assert.AreEqual("i", msg.Signature);
        }

        [TestMethod]
        public void SetUInt32()
        {
            MsgArg msg = new MsgArg();
            msg.Value = (UInt32)12345;
            Assert.AreEqual((uint)12345, msg.Value);
            Assert.AreEqual("u", msg.Signature);
        }

        [TestMethod]
        public void SetInt16()
        {
            MsgArg msg = new MsgArg();
            msg.Value = (Int16)12345;
            Assert.AreEqual((short)12345, msg.Value);
            Assert.AreEqual("n", msg.Signature);
        }

        [TestMethod]
        public void SetUInt16()
        {
            MsgArg msg = new MsgArg();
            msg.Value = (UInt16)12345;
            Assert.AreEqual((ushort)12345, msg.Value);
            Assert.AreEqual("q", msg.Signature);
        }

        [TestMethod]
        public void SetInt64()
        {
            MsgArg msg = new MsgArg();
            msg.Value = (Int64)12345;
            Assert.AreEqual((long)12345, msg.Value);
            Assert.AreEqual("x", msg.Signature);
        }

        [TestMethod]
        public void SetUInt64()
        {
            MsgArg msg = new MsgArg();
            msg.Value = (UInt64)12345;
            Assert.AreEqual((ulong)12345, msg.Value);
            Assert.AreEqual("t", msg.Signature);
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

        [TestMethod]
        public void SetInt32Array()
        {
            MsgArg msg = new MsgArg();
            msg.Value = new int[] { 1, 2, 3, 4, 5 };
            Assert.AreEqual("ai", msg.Signature);
            var v = msg.Value;
            Assert.IsNotNull(v);
            Assert.IsInstanceOfType(v, typeof(int[]));
            int[] arr = (int[])v;
            Assert.AreEqual(5, arr.Length);
        }
        [TestMethod]
        public void SetBoolArray()
        {
            MsgArg msg = new MsgArg();
            msg.Value = new bool[] { true, false, true };
            Assert.AreEqual("ab", msg.Signature);
            var v = msg.Value;
            Assert.IsNotNull(v);
            Assert.IsInstanceOfType(v, typeof(bool[]));
            bool[] arr = (bool[])v;
            Assert.AreEqual(3, arr.Length);
            Assert.IsTrue(arr[0]);
            Assert.IsFalse(arr[1]);
            Assert.IsTrue(arr[2]);
        }
    }
}
