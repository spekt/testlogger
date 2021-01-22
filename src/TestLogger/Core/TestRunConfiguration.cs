// Copyright (c) Spekt Contributors. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

namespace Spekt.TestLogger.Core
{
    /// <summary>
    /// Configuration for the Test Run.
    /// </summary>
    /// <remarks>
    /// Test run configuration represents immutable settings read from the underlying test platform.
    /// See <see cref="TestRunStartWorkflow"/>.
    /// </remarks>
    public class TestRunConfiguration
    {
        public string AssemblyPath { get; init; }

        public string TargetFramework { get; init; }
    }
}