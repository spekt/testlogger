// Copyright (c) Spekt Contributors. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

namespace Spekt.TestLogger.UnitTests
{
    using Microsoft.VisualStudio.TestPlatform.ObjectModel;
    using Microsoft.VisualStudio.TestTools.UnitTesting;
    using Spekt.TestLogger.UnitTests.Builders;
    using TestResult = Microsoft.VisualStudio.TestPlatform.ObjectModel.TestResult;

    [TestClass]
    public class TestResultInfoTests
    {
        [TestMethod]
        public void GetHashCodeShouldReturnAnUniqueHash()
        {
            var resultInfo = new TestResultInfoBuilder(string.Empty, string.Empty, string.Empty).Build();

            Assert.AreNotEqual(new TestResult(new TestCase()).GetHashCode(), resultInfo.GetHashCode());
        }

        [TestMethod]
        public void EqualsShouldReturnFalseForNonTestResultInfoObject()
        {
            var resultInfo = new TestResultInfoBuilder(string.Empty, string.Empty, string.Empty).Build();

            Assert.IsFalse(resultInfo.Equals(new ()));
        }

        [TestMethod]
        public void EqualsShouldReturnFalseIfErrorMessageOrStackTraceDoNotMatch()
        {
            var r1 = new TestResultInfoBuilder(string.Empty, string.Empty, string.Empty).WithErrorMessage("error 1").Build();
            var r2 = new TestResultInfoBuilder(string.Empty, string.Empty, string.Empty).WithErrorMessage("error 2").Build();

            Assert.IsFalse(r1.Equals(r2));
        }

        [TestMethod]
        public void EqualsShouldReturnTrueIfErrorMessageAndStackTraceMatch()
        {
            var result = new TestResult(new TestCase());
            var r1 = new TestResultInfoBuilder(string.Empty, string.Empty, string.Empty).Build();
            var r2 = new TestResultInfoBuilder("ns", "type", "method").Build();

            Assert.IsFalse(r1 == r2);
            Assert.IsTrue(Equals(r1, r2));
            Assert.IsTrue(r1.Equals(r2));
        }
    }
}