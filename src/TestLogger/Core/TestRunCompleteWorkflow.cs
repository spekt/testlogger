// Copyright (c) Spekt Contributors. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

namespace Spekt.TestLogger.Core
{
    using System;
    using System.Collections.Generic;
    using System.Globalization;
    using System.IO;
    using Spekt.TestLogger.Platform;

    public static class TestRunCompleteWorkflow
    {
        public static void Complete(
            this ITestRun testRun,
            IReadOnlyCollection<TestAttachmentInfo> testAttachmentInfos,
            List<TestResultInfo> results,
            List<TestMessageInfo> messages)
        {
            // Update the test run complete timestamp and run level attachments
            testRun.RunConfiguration.EndTime = DateTime.UtcNow;
            testRun.RunConfiguration.Attachments = testAttachmentInfos;

            // Prepare test results file from logger configuration
            var logFilePath = testRun.LoggerConfiguration
                .GetFormattedLogFilePath(testRun.RunConfiguration);
            CreateResultsDirectory(testRun.FileSystem, Path.GetDirectoryName(logFilePath));

            var content = testRun.Serializer.Serialize(
                testRun.LoggerConfiguration,
                testRun.RunConfiguration,
                results,
                messages);
            testRun.FileSystem.Write(logFilePath, content);

            testRun.ConsoleOutput.WriteMessage(string.Format(
                CultureInfo.CurrentCulture,
                "Results File: {0}",
                logFilePath));
        }

        private static void CreateResultsDirectory(IFileSystem fs, string path)
        {
            if (string.IsNullOrEmpty(path))
            {
                return;
            }

            fs.CreateDirectory(path);
        }
    }
}
