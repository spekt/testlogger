// Copyright (c) Spekt Contributors. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

namespace Spekt.TestLogger.UnitTests.Extensions
{
    using System.Collections.Generic;
    using Microsoft.VisualStudio.TestPlatform.ObjectModel;
    using Microsoft.VisualStudio.TestTools.UnitTesting;
    using Spekt.TestLogger.Core;
    using Spekt.TestLogger.Extensions;
    using Spekt.TestLogger.UnitTests.Builders;

    [TestClass]
    public class NUnitTestAdapterTests
    {
        private const string DummyNamespace = "DummyNamespace";
        private const string DummyType = "DummyType";
        private const string DummyMethod = "DummyMethod";
        private readonly NUnitTestAdapter adapter;
        private readonly TestResultInfo passTestResultInfo;
        private readonly TestResultInfo failTestResultInfo;
        private List<Trait> explicitTraits = new List<Trait>();

        public NUnitTestAdapterTests()
        {
            this.adapter = new NUnitTestAdapter();
            this.explicitTraits.Add(new Trait("Explicit", string.Empty));

            this.passTestResultInfo = new TestResultInfoBuilder(DummyNamespace, DummyType, DummyMethod).WithOutcome(TestOutcome.Passed).Build();
            this.failTestResultInfo = new TestResultInfoBuilder(DummyNamespace, DummyType, DummyMethod).WithOutcome(TestOutcome.Failed).Build();
        }

        [DataTestMethod]
        [DataRow(TestOutcome.Passed)]
        [DataRow(TestOutcome.Skipped)]
        [DataRow(TestOutcome.Failed)]
        [DataRow(TestOutcome.NotFound)]
        public void TransformResultsShouldNotModifyNonInclusiveTests(TestOutcome outcome)
        {
            var results = new List<TestResultInfo>
            {
                new TestResultInfoBuilder(DummyNamespace, DummyType, DummyMethod).WithOutcome(outcome).Build(),
                new TestResultInfoBuilder(DummyNamespace, DummyType, DummyMethod).WithOutcome(outcome).WithTraits(this.explicitTraits).Build(),
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
            var results = new List<TestResultInfo>
            {
                new TestResultInfoBuilder(DummyNamespace, DummyType, DummyMethod).WithOutcome(TestOutcome.None).Build(),
            };

            var modifiedResults = this.adapter.TransformResults(results, new ());

            Assert.AreEqual(1, modifiedResults.Count);
            Assert.AreEqual(TestOutcome.Skipped, modifiedResults[0].Outcome);
        }
    }
}
