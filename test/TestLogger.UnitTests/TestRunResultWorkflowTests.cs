// Copyright (c) Spekt Contributors. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

namespace Spekt.TestLogger.UnitTests
{
    using System;
    using System.Collections.Generic;
    using System.Threading.Tasks;
    using Microsoft.VisualStudio.TestPlatform.ObjectModel;
    using Microsoft.VisualStudio.TestTools.UnitTesting;
    using Spekt.TestLogger.Core;
    using Spekt.TestLogger.UnitTests.TestDoubles;
    using TestResult = Microsoft.VisualStudio.TestPlatform.ObjectModel.TestResult;

    [TestClass]
    public class TestRunResultWorkflowTests
    {
        private const string DummySourceFile = "/tmp/test-project/DummyTests.cs";
        private static readonly Uri DummyAdapterUri = new("adapter://Microsoft/TestPlatform/DummyTestAdapter");
        private readonly TestResultStore testResultStore;
        private readonly TestCase dummyTestCase;
        private readonly ITestRun testRun;

        public TestRunResultWorkflowTests()
        {
            this.testResultStore = new TestResultStore();
            this.dummyTestCase = new TestCase(
                "SampleNamespace.SampleClass.SampleTest",
                DummyAdapterUri,
                DummySourceFile);
            this.testRun = new TestRunBuilder()
                .WithConsoleOutput(new FakeConsoleOutput())
                .WithLoggerConfiguration(new LoggerConfiguration(BasicConfig()))
                .WithSerializer(new JsonTestResultSerializer())
                .WithStore(this.testResultStore).Build();
        }

        [TestMethod]
        [DataRow(TestOutcome.Passed)]
        [DataRow(TestOutcome.Failed)]
        [DataRow(TestOutcome.Skipped)]
        public void ResultShouldCaptureTestCaseAndResult(TestOutcome testOutcome)
        {
            var testResult = CreateTestResult(this.dummyTestCase, testOutcome);

            this.testRun.Result(testResult);

            this.testResultStore.Pop(out var results, out _);
            Assert.AreEqual(1, results.Count);
            Assert.AreEqual("SampleNamespace", results[0].Namespace);
            Assert.AreEqual("SampleClass", results[0].Type);
            Assert.AreEqual("SampleNamespace.SampleClass", results[0].FullTypeName);
            Assert.AreEqual("SampleTest", results[0].Method);
            Assert.AreEqual("SampleNamespace.SampleClass.SampleTest", results[0].DisplayName);
            Assert.AreEqual(DummySourceFile, results[0].AssemblyPath);

            Assert.AreEqual(testOutcome, results[0].Outcome);
            Assert.AreEqual("Dummy error", results[0].ErrorMessage);
            Assert.AreEqual("Dummy stacktrace", results[0].ErrorStackTrace);

            Assert.AreEqual(TimeSpan.FromSeconds(30), results[0].Duration);
            Assert.AreEqual(DateTimeOffset.MinValue, results[0].StartTime);
            Assert.AreEqual(DateTimeOffset.MaxValue, results[0].EndTime);

            Assert.AreEqual(0, results[0].Messages.Count);
            Assert.AreEqual(0, results[0].Traits.Count);
            Assert.AreEqual(0, results[0].Properties.Count);
            Assert.AreEqual(1, results[0].Attachments.Count);
        }

        [TestMethod]
        public async Task ResultShouldCaptureTestCaseAndResultForParallelRun()
        {
            var testResult = new TestResult(this.dummyTestCase);
            var tasks = new List<Task>();

            for (var i = 0; i < 100; i++)
            {
                tasks.Add(Task.Run(() => this.testRun.Result(testResult)));
            }

            await Task.WhenAll(tasks);

            this.testResultStore.Pop(out var results, out _);
            Assert.AreEqual(100, results.Count);
        }

        [TestMethod]
        public void ResultShouldCaptureTestProperties()
        {
            // This property will be ignored.
            var testproperty = TestProperty.Register("TestProperty", "TestLabel", typeof(string), typeof(TestCase));
            this.dummyTestCase.SetPropertyValue(testproperty, "TestValue");
            var testResult = CreateTestResult(this.dummyTestCase, TestOutcome.Passed);

            this.testRun.Result(testResult);

            this.testResultStore.Pop(out var results, out _);
            Assert.AreEqual(1, results.Count);
            Assert.AreEqual(0, results[0].Properties.Count);
        }

        [TestMethod]
        public void ResultShouldCaptureAttachmentsWithRelativePaths()
        {
            var testResult = CreateTestResult(this.dummyTestCase, TestOutcome.Passed);

            this.testRun.Result(testResult);

            this.testResultStore.Pop(out var results, out _);
            Assert.AreEqual(1, results.Count);
            Assert.AreEqual(1, results[0].Attachments.Count);
            Assert.AreEqual("subdir/log.txt", results[0].Attachments[0].FilePath);
        }

        private static Dictionary<string, string> BasicConfig()
        {
            return new Dictionary<string, string>
            {
                { DefaultLoggerParameterNames.TestRunDirectory, "/dir" },
                { LoggerConfiguration.LogFilePathKey, @"/dir/results.json" },
                { LoggerConfiguration.UseRelativeAttachmentPathKey, "true" }
            };
        }

        private static TestResult CreateTestResult(TestCase testCase, TestOutcome outcome)
        {
            var attachmentSet = new AttachmentSet(new Uri("executor://dummy"), "Dummy Attachment Set");
            var attachment = new UriDataAttachment(new Uri("file:///dir/subdir/log.txt"), "log.txt");
            attachmentSet.Attachments.Add(attachment);

            var result = new TestResult(testCase)
            {
                Outcome = outcome,
                ErrorMessage = "Dummy error",
                ErrorStackTrace = "Dummy stacktrace",
                Duration = TimeSpan.FromSeconds(30),
                StartTime = DateTimeOffset.MinValue,
                EndTime = DateTimeOffset.MaxValue
            };
            result.Attachments.Add(attachmentSet);
            return result;
        }
    }
}
