// Copyright (c) Spekt Contributors. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

namespace Spekt.TestLogger.UnitTests
{
    using Microsoft.VisualStudio.TestPlatform.ObjectModel.Logging;
    using Microsoft.VisualStudio.TestTools.UnitTesting;
    using Spekt.TestLogger.Core;

    [TestClass]
    public class TestRunMessageWorkflowTests
    {
        private readonly TestResultStore store;

        public TestRunMessageWorkflowTests()
        {
            this.store = new TestResultStore();
        }

        [TestMethod]
        public void MessageShouldStoreRunMessages()
        {
            var testRun = new TestRunBuilder().WithStore(this.store).Build();
            var messageEvent = new TestRunMessageEventArgs(
                    TestMessageLevel.Informational,
                    "Dummy message");

            testRun.Message(messageEvent);

            testRun.Store.Pop(out _, out var messages);
            Assert.AreEqual(1, messages.Count);
            Assert.AreEqual(TestMessageLevel.Informational, messages[0].Level);
            Assert.AreEqual("Dummy message", messages[0].Message);
        }
    }
}