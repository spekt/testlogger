// Copyright (c) Spekt Contributors. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

namespace Spekt.TestLogger.Core
{
    using System;
    using Microsoft.VisualStudio.TestPlatform.ObjectModel.Logging;

    /// <summary>
    /// A message generated during the test run.
    /// </summary>
    public class TestMessageInfo
    {
        public TestMessageInfo(TestMessageLevel level, string message)
        {
            this.Level = level;
            this.Message = message ?? throw new ArgumentNullException(nameof(message));
        }

        // TODO: Should TestMessageInfo be platform-agnostic?
        // TestMessageLevel is VSTest-specific API.
        public TestMessageLevel Level { get; }

        public string Message { get; }
    }
}