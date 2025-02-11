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
    public class MSTestAdapterTests
    {
        private const string Method = "Method";

        [TestMethod]
        public void TransformShouldOverwriteMethodWithNonEmptyValues()
        {
            var param = Guid.NewGuid().ToString();

            var testResults = new List<TestResultInfo>
            {
                new TestResultInfoBuilder("namespace", "type", Method).WithDisplayName($"Method(\"{param}\")").Build(),
            };

            var sut = new MSTestAdapter();

            var transformedResults = sut.TransformResults(testResults, new List<TestMessageInfo>());

            Assert.AreEqual(1, transformedResults.Count);
            Assert.IsTrue(transformedResults.Single().Method.Contains(param));
        }

        [DataTestMethod]
        [DataRow(null)]
        [DataRow("")]
        [DataRow("    ")]
        public void TransformShouldNoitOverwriteMethodEmptyValues(string displayName)
        {
            var testResults = new List<TestResultInfo>
            {
                new TestResultInfoBuilder("namespace", "type", Method).WithDisplayName(displayName).Build(),
            };

            var sut = new MSTestAdapter();

            var transformedResults = sut.TransformResults(testResults, new List<TestMessageInfo>());

            Assert.AreEqual(1, transformedResults.Count);
            Assert.AreEqual(Method, transformedResults.Single().Method);
        }

        [TestMethod]
        public void TransformResultShouldAddProperties()
        {
            var testResults = new List<TestResultInfo>
            {
                new TestResultInfoBuilder("namespace", "type", Method)
                    .WithProperty("Microsoft.VisualStudio.TestTools.UnitTesting.TestContext.TestProperty", new[] { "c1", "c2" })
                    .Build(),
            };

            var sut = new MSTestAdapter();

            var transformedResults = sut.TransformResults(testResults, new List<TestMessageInfo>());

            Assert.AreEqual(1, transformedResults.Count);
            Assert.AreEqual(1, transformedResults[0].Properties.Count);
            Assert.AreEqual(Method, transformedResults.Single().Method);
            CollectionAssert.AreEquivalent(new[] { "c1", "c2" }, (string[])transformedResults[0].Properties.Single(p => p.Key == "CustomProperty").Value);
        }
    }
}
