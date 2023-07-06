// Copyright (c) Spekt Contributors. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

namespace Spekt.TestLogger.UnitTests.Utilities
{
    using System;
    using System.Linq;
    using Microsoft.VisualStudio.TestPlatform.ObjectModel;
    using Microsoft.VisualStudio.TestTools.UnitTesting;
    using Spekt.TestLogger.Utilities;

    [TestClass]
    public class AttachmentSetExtensionsTests
    {
        private readonly AttachmentSet attachmentSet;

        public AttachmentSetExtensionsTests()
        {
            this.attachmentSet = new AttachmentSet(new Uri("//dummyUri"), "dummyDescription");
        }

        [TestMethod]
        public void ToAttachmentsShouldConvertLocalPaths()
        {
            this.attachmentSet.Attachments.Add(new UriDataAttachment(new Uri("file:///tmp/x.txt"), "x"));

            var attachments = this.attachmentSet.ToAttachments().ToArray();

            Assert.AreEqual(1, attachments.Length);
            Assert.AreEqual("/tmp/x.txt", attachments[0].FilePath);
            Assert.AreEqual("x", attachments[0].Description);
        }

        [TestMethod]
        public void ToAttachmentsShouldReturnOriginalStringIfPathIsNotWellformed()
        {
            this.attachmentSet.Attachments.Add(new UriDataAttachment(new Uri("/tmp/x.txt", UriKind.Relative), "x"));

            var attachments = this.attachmentSet.ToAttachments().ToArray();

            Assert.AreEqual(1, attachments.Length);
            Assert.AreEqual("/tmp/x.txt", attachments[0].FilePath);
            Assert.AreEqual("x", attachments[0].Description);
        }
    }
}
