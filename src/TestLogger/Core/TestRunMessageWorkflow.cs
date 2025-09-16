// Copyright (c) Spekt Contributors. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

namespace Spekt.TestLogger.Core
{
    using Microsoft.Testing.Platform.Extensions.Messages;
    using Microsoft.VisualStudio.TestPlatform.ObjectModel.Logging;

    public static class TestRunMessageWorkflow
    {
        /// <summary>
        /// (MTP) Processes a <see cref="TestNodeUpdateMessage"/> and logs any standard output or error messages to the test run.
        /// </summary>
        /// <param name="testRun">The test run to log messages to.</param>
        /// <param name="testNodeUpdateMessage">The test node update message containing properties to log.</param>
        public static void Message(this ITestRun testRun, TestNodeUpdateMessage testNodeUpdateMessage)
        {
            foreach (var property in testNodeUpdateMessage.TestNode.Properties)
            {
#pragma warning disable TPEXP // Type is for evaluation purposes only and is subject to change or removal in future updates. Suppress this diagnostic to proceed.
                if (property is StandardErrorProperty stdErr)
                {
                    testRun.Message(TestMessageLevel.Error, stdErr.StandardError);
                }
                else if (property is StandardOutputProperty stdOut)
                {
                    testRun.Message(TestMessageLevel.Informational, stdOut.StandardOutput);
                }
#pragma warning restore TPEXP // Type is for evaluation purposes only and is subject to change or removal in future updates. Suppress this diagnostic to proceed.
            }
        }

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