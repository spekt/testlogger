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