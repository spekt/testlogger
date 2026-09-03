// Copyright (c) Spekt Contributors. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

namespace Spekt.TestLogger.VSTest.Core
{
    using Spekt.TestLogger.Core;

    public static class VSTestTestRunMessageWorkflow
    {
        public static void Message(this ITestRun testRun, Microsoft.VisualStudio.TestPlatform.ObjectModel.Logging.TestRunMessageEventArgs messageEvent)
        {
            var level = (TestMessageLevel)(int)messageEvent.Level;
            testRun.Message(level, messageEvent.Message);
        }
    }
}
