// Copyright (c) Spekt Contributors. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

namespace Spekt.TestLogger.UnitTests
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using Microsoft.VisualStudio.TestPlatform.ObjectModel;
    using Microsoft.VisualStudio.TestPlatform.ObjectModel.Client;
    using Microsoft.VisualStudio.TestTools.UnitTesting;
    using Spekt.TestLogger.Core;

    [TestClass]
    public class TestRunStartWorkflowTests
    {
        [TestMethod]
        public void StartShouldThrowNotImplementedException()
        {
            var testRun = new TestRun();
            var tests = new List<TestCase> { new TestCase() };
            var criteria = new TestRunCriteria(tests: tests, frequencyOfRunStatsChangeEvent: 10);
            var testRunStartEvent = new TestRunStartEventArgs(criteria);
            Assert.ThrowsException<NotImplementedException>(() => testRun.Start(testRunStartEvent));
        }
    }
}