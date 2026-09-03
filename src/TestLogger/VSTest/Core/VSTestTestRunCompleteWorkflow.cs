// Copyright (c) Spekt Contributors. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

namespace Spekt.TestLogger.VSTest.Core
{
    using System.IO;
    using System.Linq;
    using Microsoft.VisualStudio.TestPlatform.ObjectModel.Client;
    using Spekt.TestLogger.Core;
    using Spekt.TestLogger.VSTest.Utilities;

    public static class VSTestTestRunCompleteWorkflow
    {
        public static void Complete(this ITestRun testRun, TestRunCompleteEventArgs completeEvent)
        {
            var logFilePath = testRun.LoggerConfiguration
                .GetFormattedLogFilePath(testRun.RunConfiguration);
            var resultsDirectory = Path.GetDirectoryName(logFilePath);
            var attachments = completeEvent.AttachmentSets.SelectMany(x => x.ToAttachments(baseDirectory: resultsDirectory, makeRelativePaths: testRun.LoggerConfiguration.UseRelativeAttachmentPaths)).ToList();

            testRun.Store.Pop(out var results, out var messages);

            // Results were already transformed per-result in VSTestTestRunResultWorkflow.Result
            // using the real VSTest TestCase. No batch transformation needed here.
            TestRunCompleteWorkflow.Complete(testRun, attachments, results, messages);
        }
    }
}
