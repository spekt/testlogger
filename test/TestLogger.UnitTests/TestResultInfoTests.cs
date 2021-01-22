// Copyright (c) Spekt Contributors. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

namespace Spekt.TestLogger.UnitTests
{
    using Microsoft.VisualStudio.TestPlatform.ObjectModel;
    using Microsoft.VisualStudio.TestTools.UnitTesting;
    using Spekt.TestLogger.Core;
    using TestResult = Microsoft.VisualStudio.TestPlatform.ObjectModel.TestResult;

    [TestClass]
    public class TestResultInfoTests
    {
        [TestMethod]
        public void GetHashCodeShouldReturnAnUniqueHash()
        {
            var result = new TestResult(new TestCase());
            var resultInfo = new TestResultInfo(result, string.Empty, string.Empty, string.Empty);

            Assert.AreNotEqual(new TestResult(new TestCase()).GetHashCode(), resultInfo.GetHashCode());
        }

        [TestMethod]
        public void EqualsShouldReturnFalseForNonTestResultInfoObject()
        {
            var result = new TestResult(new TestCase());
            var resultInfo = new TestResultInfo(result, string.Empty, string.Empty, string.Empty);

            Assert.IsFalse(resultInfo.Equals(new ()));
        }

        [TestMethod]
        public void EqualsShouldReturnFalseIfErrorMessageOrStackTraceDoNotMatch()
        {
            var result1 = new TestResult(new TestCase()) { ErrorMessage = "error 1" };
            var result2 = new TestResult(new TestCase()) { ErrorMessage = "error 2" };
            var r1 = new TestResultInfo(result1, string.Empty, string.Empty, string.Empty);
            var r2 = new TestResultInfo(result2, string.Empty, string.Empty, string.Empty);

            Assert.IsFalse(r1.Equals(r2));
        }

        [TestMethod]
        public void EqualsShouldReturnTrueIfErrorMessageAndStackTraceMatch()
        {
            var result = new TestResult(new TestCase());
            var r1 = new TestResultInfo(result, string.Empty, string.Empty, string.Empty);
            var r2 = new TestResultInfo(result, "ns", "type", "method");

            Assert.IsFalse(r1 == r2);
            Assert.IsTrue(Equals(r1, r2));
            Assert.IsTrue(r1.Equals(r2));
        }
    }
}