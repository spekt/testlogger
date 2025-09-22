// Copyright (c) Spekt Contributors. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

namespace JUnit.Xml.TestLogger.UnitTests
{
    using System;
    using System.Collections.Generic;
    using System.Diagnostics;
    using System.IO;
    using System.Linq;
    using System.Xml.Linq;
    using System.Xml.XPath;
    using Microsoft.VisualStudio.TestPlatform.Extension.Junit.Xml.TestLogger;
    using Microsoft.VisualStudio.TestPlatform.ObjectModel;
    using Microsoft.VisualStudio.TestPlatform.ObjectModel.Logging;
    using Microsoft.VisualStudio.TestTools.UnitTesting;
    using Spekt.TestLogger.Core;
    using Spekt.TestLogger.Utilities;
    using TestSuite = Microsoft.VisualStudio.TestPlatform.Extension.Junit.Xml.TestLogger.JunitXmlSerializer.TestSuite;

    [TestClass]
    public class JUnitXmlTestSerializerTests
    {
        private const string DummyTestResultsDirectory = "/tmp/testresults";
        private const string TestNamespace = "TestNamespace";
        private const string TestClass = "TestClass";
        private const string TestMethod = "TestMethod";
        private const string TestDisplayName = "Test Display Name";
        private const string TestDllPath = "/path/to/test.dll";
        private const string TestCsPath = "/path/to/test.cs";

        [TestMethod]
        public void InitializeShouldThrowIfEventsIsNull()
        {
            Assert.ThrowsExactly<ArgumentNullException>(() => new JUnitXmlTestLogger().Initialize(null, DummyTestResultsDirectory));
        }

        [TestMethod]
        public void CreateTestSuiteShouldReturnEmptyGroupsIfTestSuitesAreExclusive()
        {
            var suite1 = CreateTestSuite("a.b");
            var suite2 = CreateTestSuite("c.d");

            var result = JunitXmlSerializer.GroupTestSuites(new[] { suite1, suite2 }).ToArray();

            Assert.AreEqual(2, result.Length);
            Assert.AreEqual("a", result[0].Name);
            Assert.AreEqual("c", result[1].Name);
        }

        [TestMethod]
        public void CreateTestSuiteShouldGroupTestSuitesByName()
        {
            var suites = new[] { CreateTestSuite("a.b.c"), CreateTestSuite("a.b.e"), CreateTestSuite("c.d") };
            var expectedXmlForA = @"<test-suite type=""TestSuite"" name=""a"" fullname=""a"" total=""10"" passed=""2"" failed=""2"" inconclusive=""2"" skipped=""2"" result=""Failed"" duration=""0""><test-suite type=""TestSuite"" name=""b"" fullname=""a.b"" total=""10"" passed=""2"" failed=""2"" inconclusive=""2"" skipped=""2"" result=""Failed"" duration=""0""><test-suite /><test-suite /></test-suite></test-suite>";
            var expectedXmlForC = @"<test-suite type=""TestSuite"" name=""c"" fullname=""c"" total=""5"" passed=""1"" failed=""1"" inconclusive=""1"" skipped=""1"" result=""Failed"" duration=""0""><test-suite /></test-suite>";

            var result = JunitXmlSerializer.GroupTestSuites(suites).ToArray();

            Assert.AreEqual(2, result.Length);
            Assert.AreEqual("c", result[0].Name);
            Assert.AreEqual(expectedXmlForC, result[0].Element.ToString(SaveOptions.DisableFormatting));
            Assert.AreEqual("a", result[1].Name);
            Assert.AreEqual(expectedXmlForA, result[1].Element.ToString(SaveOptions.DisableFormatting));
        }

        [TestMethod]
        public void TestCaseSystemOutShouldBeSanitized()
        {
            var serializer = new JunitXmlSerializer();
            var result = CreateTestResultInfo(
                messages: new List<TestResultMessage>
                {
                    new TestResultMessage(TestResultMessage.StandardOutCategory, "Console output with <xml> & characters")
                });

            var cdataContent = SerializeAndExtractElementContent(serializer, result, "system-out");
            Assert.AreEqual("Console output with <xml> & characters\n", cdataContent);
        }

        [TestMethod]
        public void TestCaseSystemErrShouldBeSanitized()
        {
            var serializer = new JunitXmlSerializer();
            var result = CreateTestResultInfo(
                messages: new List<TestResultMessage>
                {
                    new TestResultMessage(TestResultMessage.StandardErrorCategory, "Error output with <xml> & characters")
                });

            var cdataContent = SerializeAndExtractElementContent(serializer, result, "system-err");
            Assert.AreEqual("Error output with <xml> & characters\n", cdataContent);
        }

        [TestMethod]
        public void TestSuiteSystemOutShouldBeSanitized()
        {
            var serializer = new JunitXmlSerializer();
            var results = new List<TestResultInfo> { CreateTestResultInfo() };
            var messages = new List<TestMessageInfo>
            {
                new TestMessageInfo(TestMessageLevel.Informational, "Framework info with <xml> & characters")
            };

            var xml = serializer.Serialize(
                CreateTestLoggerConfiguration(),
                CreateTestRunConfiguration(),
                results,
                messages);

            var doc = XDocument.Parse(xml);
            var systemOutElement = doc.XPathSelectElement("//testsuite/system-out");

            Assert.IsNotNull(systemOutElement);
            Assert.IsTrue(systemOutElement.FirstNode is System.Xml.Linq.XText);
            StringAssert.Contains(systemOutElement.Value, "Framework info with <xml> & characters");
        }

        [TestMethod]
        public void TestSuiteSystemErrShouldBeSanitized()
        {
            var serializer = new JunitXmlSerializer();
            var results = new List<TestResultInfo> { CreateTestResultInfo() };
            var messages = new List<TestMessageInfo>
            {
                new TestMessageInfo(TestMessageLevel.Error, "Error message with <xml> & characters")
            };

            var xml = serializer.Serialize(
                CreateTestLoggerConfiguration(),
                CreateTestRunConfiguration(),
                results,
                messages);

            var doc = XDocument.Parse(xml);
            var systemErrElement = doc.XPathSelectElement("//testsuite/system-err");

            Assert.IsNotNull(systemErrElement);
            Assert.IsTrue(systemErrElement.FirstNode is System.Xml.Linq.XText);
            StringAssert.Contains(systemErrElement.Value, "Error - Error message with <xml> & characters");
        }

        private static LoggerConfiguration CreateTestLoggerConfiguration()
        {
            return new LoggerConfiguration(new Dictionary<string, string>
            {
                { LoggerConfiguration.LogFilePathKey, "/tmp/testresults.xml" }
            });
        }

        private static TestRunConfiguration CreateTestRunConfiguration()
        {
            return new TestRunConfiguration { StartTime = DateTime.Now };
        }

        private static TestResultInfo CreateTestResultInfo(TestOutcome outcome = TestOutcome.Passed, List<TestResultMessage> messages = null)
        {
            return new TestResultInfo(
                TestNamespace,
                TestClass,
                TestMethod,
                $"{TestNamespace}.{TestClass}.{TestMethod}",
                outcome,
                TestDisplayName,
                TestDisplayName,
                TestDllPath,
                TestCsPath,
                42,
                DateTime.Now,
                DateTime.Now.AddSeconds(1),
                TimeSpan.FromSeconds(1),
                null,
                null,
                messages ?? new List<TestResultMessage>(),
                new List<TestAttachmentInfo>(),
                new List<Trait>(),
                "executor://dummy",
                null);
        }

        private static string SerializeAndExtractElementContent(JunitXmlSerializer serializer, TestResultInfo result, string elementName)
        {
            var xml = serializer.Serialize(
                CreateTestLoggerConfiguration(),
                CreateTestRunConfiguration(),
                new List<TestResultInfo> { result },
                new List<TestMessageInfo>());

            var doc = XDocument.Parse(xml);
            var testCaseElement = doc.XPathSelectElement("//testcase");
            var targetElement = testCaseElement.Element(elementName);

            Assert.IsNotNull(targetElement, $"Element '{elementName}' not found in testcase");
            Assert.IsTrue(targetElement.FirstNode is System.Xml.Linq.XText, $"Element '{elementName}' should contain text node");

            return targetElement.Value;
        }

        private static TestSuite CreateTestSuite(string name)
        {
            return new TestSuite
            {
                Element = new XElement("test-suite"),
                Name = "n",
                FullName = name,
                Total = 5,
                Passed = 1,
                Failed = 1,
                Inconclusive = 1,
                Skipped = 1,
                Error = 1
            };
        }
    }
}
