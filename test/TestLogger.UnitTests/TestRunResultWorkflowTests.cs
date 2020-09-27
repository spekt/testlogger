// Copyright (c) Spekt Contributors. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

namespace Spekt.TestLogger.UnitTests
{
    using System;
    using Microsoft.VisualStudio.TestPlatform.ObjectModel;
    using Microsoft.VisualStudio.TestPlatform.ObjectModel.Logging;
    using Microsoft.VisualStudio.TestTools.UnitTesting;
    using Spekt.TestLogger.Core;
    using TestResult = Microsoft.VisualStudio.TestPlatform.ObjectModel.TestResult;

    [TestClass]
    public class TestRunResultWorkflowTests
    {
        [TestMethod]
        public void ResultShouldThrowNotImplementedException()
        {
            var testRun = new TestRunBuilder().WithStore(new TestResultStore()).Build();
            var resultEvent = new TestResultEventArgs(new TestResult(new TestCase()));
            Assert.ThrowsException<NotImplementedException>(() => testRun.Result(resultEvent));
        }
    }
}