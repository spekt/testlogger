// Copyright (c) Spekt Contributors. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

namespace Spekt.TestLogger.Extensions
{
    using System.Collections.Generic;
    using Spekt.TestLogger.Core;

    public class MSTestAdapter : ITestAdapter
    {
        public List<TestResultInfo> TransformResults(List<TestResultInfo> results, List<TestMessageInfo> messages)
        {
            // MS Test puts test parameters in the DisplayName and not in the FullyQualifiedName.
            // So we use the DisplayName whenever it is available.
            foreach (var result in results)
            {
                string displayName = result.TestResultDisplayName;
                string method = result.Method;

                if (string.IsNullOrWhiteSpace(displayName))
                {
                    // Preserving method because result display name was empty
                }
                else if (method != displayName)
                {
                    result.Method = displayName;
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

            // Parse property traits
            var properties = new List<KeyValuePair<string, object>>();
            foreach (var property in testCase.Properties)
            {
                switch (property.Id)
                {
                    case "Microsoft.VisualStudio.TestTools.UnitTesting.TestContext.TestProperty":
                        var propertyValue = testCase.GetPropertyValue(property) as string[];
                        properties.Add(new KeyValuePair<string, object>("CustomProperty", propertyValue));
                        break;
                }
            }

            result.Properties = properties;
        }
    }
}
