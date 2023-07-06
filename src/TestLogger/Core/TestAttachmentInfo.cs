// Copyright (c) Spekt Contributors. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

namespace Spekt.TestLogger.Core
{
    using System;

    /// <summary>
    /// An attachment for a test suite or a single test.
    /// </summary>
    public class TestAttachmentInfo
    {
        public TestAttachmentInfo(string filePath, string description)
        {
            this.FilePath = filePath ?? throw new ArgumentNullException(nameof(filePath));
            this.Description = description;
        }

        public string FilePath { get; }

        public string Description { get; }
    }
}
