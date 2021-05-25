// Copyright (c) Spekt Contributors. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

namespace Spekt.TestLogger.Extensions
{
    using Spekt.TestLogger.Core;

    public class TestAdapterFactory : ITestAdapterFactory
    {
        public ITestAdapter CreateTestAdapter(string executorUri)
        {
            DebugLogger.WriteLine($"Create test adapter using executorUri: {executorUri}");

            if (!string.IsNullOrEmpty(executorUri) && executorUri.ToLowerInvariant().Contains("xunit"))
            {
                return new XunitTestAdapter();
            }
            else if (!string.IsNullOrEmpty(executorUri) && executorUri.ToLowerInvariant().Contains("mstest"))
            {
                return new MSTestAdapter();
            }

            return new DefaultTestAdapter();
        }
    }
}