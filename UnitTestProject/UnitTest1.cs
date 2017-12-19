using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using TestLibCake;

namespace UnitTestProject
{
    [TestClass]
    public class UnitTest1
    {
        [TestMethod]
        public void TestMethod1()
        {
            var obj = new Class1();
            Assert.AreEqual(16, obj.Method(4));
            Assert.AreEqual(1, obj.Method(1));
            Assert.AreEqual(25, obj.Method(-5));
        }
    }
}
