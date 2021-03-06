// Copyright (c) Spekt Contributors. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

namespace Spekt.TestLogger.UnitTests.Platform
{
    using Microsoft.VisualStudio.TestTools.UnitTesting;
    using Spekt.TestLogger.UnitTests.TestDoubles;

    [TestClass]
    public class FakeFileSystemTests : BaseFileSystemTests
    {
        public FakeFileSystemTests()
            : base(new FakeFileSystem())
        {
        }
    }
}