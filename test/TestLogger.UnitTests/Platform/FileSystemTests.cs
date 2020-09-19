// Copyright (c) Spekt Contributors. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

namespace Spekt.TestLogger.UnitTests.Platform
{
    using System;
    using Microsoft.VisualStudio.TestTools.UnitTesting;
    using Spekt.TestLogger.Platform;

    [TestClass]
    public class FileSystemTests
    {
        [TestMethod]
        public void FileSystemWriteShouldThrowNotImplementedException()
        {
            var fs = new FileSystem();
            Assert.ThrowsException<NotImplementedException>(() => fs.Write(string.Empty));
        }

        [TestMethod]
        public void GetFullPathShouldReturnAbsolutePath()
        {
            var fs = new FileSystem();

            Assert.AreEqual("/tmp", fs.GetFullPath("/tmp/xx/.."));
        }
    }
}