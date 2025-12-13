using System;
using System.Threading.Tasks;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace NUnit.Xml.TestLogger.Tests2
{
    [TestClass]
    public class AttachmentTest
    {
        public TestContext TestContext { get; set; }

        [TestMethod]
        public void TestAddAttachment()
        {
            TestContext.AddResultFile("/tmp/x.txt");
        }
    }
}
