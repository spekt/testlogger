// Copyright (c) Spekt Contributors. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

namespace Spekt.TestLogger.UnitTests.Extensions
{
    using Microsoft.VisualStudio.TestTools.UnitTesting;
    using Spekt.TestLogger.Extensions;

    [TestClass]
    public class TestAdapterFactoryTests
    {
        [TestMethod]
        [DataRow("")]
        [DataRow(null)]
        [DataRow("executor://NotExitTestExecutor")]
        public void CreateTestAdapterShouldReturnDefaultAdapterIfExecutorUriIsInvalid(string executorUri)
        {
            var factory = new TestAdapterFactory();

            var adapter = factory.CreateTestAdapter(executorUri);

            Assert.IsTrue(adapter is DefaultTestAdapter);
        }

        [TestMethod]
        [DataRow("executor://NUnit3TestExecutor")]
        public void CreateTestAdapterShouldReturnNUnitAdapterForNUnitFramework(string executorUri)
        {
            var factory = new TestAdapterFactory();

            var adapter = factory.CreateTestAdapter(executorUri);

            Assert.IsTrue(adapter is NUnitTestAdapter);
        }

        [TestMethod]
        public void CreateTestAdapterShouldReturnXunitAdapterForXunitFramework()
        {
            var factory = new TestAdapterFactory();

            var adapter = factory.CreateTestAdapter("executor://xunit/VsTestRunner2/net");

            Assert.IsTrue(adapter is XunitTestAdapter);
        }

        [TestMethod]
        public void CreateTestAdapterShouldReturnMSTestAdapterForMSTest()
        {
            var factory = new TestAdapterFactory();

            var adapter = factory.CreateTestAdapter("executor://MSTestAdapter/v2");

            Assert.IsTrue(adapter is MSTestAdapter);
        }
    }
}