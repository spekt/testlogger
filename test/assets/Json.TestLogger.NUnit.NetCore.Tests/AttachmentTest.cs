using System;
using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;
using NUnit.Framework;

namespace NUnit.Xml.TestLogger.NetFull.Tests
{
    [TestFixture]
    public class AttachmentTest
    {
        [Test]
        public void TestAddAttachment()
        {
            TestContext.AddTestAttachment("/tmp/x.txt", "/tmp/x.txt");
        }
    }
}
