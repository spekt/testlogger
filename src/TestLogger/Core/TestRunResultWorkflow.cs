// Copyright (c) Spekt Contributors. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

namespace Spekt.TestLogger.Core
{
    using System;
    using Microsoft.VisualStudio.TestPlatform.ObjectModel.Logging;

    public static class TestRunResultWorkflow
    {
        public static void Result(this ITestRun testRun, TestResultEventArgs resultEvent)
        {
            testRun.Store.Add(new TestResultInfo(resultEvent.Result));
#if NONE
            if (TryParseName(result.TestCase.FullyQualifiedName, out var typeName, out var methodName, out _))
            {
                lock (this.resultsGuard)
                {
                    this.results.Add(new TestResultInfo(
                        result,
                        typeName,
                        methodName));
                }
            }
#endif
        }
    }
}