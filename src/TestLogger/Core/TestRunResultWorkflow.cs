// Copyright (c) Spekt Contributors. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

namespace Spekt.TestLogger.Core
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using Microsoft.Testing.Platform.Extensions.Messages;
    using Microsoft.VisualStudio.TestPlatform.ObjectModel;
    using Microsoft.VisualStudio.TestPlatform.ObjectModel.Logging;
    using Spekt.TestLogger.Utilities;

    public static class TestRunResultWorkflow
    {
        // Parser instances exist per run so we can throttle error notifications to once per run.
        private static readonly TestCaseNameParser Parser = new ();
        private static readonly LegacyTestCaseNameParser LegacyParser = new ();

        public static void Result(this ITestRun testRun, TestNodeUpdateMessage testNodeUpdateMessage, Dictionary<TestNodeUid, List<TestNodeFileArtifact>> testAttachmentsByTestNode)
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

            Func<string, string> sanitize = testRun.Serializer.InputSanitizer.Sanitize;

            testRun.Store.Add(new TestResultInfo(
                sanitize(parsedName.Namespace),
                sanitize(parsedName.Type),
                sanitize(parsedName.Method),
                sanitize(fqn),
                GetOutcome(state),
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