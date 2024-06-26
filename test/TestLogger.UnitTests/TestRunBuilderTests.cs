// Copyright (c) Spekt Contributors. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

namespace Spekt.TestLogger.UnitTests
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using Microsoft.VisualStudio.TestPlatform.ObjectModel;
    using Microsoft.VisualStudio.TestPlatform.ObjectModel.Client;
    using Microsoft.VisualStudio.TestPlatform.ObjectModel.Logging;
    using Microsoft.VisualStudio.TestTools.UnitTesting;
    using Spekt.TestLogger.Core;
    using Spekt.TestLogger.UnitTests.TestDoubles;

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
            var config = new LoggerConfiguration(new Dictionary<string, string>()
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
        public void SubscribeShouldSetupTraceAndThrowExceptionForEvents()
        {
            var testEvents = new MockTestLoggerEvents();
            var consoleOutput = new FakeConsoleOutput();

            this.testRunBuilder.WithConsoleOutput(consoleOutput).Subscribe(testEvents);

            Assert.ThrowsException<NullReferenceException>(() => testEvents.RaiseTestRunMessage(TestMessageLevel.Error, "dummy message"));
            Assert.ThrowsException<NullReferenceException>(() => testEvents.RaiseTestResult(new Microsoft.VisualStudio.TestPlatform.ObjectModel.TestResult(new TestCase())));
            Assert.ThrowsException<NullReferenceException>(() => testEvents.RaiseTestRunComplete(null));
            Assert.AreEqual(3, consoleOutput.Messages.Count);
            Assert.IsTrue(consoleOutput.Messages.All(x => x.Item1 == "stderr"));
            StringAssert.Contains(consoleOutput.Messages[0].Item2, "Unexpected error in TestRunMessage workflow");
            StringAssert.Contains(consoleOutput.Messages[1].Item2, "Unexpected error in TestResult workflow");
            StringAssert.Contains(consoleOutput.Messages[2].Item2, "Unexpected error in TestRunComplete workflow");
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