// Copyright (c) Spekt Contributors. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

namespace Spekt.TestLogger.Mtp.Core
{
    using Microsoft.Testing.Platform.Extensions.Messages;
    using Spekt.TestLogger.Core;

    public static class MtpTestRunMessageWorkflow
    {
        public static void Message(this ITestRun testRun, TestNodeUpdateMessage testNodeUpdateMessage)
        {
            foreach (var property in testNodeUpdateMessage.TestNode.Properties)
            {
#pragma warning disable TPEXP
                if (property is StandardErrorProperty stdErr)
                {
                    testRun.Message(TestMessageLevel.Error, stdErr.StandardError);
                }
                else if (property is StandardOutputProperty stdOut)
                {
                    testRun.Message(TestMessageLevel.Informational, stdOut.StandardOutput);
                }
#pragma warning restore TPEXP
            }
        }
    }
}
