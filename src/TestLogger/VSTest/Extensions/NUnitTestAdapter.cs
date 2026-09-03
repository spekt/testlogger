// Copyright (c) Spekt Contributors. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

namespace Spekt.TestLogger.VSTest.Extensions
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using Spekt.TestLogger.Core;

    public class NUnitTestAdapter : ITestAdapter
    {
        private const string ExplicitLabel = "Explicit";

        public List<TestResultInfo> TransformResults(List<TestResultInfo> results, List<TestMessageInfo> messages, object testCase = null)
        {
            foreach (var result in results)
            {
                // Mark tests with Explicit attribute as Skipped instead of Inconclusive. Explicit
                // is passed as a trait in the test platform. NUnit explicit attribute spec:
                // https://docs.nunit.org/articles/nunit/writing-tests/attributes/explicit.html
                if (result.Outcome == TestOutcome.None &&
                    result.Traits.Any(trait => trait.Name.Equals(ExplicitLabel, StringComparison.OrdinalIgnoreCase)))
                {
                    result.Outcome = TestOutcome.Skipped;
                }

                CreateProperties(result, testCase);
            }

            return results;
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
            foreach (var property in propertiesValue)
            {
                var id = property.GetType().GetProperty("Id")?.GetValue(property) as string;
                switch (id)
                {
                    case "NUnit.Seed":
                    case "NUnit.TestCategory":
                        properties.Add(new KeyValuePair<string, object>(id, InvokeGetPropertyValue(testCaseObj, property)));
                        break;
                    case "NUnit.Category":
                        properties.Add(new KeyValuePair<string, object>("CustomProperty", InvokeGetPropertyValue(testCaseObj, property)));
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
