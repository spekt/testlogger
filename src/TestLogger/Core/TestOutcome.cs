// Copyright (c) Spekt Contributors. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

namespace Spekt.TestLogger.Core
{
    /// <summary>
    /// Represents the outcome of a test.
    /// </summary>
    public enum TestOutcome
    {
        /// <summary>
        /// Test outcome is none.
        /// </summary>
        None = 0,

        /// <summary>
        /// Test passed.
        /// </summary>
        Passed = 1,

        /// <summary>
        /// Test failed.
        /// </summary>
        Failed = 2,

        /// <summary>
        /// Test was skipped.
        /// </summary>
        Skipped = 3,

        /// <summary>
        /// Test was not found.
        /// </summary>
        NotFound = 4
    }
}
