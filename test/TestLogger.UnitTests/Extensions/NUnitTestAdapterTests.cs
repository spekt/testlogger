// Copyright (c) Spekt Contributors. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

namespace Spekt.TestLogger.UnitTests.Extensions
{
    using System.Collections.Generic;
    using Microsoft.VisualStudio.TestPlatform.ObjectModel;
    using Microsoft.VisualStudio.TestTools.UnitTesting;
    using Spekt.TestLogger.Core;
    using Spekt.TestLogger.Extensions;

    [TestClass]
    public class NUnitTestAdapterTests
    {
        private const string DummyNamespace = "DummyNamespace";
        private const string DummyType = "DummyType";
        private const string DummyMethod = "DummyMethod";
        private readonly NUnitTestAdapter adapter;
        private readonly TestCase dummyTestCase;
        private readonly TestCase explicitTestCase;
        private readonly TestResultInfo passTestResultInfo;
        private readonly TestResultInfo failTestResultInfo;

        public NUnitTestAdapterTests()
        {
            this.adapter = new NUnitTestAdapter();
            this.dummyTestCase = new TestCase();
            this.explicitTestCase = new TestCase();
            this.explicitTestCase.Traits.Add("Explicit", string.Empty);

            var passTestResult = new Microsoft.VisualStudio.TestPlatform.ObjectModel.TestResult(this.dummyTestCase) { Outcome = TestOutcome.Passed };
            this.passTestResultInfo = new TestResultInfo(passTestResult, DummyNamespace, DummyType, DummyMethod);

            var failTestResult = new Microsoft.VisualStudio.TestPlatform.ObjectModel.TestResult(this.dummyTestCase) { Outcome = TestOutcome.Failed };
            this.failTestResultInfo = new TestResultInfo(failTestResult, DummyNamespace, DummyType, DummyMethod);
        }

        [DataTestMethod]
        [DataRow(TestOutcome.Passed)]
        [DataRow(TestOutcome.Skipped)]
        [DataRow(TestOutcome.Failed)]
        [DataRow(TestOutcome.NotFound)]
        public void TransformResultsShouldNotModifyNonInclusiveTests(TestOutcome outcome)
        {
            var result = new Microsoft.VisualStudio.TestPlatform.ObjectModel.TestResult(this.dummyTestCase) { Outcome = outcome };
            var explicitResult = new Microsoft.VisualStudio.TestPlatform.ObjectModel.TestResult(this.explicitTestCase) { Outcome = outcome };
            var results = new List<TestResultInfo>
            {
                new TestResultInfo(result, DummyNamespace, DummyType, DummyMethod),
                new TestResultInfo(explicitResult, DummyNamespace, DummyType, DummyMethod)
            };

            var modifiedResults = this.adapter.TransformResults(results, new ());

            Assert.AreEqual(2, modifiedResults.Count);
            Assert.AreEqual(outcome, modifiedResults[0].Outcome);
            Assert.AreEqual(outcome, modifiedResults[1].Outcome);
        }

        [TestMethod]
        public void TransformResultsShouldNotModifyTestWithoutExplicitAttribute()
        {
            var results = new List<TestResultInfo> { this.passTestResultInfo, this.failTestResultInfo };

            var modifiedResults = this.adapter.TransformResults(results, new ());

            Assert.AreEqual(2, modifiedResults.Count);
            Assert.AreEqual(TestOutcome.Passed, modifiedResults[0].Outcome);
            Assert.AreEqual(TestOutcome.Failed, modifiedResults[1].Outcome);
        }

        [TestMethod]
        public void TransformResultShouldModifyTestWithExplicitAttributeAndNoOutcome()
        {
            var explicitResult = new Microsoft.VisualStudio.TestPlatform.ObjectModel.TestResult(this.explicitTestCase) { Outcome = TestOutcome.None };
            var results = new List<TestResultInfo>
            {
                new TestResultInfo(explicitResult, DummyNamespace, DummyType, DummyMethod)
            };

            var modifiedResults = this.adapter.TransformResults(results, new ());

            Assert.AreEqual(1, modifiedResults.Count);
            Assert.AreEqual(TestOutcome.Skipped, modifiedResults[0].Outcome);
        }
    }
}
