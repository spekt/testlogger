// Copyright (c) Spekt Contributors. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

namespace Spekt.TestLogger.Core
{
    using System;

    /// <summary>
    /// Logger class which writes to the console additional output used for
    /// debugging issues with the logger itself.
    /// </summary>
    public static class DebugLogger
    {
        internal const string DebugLoggerKey = "DebugLogger";
        private static bool debugEnabled;

        /// <summary>
        /// Writes a message to the console if logger debugging flag has been set.
        /// </summary>
        public static void WriteLine(string line)
        {
            if (debugEnabled)
            {
                Console.WriteLine(
                    $"Logger Debugging: " +
                    $"[{DateTime.Now:yyyy-dd-mm HH:mm:ss}] " +
                    $"{line}");
            }
        }

        internal static void EnableLogging()
        {
            debugEnabled = true;
            WriteLine("Logging Enabeled");
        }
    }
}
