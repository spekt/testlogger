// Copyright (c) Spekt Contributors. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

namespace NUnit.Xml.TestLogger.AcceptanceTests
{
    using System;
    using System.Globalization;
    using System.IO;
    using System.Linq;
    using System.Xml.Linq;
    using System.Xml.XPath;
    using global::TestLogger.Fixtures;
    using Microsoft.VisualStudio.TestTools.UnitTesting;

    [TestClass]
    public class NUnitTestLoggerAcceptanceTests
    {
        private const string AssetName = "NUnit.Xml.TestLogger.NetCore.Tests";
        private const string VstestResultsFile = "test-results-vstest.xml";
        private const string MtpResultsFile = "test-results-mtp.xml";

        [ClassInitialize]
        public static void SuiteInitialize(TestContext context)
        {
            // Run VSTest tests
            var vstestLoggerArgs = "nunit;LogFilePath=test-results-vstest.xml";
            _ = DotnetTestFixture
                .Create()
                .WithBuild()
                .WithRunSettings("-- NUnit.ShowInternalProperties=true")
                .Execute(AssetName, vstestLoggerArgs, collectCoverage: false, "test-results-vstest.xml", isMTP: false);

            // Run MTP tests
            var mtpLoggerArgs = "--report-spekt-nunit --report-spekt-nunit-filename test-results-mtp.xml";
            _ = DotnetTestFixture
                .Create()
                .WithBuild()
                .WithRunSettings("NUnit.ShowInternalProperties=true")
                .Execute(AssetName, mtpLoggerArgs, collectCoverage: false, "test-results-mtp.xml", isMTP: true);
        }

        [TestMethod]
        [DataRow("test-results-vstest.xml")]
        [DataRow("test-results-mtp.xml")]
        public void TestRunWithLoggerAndFilePathShouldCreateResultsFile(string resultFileName)
        {
            var resultsFile = Path.Combine(AssetName.ToAssetDirectoryPath(), resultFileName);
            Assert.IsTrue(File.Exists(resultsFile));
        }

        [TestMethod]
        [DataRow("test-results-vstest.xml")]
        [DataRow("test-results-mtp.xml")]
        public void TestResultFileShouldContainTestRunInformation(string resultFileName)
        {
            var resultsFile = Path.Combine(AssetName.ToAssetDirectoryPath(), resultFileName);
            var resultsXml = XDocument.Load(resultsFile);
            var node = resultsXml.XPathSelectElement("/test-run");

            Assert.IsNotNull(node);
            Assert.AreEqual("55", node.Attribute(XName.Get("testcasecount")).Value);
            Assert.AreEqual("27", node.Attribute(XName.Get("passed")).Value);
            Assert.AreEqual("14", node.Attribute(XName.Get("failed")).Value);

            // MTP reports inconclusive tests as skipped, so handle both cases
            if (resultFileName.Contains("mtp"))
            {
                Assert.AreEqual("0", node.Attribute(XName.Get("inconclusive")).Value);
                Assert.AreEqual("14", node.Attribute(XName.Get("skipped")).Value);
            }
            else
            {
                Assert.AreEqual("6", node.Attribute(XName.Get("inconclusive")).Value);
                Assert.AreEqual("8", node.Attribute(XName.Get("skipped")).Value);
            }

            Assert.AreEqual("Failed", node.Attribute(XName.Get("result")).Value);

            // Start time and End time should be valid dates
            Convert.ToDateTime(node.Attribute(XName.Get("start-time")).Value);
            Convert.ToDateTime(node.Attribute(XName.Get("end-time")).Value);
        }

        [TestMethod]
        [DataRow("test-results-vstest.xml")]
        [DataRow("test-results-mtp.xml")]
        public void TestResultFileShouldContainAssemblyTestSuite(string resultFileName)
        {
            var resultsFile = Path.Combine(AssetName.ToAssetDirectoryPath(), resultFileName);
            var resultsXml = XDocument.Load(resultsFile);
            var node = resultsXml.XPathSelectElement("/test-run/test-suite[@type='Assembly']");

            Assert.IsNotNull(node);
            Assert.AreEqual("55", node.Attribute(XName.Get("total")).Value);
            Assert.AreEqual("27", node.Attribute(XName.Get("passed")).Value);
            Assert.AreEqual("14", node.Attribute(XName.Get("failed")).Value);

            // MTP reports inconclusive tests as skipped, so handle both cases
            if (resultFileName.Contains("mtp"))
            {
                Assert.AreEqual("0", node.Attribute(XName.Get("inconclusive")).Value);
                Assert.AreEqual("14", node.Attribute(XName.Get("skipped")).Value);
            }
            else
            {
                Assert.AreEqual("6", node.Attribute(XName.Get("inconclusive")).Value);
                Assert.AreEqual("8", node.Attribute(XName.Get("skipped")).Value);
            }

            Assert.AreEqual("Failed", node.Attribute(XName.Get("result")).Value);
            Assert.AreEqual("NUnit.Xml.TestLogger.NetCore.Tests.dll", node.Attribute(XName.Get("name")).Value);
            Assert.AreEqual(AssetName.ToAssetAssemblyPath("net8.0"), node.Attribute(XName.Get("fullname")).Value);

            var startTimeStr = node.Attribute(XName.Get("start-time"))?.Value;
            var endTimeStr = node.Attribute(XName.Get("end-time"))?.Value;
            Assert.IsNotNull(startTimeStr);
            Assert.IsNotNull(endTimeStr);

            var startTime = Convert.ToDateTime(startTimeStr);
            var endTime = Convert.ToDateTime(endTimeStr);
            Assert.IsTrue(startTime < endTime, "test suite start time should be before end time");
        }

        [TestMethod]
        [DataRow("test-results-vstest.xml")]
        [DataRow("test-results-mtp.xml")]
        public void TestResultFileShouldContainNamespaceTestSuiteForNetFull(string resultFileName)
        {
            // Two namespaces in test asset are:
            // NUnit.Xml.TestLogger.NetFull.Tests and NUnit.Xml.TestLogger.Tests2
            var query = string.Format("/test-run//test-suite[@type='TestSuite' and @name='NetFull']");
            var resultsFile = Path.Combine(AssetName.ToAssetDirectoryPath(), resultFileName);
            var resultsXml = XDocument.Load(resultsFile);
            var node = resultsXml.XPathSelectElement(query);

            Assert.IsNotNull(node);
            Assert.AreEqual("31", node.Attribute(XName.Get("total")).Value);
            Assert.AreEqual("17", node.Attribute(XName.Get("passed")).Value);
            Assert.AreEqual("7", node.Attribute(XName.Get("failed")).Value);

            // MTP reports inconclusive tests as skipped, so handle both cases
            if (resultFileName.Contains("mtp"))
            {
                Assert.AreEqual("0", node.Attribute(XName.Get("inconclusive")).Value);
                Assert.AreEqual("7", node.Attribute(XName.Get("skipped")).Value);
            }
            else
            {
                Assert.AreEqual("3", node.Attribute(XName.Get("inconclusive")).Value);
                Assert.AreEqual("4", node.Attribute(XName.Get("skipped")).Value);
            }

            Assert.AreEqual("Failed", node.Attribute(XName.Get("result")).Value);
            Assert.AreEqual("NUnit.Xml.TestLogger.NetFull", node.Attribute(XName.Get("fullname")).Value);
        }

        [TestMethod]
        [DataRow("test-results-vstest.xml")]
        [DataRow("test-results-mtp.xml")]
        public void TestResultFileShouldContainNamespaceTestSuiteForTests2(string resultFileName)
        {
            // Two namespaces in test asset are:
            // NUnit.Xml.TestLogger.NetFull.Tests and NUnit.Xml.TestLogger.Tests2
            var query = "/test-run//test-suite[@type='TestSuite' and @name='Tests2']";
            var resultsFile = Path.Combine(AssetName.ToAssetDirectoryPath(), resultFileName);
            var resultsXml = XDocument.Load(resultsFile);
            var node = resultsXml.XPathSelectElement(query);

            Assert.IsNotNull(node);
            Assert.AreEqual("24", node.Attribute(XName.Get("total")).Value);
            Assert.AreEqual("10", node.Attribute(XName.Get("passed")).Value);
            Assert.AreEqual("7", node.Attribute(XName.Get("failed")).Value);

            // MTP reports inconclusive tests as skipped, so handle both cases
            if (resultFileName.Contains("mtp"))
            {
                Assert.AreEqual("0", node.Attribute(XName.Get("inconclusive")).Value);
                Assert.AreEqual("7", node.Attribute(XName.Get("skipped")).Value);
            }
            else
            {
                Assert.AreEqual("3", node.Attribute(XName.Get("inconclusive")).Value);
                Assert.AreEqual("4", node.Attribute(XName.Get("skipped")).Value);
            }

            Assert.AreEqual("Failed", node.Attribute(XName.Get("result")).Value);
            Assert.AreEqual("NUnit.Xml.TestLogger.Tests2", node.Attribute(XName.Get("fullname")).Value);
        }

        [TestMethod]
        [DataRow("test-results-vstest.xml", "NUnit.Xml.TestLogger.NetFull.Tests")]
        [DataRow("test-results-mtp.xml", "NUnit.Xml.TestLogger.NetFull.Tests")]
        [DataRow("test-results-vstest.xml", "NUnit.Xml.TestLogger.Tests2")]
        [DataRow("test-results-mtp.xml", "NUnit.Xml.TestLogger.Tests2")]
        public void TestResultFileShouldContainPartsOfNamespaceTestSuite(string resultFileName, string testNamespace)
        {
            // Two namespaces in test asset are:
            // NUnit.Xml.TestLogger.NetFull.Tests and NUnit.Xml.TestLogger.Tests2
            var fullName = string.Empty;
            var resultsFile = Path.Combine(AssetName.ToAssetDirectoryPath(), resultFileName);
            var resultsXml = XDocument.Load(resultsFile);
            foreach (var part in testNamespace.Split("."))
            {
                var query = $"/test-run//test-suite[@type='TestSuite' and @name='{part}']";
                var node = resultsXml.XPathSelectElement(query);
                fullName = fullName == string.Empty ? part : fullName + "." + part;

                Assert.IsNotNull(node);
                Assert.AreEqual("Failed", node.Attribute(XName.Get("result")).Value);
                Assert.AreEqual(fullName, node.Attribute(XName.Get("fullname")).Value);
            }
        }

        [TestMethod]
        [DataRow("test-results-vstest.xml")]
        [DataRow("test-results-mtp.xml")]
        public void TestResultFileShouldContainTestCasePropertiesForTestWithPropertyAttributes(string resultFileName)
        {
            var testNamespace = "NUnit.Xml.TestLogger.NetFull.Tests";
            var query = $"/test-run//test-case[@fullname='{testNamespace}.UnitTest1.WithProperty']";
            var resultsFile = Path.Combine(AssetName.ToAssetDirectoryPath(), resultFileName);
            var resultsXml = XDocument.Load(resultsFile);
            var testCaseElement = resultsXml.XPathSelectElement(query);
            Assert.IsNotNull(testCaseElement, "test-case element");

            var propertiesElement = testCaseElement.Element("properties");
            Assert.IsNotNull(propertiesElement, "properties element");
            Assert.AreEqual(1, propertiesElement.Descendants().Count());

            var propertyElement = propertiesElement.Element("property");
            Assert.IsNotNull(propertyElement, "property element");
            Assert.AreEqual("Property name", propertyElement.Attribute("name")?.Value);
            Assert.AreEqual("Property value", propertyElement.Attribute("value")?.Value);
        }

        [TestMethod]
        [DataRow("test-results-vstest.xml", "NUnit.Xml.TestLogger.Tests2")]
        [DataRow("test-results-mtp.xml", "NUnit.Xml.TestLogger.Tests2")]
        public void TestResultFileTestCasesShouldContainValidStartAndEndTimes(string resultFileName, string testNamespace)
        {
            var query = $"/test-run//test-case[@fullname='{testNamespace}.UnitTest1.PassTest11']";
            var resultsFile = Path.Combine(AssetName.ToAssetDirectoryPath(), resultFileName);
            var resultsXml = XDocument.Load(resultsFile);
            var testCaseElement = resultsXml.XPathSelectElement(query);
            Assert.IsNotNull(testCaseElement, "test-case element");

            var startTimeStr = testCaseElement.Attribute(XName.Get("start-time"))?.Value;
            var endTimeStr = testCaseElement.Attribute(XName.Get("end-time"))?.Value;
            Assert.IsNotNull(startTimeStr);
            Assert.IsNotNull(endTimeStr);

            string dateFormat = "yyyy-MM-ddTHH:mm:ssZ";
            var startTime = DateTime.ParseExact(startTimeStr, dateFormat, CultureInfo.InvariantCulture);
            var endTime = DateTime.ParseExact(endTimeStr, dateFormat, CultureInfo.InvariantCulture);

            // Assert that start and end times are in the right order and they are recent (i.e. not default DateTime values)
            Assert.IsTrue(startTime < endTime, "test case start time should be before end time");
            var timeDiff = (DateTime.UtcNow - startTime.ToUniversalTime()).Duration();
            Assert.IsTrue(timeDiff < TimeSpan.FromMinutes(1), "test case start time should not be too far in the past, difference was {0}", timeDiff);
        }

        // [DataRow("test-results-mtp.xml")] // NOT SUPPORTED: MTP does not include the Seed property in the test case
        [TestMethod]
        [DataRow("test-results-vstest.xml")]
        public void TestResultFileTestCasesShouldContainSeed(string resultFileName)
        {
            var query = "/test-run//test-case[@fullname='NUnit.Xml.TestLogger.Tests2.RandomizerTests.Sort_RandomData_IsSorted']";
            var resultsFile = Path.Combine(AssetName.ToAssetDirectoryPath(), resultFileName);
            var resultsXml = XDocument.Load(resultsFile);
            var testCaseElement = resultsXml.XPathSelectElement(query);
            Assert.IsNotNull(testCaseElement, "test-case element");

            var seedValue = testCaseElement.Attribute(XName.Get("seed"))?.Value;
            Assert.IsNotNull(seedValue);
        }

        [TestMethod]
        [DataRow("test-results-vstest.xml")]
        [DataRow("test-results-mtp.xml")]
        public void TestResultFileShouldNotContainTestCasePropertiesForTestWithNoPropertyAttributes(string resultFileName)
        {
            var testNamespace = "NUnit.Xml.TestLogger.NetFull.Tests";
            var query = $"/test-run//test-case[@fullname='{testNamespace}.UnitTest1.NoProperty']";
            var resultsFile = Path.Combine(AssetName.ToAssetDirectoryPath(), resultFileName);
            var resultsXml = XDocument.Load(resultsFile);
            var testCaseElement = resultsXml.XPathSelectElement(query);
            Assert.IsNotNull(testCaseElement, "test-case element");

            var propertiesElement = testCaseElement.Element("properties");
            Assert.IsNull(propertiesElement, "properties element");
        }

        [TestMethod]
        [DataRow("test-results-vstest.xml")]
        [DataRow("test-results-mtp.xml")]
        public void TestResultFileShouldContainTestCaseCategoryForTestWithCategory(string resultFileName)
        {
            var query = "/test-run//test-case[@fullname='NUnit.Xml.TestLogger.Tests2.UnitTest1.PassTest11']";
            var resultsFile = Path.Combine(AssetName.ToAssetDirectoryPath(), resultFileName);
            var resultsXml = XDocument.Load(resultsFile);
            var testCaseElement = resultsXml.XPathSelectElement(query);
            Assert.IsNotNull(testCaseElement, "test-case element");

            // MTP doesn't emit Description property
            if (resultFileName.Contains("vstest"))
            {
                var propertiesElement = testCaseElement.Element("properties");
                Assert.IsNotNull(propertiesElement, "properties element");
                Assert.AreEqual(1, propertiesElement.Descendants().Count());

                var propertyElement = propertiesElement.Element("property");
                Assert.IsNotNull(propertyElement, "property element");
                Assert.AreEqual("Description", propertyElement.Attribute("name")?.Value);
            }
        }

        [TestMethod]
        [DataRow("test-results-vstest.xml")]
        [DataRow("test-results-mtp.xml")]
        public void TestResultFileShouldContainTestCaseCategoryForTestWithDescription(string resultFileName)
        {
            var testNamespace = "NUnit.Xml.TestLogger.NetFull.Tests";
            var query = $"/test-run//test-case[@fullname='{testNamespace}.UnitTest1.WithCategory']";
            var resultsFile = Path.Combine(AssetName.ToAssetDirectoryPath(), resultFileName);
            var resultsXml = XDocument.Load(resultsFile);
            var testCaseElement = resultsXml.XPathSelectElement(query);
            Assert.IsNotNull(testCaseElement, "test-case element");

            var propertiesElement = testCaseElement.Element("properties");
            Assert.IsNotNull(propertiesElement, "properties element");
            Assert.AreEqual(1, propertiesElement.Descendants().Count());

            if (resultFileName.Contains("vstest"))
            {
                var propertyElement = propertiesElement.Element("property");
                Assert.IsNotNull(propertyElement, "property element");
                Assert.AreEqual("Category", propertyElement.Attribute("name")?.Value);
                Assert.AreEqual("Nunit Test Category", propertyElement.Attribute("value")?.Value);
            }
        }

        [TestMethod]
        [DataRow("test-results-vstest.xml")]
        [DataRow("test-results-mtp.xml")]
        public void TestResultFileShouldNotContainTestCaseCategoryForTestWithMultipleCategory(string resultFileName)
        {
            var testNamespace = "NUnit.Xml.TestLogger.NetFull.Tests";
            var query = $"/test-run//test-case[@fullname='{testNamespace}.UnitTest1.MultipleCategories']";
            var resultsFile = Path.Combine(AssetName.ToAssetDirectoryPath(), resultFileName);
            var resultsXml = XDocument.Load(resultsFile);
            var testCaseElement = resultsXml.XPathSelectElement(query);
            Assert.IsNotNull(testCaseElement, "test-case element");

            var propertiesElement = testCaseElement.Element("properties");
            Assert.IsNotNull(propertiesElement, "properties element");
            Assert.AreEqual(2, propertiesElement.Descendants().Count());

            // Verify first category for VSTest. MTP category name to Category value.
            var propertyElement = propertiesElement.XPathSelectElement("descendant::property[@value='Category2']");
            if (resultFileName.Contains("vstest"))
            {
                Assert.IsNotNull(propertyElement, "property element");
                Assert.AreEqual("Category", propertyElement.Attribute("name")?.Value);
            }

            // Verify second category
            // MTP emits a category as a property name.
            if (resultFileName.Contains("vstest"))
            {
                propertyElement = propertiesElement.XPathSelectElement("descendant::property[@value='Category1']");
                Assert.IsNotNull(propertyElement, "property element");
                Assert.AreEqual("Category", propertyElement.Attribute("name")?.Value);
                return;
            }
        }

        [TestMethod]
        [DataRow("test-results-vstest.xml")]
        [DataRow("test-results-mtp.xml")]
        public void TestResultFileShouldContainTestCaseCategoryAndPropertyForTestWithMultipleProperties(string resultFileName)
        {
            var testNamespace = "NUnit.Xml.TestLogger.NetFull.Tests";
            var query = $"/test-run//test-case[@fullname='{testNamespace}.UnitTest1.WithCategoryAndProperty']";
            var resultsFile = Path.Combine(AssetName.ToAssetDirectoryPath(), resultFileName);
            var resultsXml = XDocument.Load(resultsFile);
            var testCaseElement = resultsXml.XPathSelectElement(query);
            Assert.IsNotNull(testCaseElement, "test-case element");

            var propertiesElement = testCaseElement.Element("properties");
            Assert.IsNotNull(propertiesElement, "properties element");
            Assert.AreEqual(2, propertiesElement.Descendants().Count());

            // Verify first category
            if (resultFileName.Contains("mtp"))
            {
                // MTP changes the Category property name to CustomProperty
                var propertyElement = propertiesElement.XPathSelectElement("descendant::property[@name='NUnit Test Category']");
                Assert.IsNotNull(propertyElement, "property element is null");
                Assert.IsEmpty(propertyElement.Attribute("value")?.Value);
            }
            else
            {
                var propertyElement = propertiesElement.XPathSelectElement("descendant::property[@name='Category']");
                Assert.IsNotNull(propertyElement, "property element is null");
                Assert.AreEqual("NUnit Test Category", propertyElement.Attribute("value")?.Value);
            }

            // Verify second property
            var propertyElement2 = propertiesElement.XPathSelectElement("descendant::property[@name='Property name']");
            Assert.IsNotNull(propertyElement2, "property element");
            Assert.AreEqual("Property value", propertyElement2.Attribute("value")?.Value);
        }

        [TestMethod]
        [DataRow("test-results-vstest.xml")]
        [DataRow("test-results-mtp.xml")]
        public void TestResultFileShouldContainTestCasePropertyForTestWithMultipleProperties(string resultFileName)
        {
            var testNamespace = "NUnit.Xml.TestLogger.NetFull.Tests";
            var query = $"/test-run//test-case[@fullname='{testNamespace}.UnitTest1.WithProperties']";
            var resultsFile = Path.Combine(AssetName.ToAssetDirectoryPath(), resultFileName);
            var resultsXml = XDocument.Load(resultsFile);
            var testCaseElement = resultsXml.XPathSelectElement(query);
            Assert.IsNotNull(testCaseElement, "test-case element");

            var propertiesElement = testCaseElement.Element("properties");
            Assert.IsNotNull(propertiesElement, "properties element");
            Assert.AreEqual(2, propertiesElement.Descendants().Count());

            // Verify first category
            var propertyElement = propertiesElement.XPathSelectElement("descendant::property[@value='Property value 1']");
            Assert.IsNotNull(propertyElement, "property element");
            Assert.AreEqual("Property name", propertyElement.Attribute("name")?.Value);

            // Verify second property
            propertyElement = propertiesElement.XPathSelectElement("descendant::property[@value='Property value 2']");
            Assert.IsNotNull(propertyElement, "property element");
            Assert.AreEqual("Property name", propertyElement.Attribute("name")?.Value);
        }

        [TestMethod]
        [DataRow("test-results-vstest.xml")]
        [DataRow("test-results-mtp.xml")]
        public void TestResultFileShouldContainTestCaseAttachments(string resultFileName)
        {
            var testNamespace = "NUnit.Xml.TestLogger.NetFull.Tests";
            var query = $"/test-run//test-case[@fullname='{testNamespace}.AttachmentTest.TestAddAttachment']";
            var resultsFile = Path.Combine(AssetName.ToAssetDirectoryPath(), resultFileName);
            var resultsXml = XDocument.Load(resultsFile);
            var testCaseElement = resultsXml.XPathSelectElement(query);
            Assert.IsNotNull(testCaseElement, "test-case element");

            var attachmentsElement = testCaseElement.Element("attachments");
            Assert.IsNotNull(attachmentsElement, "attachments element");
            Assert.AreEqual(3, attachmentsElement.Descendants().Count());

            var attachmentElement = attachmentsElement.Descendants().First();
            Assert.IsNotNull(attachmentElement, "attachment element");
            StringAssert.Contains(attachmentElement.Descendants().First().Value, "x.txt");
            StringAssert.Contains(attachmentElement.Descendants().Last().Value, "description");
        }
    }
}
