// Copyright (c) Spekt Contributors. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

namespace Spekt.TestLogger.UnitTests.Platform
{
    using System;
    using System.IO;
    using System.Runtime.InteropServices;
    using Microsoft.VisualStudio.TestTools.UnitTesting;
    using Spekt.TestLogger.Platform;

    [TestClass]
    public abstract class BaseFileSystemTests
    {
        private readonly IFileSystem fileSystem;

        protected BaseFileSystemTests(IFileSystem fileSystem)
        {
            this.fileSystem = fileSystem;
        }

        [TestMethod]
        public void ReadShouldThrowIfFileDoesNotExist()
        {
            var dummyFile = GetTempFile("dummyFileDoesNotExist.txt");
            Assert.ThrowsException<ArgumentException>(() => this.fileSystem.Read(dummyFile));
        }

        [TestMethod]
        public void ReadShouldReturnContentOfTheFile()
        {
            var dummyFile = GetTempFile("dummyFileRead.txt");
            this.fileSystem.Write(dummyFile, "Dummy content");

            Assert.AreEqual("Dummy content", this.fileSystem.Read(dummyFile));
        }

        [TestMethod]
        public void WriteShouldSaveFileWithContent()
        {
            var dummyFile = GetTempFile("dummyFile.txt");
            this.fileSystem.Delete(dummyFile);

            this.fileSystem.Write(dummyFile, "Dummy content");

            Assert.AreEqual("Dummy content", this.fileSystem.Read(dummyFile));
        }

        [TestMethod]
        public void DeleteShouldRemoveTheFile()
        {
            var dummyFile = GetTempFile("dummyDeleteFile.txt");
            this.fileSystem.Write(dummyFile, "dummyContent");

            this.fileSystem.Delete(dummyFile);

            Assert.ThrowsException<ArgumentException>(() => this.fileSystem.Read(dummyFile));
        }

        private static string GetTempFile(string fileName)
        {
            var tempDir = Path.GetTempPath();
            return Path.Combine(tempDir, fileName);
        }
    }
}