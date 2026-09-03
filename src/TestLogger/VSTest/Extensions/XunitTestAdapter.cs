// Copyright (c) Spekt Contributors. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

namespace Spekt.TestLogger.VSTest.Extensions
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using Spekt.TestLogger.Core;

    public class XunitTestAdapter : ITestAdapter
    {
        private const string SkipReasonCategory = "skipReason";

        public List<TestResultInfo> TransformResults(
            List<TestResultInfo> results,
            List<TestMessageInfo> messages,
            object testCase = null)
        {
            var transformedResults = new List<TestResultInfo>();

            // Process all the messages collected during the test run
            // If one ends with [SKIP], then the next message contains the skip reason.
            var skippedTestNamesWithReason = new Dictionary<string, string>();
            for (int i = 0; i < messages.Count; i++)
            {
                string message = messages[i].Message;
                if (!message.EndsWith("[SKIP]"))
                {
                    continue;
                }

                // remove the gunk ...
                int from = message.IndexOf("]") + 1;
                int to = message.LastIndexOf("[") - from;
                string testName = message.Substring(from, to).Trim();

                string reasonMessage = messages[++i].Message;
                from = reasonMessage.IndexOf("]") + 1;
                string reason = reasonMessage.Substring(from).Trim();

                skippedTestNamesWithReason.Add(testName, reason);
            }

            foreach (var result in results)
            {
                if (skippedTestNamesWithReason.TryGetValue(result.TestCaseDisplayName, out var skipReason))
                {
                    // TODO: Defining a new category for now...
                    result.Messages.Add(new TestResultMessage(SkipReasonCategory, skipReason));
                }
                else if (result.Outcome == TestOutcome.Skipped)
                {
                    // Fallback: run level skip messages are not available when transforming
                    // per result, so derive the reason from the result's own StandardOut
                    // message, if any. Skipped tests never execute, so their StandardOut
                    // carries only runner-generated text (the skip reason).
                    var reason = result.Messages
                        .FirstOrDefault(m => TestResultMessage.StandardOutCategory.Equals(m.Category, StringComparison.OrdinalIgnoreCase)
                            && !string.IsNullOrWhiteSpace(m.Text))?.Text.Trim();
                    if (!string.IsNullOrEmpty(reason))
                    {
                        result.Messages.Add(new TestResultMessage(SkipReasonCategory, reason));
                    }
                }

                string displayName = result.TestResultDisplayName;

                // Add parameters for theories.
                if (string.IsNullOrWhiteSpace(displayName) == false &&
                    displayName.IndexOf("(") is int i &&
                    i > 0)
                {
                    result.Method += displayName.Substring(i);
                }

                CreateProperties(result, testCase);

                transformedResults.Add(result);
            }

            return transformedResults;
        }

        private static void CreateProperties(TestResultInfo result, object testCaseObj)
        {
            if (testCaseObj == null)
            {
                return;
            }

            // Use reflection to avoid hard reference to Microsoft.TestPlatform.ObjectModel in Core assembly.
            var testCaseType = testCaseObj.GetType();
            var propertiesProp = testCaseType.GetProperty("Properties");
            var propertiesValue = propertiesProp?.GetValue(testCaseObj) as System.Collections.IEnumerable;
            if (propertiesValue == null)
            {
                return;
            }

            var properties = new List<KeyValuePair<string, object>>();

            // Parse test traits via Trait decorator
            foreach (var property in propertiesValue)
            {
                var id = property.GetType().GetProperty("Id")?.GetValue(property) as string;
                switch (id)
                {
                    case "Xunit.Trait":
                        var propertyValue = InvokeGetPropertyValue(testCaseObj, property) as string[];
                        properties.Add(new KeyValuePair<string, object>("CustomProperty", propertyValue));
                        break;
                }
            }

            result.Properties = properties;
        }

        private static object InvokeGetPropertyValue(object testCase, object property)
        {
            var method = testCase.GetType().GetMethod("GetPropertyValue", new[] { property.GetType() });
            return method?.Invoke(testCase, new[] { property });
        }
    }
}