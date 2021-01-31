// Copyright (c) Spekt Contributors. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

namespace Spekt.TestLogger.UnitTests.Utilities
{
     using Microsoft.VisualStudio.TestTools.UnitTesting;
     using Spekt.TestLogger.Utilities;

     [TestClass]
     public class StringExtensionsTests
     {
         [TestMethod]
         [DataRow(null)]
         [DataRow("")]
         public void ReplaceInvalidXmlCharShouldIgnoreEmptyOrNullInput(string input)
         {
             Assert.AreEqual(input, input.ReplaceInvalidXmlChar());
         }

         [TestMethod]
         [DataRow("aa\0\vbb", @"aa\u0000\u000bbb")]
         [DataRow("aa\u0080", @"aa\u0080")]
         public void ReplaceInvalidXmlCharShouldReplaceInvalidXmlCharWithUnicode(string input, string output)
         {
             Assert.AreEqual(output, input.ReplaceInvalidXmlChar());
         }

         [TestMethod]
         public void SubstringAfterDotShouldSplitAndGetLastPartOfString()
         {
             Assert.AreEqual("c", "a.b.c".SubstringAfterDot());
         }

         [TestMethod]
         public void SubstringAfterDotShouldNotSplitIfInputDoesNotHaveDot()
         {
             Assert.AreEqual("abc", "abc".SubstringAfterDot());
         }

         [TestMethod]
         [DataRow(null)]
         [DataRow("")]
         public void SubstringAfterDotShouldReturnEmptyIfInputIsNullOrEmpty(string input)
         {
             Assert.AreEqual(string.Empty, input.SubstringAfterDot());
         }

         [TestMethod]
         public void SubstringBeforeDotShouldSplitAndGetFirstPartOfString()
         {
             Assert.AreEqual("a.b", "a.b.c".SubstringBeforeDot());
         }

         [TestMethod]
         public void SubstringBeforeDotShouldReturnEmptyIfInputDoesNotHaveDot()
         {
             Assert.AreEqual(string.Empty, "c".SubstringBeforeDot());
         }

         [TestMethod]
         [DataRow(null)]
         [DataRow("")]
         public void SubstringBeforeDotShouldReturnEmptyIfInputIsNullOrEmpty(string input)
         {
             Assert.AreEqual(string.Empty, input.SubstringBeforeDot());
         }
     }
}