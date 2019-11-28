// Copyright (c) Spekt Contributors. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

namespace Spekt.TestLogger.UnitTests
{
    using System;
    using System.Collections.Generic;
    using System.Diagnostics;
    using System.IO;
    using System.Linq;
    using System.Xml.Linq;
    using System.Xml.XPath;
    using Json.TestLogger;
    using Microsoft.VisualStudio.TestPlatform.ObjectModel;
    using Microsoft.VisualStudio.TestPlatform.ObjectModel.Client;
    using Microsoft.VisualStudio.TestPlatform.ObjectModel.Logging;
    using Microsoft.VisualStudio.TestTools.UnitTesting;

    using Spekt.TestLogger;
    using Spekt.TestLogger.UnitTests.TestDoubles;

    [TestClass]
    public class TestLoggerTests
    {
        private JsonTestLogger logger;
        private DummyTestLoggerEvents dummyEvents;
        private string resultsPath = "/tmp/temp-result-dir";
        private Dictionary<string, string> loggerParams;

        public TestLoggerTests()
        {
            this.logger = new JsonTestLogger();
            this.dummyEvents = new DummyTestLoggerEvents();
            this.loggerParams = new Dictionary<string, string> { { DefaultLoggerParameterNames.TestRunDirectory, this.resultsPath } };
        }

        [TestMethod]
        public void TestLoggerInitializeShouldThrowForNullEvents()
        {
            Assert.ThrowsException<ArgumentNullException>(() => this.logger.Initialize(null, string.Empty));
        }

        [TestMethod]
        public void TestLoggerInitializeShouldThrowForNullResultsDir()
        {
            Assert.ThrowsException<ArgumentNullException>(() => this.logger.Initialize(this.dummyEvents, testResultsDirPath: null));
        }

        [TestMethod]
        public void TestLoggerInitializeShouldSubscribeToRunEvents()
        {
            this.logger.Initialize(this.dummyEvents, this.resultsPath);

            Assert.IsTrue(this.dummyEvents.TestRunEventsSubscribed());
            Assert.IsFalse(this.dummyEvents.TestDiscoveryEventsSubscribed());
        }

        [TestMethod]
        public void TestLoggerInitializeWithParametersShouldThrowForNullEvents()
        {
            Assert.ThrowsException<ArgumentNullException>(() => this.logger.Initialize(null, new Dictionary<string, string>()));
        }

        [TestMethod]
        public void TestLoggerInitializeWithParametersShouldThrowForNullParameters()
        {
            Assert.ThrowsException<ArgumentNullException>(() => this.logger.Initialize(this.dummyEvents, parameters: null));
        }

        [TestMethod]
        public void TestLoggerInitializeWithParametersShouldThrowIfOutputDirectoryIsNotSet()
        {
            Assert.ThrowsException<ArgumentException>(() => this.logger.Initialize(this.dummyEvents, new Dictionary<string, string>()));
        }

        [TestMethod]
        public void TestLoggerInitializeWithParametersShouldThrowIfOutputDirectoryIsNull()
        {
            this.loggerParams[DefaultLoggerParameterNames.TestRunDirectory] = null;
            Assert.ThrowsException<ArgumentNullException>(() => this.logger.Initialize(this.dummyEvents, this.loggerParams));
        }

        [TestMethod]
        public void TestLoggerInitializeWithParametersShouldThrowIfLogFilePathIsNull()
        {
            this.loggerParams[TestLogger.LogFilePathKey] = null;
            this.loggerParams[DefaultLoggerParameterNames.TestRunDirectory] = this.resultsPath;

            Assert.ThrowsException<ArgumentNullException>(() => this.logger.Initialize(this.dummyEvents, this.loggerParams));
        }

        [TestMethod]
        public void TestLoggerInitializeWithParametersShouldSubscribeToRunEvents()
        {
            this.logger.Initialize(this.dummyEvents, this.loggerParams);

            Assert.IsTrue(this.dummyEvents.TestRunEventsSubscribed());
            Assert.IsFalse(this.dummyEvents.TestDiscoveryEventsSubscribed());
        }

        [TestMethod]
        public void TestRunShouldInvokeEventHandlers()
        {
            // TODO fix this dummy test with asserts
            var simulator = new TestRunSimulator(this.logger);

            simulator.Run();
        }
    }
}
