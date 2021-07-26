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
    public class MSTestAdapterTests
    {
        private const string Method = "Method";

        [TestMethod]
        public void TransformShouldOverwriteMethodWithNonEmptyValues()
        {
            var param = Guid.NewGuid().ToString();
            var result = new TestResult(new TestCase())
            {
                DisplayName = $"Method(\"{param}\")",
            };

            var testResults = new List<TestResultInfo>
            {
                new TestResultInfo(result, "namespace", "type", Method),
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
            var result = new TestResult(new TestCase())
            {
                DisplayName = displayName,
            };

            var testResults = new List<TestResultInfo>
            {
                new TestResultInfo(result, "namespace", "type", Method),
            };

            var sut = new MSTestAdapter();

            var transformedResults = sut.TransformResults(testResults, new List<TestMessageInfo>());

            Assert.AreEqual(1, transformedResults.Count);
            Assert.AreEqual(Method, transformedResults.Single().Method);
        }
    }
}
