// Copyright (c) Spekt Contributors. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

namespace Spekt.TestLogger.UnitTests
{
    using System;
    using System.IO;
    using Microsoft.VisualStudio.TestTools.UnitTesting;
    using Spekt.TestLogger.Core;

    [TestClass]
    public class LegacyTestCaseNameParserTests
    {
        [DataTestMethod]
        [DataRow("z.a.b", "z", "a", "b")]

        // Cover all expected cases of different parenthesis locations, handling normal strings
        [DataRow("z.y.x.a.b(\"arg\",2)", "z.y.x", "a", "b(\"arg\",2)")]
        [DataRow("z.y.x.a(\"arg\",2).b", "z.y.x", "a(\"arg\",2)", "b")]
        [DataRow("z.y.x.a(\"arg\",2).b(\"arg\",2)", "z.y.x", "a(\"arg\",2)", "b(\"arg\",2)")]
        [DataRow("z.y.x.a(\"arg.())(\",2).b", "z.y.x", "a(\"arg.())(\",2)", "b")]

        // Cover select cases with characters in strings that could cause issues
        [DataRow("z.y.x.a.b(\"arg\",\"\\\"\")", "z.y.x", "a", "b(\"arg\",\"\\\"\")")]
        [DataRow("z.y.x.a.b(\"arg\",\")(\")", "z.y.x", "a", "b(\"arg\",\")(\")")]

        // Tests with longer type and method names
        [DataRow("z.y.x.ape.bar(\"ar.g\",\"\\\"\")", "z.y.x", "ape", "bar(\"ar.g\",\"\\\"\")")]
        [DataRow("z.y.x.ape.bar(\"ar.g\",\")(\")", "z.y.x", "ape", "bar(\"ar.g\",\")(\")")]

        // See nunit.testlogger #66.
        [DataRow("z.y.x.ape.bar(a\\b)", "z.y.x", "ape", "bar(a\\b)")]

        // Test with tuple arguments
        [DataRow("z.a.b((0,1))", "z", "a", "b((0,1))")]
        [DataRow("z.a.b((\"arg\",1))", "z", "a", "b((\"arg\",1))")]
        [DataRow("z.a.b((0,1),(2,3))", "z", "a", "b((0,1),(2,3))")]
        [DataRow("z.a.b((0,(0,1)),(0,1))", "z", "a", "b((0,(0,1)),(0,1))")]

        // See nunit.testlogger #90
        [DataRow("z.y.x.ape.bar('A',False)", "z.y.x", "ape", "bar('A',False)")]
        [DataRow("z.y.x.ape.bar('\"',False)", "z.y.x", "ape", "bar('\"',False)")]
        [DataRow("z.y.x.ape.bar('(',False)", "z.y.x", "ape", "bar('(',False)")]
        [DataRow("z.y.x.ape.bar(')',False)", "z.y.x", "ape", "bar(')',False)")]
        [DataRow("z.y.x.ape.bar('.',False)", "z.y.x", "ape", "bar('.',False)")]
        [DataRow("z.y.x.ape.bar('\\'',False)", "z.y.x", "ape", "bar('\\'',False)")]
        [DataRow("z.y.x.ape.bar('\\\\',False)", "z.y.x", "ape", "bar('\\\\',False)")]
        public void Parse_ParsesAllParseableInputs_WithoutConsoleOutput(string testCaseName, string expectedNamespace, string expectedType, string expectedMethod)
        {
            using (var sw = new StringWriter())
            {
                Console.SetOut(sw);
                var actual = new LegacyTestCaseNameParser().Parse(testCaseName);

                Assert.AreEqual(expectedNamespace, actual.Namespace);
                Assert.AreEqual(expectedType, actual.Type);
                Assert.AreEqual(expectedMethod, actual.Method);
                Assert.AreEqual(0, sw.ToString().Length);
            }
        }

        [DataTestMethod]
        [DataRow("a.b", LegacyTestCaseNameParser.TestCaseParserUnknownNamespace, "a", "b")]

        // Cover all expected cases of different parenthesis locations, handling normal strings
        [DataRow("a.b(\"arg\",2)", LegacyTestCaseNameParser.TestCaseParserUnknownNamespace, "a", "b(\"arg\",2)")]
        [DataRow("a(\"arg\",2).b", LegacyTestCaseNameParser.TestCaseParserUnknownNamespace, "a(\"arg\",2)", "b")]
        [DataRow("a(\"arg\",2).b(\"arg\",2)", LegacyTestCaseNameParser.TestCaseParserUnknownNamespace, "a(\"arg\",2)", "b(\"arg\",2)")]

        // Examples with period in non string
        [DataRow("a.b(0.5f)", LegacyTestCaseNameParser.TestCaseParserUnknownNamespace, "a", "b(0.5f)")]
        public void Parse_ParsesAllParseableInputsWithoutNamespace_WithConsoleOutput(string testCaseName, string expectedNamespace, string expectedType, string expectedMethod)
        {
            using var sw = new StringWriter();
            Console.SetOut(sw);
            var actual = new LegacyTestCaseNameParser().Parse(testCaseName);

            Assert.AreEqual(expectedNamespace, actual.Namespace);
            Assert.AreEqual(expectedType, actual.Type);
            Assert.AreEqual(expectedMethod, actual.Method);

            // Remove the trailing new line before comparing.
            Assert.AreEqual(LegacyTestCaseNameParser.TestCaseParserError, sw.ToString().Replace(sw.NewLine, string.Empty));
        }

        [DataTestMethod]
        [DataRow("x.()x()")]
        [DataRow("x")]
        [DataRow("..Z")]
        [DataRow("z.y.X(x")]
        [DataRow("z.y.x(")]
        [DataRow("z.y.X\"x")]
        [DataRow("z.y.x\"")]
        [DataRow("z.y.X\\x")]
        [DataRow("z.y.x\\")]
        [DataRow("z.y.X\\)")]
        [DataRow("z.y.X\')")]
        [DataRow("z.y.\'x")]
        [DataRow("z.y.x))")]
        [DataRow("z.y.x()x")]
        [DataRow("z.y.x.")]
        [DataRow("z.y.x.)")]
        [DataRow("z.y.x.\"\")")]
        [DataRow("z.a.b((0,1)")]
        [DataRow("z.a.b((0,1)))")]
        [DataRow("z.a.b((0,(0,1))")]
        [DataRow("z.a.b((0,(0,1))))")]
        public void Parse_FailsGracefullyOnNonParseableInputs_WithConsoleOutput(string testCaseName)
        {
            using var sw = new StringWriter();
            Console.SetOut(sw);
            var actual = new LegacyTestCaseNameParser().Parse(testCaseName);

            Assert.AreEqual(LegacyTestCaseNameParser.TestCaseParserUnknownNamespace, actual.Namespace);
            Assert.AreEqual(LegacyTestCaseNameParser.TestCaseParserUnknownType, actual.Type);
            Assert.AreEqual(testCaseName, actual.Method);

            // Remove the trailing new line before comparing.
            Assert.AreEqual(LegacyTestCaseNameParser.TestCaseParserError, sw.ToString().Replace(sw.NewLine, string.Empty));
        }
    }
}