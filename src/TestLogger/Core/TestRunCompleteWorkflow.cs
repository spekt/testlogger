// Copyright (c) Spekt Contributors. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

namespace Spekt.TestLogger.Core
{
    using System;
    using System.Globalization;
    using System.IO;
    using System.Linq;
    using System.Security.Cryptography.X509Certificates;
    using Microsoft.VisualStudio.TestPlatform.ObjectModel.Client;
    using Spekt.TestLogger.Platform;
    using Spekt.TestLogger.Utilities;

    public static class TestRunCompleteWorkflow
    {
        public static void Complete(this ITestRun testRun, TestRunCompleteEventArgs completeEvent)
        {
            // Update the test run complete timestamp and run level attachments
            testRun.RunConfiguration.EndTime = DateTime.UtcNow;
            testRun.RunConfiguration.Attachments = completeEvent.AttachmentSets.SelectMany(x => x.ToAttachments()).ToList();

            // Freeze and reset the test result store
            testRun.Store.Pop(out var results, out var messages);

            // Transform the results with adapter specific hooks
            var transformedResults = results;
            if (transformedResults.Any())
            {
                var executorUri = transformedResults[0].ExecutorUri;
                var adapter = testRun.AdapterFactory.CreateTestAdapter(executorUri);
                transformedResults = adapter.TransformResults(results, messages);
            }

            // Prepare test results file from logger configuration
            var logFilePath = testRun.LoggerConfiguration
                .GetFormattedLogFilePath(testRun.RunConfiguration);
            CreateResultsDirectory(testRun.FileSystem, Path.GetDirectoryName(logFilePath));

            var content = testRun.Serializer.Serialize(
                testRun.LoggerConfiguration,
                testRun.RunConfiguration,
                transformedResults,
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
