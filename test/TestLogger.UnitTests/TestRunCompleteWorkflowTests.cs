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
                .WithLoggerConfiguration(new LoggerConfiguration(new() { { LoggerConfiguration.LogFilePathKey, "/tmp/results.json" } }))
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
            this.testRun.Result(new TestResult(new TestCase()));
            this.testRun.Message(new TestRunMessageEventArgs(TestMessageLevel.Informational, "dummy message"));

            this.testRun.Complete(this.testRunCompleteEvent);

            Assert.AreEqual(DateTime.Now.Date, this.testRun.RunConfiguration.EndTime.ToLocalTime().Date);
        }

        [TestMethod]
        public void CompleteShouldFreezeAndResetResultStore()
        {
            this.testRun.Result(new TestResult(new TestCase()));
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
            var results = JsonSerializer.Deserialize<JsonTestResultSerializer.TestReport>(this.fileSystem.Read(logFilePath));
            var assembly = results.TestAssemblies.First();
            Assert.AreEqual("/tmp/test.dll", assembly.Name);
            Assert.AreEqual("C", assembly.Fixtures.First().Name);
            Assert.AreEqual(2, assembly.Fixtures.First().Tests.Count());
        }

        [TestMethod]
        public void CompleteShouldWriteTestResultsForRelativeLogFilePath()
        {
            var relativePathTestRun = new TestRunBuilder()
                .WithLoggerConfiguration(new LoggerConfiguration(new() { { LoggerConfiguration.LogFilePathKey, "results.json" } }))
                .WithFileSystem(this.fileSystem)
                .WithConsoleOutput(new FakeConsoleOutput())
                .WithStore(new TestResultStore())
                .WithSerializer(new JsonTestResultSerializer())
                .Build();
            SimulateTestResult(relativePathTestRun);

            relativePathTestRun.Complete(this.testRunCompleteEvent);

            var logFilePath = relativePathTestRun.LoggerConfiguration.LogFilePath;
            Assert.IsFalse(string.IsNullOrEmpty(this.fileSystem.Read(logFilePath)));
        }

        [TestMethod]
        public void CompleteShouldPassAllMessagesToSerializer()
        {
            SimulateTestResult(this.testRun);

            this.testRun.Complete(this.testRunCompleteEvent);

            var logFilePath = this.testRun.LoggerConfiguration.LogFilePath;
            var results = JsonSerializer.Deserialize<JsonTestResultSerializer.TestReport>(this.fileSystem.Read(logFilePath));
            var expectedMessages = TestRunMessageEventArgs();

            Assert.AreEqual(expectedMessages.Count, results.TestMessages.Count());
            expectedMessages
                .ForEach(exp => Assert.IsTrue(
                    results.TestMessages.SingleOrDefault(act => act.Level == exp.Level && act.Message == exp.Message) is TestMessageInfo));
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
            testRun.Result(passingResult);
            testRun.Result(failingResult);
            TestRunMessageEventArgs().ForEach(x => testRun.Message(x));
        }

        private static List<TestRunMessageEventArgs> TestRunMessageEventArgs()
        {
            return new()
            {
                new(TestMessageLevel.Informational, "9CB2F5CB-0B24-48F6-9FBC-17E794FF589D"),
                new(TestMessageLevel.Informational, "9A87B0DF-60F3-45A3-A662-59527EC2B114"),
                new(TestMessageLevel.Warning, "A0DE6354-C727-4F5F-9D29-ECE75B533424"),
                new(TestMessageLevel.Warning, "73D2D4B1-A513-4D9C-B876-9B16202F0BCB"),
                new(TestMessageLevel.Error, "18E79A95-528E-428C-BCCF-BBC1B53CB46C"),
                new(TestMessageLevel.Error, "E6FB8071-0745-4A9C-A685-8BC5E9E6FE32"),
            };
        }
    }
}
