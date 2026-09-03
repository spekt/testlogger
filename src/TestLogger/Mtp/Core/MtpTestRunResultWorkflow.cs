// Copyright (c) Spekt Contributors. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

namespace Spekt.TestLogger.Mtp.Core
{
    using System;
    using System.Collections.Generic;
    using System.IO;
    using System.Linq;
    using System.Reflection;
    using Microsoft.Testing.Platform.Extensions.Messages;
    using Microsoft.Testing.Platform.Extensions.TestFramework;
    using Spekt.TestLogger.Core;
    using Spekt.TestLogger.Mtp.Utilities;

    public static class MtpTestRunResultWorkflow
    {
        private static TestCaseNameParser parser;
        private static LegacyTestCaseNameParser legacyParser;

        public static void Result(this ITestRun testRun, TestNodeUpdateMessage testNodeUpdateMessage, ITestFramework testFramework)
        {
            if (testNodeUpdateMessage.TestNode.Properties.SingleOrDefault<TestNodeStateProperty>() is not { } state ||
                state is InProgressTestNodeStateProperty)
            {
                return;
            }

            parser ??= new TestCaseNameParser(testRun.ConsoleOutput);
            legacyParser ??= new LegacyTestCaseNameParser(testRun.ConsoleOutput);

            testRun.LoggerConfiguration.Values.TryGetValue(LoggerConfiguration.ParserKey, out string parserVal);

            Func<string, string> sanitize = testRun.Serializer.InputSanitizer.Sanitize;

            var attachments = testNodeUpdateMessage.TestNode.Properties.OfType<FileArtifactProperty>().ToAttachments(baseDirectory: TestRunResultWorkflow.GetTestResultDirectory(testRun), makeRelativePaths: testRun.LoggerConfiguration.UseRelativeAttachmentPaths).ToList();

            var (errorMessage, errorStackTrace) = state switch
            {
                FailedTestNodeStateProperty failed => (failed.Exception.Message ?? failed.Explanation, failed.Exception.StackTrace),
                ErrorTestNodeStateProperty error => (error.Exception.Message ?? error.Explanation, error.Exception.StackTrace),
                _ => (string.Empty, string.Empty),
            };

            string filePath = null;
            int lineNumber = -1;
            var traits = new List<Trait>();
            var messages = new List<TestResultMessage>();
            DateTime startTime = default;
            DateTime endTime = default;
            TimeSpan duration = default;

            string @namespace = string.Empty;
            string type = string.Empty;
            string method = string.Empty;
            string fqn = string.Empty;

            TestMethodIdentifierProperty methodIdentifier = null;
            foreach (var property in testNodeUpdateMessage.TestNode.Properties)
            {
                if (property is TestFileLocationProperty testFileLocation)
                {
                    filePath = testFileLocation.FilePath;
                    lineNumber = testFileLocation.LineSpan.Start.Line;
                }
#pragma warning disable TPEXP
                else if (property is StandardErrorProperty stdErr)
                {
                    messages.Add(new TestResultMessage(TestResultMessage.StandardErrorCategory, stdErr.StandardError));
                }
                else if (property is StandardOutputProperty stdOut)
                {
                    messages.Add(new TestResultMessage(TestResultMessage.StandardOutCategory, stdOut.StandardOutput));
                }
#pragma warning restore TPEXP
                else if (property is TestMetadataProperty metadata)
                {
                    traits.Add(new Trait(metadata.Key, metadata.Value));
                }
                else if (property is TestMethodIdentifierProperty methodIdProp)
                {
                    methodIdentifier = methodIdProp;
                }
                else if (property is TimingProperty timing)
                {
                    startTime = timing.GlobalTiming.StartTime.UtcDateTime;
                    endTime = timing.GlobalTiming.EndTime.UtcDateTime;
                    duration = timing.GlobalTiming.Duration;
                }
            }

            var parseResult = parser.Parse(methodIdentifier, testNodeUpdateMessage.TestNode);
            @namespace = parseResult.Namespace;
            type = parseResult.Type;
            method = parseResult.Method;
            fqn = parseResult.FullyQualifiedName;

            var assemblyPath = Assembly.GetEntryAssembly()?.Location;
            if (string.IsNullOrEmpty(assemblyPath))
            {
                assemblyPath = "UnknownAssembly";
            }

            testRun.Store.Add(new TestResultInfo(
                sanitize(@namespace),
                sanitize(type),
                sanitize(method),
                sanitize(fqn),
                GetOutcome(state),
                sanitize(testNodeUpdateMessage.TestNode.DisplayName),
                sanitize(testNodeUpdateMessage.TestNode.DisplayName),
                sanitize(assemblyPath),
                sanitize(filePath),
                lineNumber: lineNumber,
                startTime,
                endTime,
                duration,
                sanitize(errorMessage),
                sanitize(errorStackTrace),
                messages,
                attachments,
                traits,
                testFramework.DisplayName));

            static TestOutcome GetOutcome(TestNodeStateProperty state)
            {
                return state switch
                {
                    PassedTestNodeStateProperty => TestOutcome.Passed,
                    FailedTestNodeStateProperty or ErrorTestNodeStateProperty or TimeoutTestNodeStateProperty or CancelledTestNodeStateProperty => TestOutcome.Failed,
                    SkippedTestNodeStateProperty => TestOutcome.Skipped,
                    _ => TestOutcome.None,
                };
            }
        }
    }
}
