// Copyright (c) Spekt Contributors. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

namespace Spekt.TestLogger.VSTest.Extensions
{
    using System.Collections.Generic;
    using Spekt.TestLogger.Core;

    public class MSTestAdapter : ITestAdapter
    {
        public List<TestResultInfo> TransformResults(List<TestResultInfo> results, List<TestMessageInfo> messages, object testCase = null)
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
                    case "Microsoft.VisualStudio.TestTools.UnitTesting.TestContext.TestProperty":
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
