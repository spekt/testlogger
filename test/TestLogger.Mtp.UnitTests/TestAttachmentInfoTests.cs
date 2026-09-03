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
    }
}