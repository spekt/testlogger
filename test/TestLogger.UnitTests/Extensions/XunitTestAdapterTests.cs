// Copyright (c) Spekt Contributors. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

namespace Spekt.TestLogger.UnitTests.Extensions
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using Microsoft.VisualStudio.TestPlatform.ObjectModel;
    using Microsoft.VisualStudio.TestPlatform.ObjectModel.Logging;
    using Microsoft.VisualStudio.TestTools.UnitTesting;
    using Spekt.TestLogger.Core;
    using Spekt.TestLogger.Extensions;
    using TestResult = Microsoft.VisualStudio.TestPlatform.ObjectModel.TestResult;

    [TestClass]
    public class XunitTestAdapterTests
    {
        private const string Source = "/tmp/test.dll";
        private static readonly Uri ExecutorUri = new ("executor://dummy");

        [TestMethod]
        public void TransformShouldAddReasonForSkippedTests()
        {
            var results = new List<TestResultInfo>
            {
                new (new TestResult(
                    new TestCase("N.C.M1", ExecutorUri, Source)),
                    "N", "C", "M1"),
                new (new TestResult(
                    new TestCase("N.C.M2", ExecutorUri, Source)),
                    "N", "C", "M2")
            };
            var messages = new List<TestMessageInfo>
            {
                new () { Level = TestMessageLevel.Informational, Message = "[xUnit.net 00:00:00.6490537]     Other message" },
                new () { Level = TestMessageLevel.Informational, Message = "[xUnit.net 00:00:00.6490557]     N.C.M2 [SKIP]" },
                new () { Level = TestMessageLevel.Informational, Message = "[SKIP] Dummy reason" }
            };
            var xunit = new XunitTestAdapter();

            var transformedResults = xunit.TransformResults(results, messages);

            Assert.AreEqual(2, transformedResults.Count);
            Assert.AreEqual("skipReason", transformedResults[1].Messages[0].Category);
            Assert.AreEqual("Dummy reason", transformedResults[1].Messages[0].Text);
        }

        [TestMethod]
        public void TransformShouldAddParameterData()
        {
            var results = new List<TestResultInfo>
            {
                new (new TestResult(
                    new TestCase("N.C.M1", ExecutorUri, Source) { DisplayName = "N.C.M1" }),
                    "N", "C", "M1"),
                new (new TestResult(
                    new TestCase("N.C.M2", ExecutorUri, Source) { DisplayName = "N.C.M2(some args)" }),
                    "N", "C", "M2")
            };
            var messages = new List<TestMessageInfo>();
            var xunit = new XunitTestAdapter();

            var transformedResults = xunit.TransformResults(results, messages);

            Assert.AreEqual(2, transformedResults.Count);
            Assert.AreEqual(1, transformedResults.Count(x => x.Method == "M1"));
            Assert.AreEqual(1, transformedResults.Count(x => x.Method == "M2(some args)"));
        }
    }
}