// Copyright (c) Spekt Contributors. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

namespace Spekt.TestLogger.UnitTests
{
    using System;
    using Microsoft.VisualStudio.TestPlatform.ObjectModel.Logging;
    using Microsoft.VisualStudio.TestTools.UnitTesting;
    using Spekt.TestLogger.Core;

    [TestClass]
    public class TestRunMessageWorkflowTests
    {
        [TestMethod]
        public void MessageShouldThrowNotImplementedException()
        {
            var testRun = new TestRun();
            var messageEvent = new TestRunMessageEventArgs(TestMessageLevel.Informational, "Dummy message");
            Assert.ThrowsException<NotImplementedException>(() => testRun.Message(messageEvent));
        }
    }
}