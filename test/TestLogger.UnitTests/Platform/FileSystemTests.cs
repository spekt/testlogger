// Copyright (c) Spekt Contributors. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

namespace Spekt.TestLogger.UnitTests.Platform
{
    using System;
    using System.Runtime.InteropServices;
    using Microsoft.VisualStudio.TestTools.UnitTesting;
    using Spekt.TestLogger.Platform;

    [TestClass]
    public class FileSystemTests
    {
        private readonly IFileSystem fileSystem;

        public FileSystemTests()
        {
            this.fileSystem = new FileSystem();
        }

        [TestMethod]
        public void FileSystemWriteShouldThrowNotImplementedException()
        {
            Assert.ThrowsException<NotImplementedException>(() => this.fileSystem.Write(string.Empty));
        }

        [TestMethod]
        public void GetFullPathShouldReturnAbsolutePath()
        {
            var expectedPath = "/tmp";
            var actualPath = "/tmp/xx/..";
            if (RuntimeInformation.IsOSPlatform(OSPlatform.Windows))
            {
                expectedPath = @"C:\tmp";
                actualPath = @"C:\tmp\xx\..";
            }

            Assert.AreEqual(expectedPath, this.fileSystem.GetFullPath(actualPath));
        }
    }
}