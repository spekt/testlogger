// Copyright (c) Spekt Contributors. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

namespace Spekt.TestLogger.Core
{
    using System.IO;

    public static class TestRunResultWorkflow
    {
        public static string GetTestResultDirectory(ITestRun testRun)
        {
            var logFilePath = testRun.LoggerConfiguration
                .GetFormattedLogFilePath(testRun.RunConfiguration);
            return Path.GetDirectoryName(logFilePath);
        }
    }
}