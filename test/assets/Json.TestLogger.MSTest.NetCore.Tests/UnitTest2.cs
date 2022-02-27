using System;
using System.Threading.Tasks;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace NUnit.Xml.TestLogger.Tests2
{
    [TestClass]
    public class UnitTest2
    {
        [TestMethod]
        public void ExampleFailure()
        {
            Assert.Fail();
        }

        [TestMethod]
        [DataRow(-1)]
        [DataRow(0)]
        [DataRow(1)]
        public void TestMethodDataRow(int value)
        {
            Assert.AreEqual(0,value);
        }

        [DataTestMethod]
        [DataRow(-1)]
        [DataRow(0)]
        [DataRow(1)]
        public void DataTestMethodDataRow(int value)
        {
            Assert.AreEqual(0, value);
        }
    }
}
