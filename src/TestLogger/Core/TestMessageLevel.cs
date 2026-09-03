// Copyright (c) Spekt Contributors. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

namespace Spekt.TestLogger.Core
{
    /// <summary>
    /// Represents the level of a test message.
    /// </summary>
    public enum TestMessageLevel
    {
        /// <summary>
        /// Informational message.
        /// </summary>
        Informational = 0,

        /// <summary>
        /// Warning message.
        /// </summary>
        Warning = 1,

        /// <summary>
        /// Error message.
        /// </summary>
        Error = 2
    }
}
