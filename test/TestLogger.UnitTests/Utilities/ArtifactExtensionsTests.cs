// Copyright (c) Spekt Contributors. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

namespace Spekt.TestLogger.UnitTests.Utilities
{
    using System;
    using System.IO;
    using System.Linq;
    using Microsoft.Testing.Platform.Extensions.Messages;
    using Microsoft.Testing.Platform.TestHost;
    using Microsoft.VisualStudio.TestTools.UnitTesting;
    using Spekt.TestLogger.Utilities;

    [TestClass]
    public class ArtifactExtensionsTests
    {
        [TestMethod]
        public void ToAttachmentsShouldConvertSessionFileArtifacts()
        {
            var artifact = new SessionFileArtifact(new SessionUid("dummySession"), new FileInfo("/tmp/x.txt"), "xdisplayname", "x");

            var attachment = artifact.ToAttachment("/tmp", makeRelativePath: false);

            Assert.AreEqual("/tmp/x.txt", attachment.FilePath);
            Assert.AreEqual("x", attachment.Description);
        }

        [TestMethod]
        public void ToAttachmentsShouldConvertFileArtifactProperties()
        {
            var artifact = new FileArtifactProperty(new FileInfo("/tmp/y.txt"), "ydisplayname", "y");
            var artifacts = new[] { artifact };

            var attachments = artifacts.ToAttachments("/tmp", makeRelativePaths: false).ToArray();

            Assert.AreEqual(1, attachments.Length);
            Assert.AreEqual("/tmp/y.txt", attachments[0].FilePath);
            Assert.AreEqual("y", attachments[0].Description);
        }

        [TestMethod]
        public void ToAttachmentShouldMakePathRelative()
        {
            var artifact = new SessionFileArtifact(new SessionUid("dummySession"), new FileInfo("/user/tests/dll1/bin/Debug/net5.0/attachment.txt"), "attachmentdisplayname", "attachment");

            var attachment = artifact.ToAttachment("/user/tests/testresults", makeRelativePath: true);

            Assert.AreEqual(Path.Combine("..", "dll1", "bin", "Debug", "net5.0", "attachment.txt"), attachment.FilePath);
            Assert.AreEqual("attachment", attachment.Description);
        }

        [TestMethod]
        public void ToAttachmentsShouldMakePathsRelative()
        {
            var artifact = new FileArtifactProperty(new FileInfo("/user/tests/dll2/bin/Debug/net5.0/attachment2.txt"), "attachment2displayname", "attachment2");

            var attachments = new[] { artifact }.ToAttachments("/user/tests/testresults", makeRelativePaths: true).ToArray();

            Assert.AreEqual(1, attachments.Length);
            Assert.AreEqual(Path.Combine("..", "dll2", "bin", "Debug", "net5.0", "attachment2.txt"), attachments[0].FilePath);
            Assert.AreEqual("attachment2", attachments[0].Description);
        }

        [TestMethod]
        [DataRow(@"C:\a\b\c\", @"C:\a\d\e.txt", @"..\..\d\e.txt")]
        [DataRow(@"C:\a\b\c", @"C:\a\d\e.txt", @"..\..\d\e.txt")]
        [DataRow(@"C:\a\b\c", @"D:\a\d\e.txt", @"..\..\d\e.txt")]
        [DataRow(@"/user/tests/testresults/", @"/user/tests/dll1/bin/Debug/net5.0/attachment.txt", @"../dll1/bin/Debug/net5.0/attachment.txt")]
        [DataRow(@"/user/tests/testresults", @"/user/tests/dll1/bin/Debug/net5.0/attachment.txt", @"../dll1/bin/Debug/net5.0/attachment.txt")]
        [DataRow(@"/user/tests/testresults/", @"/user/tests/attachment.txt", @"../attachment.txt")]
        [DataRow(@"/user/tests/testresults/", @"/user/tests/testresults/attachment.txt", @"attachment.txt")]
        [DataRow(@"/user/tests/testresults/", @"attachment.txt", @"attachment.txt")]
        [DataRow(@"/user/tests/testresults/", @"../attachment.txt", @"../attachment.txt")]
        [DataRow(@"/user/tests/testresults/", @"/root/attachment.txt", @"../../../root/attachment.txt")]
        public void MakeRelativePathShouldReturnRelativePath(string basePath, string targetPath, string expectedRelativePath)
        {
            if (!basePath.Contains(Path.DirectorySeparatorChar.ToString()))
            {
                return; // Skip OS incompatible test case
            }

            var relativePath = ArtifactExtensions.MakeRelativePath(basePath, targetPath);

            Assert.AreEqual(expectedRelativePath, relativePath);
        }
    }
}
