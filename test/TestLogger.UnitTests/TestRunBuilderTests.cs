// Copyright (c) Spekt Contributors. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

namespace Spekt.TestLogger.UnitTests
{
    using System;
    using Microsoft.VisualStudio.TestPlatform.ObjectModel;
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
        public void TestRunBuilderShouldCreateDefaultRunConfiguration()
        {
            var run = this.testRunBuilder.Build();

            Assert.IsNotNull(run.RunConfiguration);
        }

        [TestMethod]
        public void TestRunBuilderShouldCreateDefaultTestAdapterFactory()
        {
            var run = this.testRunBuilder.Build();

            Assert.IsNotNull(run.AdapterFactory);
        }

        [TestMethod]
        public void WithLoggerConfigurationShouldSetTestLoggerConfiguration()
        {
            var config = new LoggerConfiguration(new()
            {
                { LoggerConfiguration.LogFilePathKey, "/tmp/results.json" },
                { DefaultLoggerParameterNames.TestRunDirectory, "/tmp" }
            });

            var run = this.testRunBuilder.WithLoggerConfiguration(config).Build();

            Assert.AreSame(config, run.LoggerConfiguration);
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