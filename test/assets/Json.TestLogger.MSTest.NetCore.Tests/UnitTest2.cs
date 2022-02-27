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
    }
}
