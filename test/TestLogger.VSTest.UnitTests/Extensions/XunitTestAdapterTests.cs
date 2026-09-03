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
    using Spekt.TestLogger.UnitTests.Builders;
    using Spekt.TestLogger.VSTest.Extensions;
    using TestMessageLevel = Spekt.TestLogger.Core.TestMessageLevel;
    using TestOutcome = Spekt.TestLogger.Core.TestOutcome;
    using TestResult = Microsoft.VisualStudio.TestPlatform.ObjectModel.TestResult;
    using TestResultMessage = Spekt.TestLogger.Core.TestResultMessage;
    using Trait = Spekt.TestLogger.Core.Trait;

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
                new(TestMessageLevel.Informational, "[xUnit.net 00:00:00.6490537]     Other message"),
                new(TestMessageLevel.Informational, "[xUnit.net 00:00:00.6490557]     N.C.M2 [SKIP]"),
                new(TestMessageLevel.Informational, "[SKIP] Dummy reason"),
            };
            var xunit = new XunitTestAdapter();

            var transformedResults = xunit.TransformResults(results, messages);

            Assert.AreEqual(2, transformedResults.Count);
            Assert.AreEqual("skipReason", transformedResults[1].Messages[0].Category);
            Assert.AreEqual("Dummy reason", transformedResults[1].Messages[0].Text);
        }

        [TestMethod]
        public void TransformShouldAddReasonFromResultMessageForSkippedTests()
        {
            var result = new TestResultInfoBuilder("N", "C", "M1")
                .WithOutcome(TestOutcome.Skipped)
                .WithDisplayName("N.C.M1")
                .Build();
            result.Messages.Add(new TestResultMessage(TestResultMessage.StandardOutCategory, "Skipped"));
            var xunit = new XunitTestAdapter();

            var transformedResults = xunit.TransformResults(new List<TestResultInfo> { result }, new List<TestMessageInfo>());

            var reasons = transformedResults[0].Messages.Where(m => m.Category == "skipReason").ToList();
            Assert.AreEqual(1, reasons.Count);
            Assert.AreEqual("Skipped", reasons[0].Text);
        }

        [TestMethod]
        public void TransformShouldNotDuplicateReasonWhenBothSourcesMatch()
        {
            var result = new TestResultInfoBuilder("N", "C", "M2")
                .WithOutcome(TestOutcome.Skipped)
                .WithDisplayName("N.C.M2")
                .Build();
            result.Messages.Add(new TestResultMessage(TestResultMessage.StandardOutCategory, "Skipped"));
            var messages = new List<TestMessageInfo>
            {
                new(TestMessageLevel.Informational, "[xUnit.net 00:00:00.6490557]     N.C.M2 [SKIP]"),
                new(TestMessageLevel.Informational, "[SKIP] Dummy reason"),
            };
            var xunit = new XunitTestAdapter();

            var transformedResults = xunit.TransformResults(new List<TestResultInfo> { result }, messages);

            var reasons = transformedResults[0].Messages.Where(m => m.Category == "skipReason").ToList();
            Assert.AreEqual(1, reasons.Count);
            Assert.AreEqual("Dummy reason", reasons[0].Text);
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

        [TestMethod]
        public void TransformResultShouldAddProperties()
        {
            var builder = new TestResultInfoBuilder("N", "C", "M1")
                .WithOutcome(TestOutcome.Passed)
                .WithTraits([new Trait("traitKey", "traitVal")])
                .WithProperty("Xunit.Trait", new string[] { "key", "val" });

            var results = new List<TestResultInfo> { builder.Build() };

            var messages = new List<TestMessageInfo>();
            var xunit = new XunitTestAdapter();

            var transformedResults = xunit.TransformResults(results, messages, builder.TestCase);
            Assert.AreEqual(1, transformedResults.Count);
            Assert.AreEqual(1, transformedResults.Count(x => x.Method == "M1"));

            var parsedProperty = transformedResults[0].Properties.First();
            Assert.AreEqual("CustomProperty", parsedProperty.Key);
            CollectionAssert.AreEquivalent(parsedProperty.Value as string[], new[] { "key", "val" });
        }
    }
}