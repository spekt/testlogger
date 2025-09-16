// Copyright (c) Spekt Contributors. All rights reserved.
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

    /// <summary>
    /// Acceptance tests evaluate the most recent output of the build.ps1 script, NOT the most
    /// recent build performed by visual studio or dotnet.build
    ///
    /// These acceptance tests look at the specific structure and contents of the produced Xml.
    /// </summary>
    [TestClass]
    public class JUnitTestLoggerAcceptanceTests
    {
        private const string AssetName = "JUnit.Xml.TestLogger.NetCore.Tests";
        private const string VstestResultsFile = "test-results-vstest.xml";
        private const string MtpResultsFile = "test-results-mtp.xml";

        [ClassInitialize]
        public static void SuiteInitialize(TestContext context)
        {
            // Run VSTest tests
            var vstestLoggerArgs = $"junit;LogFilePath={VstestResultsFile}";
            _ = DotnetTestFixture
                .Create()
                .WithBuild()
                .Execute(AssetName, vstestLoggerArgs, collectCoverage: false, VstestResultsFile, isMTP: false);

            // Run MTP tests
            var mtpLoggerArgs = $"--report-junit --report-junit-filename {MtpResultsFile}";
            _ = DotnetTestFixture
                .Create()
                .WithBuild()
                .Execute(AssetName, mtpLoggerArgs, collectCoverage: false, MtpResultsFile, isMTP: true);
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
        public void TestResultFileShouldContainTestSuitesInformation(string resultFileName)
        {
            var resultsFile = Path.Combine(AssetName.ToAssetDirectoryPath(), resultFileName);
            var resultsXml = XDocument.Load(resultsFile);
            var node = resultsXml.XPathSelectElement("/testsuites");

            Assert.IsNotNull(node);
        }

        [TestMethod]
        [DataRow("test-results-vstest.xml")]
        [DataRow("test-results-mtp.xml")]
        public void TestResultFileShouldContainTestSuiteInformation(string resultFileName)
        {
            var resultsFile = Path.Combine(AssetName.ToAssetDirectoryPath(), resultFileName);
            var resultsXml = XDocument.Load(resultsFile);
            var node = resultsXml.XPathSelectElement("/testsuites/testsuite");

            Assert.IsNotNull(node);
            Assert.AreEqual("JUnit.Xml.TestLogger.NetCore.Tests.dll", node.Attribute(XName.Get("name")).Value);
            Assert.AreEqual(Environment.MachineName, node.Attribute(XName.Get("hostname")).Value);

            // MTP marks Inconclusive tests as Skipped (NUnit sends TestOutcome.None)
            var skipCount = resultFileName == "test-results-mtp.xml" ? 14 : 8;
            Assert.AreEqual("53", node.Attribute(XName.Get("tests")).Value);
            Assert.AreEqual("15", node.Attribute(XName.Get("failures")).Value);
            Assert.AreEqual($"{skipCount}", node.Attribute(XName.Get("skipped")).Value);

            // Errors is zero becasue we don't get errors as a test outcome from .net
            Assert.AreEqual("0", node.Attribute(XName.Get("errors")).Value);

            Convert.ToDouble(node.Attribute(XName.Get("time")).Value);
            Convert.ToDateTime(node.Attribute(XName.Get("timestamp")).Value);
        }

        [TestMethod]
        [DataRow("test-results-vstest.xml")]
        [DataRow("test-results-mtp.xml")]
        public void TestResultFileShouldContainTestCases(string resultFileName)
        {
            var resultsFile = Path.Combine(AssetName.ToAssetDirectoryPath(), resultFileName);
            var resultsXml = XDocument.Load(resultsFile);
            var node = resultsXml.XPathSelectElements("/testsuites/testsuite").Descendants();
            var testcases = node.Where(x => x.Name.LocalName == "testcase").ToList();

            // Check all test cases
            Assert.IsNotNull(node);
            Assert.AreEqual(53, testcases.Count());
            Assert.IsTrue(testcases.All(x => double.TryParse(x.Attribute("time").Value, out _)));

            // Check failures
            var failures = testcases
                .Where(x => x.Descendants().Any(y => y.Name.LocalName == "failure"))
                .ToList();

            Assert.AreEqual(15, failures.Count());
            Assert.IsTrue(failures.All(x => x.Descendants().First().Attribute("type").Value == "failure"));

            // Check failures
            var skips = testcases
                .Where(x => x.Descendants().Any(y => y.Name.LocalName == "skipped"))
                .ToList();

            // MTP marks Inconclusive tests as Skipped (NUnit sends TestOutcome.None)
            var skipCount = resultFileName == MtpResultsFile ? 14 : 8;
            Assert.AreEqual(skipCount, skips.Count());
        }

        [TestMethod]
        [DataRow("test-results-vstest.xml")]
        [DataRow("test-results-mtp.xml")]
        public void TestResultFileShouldContainStandardOut(string resultFileName)
        {
            var resultsFile = Path.Combine(AssetName.ToAssetDirectoryPath(), resultFileName);
            var resultsXml = XDocument.Load(resultsFile);
            var node = resultsXml.XPathSelectElement("/testsuites/testsuite/system-out");

            Assert.Contains("{2010CAE3-7BC0-4841-A5A3-7D5F947BB9FB}", node.Value);
            Assert.Contains("{998AC9EC-7429-42CD-AD55-72037E7AF3D8}", node.Value);
            Assert.Contains("{EEEE1DA6-6296-4486-BDA5-A50A19672F0F}", node.Value);
            Assert.Contains("{C33FF4B5-75E1-4882-B968-DF9608BFE7C2}", node.Value);
        }

        [TestMethod]
        [DataRow("test-results-vstest.xml")]
        [DataRow("test-results-mtp.xml")]
        public void TestResultFileShouldContainErrordOut(string resultFileName)
        {
            var resultsFile = Path.Combine(AssetName.ToAssetDirectoryPath(), resultFileName);
            var resultsXml = XDocument.Load(resultsFile);
            var node = resultsXml.XPathSelectElement("/testsuites/testsuite/system-err");

            Assert.Contains("{D46DFA10-EEDD-49E5-804D-FE43051331A7}", node.Value);
            Assert.Contains("{33F5FD22-6F40-499D-98E4-481D87FAEAA1}", node.Value);
        }

        [TestMethod]
        [DataRow("test-results-vstest.xml")]
        [DataRow("test-results-mtp.xml")]
        public void TestResultFileShouldContainNUnitCategoryAsProperty(string resultFileName)
        {
            var resultsFile = Path.Combine(AssetName.ToAssetDirectoryPath(), resultFileName);
            var resultsXml = XDocument.Load(resultsFile);
            var tesuites = resultsXml.XPathSelectElement("/testsuites/testsuite");
            var testcase = tesuites
                .Nodes()
                .FirstOrDefault(n =>
                {
                    var element = n as XElement;
                    return element.Attribute("classname")?.Value == "JUnit.Xml.TestLogger.NetFull.Tests.UnitTest1" &&
                           element.Attribute("name")?.Value == "WithProperties";
                });
            Assert.IsNotNull(testcase);

            var properties = (testcase as XElement)
                .Nodes()
                .FirstOrDefault(n => (n as XElement)?.Name == "properties") as XElement;
            Assert.IsNotNull(properties);
            Assert.AreEqual(2, properties.Nodes().Count());
            var propertyElements = properties.Nodes().ToList();

            Assert.AreEqual("Property name", (propertyElements[0] as XElement).Attribute("name").Value);
            Assert.IsTrue(propertyElements.Any(p => (p as XElement).Attribute("value").Value == "Property value 1"));
            Assert.IsTrue(propertyElements.Any(p => (p as XElement).Attribute("value").Value == "Property value 2"));
        }

        [TestMethod]
        [DataRow("test-results-vstest.xml")]
        [DataRow("test-results-mtp.xml")]
        public void LoggedXmlValidatesAgainstXsdSchema(string resultFileName)
        {
            var resultsFile = Path.Combine(AssetName.ToAssetDirectoryPath(), resultFileName);
            var validator = new JunitXmlValidator();
            var result = validator.IsValid(File.ReadAllText(resultsFile));
            Assert.IsTrue(result);
        }
    }
}
