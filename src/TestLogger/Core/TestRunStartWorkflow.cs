// Copyright (c) Spekt Contributors. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

namespace Spekt.TestLogger.Core
{
    using System;

    public static class TestRunStartWorkflow
    {
        public static TestRunConfiguration Start(this ITestRun testRun, string assemblyPath, string targetFramework)
        {
            return new TestRunConfiguration
            {
                AssemblyPath = assemblyPath,
                TargetFramework = targetFramework,
                StartTime = DateTime.UtcNow
            };
        }
    }
}