// Copyright (c) Spekt Contributors. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

namespace Spekt.TestLogger.UnitTests.Platform
{
    using Microsoft.VisualStudio.TestTools.UnitTesting;
    using Spekt.TestLogger.Platform;

    [TestClass]
    public class FileSystemTests : BaseFileSystemTests
    {
        public FileSystemTests()
            : base(new FileSystem())
        {
        }
    }
}