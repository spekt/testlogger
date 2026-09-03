// Copyright (c) Spekt Contributors. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

namespace Spekt.TestLogger.VSTest.Core
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using Spekt.TestLogger.Core;
    using Spekt.TestLogger.VSTest.Utilities;
    using VSTestOutcome = Microsoft.VisualStudio.TestPlatform.ObjectModel.TestOutcome;
    using VSTestTestCase = Microsoft.VisualStudio.TestPlatform.ObjectModel.TestCase;
    using VSTestTestResult = Microsoft.VisualStudio.TestPlatform.ObjectModel.TestResult;

    public static class VSTestTestRunResultWorkflow
    {
        private static TestCaseNameParser parser;
        private static LegacyTestCaseNameParser legacyParser;

        public static void Result(this ITestRun testRun, VSTestTestResult result)
        {
            parser ??= new TestCaseNameParser(testRun.ConsoleOutput);
            legacyParser ??= new LegacyTestCaseNameParser(testRun.ConsoleOutput);

            var fqn = result.TestCase.FullyQualifiedName;
            testRun.LoggerConfiguration.Values.TryGetValue(LoggerConfiguration.ParserKey, out string parserVal);
            var parsedName = parserVal switch
            {
                string x when x.Equals("Legacy", StringComparison.OrdinalIgnoreCase) => legacyParser.Parse(fqn),
                _ => parser.Parse(fqn),
            };

            var attachments = result.Attachments.SelectMany(x => x.ToAttachments(baseDirectory: TestRunResultWorkflow.GetTestResultDirectory(testRun), makeRelativePaths: testRun.LoggerConfiguration.UseRelativeAttachmentPaths)).ToList();

            Func<string, string> sanitize = testRun.Serializer.InputSanitizer.Sanitize;

            var traits = GetTraits(result.TestCase, sanitize);
            var coreOutcome = (TestOutcome)(int)result.Outcome;
            var resultInfo = new TestResultInfo(
                sanitize(parsedName.Namespace),
                sanitize(parsedName.Type),
                sanitize(parsedName.Method),
                sanitize(fqn),
                coreOutcome,
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
                attachments,
                traits,
                result.TestCase.ExecutorUri?.ToString());

            // Apply framework-specific transformations using the real VSTest TestCase.
            // Use per-result inline transformation so properties are extracted from the real ObjectModel TestCase.
            var executorUri = result.TestCase.ExecutorUri?.ToString();
            var adapter = new Spekt.TestLogger.VSTest.Extensions.TestAdapterFactory().CreateTestAdapter(executorUri);
            var transformed = adapter.TransformResults(new List<TestResultInfo> { resultInfo }, new List<TestMessageInfo>(), result.TestCase);
            testRun.Store.Add(transformed[0]);
        }

        private static List<Trait> GetTraits(VSTestTestCase testCase, Func<string, string> sanitize)
        {
            var traits = new List<Trait>();
            var traitProperty = testCase.Properties.FirstOrDefault(p => p.Id == "TestObject.Traits");
            if (traitProperty != null)
            {
                var traitValues = testCase.GetPropertyValue(traitProperty, Enumerable.Empty<KeyValuePair<string, string>>());

                foreach (var kvp in traitValues)
                {
                    traits.Add(new Trait(sanitize(kvp.Key), sanitize(kvp.Value)));
                }
            }

            return traits;
        }
    }
}
