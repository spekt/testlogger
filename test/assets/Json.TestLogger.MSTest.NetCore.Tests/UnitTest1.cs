using System;
using System.Threading.Tasks;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace NUnit.Xml.TestLogger.NetFull.Tests
{
    [TestClass]
    public class UnitTest1
    {
        [TestMethod]
        public void ExampleFailure()
        {
            Assert.Fail();
        }
    }
}