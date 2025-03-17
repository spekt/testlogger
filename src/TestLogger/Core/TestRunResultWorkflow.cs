// Copyright (c) Spekt Contributors. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

namespace Spekt.TestLogger.Core
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using Microsoft.Testing.Platform.Extensions.Messages;
    using Microsoft.Testing.Platform.Extensions.TestFramework;
    using Microsoft.VisualStudio.TestPlatform.ObjectModel;
    using Spekt.TestLogger.Utilities;

    public static class TestRunResultWorkflow
    {
        // Parser instances exist per run so we can throttle error notifications to once per run.
        private static readonly TestCaseNameParser Parser = new ();
        private static readonly LegacyTestCaseNameParser LegacyParser = new ();

        public static void Result(this ITestRun testRun, TestNodeUpdateMessage testNodeUpdateMessage, Dictionary<TestNodeUid, List<TestNodeFileArtifact>> testAttachmentsByTestNode, ITestFramework testFramework)
        {
            if (testNodeUpdateMessage.Properties.SingleOrDefault<TestNodeStateProperty>() is not { } state ||
                state is InProgressTestNodeStateProperty)
            {
                return;
            }

            var fqn = testNodeUpdateMessage.TestNode.DisplayName;
            testRun.LoggerConfiguration.Values.TryGetValue(LoggerConfiguration.ParserKey, out string parserVal);
            var parsedName = parserVal switch
            {
                string x when x.Equals("Legacy", StringComparison.OrdinalIgnoreCase) => LegacyParser.Parse(fqn),
                _ => Parser.Parse(fqn),
            };

            var @namespace = parsedName.Namespace;
            var type = parsedName.Type;
            var method = parsedName.Method;

            Func<string, string> sanitize = testRun.Serializer.InputSanitizer.Sanitize;

            var attachments = testAttachmentsByTestNode.TryGetValue(testNodeUpdateMessage.TestNode.Uid, out var artifacts)
                ? artifacts.Select(x => new TestAttachmentInfo(x.FileInfo.FullName, x.Description)).ToList()
                : new List<TestAttachmentInfo>();

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

            foreach (var property in testNodeUpdateMessage.TestNode.Properties)
            {
                if (property is TestFileLocationProperty testFileLocation)
                {
                    filePath = testFileLocation.FilePath;
                    lineNumber = testFileLocation.LineSpan.Start.Line;
                }
                else if (property is KeyValuePairStringProperty keyValuePairString)
                {
                    traits.Add(new Trait(keyValuePairString.Key, keyValuePairString.Value));
                }
#pragma warning disable TPEXP // Type is for evaluation purposes only and is subject to change or removal in future updates. Suppress this diagnostic to proceed.
                else if (property is StandardErrorProperty stdErr)
                {
                    messages.Add(new TestResultMessage(TestResultMessage.StandardErrorCategory, stdErr.StandardError));
                }
                else if (property is StandardOutputProperty stdOut)
                {
                    messages.Add(new TestResultMessage(TestResultMessage.StandardOutCategory, stdOut.StandardOutput));
                }
#pragma warning restore TPEXP // Type is for evaluation purposes only and is subject to change or removal in future updates. Suppress this diagnostic to proceed.
                else if (property is TestMetadataProperty metadata)
                {
                    traits.Add(new Trait(metadata.Key, metadata.Value));
                }
                else if (property is TestMethodIdentifierProperty methodIdentifier)
                {
                    @namespace = methodIdentifier.Namespace;
                    type = methodIdentifier.TypeName;
                    method = methodIdentifier.MethodName;
                }
                else if (property is TimingProperty timing)
                {
                    startTime = timing.GlobalTiming.StartTime.UtcDateTime;
                    endTime = timing.GlobalTiming.EndTime.UtcDateTime;
                    duration = timing.GlobalTiming.Duration;
                }
            }

            testRun.Store.Add(new TestResultInfo(
                sanitize(@namespace),
                sanitize(type),
                sanitize(method),
                sanitize(fqn),
                GetOutcome(state),
                sanitize(testNodeUpdateMessage.TestNode.DisplayName),
                sanitize(testNodeUpdateMessage.TestNode.DisplayName),
                sanitize(string.Empty), // TODO
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
                testFramework.DisplayName,
                null)); // TODO: This ends up being used in ITestAdapter implementations. The usage should be revised to better understand how to fix it.

            static TestOutcome GetOutcome(TestNodeStateProperty state)
            {
                return state switch
                {
                    PassedTestNodeStateProperty => TestOutcome.Passed,
                    FailedTestNodeStateProperty or ErrorTestNodeStateProperty or TimeoutTestNodeStateProperty or CancelledTestNodeStateProperty => TestOutcome.Failed,
                    SkippedTestNodeStateProperty => TestOutcome.Skipped,
                    _ => TestOutcome.None, // TODO: Should this throw?
                };
            }
        }

        public static void Result(this ITestRun testRun, TestResult result)
        {
            var fqn = result.TestCase.FullyQualifiedName;
            testRun.LoggerConfiguration.Values.TryGetValue(LoggerConfiguration.ParserKey, out string parserVal);
            var parsedName = parserVal switch
            {
                string x when x.Equals("Legacy", StringComparison.OrdinalIgnoreCase) => LegacyParser.Parse(fqn),
                _ => Parser.Parse(fqn),
            };

            Func<string, string> sanitize = testRun.Serializer.InputSanitizer.Sanitize;

            testRun.Store.Add(new TestResultInfo(
                sanitize(parsedName.Namespace),
                sanitize(parsedName.Type),
                sanitize(parsedName.Method),
                sanitize(fqn),
                result.Outcome,
                sanitize(result.DisplayName),
                sanitize(result.TestCase.DisplayName),
                sanitize(result.TestCase.Source),
                sanitize(result.TestCase.CodeFilePath),
                result.TestCase.LineNumber,
                result.StartTime.UtcDateTime,
                result.EndTime.UtcDateTime,
                result.Duration,
                sanitize(result.ErrorMessage),
                sanitize(result.ErrorStackTrace),
                result.Messages.Select(x => new TestResultMessage(sanitize(x.Category), sanitize(x.Text))).ToList(),
                result.Attachments.SelectMany(x => x.ToAttachments()).ToList(),
                result.TestCase.Traits.Select(x => new Trait(sanitize(x.Name), sanitize(x.Value))).ToList(),
                result.TestCase.ExecutorUri?.ToString(),
                result.TestCase));
        }
    }
}