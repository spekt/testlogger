// Copyright (c) Spekt Contributors. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

namespace Spekt.TestLogger.Core
{
    using Microsoft.VisualStudio.TestPlatform.ObjectModel.Logging;

    public static class TestRunMessageWorkflow
    {
        public static void Message(this ITestRun testRun, TestRunMessageEventArgs messageEvent)
            => Message(testRun, messageEvent.Level, messageEvent.Message);

        public static void Message(this ITestRun testRun, TestMessageLevel messageLevel, string message)
        {
            testRun.Store.Add(
                new TestMessageInfo(
                    messageLevel,
                    testRun.Serializer.InputSanitizer.Sanitize(message)));
        }
    }
}