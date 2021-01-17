// Copyright (c) Spekt Contributors. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

namespace Spekt.TestLogger.UnitTests.TestDoubles
{
    using System.Collections.Generic;
    using Spekt.TestLogger.Core;

    public class JsonTestResultSerializer : ITestResultSerializer
    {
        public string Serialize(
            LoggerConfiguration loggerConfiguration,
            TestRunConfiguration runConfiguration,
            List<TestResultInfo> results)
        {
            return string.Empty;
        }
    }
}