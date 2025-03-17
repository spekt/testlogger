// Copyright (c) Spekt Contributors. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

namespace Spekt.TestLogger.Extensions
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using Spekt.TestLogger.Core;

    public class NUnitTestAdapter : ITestAdapter
    {
        private const string ExplicitLabel = "Explicit";

        public List<TestResultInfo> TransformResults(List<TestResultInfo> results, List<TestMessageInfo> messages)
        {
            foreach (var result in results)
            {
                // Mark tests with Explicit attribute as Skipped instead of Inconclusive. Explicit
                // is passed as a trait in the test platform. NUnit explicit attribute spec:
                // https://docs.nunit.org/articles/nunit/writing-tests/attributes/explicit.html
                if (result.Outcome == Microsoft.VisualStudio.TestPlatform.ObjectModel.TestOutcome.None &&
                    result.Traits.Any(trait => trait.Name.Equals(ExplicitLabel, StringComparison.OrdinalIgnoreCase)))
                {
                    result.Outcome = Microsoft.VisualStudio.TestPlatform.ObjectModel.TestOutcome.Skipped;
                }

                CreateProperties(result);
            }

            return results;
        }

        private static void CreateProperties(TestResultInfo result)
        {
            if (result.TestCase is not { } testCase)
            {
                return;
            }

            // NUnit adapter uses Seed and TestCategory in TestCase Properties.
            // Populate these properties if available.
            var properties = new List<KeyValuePair<string, object>>();
            foreach (var property in testCase.Properties)
            {
                switch (property.Id)
                {
                    case "NUnit.Seed":
                    case "NUnit.TestCategory":
                        properties.Add(new KeyValuePair<string, object>(property.Id, testCase.GetPropertyValue(property)));
                        break;
                    case "NUnit.Category":
                        properties.Add(new KeyValuePair<string, object>("CustomProperty", testCase.GetPropertyValue(property)));
                        break;
                }
            }

            result.Properties = properties;
        }
    }
}
