// Copyright (c) Spekt Contributors. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

namespace Spekt.TestLogger.UnitTests
{
    using System;
    using System.Collections.Generic;
    using Microsoft.VisualStudio.TestPlatform.ObjectModel;
    using Microsoft.VisualStudio.TestTools.UnitTesting;

    using Spekt.TestLogger;
    using Spekt.TestLogger.Core;
    using Spekt.TestLogger.UnitTests.TestDoubles;

    [TestClass]
    public class TestLoggerTests
    {
        private readonly Dictionary<string, string> loggerParams;
        private readonly TestLogger logger;
        private readonly MockTestLoggerEvents mockEvents;
        private readonly string resultsPath = "/tmp/temp-result-dir";

        public TestLoggerTests()
        {
            this.logger = new TestableTestLogger();
            this.mockEvents = new MockTestLoggerEvents();
            this.loggerParams = new Dictionary<string, string> { { DefaultLoggerParameterNames.TestRunDirectory, this.resultsPath } };
        }

        [TestMethod]
        public void TestLoggerShouldCreateAbstractLoggerWithSerializer()
        {
            Assert.IsNotNull(new ValidTestLogger(new JsonTestResultSerializer()));
        }

        [TestMethod]
        public void TestLoggerWithNullFileSystemShouldThrowArgumentNullException()
        {
            Assert.ThrowsException<ArgumentNullException>(() => new TestLoggerWithNullFileSystem());
        }

        [TestMethod]
        public void TestLoggerWithNullConsoleOutputShouldThrowArgumentNullException()
        {
            Assert.ThrowsException<ArgumentNullException>(() => new TestLoggerWithNullConsoleOutput());
        }

        [TestMethod]
        public void TestLoggerWithNullResultStoreShouldThrowArgumentNullException()
        {
            Assert.ThrowsException<ArgumentNullException>(() => new TestLoggerWithNullResultStore());
        }

        [TestMethod]
        public void TestLoggerWithNullResultSerializerShouldThrowArgumentNullException()
        {
            Assert.ThrowsException<ArgumentNullException>(() => new TestLoggerWithNullResultSerializer());
        }

        [TestMethod]
        public void TestLoggerInitializeShouldThrowForNullEvents()
        {
            Assert.ThrowsException<ArgumentNullException>(() => this.logger.Initialize(null, string.Empty));
        }

        [TestMethod]
        public void TestLoggerInitializeShouldThrowForNullResultsDir()
        {
            Assert.ThrowsException<ArgumentNullException>(() => this.logger.Initialize(this.mockEvents, testResultsDirPath: null));
        }

        [TestMethod]
        public void TestLoggerInitializeShouldSubscribeToRunEvents()
        {
            this.logger.Initialize(this.mockEvents, this.resultsPath);

            Assert.IsTrue(this.mockEvents.TestRunEventsSubscribed());
            Assert.IsFalse(this.mockEvents.TestDiscoveryEventsSubscribed());
        }

        [TestMethod]
        public void TestLoggerInitializeWithParametersShouldThrowForNullEvents()
        {
            Assert.ThrowsException<ArgumentNullException>(() => this.logger.Initialize(null, new Dictionary<string, string>()));
        }

        [TestMethod]
        public void TestLoggerInitializeWithParametersShouldThrowForNullParameters()
        {
            Assert.ThrowsException<ArgumentNullException>(() => this.logger.Initialize(this.mockEvents, parameters: null));
        }

        [TestMethod]
        public void TestLoggerInitializeWithParametersShouldThrowIfOutputDirectoryIsNotSet()
        {
            Assert.ThrowsException<ArgumentException>(() => this.logger.Initialize(this.mockEvents, new Dictionary<string, string>()));
        }

        [TestMethod]
        public void TestLoggerInitializeWithParametersShouldThrowIfOutputDirectoryIsNull()
        {
            this.loggerParams[DefaultLoggerParameterNames.TestRunDirectory] = null;
            Assert.ThrowsException<ArgumentNullException>(() => this.logger.Initialize(this.mockEvents, this.loggerParams));
        }

        [TestMethod]
        public void TestLoggerInitializeWithParametersShouldThrowIfLogFilePathIsNull()
        {
            this.loggerParams[TestLogger.LogFilePathKey] = null;
            this.loggerParams[DefaultLoggerParameterNames.TestRunDirectory] = this.resultsPath;

            Assert.ThrowsException<ArgumentNullException>(() => this.logger.Initialize(this.mockEvents, this.loggerParams));
        }

        [TestMethod]
        public void TestLoggerInitializeWithParametersLogFilePathShouldSubscribeToRunEvents()
        {
            this.loggerParams[TestLogger.LogFilePathKey] = this.resultsPath;

            this.logger.Initialize(this.mockEvents, this.loggerParams);

            Assert.IsTrue(this.mockEvents.TestRunEventsSubscribed());
            Assert.IsFalse(this.mockEvents.TestDiscoveryEventsSubscribed());
        }

        [TestMethod]
        public void TestLoggerInitializeWithParametersShouldSubscribeToRunEvents()
        {
            this.logger.Initialize(this.mockEvents, this.loggerParams);

            Assert.IsTrue(this.mockEvents.TestRunEventsSubscribed());
            Assert.IsFalse(this.mockEvents.TestDiscoveryEventsSubscribed());
        }

        // [TestMethod]
        public void TestRunCompleteShouldCreateAResultFile()
        {
            var logger = new TestableTestLogger();
            using (var simulator = new TestRunSimulator(logger))
            {
                simulator.Run();
            }
        }

        // [TestMethod]
        public void TestRunShouldInvokeEventHandlers()
        {
            // TODO fix this dummy test with asserts
            var simulator = new TestRunSimulator(this.logger);

            simulator.Run();
        }

        private class ValidTestLogger : TestLogger
        {
            public ValidTestLogger(ITestResultSerializer serializer)
                : base(serializer)
            {
            }

            protected override string DefaultTestResultFile => "DummyResult.json";
        }

        private class TestLoggerWithNullConsoleOutput : TestLogger
        {
            public TestLoggerWithNullConsoleOutput()
                : base(new FakeFileSystem(), null, new TestResultStore(), new JsonTestResultSerializer())
            {
            }

            protected override string DefaultTestResultFile => "DummyResult.json";
        }

        private class TestLoggerWithNullFileSystem : TestLogger
        {
            public TestLoggerWithNullFileSystem()
                : base(null, new FakeConsoleOutput(), new TestResultStore(), new JsonTestResultSerializer())
            {
            }

            protected override string DefaultTestResultFile => "DummyResult.json";
        }

        private class TestLoggerWithNullResultStore : TestLogger
        {
            public TestLoggerWithNullResultStore()
                : base(new FakeFileSystem(), new FakeConsoleOutput(), null, new JsonTestResultSerializer())
            {
            }

            protected override string DefaultTestResultFile => "DummyResult.json";
        }

        private class TestLoggerWithNullResultSerializer : TestLogger
        {
            public TestLoggerWithNullResultSerializer()
                : base(new FakeFileSystem(), new FakeConsoleOutput(), new TestResultStore(), null)
            {
            }

            protected override string DefaultTestResultFile => "DummyResult.json";
        }
    }
}
