using System;
using TestLibCake;
using NUnit.Framework;

namespace UnitTestProject
{
    [TestFixture]
    public class UnitTest1
    {
        [Test]
        public void TestMethod1()
        {
            var obj = new Class1();
            Assert.AreEqual(16, obj.Method(4));
            Assert.AreEqual(1, obj.Method(1));
            Assert.AreEqual(26, obj.Method(-5));
        }
    }
}
