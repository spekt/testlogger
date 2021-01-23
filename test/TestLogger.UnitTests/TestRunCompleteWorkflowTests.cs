// Copyright (c) Spekt Contributors. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

namespace Spekt.TestLogger.UnitTests
{
    using System;
    using System.Collections.Generic;
    using System.Collections.ObjectModel;
    using System.Linq;
    using Microsoft.VisualStudio.TestPlatform.ObjectModel;
    using Microsoft.VisualStudio.TestPlatform.ObjectModel.Client;
    using Microsoft.VisualStudio.TestPlatform.ObjectModel.Logging;
    using Microsoft.VisualStudio.TestTools.UnitTesting;
    using Newtonsoft.Json;
    using Spekt.TestLogger.Core;
    using Spekt.TestLogger.UnitTests.TestDoubles;
    using JsonSerializer = System.Text.Json.JsonSerializer;
    using TestResult = Microsoft.VisualStudio.TestPlatform.ObjectModel.TestResult;

    [TestClass]
    public class TestRunCompleteWorkflowTests
    {
        private readonly FakeFileSystem fileSystem;
        private readonly ITestRun testRun;
        private readonly TestRunCompleteEventArgs testRunCompleteEvent;

        public TestRunCompleteWorkflowTests()
        {
            this.fileSystem = new FakeFileSystem();
            this.testRun = new TestRunBuilder()
                .WithLoggerConfiguration(new LoggerConfiguration(new () { { LoggerConfiguration.LogFilePathKey, "/tmp/results.json" } }))
                .WithFileSystem(this.fileSystem)
                .WithConsoleOutput(new FakeConsoleOutput())
                .WithStore(new TestResultStore())
                .WithSerializer(new JsonTestResultSerializer())
                .Build();
            this.testRunCompleteEvent = new TestRunCompleteEventArgs(
                stats: new TestRunStatistics(),
                isCanceled: false,
                isAborted: false,
                error: null,
                attachmentSets: new Collection<AttachmentSet>(),
                elapsedTime: TimeSpan.Zero);
        }

        [TestMethod]
        public void CompleteShouldUpdateTestRunCompleteTimestamp()
        {
            this.testRun.Result(new TestResultEventArgs(new TestResult(new TestCase())));
            this.testRun.Message(new TestRunMessageEventArgs(TestMessageLevel.Informational, "dummy message"));

            this.testRun.Complete(this.testRunCompleteEvent);

            Assert.AreEqual(DateTime.Now.Date, this.testRun.RunConfiguration.EndTime.ToLocalTime().Date);
        }

        [TestMethod]
        public void CompleteShouldFreezeAndResetResultStore()
        {
            this.testRun.Result(new TestResultEventArgs(new TestResult(new TestCase())));
            this.testRun.Message(new TestRunMessageEventArgs(TestMessageLevel.Informational, "dummy message"));

            this.testRun.Complete(this.testRunCompleteEvent);

            this.testRun.Store.Pop(out var results, out var messages);
            Assert.AreEqual(0, results.Count);
            Assert.AreEqual(0, messages.Count);
        }

        [TestMethod]
        public void CompleteShouldWriteTestResults()
        {
            SimulateTestResult(this.testRun);

            this.testRun.Complete(this.testRunCompleteEvent);

            var logFilePath = this.testRun.LoggerConfiguration.LogFilePath;
            Assert.IsFalse(string.IsNullOrEmpty(this.fileSystem.Read(logFilePath)));
        }

        [TestMethod]
        public void CompleteShouldSerializeTestResults()
        {
            SimulateTestResult(this.testRun);

            this.testRun.Complete(this.testRunCompleteEvent);

            var logFilePath = this.testRun.LoggerConfiguration.LogFilePath;
            var results = JsonSerializer.Deserialize<List<JsonTestResultSerializer.TestAssembly>>(this.fileSystem.Read(logFilePath), null);
            Assert.AreEqual("/tmp/test.dll", results[0].Name);
            Assert.AreEqual("C", results[0].Fixtures.First().Name);
            Assert.AreEqual(2, results[0].Fixtures.First().Tests.Count());
        }

        [TestMethod]
        public void CompleteShouldWriteTestResultsForRelativeLogFilePath()
        {
            var testRun = new TestRunBuilder()
                .WithLoggerConfiguration(new LoggerConfiguration(new () { { LoggerConfiguration.LogFilePathKey, "results.json" } }))
                .WithFileSystem(this.fileSystem)
                .WithConsoleOutput(new FakeConsoleOutput())
                .WithStore(new TestResultStore())
                .WithSerializer(new JsonTestResultSerializer())
                .Build();
            SimulateTestResult(this.testRun);

            testRun.Complete(this.testRunCompleteEvent);

            var logFilePath = testRun.LoggerConfiguration.LogFilePath;
            Assert.IsFalse(string.IsNullOrEmpty(this.fileSystem.Read(logFilePath)));
        }

        private static void SimulateTestResult(ITestRun testRun)
        {
            var source = "/tmp/test.dll";
            var executorUri = new Uri("executor://dummy");
            var passingResult =
                new TestResult(new TestCase("NS.C.TM1", executorUri, source))
                    { Outcome = TestOutcome.Passed };
            var failingResult =
                new TestResult(new TestCase("NS.C.TM2", executorUri, source))
                    { Outcome = TestOutcome.Failed };
            testRun.Result(new TestResultEventArgs(passingResult));
            testRun.Result(new TestResultEventArgs(failingResult));
        }
    }
}