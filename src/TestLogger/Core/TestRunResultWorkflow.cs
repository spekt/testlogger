// Copyright (c) Spekt Contributors. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

namespace Spekt.TestLogger.Core
{
    using System;
    using System.Linq;
    using Microsoft.VisualStudio.TestPlatform.ObjectModel;
    using Microsoft.VisualStudio.TestPlatform.ObjectModel.Logging;

    public static class TestRunResultWorkflow
    {
        // Parser instances exist per run so we can throttle error notifications to once per run.
        private static readonly TestCaseNameParser Parser = new ();
        private static readonly LegacyTestCaseNameParser LegacyParser = new ();

        public static void Result(this ITestRun testRun, TestResultEventArgs resultEvent)
        {
            var fqn = resultEvent.Result.TestCase.FullyQualifiedName;
            testRun.LoggerConfiguration.Values.TryGetValue(LoggerConfiguration.ParserKey, out string parserVal);
            var parsedName = parserVal switch
            {
                string x when x.Equals("Legacy", StringComparison.OrdinalIgnoreCase) => LegacyParser.Parse(fqn),
                _ => Parser.Parse(fqn),
            };

            var result = resultEvent.Result;
            var sanitize = testRun.Serializer.InputSanitizer.Sanitize;

            testRun.Store.Add(new TestResultInfo(
                sanitize(parsedName.Namespace),
                sanitize(parsedName.Type),
                sanitize(parsedName.Method),
                result.Outcome,
                result.DisplayName,
                result.TestCase.Source,
                result.StartTime.UtcDateTime,
                result.EndTime.UtcDateTime,
                result.Duration,
                sanitize(result.ErrorMessage),
                sanitize(result.ErrorStackTrace),
                result.Messages.Select(x => new TestResultMessage(sanitize(x.Category), sanitize(x.Text))).ToList(),
                result.Traits,
                result.TestCase.ExecutorUri?.ToString()));
        }
    }
}