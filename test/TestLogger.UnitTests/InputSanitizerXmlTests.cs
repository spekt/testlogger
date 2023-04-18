// Copyright (c) Spekt Contributors. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

namespace Spekt.TestLogger.UnitTests
{
    using Microsoft.VisualStudio.TestTools.UnitTesting;
    using Spekt.TestLogger.Core;

    [TestClass]
    public class InputSanitizerXmlTests
    {
        [TestMethod]
        [DataRow(null)]
        [DataRow("")]
        public void ReplaceInvalidXmlCharShouldIgnoreEmptyOrNullInput(string input)
        {
            var sut = new InputSanitizerXml();
            Assert.AreEqual(input, sut.Sanitize(input));
        }

        [TestMethod]
        [DataRow("aa\u0080", @"aa\u0080")]
        [DataRow("aa\u0081", @"aa\u0081")]
        [DataRow("aa\0\vbb", @"aa\u0000\u000bbb")]
        [DataRow("aa\uFFFE", @"aa\ufffe")]
        [DataRow("aa\u001F", @"aa\u001f")] // 0x1F from original JUnit logger bug report.
        public void ReplaceInvalidXmlCharShouldReplaceInvalidXmlCharWithUnicode(string input, string output)
        {
            var sut = new InputSanitizerXml();
            Assert.AreEqual(output, sut.Sanitize(input));
        }
    }
}