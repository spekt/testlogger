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
            Func<string, string> sanitize = testRun.Serializer.InputSanitizer.Sanitize;

            testRun.Store.Add(new TestResultInfo(
                sanitize(parsedName.Namespace),
                sanitize(parsedName.Type),
                sanitize(parsedName.Method),
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
                result.TestCase.Traits.Select(x => new Trait(sanitize(x.Name), sanitize(x.Value))).ToList(),
                result.TestCase.ExecutorUri?.ToString()));
        }
    }
}