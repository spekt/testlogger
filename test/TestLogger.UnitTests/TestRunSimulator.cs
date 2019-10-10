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

    using Spekt.TestLogger;
    using Spekt.TestLogger.UnitTests.TestDoubles;

    public class TestRunSimulator
    {
        public static readonly string TestResultsDirectory = "/tmp/temp-results-dir";

        private TestLogger logger;
        private DummyTestLoggerEvents loggerEvents;

        public TestRunSimulator(TestLogger logger)
        {
            this.logger = logger;
            this.loggerEvents = new DummyTestLoggerEvents();
        }

        public void Run()
        {
            // TODO: run should eventually simulate both xunit, nunit adapters
            // We should add a sequence of testclass/testmethod as input for replay
            this.logger.Initialize(this.loggerEvents, TestResultsDirectory);

            // Simulate test run start event
            var sources = new List<string> { "dummy source" };
            var testRunCriteria = new TestRunCriteria(sources: sources, frequencyOfRunStatsChangeEvent: 10);
            this.loggerEvents.RaiseTestRunStart(testRunCriteria);

            // Simulate a fake message event
            this.loggerEvents.RaiseTestRunMessage(TestMessageLevel.Informational, "dummy message");

            // Simulate a test result
            var testCase = new TestCase();
            var testResult = new TestResult(testCase);
            this.loggerEvents.RaiseTestResult(testResult);

            // Simulate test run completion
            var stats = new TestRunStatistics(new Dictionary<TestOutcome, long> { { TestOutcome.Passed, 1 } });
            this.loggerEvents.RaiseTestRunComplete(stats);
        }
    }
}
