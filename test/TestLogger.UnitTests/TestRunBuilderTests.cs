// Copyright (c) Spekt Contributors. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

namespace Spekt.TestLogger.UnitTests
{
    using System;
    using Microsoft.VisualStudio.TestTools.UnitTesting;
    using Spekt.TestLogger.Core;

    [TestClass]
    public class TestRunBuilderTests
    {
        private readonly ITestRunBuilder testRunBuilder;

        public TestRunBuilderTests()
        {
            this.testRunBuilder = new TestRunBuilder();
        }

        [TestMethod]
        public void WithStoreShouldThrowForNullTestResultStore()
        {
            Assert.ThrowsException<ArgumentNullException>(() => this.testRunBuilder.WithStore(null));
        }

        [TestMethod]
        public void WithSerializerShouldThrowForNullTestResultSerializer()
        {
            Assert.ThrowsException<ArgumentNullException>(() => this.testRunBuilder.WithSerializer(null));
        }

        [DataTestMethod]
        [DataRow("")]
        [DataRow(null)]
        public void WithResultFileShouldThrowForNullOrEmptyFileName(string fileName)
        {
            Assert.ThrowsException<ArgumentNullException>(() => this.testRunBuilder.WithResultFile(fileName));
        }

        [TestMethod]
        public void SubscribeShouldThrowForNullLoggerEvents()
        {
            Assert.ThrowsException<ArgumentNullException>(() => this.testRunBuilder.Subscribe(null));
        }

        [TestMethod]
        public void WithFileSystemShouldThrowForNullFileSystem()
        {
            Assert.ThrowsException<ArgumentNullException>(() => this.testRunBuilder.WithFileSystem(null));
        }

        [TestMethod]
        public void WithConsoleOutputShouldThrowForNullConsoleImplementation()
        {
            Assert.ThrowsException<ArgumentNullException>(() => this.testRunBuilder.WithConsoleOutput(null));
        }
    }
}