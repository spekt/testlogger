﻿// Copyright (c) Spekt Contributors. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

namespace JUnit.Xml.TestLogger.AcceptanceTests
{
    using System;
    using System.IO;
    using System.Linq;
    using System.Xml.Linq;
    using System.Xml.XPath;
    using global::TestLogger.Fixtures;
    using Microsoft.VisualStudio.TestTools.UnitTesting;
    using Spekt.TestLogger.Core;

    /// <summary>
    /// Acceptance tests evaluate the most recent output of the build.ps1 script, NOT the most
    /// recent build performed by visual studio or dotnet.build
    ///
    /// These acceptance tests look at the specific places output is expected to change because of
    /// the format option specified. Accordingly, these tests cannot protect against other changes
    /// occurring due to the formatting option.
    /// </summary>
    [TestClass]
    public class JUnitTestLoggerFormatOptionsAcceptanceTests
    {
        private const string AssetName = "JUnit.Xml.TestLogger.NetCore.Tests";

        [TestMethod]
        public void FailureBodyFormat_Default_ShouldntStartWithMessage()
        {
            var loggerArgs = "junit;LogFilePath=failure-default-test-results.xml;FailureBodyFormat=Default";

            var resultsFile = DotnetTestFixture
                                .Create()
                                .WithBuild()
                                .Execute("JUnit.Xml.TestLogger.NetCore.Tests", loggerArgs, collectCoverage: false, "failure-default-test-results.xml");

            XDocument resultsXml = XDocument.Load(resultsFile);
            var failures = resultsXml.XPathSelectElements("/testsuites/testsuite")
                .Descendants()
                .Where(x => x.Name.LocalName == "failure")
                .ToList();
            foreach (var failure in failures)
            {
                // Strip new line and carrige return. These may be inconsistent depending on
                // environment settings
                var message = failure.Attribute("message").Value.Replace("\r", string.Empty).Replace("\n", string.Empty);
                var body = failure.Value.Replace("\r", string.Empty).Replace("\n", string.Empty);

                Assert.IsFalse(body.StartsWith(message));
            }

            Assert.IsTrue(new JunitXmlValidator().IsValid(resultsXml));
        }

        [TestMethod]
        public void FailureBodyFormat_Verbose_ShouldNotContainConsoleOut()
        {
            var loggerArgs = "junit;LogFilePath=failure-verbose-test-results.xml;FailureBodyFormat=Default";

            var resultsFile = DotnetTestFixture
                                .Create()
                                .WithBuild()
                                .Execute("JUnit.Xml.TestLogger.NetCore.Tests", loggerArgs, collectCoverage: false, "failure-verbose-test-results.xml");

            XDocument resultsXml = XDocument.Load(resultsFile);
            var failures = resultsXml.XPathSelectElements("/testsuites/testsuite")
                .Descendants()
                .Where(x => x.Name.LocalName == "failure")
                .ToList();
            Assert.AreEqual(0, failures.Count(x => x.Value.Contains("{EEEE1DA6-6296-4486-BDA5-A50A19672F0F}")));
            Assert.AreEqual(0, failures.Count(x => x.Value.Contains("{C33FF4B5-75E1-4882-B968-DF9608BFE7C2}")));
            Assert.IsTrue(new JunitXmlValidator().IsValid(resultsXml));
        }

        [TestMethod]
        public void FailureBodyFormat_Verbose_ShouldStartWithMessage()
        {
            var loggerArgs = "junit;LogFilePath=failure-verbose-test-results.xml;FailureBodyFormat=Verbose";

            var resultsFile = DotnetTestFixture
                                .Create()
                                .WithBuild()
                                .Execute("JUnit.Xml.TestLogger.NetCore.Tests", loggerArgs, collectCoverage: false, "failure-verbose-test-results.xml");

            XDocument resultsXml = XDocument.Load(resultsFile);
            var failures = resultsXml.XPathSelectElements("/testsuites/testsuite")
                .Descendants()
                .Where(x => x.Name.LocalName == "failure")
                .ToList();
            foreach (var failure in failures)
            {
                // Strip new line and carrige return. These may be inconsistent depending on
                // environment settings
                var message = failure.Attribute("message").Value.Replace("\r", string.Empty).Replace("\n", string.Empty);
                var body = failure.Value.Replace("\r", string.Empty).Replace("\n", string.Empty);

                Assert.IsTrue(body.Trim().StartsWith(message.Trim()));
            }

            var validator = new JunitXmlValidator();
            Assert.IsTrue(validator.IsValid(resultsXml));
        }

        [TestMethod]
        public void FailureBodyFormat_Verbose_ShouldContainConsoleOut()
        {
            var loggerArgs = "junit;LogFilePath=failure-verbose-test-results.xml;FailureBodyFormat=Verbose";

            var resultsFile = DotnetTestFixture
                                .Create()
                                .WithBuild()
                                .Execute("JUnit.Xml.TestLogger.NetCore.Tests", loggerArgs, collectCoverage: false, "failure-verbose-test-results.xml");

            XDocument resultsXml = XDocument.Load(resultsFile);
            var failures = resultsXml.XPathSelectElements("/testsuites/testsuite")
                .Descendants()
                .Where(x => x.Name.LocalName == "failure")
                .ToList();
            Assert.AreEqual(1, failures.Count(x => x.Value.Contains("{EEEE1DA6-6296-4486-BDA5-A50A19672F0F}")));
            Assert.AreEqual(1, failures.Count(x => x.Value.Contains("{C33FF4B5-75E1-4882-B968-DF9608BFE7C2}")));
            Assert.IsTrue(new JunitXmlValidator().IsValid(resultsXml));
        }

        [TestMethod]
        public void MethodFormat_Default_ShouldBeOnlyTheMethod()
        {
            var loggerArgs = "junit;LogFilePath=method-default-test-results.xml;MethodFormat=Default";

            var resultsFile = DotnetTestFixture
                                .Create()
                                .WithBuild()
                                .Execute("JUnit.Xml.TestLogger.NetCore.Tests", loggerArgs, collectCoverage: false, "method-default-test-results.xml");

            XDocument resultsXml = XDocument.Load(resultsFile);
            var testcases = resultsXml.XPathSelectElements("/testsuites/testsuite")
                .Descendants()
                .Where(x => x.Name.LocalName == "testcase")
                .ToList();
            foreach (var testcase in testcases)
            {
                var parsedName = new TestCaseNameParser().Parse(testcase.Attribute("name").Value);

                // A method name only will not be parsable into two pieces
                Assert.AreEqual(parsedName.Type, TestCaseNameParser.TestCaseParserUnknownType);
            }

            Assert.IsTrue(new JunitXmlValidator().IsValid(resultsXml));
        }

        [TestMethod]
        public void MethodFormat_Class_ShouldIncludeClass()
        {
            var loggerArgs = "junit;LogFilePath=method-class-test-results.xml;MethodFormat=Class";

            var resultsFile = DotnetTestFixture
                                .Create()
                                .WithBuild()
                                .Execute("JUnit.Xml.TestLogger.NetCore.Tests", loggerArgs, collectCoverage: false, "method-class-test-results.xml");

            XDocument resultsXml = XDocument.Load(resultsFile);
            var testcases = resultsXml.XPathSelectElements("/testsuites/testsuite")
                .Descendants()
                .Where(x => x.Name.LocalName == "testcase")
                .ToList();
            foreach (var testcase in testcases)
            {
                // Note the new parser can't handle the names with just class.method
                var parsedName = new LegacyTestCaseNameParser().Parse(testcase.Attribute("name").Value);

                // If the name is parsable into two pieces, then we have a two piece name and
                // consider that to be a passing result.
                Assert.AreNotEqual(parsedName.Type, TestCaseNameParser.TestCaseParserUnknownType);
            }

            Assert.IsTrue(new JunitXmlValidator().IsValid(resultsXml));
        }

        [TestMethod]
        public void MethodFormat_Full_ShouldIncludeNamespaceAndClass()
        {
            var loggerArgs = "junit;LogFilePath=method-full-test-results.xml;MethodFormat=Full";

            var resultsFile = DotnetTestFixture
                                .Create()
                                .WithBuild()
                                .Execute("JUnit.Xml.TestLogger.NetCore.Tests", loggerArgs, collectCoverage: false, "method-full-test-results.xml");

            XDocument resultsXml = XDocument.Load(resultsFile);
            var testcases = resultsXml.XPathSelectElements("/testsuites/testsuite")
                .Descendants()
                .Where(x => x.Name.LocalName == "testcase")
                .ToList();
            foreach (var testcase in testcases)
            {
                var parsedName = new TestCaseNameParser().Parse(testcase.Attribute("name").Value);

                // We expect the full name would be the class name plus the parsed method
                var expectedFullName = parsedName.Namespace + "." + parsedName.Type + "." + parsedName.Method;

                // If the name is parsable into two pieces, then we have at least a two piece name
                Assert.AreNotEqual(parsedName.Type, TestCaseNameParser.TestCaseParserUnknownType);
                Assert.AreEqual(expectedFullName, testcase.Attribute("name").Value);
            }

            Assert.IsTrue(new JunitXmlValidator().IsValid(resultsXml));
        }
    }
}
