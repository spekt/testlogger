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
    using Spekt.TestLogger.UnitTests.Builders;
    using TestResult = Microsoft.VisualStudio.TestPlatform.ObjectModel.TestResult;

    [TestClass]
    public class XunitTestAdapterTests
    {
        [TestMethod]
        public void TransformShouldAddReasonForSkippedTests()
        {
            var results = new List<TestResultInfo>
            {
                new TestResultInfoBuilder("N", "C", "M1").WithDisplayName("N.C.M1").Build(),
                new TestResultInfoBuilder("N", "C", "M2").WithDisplayName("N.C.M2").Build(),
            };
            var messages = new List<TestMessageInfo>
            {
                new (TestMessageLevel.Informational, "[xUnit.net 00:00:00.6490537]     Other message"),
                new (TestMessageLevel.Informational, "[xUnit.net 00:00:00.6490557]     N.C.M2 [SKIP]"),
                new (TestMessageLevel.Informational, "[SKIP] Dummy reason"),
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
                new TestResultInfoBuilder("N", "C", "M1").WithDisplayName("N.C.M1").Build(),
                new TestResultInfoBuilder("N", "C", "M2").WithDisplayName("N.C.M2(some args)").Build(),
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