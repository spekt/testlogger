// Copyright (c) Spekt Contributors. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

namespace Spekt.TestLogger.VSTest.Core
{
    using System;
    using Microsoft.VisualStudio.TestPlatform.ObjectModel.Client;
    using Spekt.TestLogger.Core;

    public static class VSTestTestRunBuilderExtensions
    {
        public static void Subscribe(this ITestRun testRun, TestLoggerEvents loggerEvents)
        {
            if (testRun == null)
            {
                throw new ArgumentNullException(nameof(testRun));
            }

            if (loggerEvents == null)
            {
                throw new ArgumentNullException(nameof(loggerEvents));
            }

            loggerEvents.TestRunStart += (_, eventArgs) =>
            {
                testRun.RunConfiguration = testRun.Start(eventArgs);
            };
            loggerEvents.TestRunMessage += (_, eventArgs) => TraceAndThrow(testRun, () => testRun.Message(eventArgs), "TestRunMessage");
            loggerEvents.TestResult += (_, eventArgs) => TraceAndThrow(testRun, () => testRun.Result(eventArgs.Result), "TestResult");
            loggerEvents.TestRunComplete += (_, eventArgs) => TraceAndThrow(testRun, () => testRun.Complete(eventArgs), "TestRunComplete");
        }

        private static void TraceAndThrow(ITestRun testRun, Action action, string source)
        {
            try
            {
                action();
            }
            catch (Exception)
            {
                testRun.ConsoleOutput?.WriteError($"Test Logger: Unexpected error in {source} workflow. Please rerun with `dotnet test --diag:log.txt` to see the stacktrace and report the issue at https://github.com/spekt/testlogger/issues/new.");
                throw;
            }
        }
    }
}
