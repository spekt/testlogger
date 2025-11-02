// Copyright (c) Spekt Contributors. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

namespace Spekt.TestLogger.UnitTests.Utilities
{
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

            var attachment = artifact.ToAttachment();

            Assert.AreEqual("/tmp/x.txt", attachment.FilePath);
            Assert.AreEqual("x", attachment.Description);
        }

        [TestMethod]
        public void ToAttachmentsShouldConvertFileArtifactProperties()
        {
            var artifact = new FileArtifactProperty(new FileInfo("/tmp/y.txt"), "ydisplayname", "y");
            var artifacts = new[] { artifact };

            var attachments = artifacts.ToAttachments().ToArray();

            Assert.AreEqual(1, attachments.Length);
            Assert.AreEqual("/tmp/y.txt", attachments[0].FilePath);
            Assert.AreEqual("y", attachments[0].Description);
        }
    }
}
