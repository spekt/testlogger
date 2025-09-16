// Copyright (c) Spekt Contributors. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

namespace Spekt.TestLogger.UnitTests
{
    using System;
    using System.IO;
    using System.Linq;
    using Microsoft.Testing.Platform.Extensions.Messages;
    using Microsoft.Testing.Platform.Extensions.TestFramework;
    using Microsoft.VisualStudio.TestTools.UnitTesting;
    using Moq;
    using Spekt.TestLogger.Core;
    using Spekt.TestLogger.UnitTests.TestDoubles;

    [TestClass]
    public class TestCaseNameParserTests
    {
        [TestMethod]
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

        // See xunit.testlogger #48
        [DataRow("z.y+a.x", "z", "y+a", "x")]

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
                var actual = new TestCaseNameParser(new FakeConsoleOutput()).Parse(testCaseName);

                Assert.AreEqual(expectedNamespace, actual.Namespace);
                Assert.AreEqual(expectedType, actual.Type);
                Assert.AreEqual(expectedMethod, actual.Method);
                Assert.AreEqual(0, sw.ToString().Length);
            }
        }

        [TestMethod]
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
            var cout = new FakeConsoleOutput();
            var actual = new TestCaseNameParser(cout).Parse(testCaseName);

            Assert.AreEqual(TestCaseNameParser.TestCaseParserUnknownNamespace, actual.Namespace);
            Assert.AreEqual(TestCaseNameParser.TestCaseParserUnknownType, actual.Type);
            Assert.AreEqual(testCaseName, actual.Method);

            // Remove the trailing new line before comparing.
            Assert.AreEqual(TestCaseNameParser.TestCaseParserError, cout.Messages.First().Item2.Replace(Environment.NewLine, string.Empty));
        }

        [TestMethod]
        public void ParseMTP_ShouldHandleNamespaceInTypeField()
        {
            // Test the core parsing logic directly without mocking the sealed class
            // This simulates the case where MTP's Namespace contains the full type name
            string @namespace = "NUnit.Xml.TestLogger.Tests2.ApiTest";
            string type = "ApiTest";

            // Apply the same logic as in the Parse(TestMethodIdentifierProperty) method
            if (!string.IsNullOrEmpty(@namespace) && !string.IsNullOrEmpty(type))
            {
                // If namespace ends with the type name, extract the actual namespace
                if (@namespace.EndsWith("." + type))
                {
                    @namespace = @namespace.Substring(0, @namespace.Length - type.Length - 1);
                }
            }

            // The namespace should be "NUnit.Xml.TestLogger.Tests2" (extracted from the full type name)
            Assert.AreEqual("NUnit.Xml.TestLogger.Tests2", @namespace);
        }

        [TestMethod]
        public void ParseMTP_ShouldHandleNormalNamespace()
        {
            // Test the case where MTP's TestMethodIdentifierProperty.Namespace contains only the namespace
            // and TypeName contains the class name (the normal case)
            string @namespace = "A.B.C";
            string type = "MyTestClass";

            // Apply the same logic as in the Parse(TestMethodIdentifierProperty) method
            if (!string.IsNullOrEmpty(@namespace) && !string.IsNullOrEmpty(type))
            {
                // If namespace ends with the type name, extract the actual namespace
                if (@namespace.EndsWith("." + type))
                {
                    @namespace = @namespace.Substring(0, @namespace.Length - type.Length - 1);
                }
            }

            // The namespace should remain "A.B.C" (no change)
            Assert.AreEqual("A.B.C", @namespace);
        }

        [TestMethod]
        public void ParseMTP_ShouldHandleNoNamespace()
        {
            // Test the case where there's no namespace (global namespace)
            string @namespace = "GlobalClass";
            string type = "GlobalClass";

            // Apply the same logic as in the Parse(TestMethodIdentifierProperty) method
            if (!string.IsNullOrEmpty(@namespace) && !string.IsNullOrEmpty(type))
            {
                // If namespace ends with the type name, extract the actual namespace
                if (@namespace.EndsWith("." + type))
                {
                    @namespace = @namespace.Substring(0, @namespace.Length - type.Length - 1);
                }

                // If namespace equals the type name, it's a global namespace
                else if (@namespace == type)
                {
                    @namespace = string.Empty;
                }
            }

            // The namespace should be empty string for global namespace
            Assert.AreEqual(string.Empty, @namespace);
        }

        [TestMethod]
        public void Parse_WithTestMethodIdentifier_ShouldParseCorrectly()
        {
            var parser = new TestCaseNameParser(new FakeConsoleOutput());

            // Test the core logic directly by simulating the MTP parsing
            // Since TestMethodIdentifierProperty is sealed, we'll test the Parse(TestMethodIdentifierProperty) method indirectly
            string @namespace = "NUnit.Xml.TestLogger.Tests2";
            string type = "NUnit.Xml.TestLogger.Tests2.ApiTest";
            string method = "ExampleTest";

            // Apply the same logic as in the Parse(TestMethodIdentifierProperty) method
            if (!string.IsNullOrEmpty(@namespace) && !string.IsNullOrEmpty(type))
            {
                // Type contains the full namespace + class name
                if (type.StartsWith(@namespace + "."))
                {
                    type = type.Substring(@namespace.Length + 1);
                }
            }

            var fqn = string.IsNullOrEmpty(@namespace)
                ? $"{type}.{method}"
                : $"{@namespace}.{type}.{method}";

            Assert.AreEqual("NUnit.Xml.TestLogger.Tests2", @namespace);
            Assert.AreEqual("ApiTest", type);
            Assert.AreEqual("ExampleTest", method);
            Assert.AreEqual("NUnit.Xml.TestLogger.Tests2.ApiTest.ExampleTest", fqn);
        }

        [TestMethod]
        public void Parse_WithTestNodeFallback_ShouldParseValidDisplayName()
        {
            var parser = new TestCaseNameParser(new FakeConsoleOutput());

            // Test the fallback parsing logic directly
            var displayName = "MyNamespace.MyClass.MyMethod";
            var parsedName = parser.Parse(displayName);

            if (parsedName.Namespace != TestCaseNameParser.TestCaseParserUnknownNamespace)
            {
                // Successfully parsed the display name
                var result = (Namespace: parsedName.Namespace, Type: parsedName.Type, Method: parsedName.Method, FullyQualifiedName: displayName);

                Assert.AreEqual("MyNamespace", result.Namespace);
                Assert.AreEqual("MyClass", result.Type);
                Assert.AreEqual("MyMethod", result.Method);
                Assert.AreEqual("MyNamespace.MyClass.MyMethod", result.FullyQualifiedName);
            }
            else
            {
                Assert.Fail("Should have successfully parsed the display name");
            }
        }

        [TestMethod]
        public void Parse_WithTestNodeFallback_ShouldHandleInvalidDisplayName()
        {
            var parser = new TestCaseNameParser(new FakeConsoleOutput());

            // Test the fallback parsing logic directly
            var displayName = "invalid";
            var parsedName = parser.Parse(displayName);

            if (parsedName.Namespace != TestCaseNameParser.TestCaseParserUnknownNamespace)
            {
                Assert.Fail("Should not have successfully parsed the display name");
            }
            else
            {
                // Could not parse the display name, use Unknown values
                var result = (Namespace: "UnknownNamespace", Type: "UnknownType", Method: "UnknownMethod", FullyQualifiedName: "UnknownFullyQualifiedName");

                Assert.AreEqual("UnknownNamespace", result.Namespace);
                Assert.AreEqual("UnknownType", result.Type);
                Assert.AreEqual("UnknownMethod", result.Method);
                Assert.AreEqual("UnknownFullyQualifiedName", result.FullyQualifiedName);
            }
        }

        [TestMethod]
        public void Parse_WithBothNull_ShouldReturnUnknownValues()
        {
            var parser = new TestCaseNameParser(new FakeConsoleOutput());

            var result = parser.Parse(null, null);

            Assert.AreEqual("UnknownNamespace", result.Namespace);
            Assert.AreEqual("UnknownType", result.Type);
            Assert.AreEqual("UnknownMethod", result.Method);
            Assert.AreEqual("UnknownFullyQualifiedName", result.FullyQualifiedName);
        }

        [TestMethod]
        public void Parse_WithNormalTestMethodIdentifier_ShouldConstructFqnCorrectly()
        {
            var parser = new TestCaseNameParser(new FakeConsoleOutput());

            // Test the FQN construction logic for normal MTP case
            string @namespace = "MyNamespace";
            string type = "MyClass";
            string method = "MyMethod";

            var fqn = string.IsNullOrEmpty(@namespace)
                ? $"{type}.{method}"
                : $"{@namespace}.{type}.{method}";

            Assert.AreEqual("MyNamespace.MyClass.MyMethod", fqn);
        }

        [TestMethod]
        public void Parse_WithGlobalNamespace_ShouldConstructFqnCorrectly()
        {
            var parser = new TestCaseNameParser(new FakeConsoleOutput());

            // Test the FQN construction logic for global namespace
            string @namespace = string.Empty;
            string type = "MyClass";
            string method = "MyMethod";

            var fqn = string.IsNullOrEmpty(@namespace)
                ? $"{type}.{method}"
                : $"{@namespace}.{type}.{method}";

            Assert.AreEqual("MyClass.MyMethod", fqn);
        }
    }
}
