// Copyright (c) Spekt Contributors. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

namespace Spekt.TestLogger.UnitTests
{
    using System;
    using System.Diagnostics;
    using System.IO;
    using System.Linq;
    using System.Xml.Linq;
    using System.Xml.XPath;
    using Microsoft.VisualStudio.TestTools.UnitTesting;
    using Spekt.TestLogger;

    [TestClass]
    public class TestLoggerTests
    {
        [TestMethod]
        public void TestLoggerInitializeShouldThrowForNullEvents()
        {
            var logger = new TestLogger();

            Assert.ThrowsException<ArgumentNullException>(() => logger.Initialize(null, string.Empty));
        }
    }
}
