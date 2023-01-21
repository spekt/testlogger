// Copyright (c) Spekt Contributors. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

namespace Spekt.TestLogger.UnitTests
{
    using System;
    using System.IO;
    using Microsoft.VisualStudio.TestTools.UnitTesting;
    using Spekt.TestLogger.Core;

    [TestClass]
    public class TestCaseNameParserTests
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

        // Strip out any line breaks
        [DataRow("z.y.x.ape.bar('aa\r\nbb',False)", "z.y.x", "ape", "bar('aabb',False)")]
        [DataRow("z.y.x.ape.bar('aa\nbb',False)", "z.y.x", "ape", "bar('aabb',False)")]
        [DataRow("z.y.x.ape.bar('aa\rbb',False)", "z.y.x", "ape", "bar('aabb',False)")]
        [DataRow("NetCore.Tests.NetCoreOnly.Issue28_Examples.ExampleTest2(True,4.8m,4.5m,(4.8, False))", "NetCore.Tests.NetCoreOnly", "Issue28_Examples", "ExampleTest2(True,4.8m,4.5m,(4.8, False))")]
        [DataRow("NetCore.Tests.NetCoreOnly.Issue28_Examples(asdf \".\\).ExampleTest2(True,4.8m,4.5m,(4.8, False))", "NetCore.Tests.NetCoreOnly", "Issue28_Examples(asdf \".\\)", "ExampleTest2(True,4.8m,4.5m,(4.8, False))")]
        [DataRow("NetCore.Tests.NetCoreOnly.Issue28_Examples(asdf \".\\).ExampleTest2", "NetCore.Tests.NetCoreOnly", "Issue28_Examples(asdf \".\\)", "ExampleTest2")]
        [DataRow("z.y.X(x", "z", "y", "X(x")]
        [DataRow("z.y.x(", "z", "y", "x(")]
        [DataRow("z.y.X\"x", "z", "y", "X\"x")]
        [DataRow("z.y.x\"", "z", "y", "x\"")]
        [DataRow("z.y.X\\x", "z", "y", "X\\x")]
        [DataRow("z.y.x\\", "z", "y", "x\\")]
        [DataRow("z.y.X\\)", "z", "y", "X\\)")]
        [DataRow("z.y.X\')", "z", "y", "X\')")]
        [DataRow("z.y.\'x", "z", "y", "\'x")]
        [DataRow("z.y.x))", "z", "y", "x))")]
        [DataRow("z.y.x()x", "z", "y", "x()x")]
        [DataRow("z.a.b((0,1)", "z", "a", "b((0,1)")]
        [DataRow("z.a.b((0,1)))", "z", "a", "b((0,1)))")]
        [DataRow("z.a.b((0,(0,1))", "z", "a", "b((0,(0,1))")]
        [DataRow("z.a.b((0,(0,1))))", "z", "a", "b((0,(0,1))))")]
        [DataRow("a.z.y.x.", "a.z", "y", "x.")]
        [DataRow("z.a(0,1).b", "z", "a(0,1)", "b")]
        [DataRow("z.a((0,1).b", "z", "a((0,1)", "b")]
        [DataRow("z.a(0,1)).b", "z", "a(0,1))", "b")]
        [DataRow("z.a(0.21).b", "z", "a(0.21)", "b")]
        [DataRow("z1234567890.a((0,1).b", "z1234567890", "a((0,1)", "b")]
        [DataRow("1234567890z.a(0,1)).b", "1234567890z", "a(0,1))", "b")]
        [DataRow("z.a1234567890(0,1)).b", "z", "a1234567890(0,1))", "b")]
        [DataRow("z.1234567890a(0,1)).b", "z", "1234567890a(0,1))", "b")]
        [DataRow("z.a(0.21).b1234567890", "z", "a(0.21)", "b1234567890")]
        [DataRow("z.a(0.21).1234567890b", "z", "a(0.21)", "1234567890b")]

        // These produce strange results but don't output errors.
        // Will revisit these if users report errors.
        [DataRow("z.y.x.", "z", "y", "x.")]
        [DataRow("z.y.x.)", "z.y", "x", ")")]
        [DataRow("z.y.x.\"\")", "z.y", "x", "\"\")")]
        public void Parse_ParsesAllParseableInputs_WithoutConsoleOutput(string testCaseName, string expectedNamespace, string expectedType, string expectedMethod)
        {
            using (var sw = new StringWriter())
            {
                Console.SetOut(sw);
                var actual = new TestCaseNameParser().Parse(testCaseName);

                Assert.AreEqual(expectedNamespace, actual.Namespace);
                Assert.AreEqual(expectedType, actual.Type);
                Assert.AreEqual(expectedMethod, actual.Method);
                Assert.AreEqual(0, sw.ToString().Length);
            }
        }

        [DataTestMethod]
        [DataRow("a.b")]
        [DataRow("a.b(\"arg\",2)")]
        [DataRow("a(\"arg\",2).b")]
        [DataRow("a(\"arg\",2).b(\"arg\",2)")]
        [DataRow("a.b(0.5f)")]
        [DataRow("x.()x()")]
        [DataRow("x")]
        [DataRow("..Z")]
        public void Parse_FailsGracefullyOnNonParseableInputs_WithConsoleOutput(string testCaseName)
        {
            using var sw = new StringWriter();

            Console.SetOut(sw);
            var actual = new TestCaseNameParser().Parse(testCaseName);

            Assert.AreEqual(TestCaseNameParser.TestCaseParserUnknownNamespace, actual.Namespace);
            Assert.AreEqual(TestCaseNameParser.TestCaseParserUnknownType, actual.Type);
            Assert.AreEqual(testCaseName, actual.Method);

            // Remove the trailing new line before comparing.
            Assert.AreEqual(TestCaseNameParser.TestCaseParserError, sw.ToString().Replace(sw.NewLine, string.Empty));
        }
    }
}
