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
    using Spekt.TestLogger.VSTest.Core;
    using DefaultLoggerParameterNames = Spekt.TestLogger.Core.DefaultLoggerParameterNames;
    using TestMessageLevel = Microsoft.VisualStudio.TestPlatform.ObjectModel.Logging.TestMessageLevel;

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
            Assert.ThrowsExactly<ArgumentNullException>(() => this.testRunBuilder.WithStore(null));
        }

        [TestMethod]
        public void WithSerializerShouldThrowForNullTestResultSerializer()
        {
            Assert.ThrowsExactly<ArgumentNullException>(() => this.testRunBuilder.WithSerializer(null));
        }

        [TestMethod]
        public void SubscribeShouldThrowForNullLoggerEvents()
        {
            var run = this.testRunBuilder.Build();
            Assert.ThrowsExactly<ArgumentNullException>(() => run.Subscribe(null));
        }

        [TestMethod]
        public void SubscribeShouldSetupTraceAndThrowExceptionForEvents()
        {
            var testEvents = new MockTestLoggerEvents();
            var consoleOutput = new FakeConsoleOutput();

            var run = this.testRunBuilder.WithConsoleOutput(consoleOutput).Build();
            run.Subscribe(testEvents);

            Assert.ThrowsExactly<NullReferenceException>(() => testEvents.RaiseTestRunMessage(TestMessageLevel.Error, "dummy message"));
            Assert.ThrowsExactly<NullReferenceException>(() => testEvents.RaiseTestResult(new Microsoft.VisualStudio.TestPlatform.ObjectModel.TestResult(new TestCase())));
            Assert.ThrowsExactly<NullReferenceException>(() => testEvents.RaiseTestRunComplete(null));
            Assert.AreEqual(3, consoleOutput.Messages.Count);
            Assert.IsTrue(consoleOutput.Messages.All(x => x.Item1 == "stderr"));
            StringAssert.Contains(consoleOutput.Messages[0].Item2, "Unexpected error in TestRunMessage workflow");
            StringAssert.Contains(consoleOutput.Messages[1].Item2, "Unexpected error in TestResult workflow");
            StringAssert.Contains(consoleOutput.Messages[2].Item2, "Unexpected error in TestRunComplete workflow");
        }

        [TestMethod]
        public void WithFileSystemShouldThrowForNullFileSystem()
        {
            Assert.ThrowsExactly<ArgumentNullException>(() => this.testRunBuilder.WithFileSystem(null));
        }

        [TestMethod]
        public void WithConsoleOutputShouldThrowForNullConsoleImplementation()
        {
            Assert.ThrowsExactly<ArgumentNullException>(() => this.testRunBuilder.WithConsoleOutput(null));
        }
    }
}