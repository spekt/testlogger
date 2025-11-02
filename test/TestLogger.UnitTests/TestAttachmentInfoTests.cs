// Copyright (c) Spekt Contributors. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

namespace Spekt.TestLogger.UnitTests
{
    using System;
    using Microsoft.VisualStudio.TestTools.UnitTesting;
    using Spekt.TestLogger.Core;

    [TestClass]
    public class TestAttachmentInfoTests
    {
        [TestMethod]
        public void TestAttachmentInfoShouldThrowForNullFilePath()
        {
            var action = () => new TestAttachmentInfo(null, "description");

            Assert.ThrowsExactly<ArgumentNullException>(action);
        }

        [TestMethod]
        public void TestAttachmentRelativeUriTest()
        {
            var attachUri = new Uri("file://user/tests/dll1/bin/Debug/net5.0/attachment.txt");
            var resultUri = new Uri("file://user/tests/testresults");

            var relativeAttachUri = resultUri.MakeRelativeUri(attachUri);

            Assert.AreEqual("../../dll1/bin/Debug/net5.0/attachment.txt", relativeAttachUri.ToString());
        }
    }
}