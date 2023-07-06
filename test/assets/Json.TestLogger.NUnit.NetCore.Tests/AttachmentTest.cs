using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
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
            var filePath = Path.Combine(Path.GetTempPath(), "x.txt");
            if (!File.Exists(filePath))
            {
                File.Create(filePath);
            }
            TestContext.AddTestAttachment(filePath, "x");
        }
    }
}
