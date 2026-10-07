// Copyright (c) Spekt Contributors. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

namespace Xunit.Xml.TestLogger.AcceptanceTests
{
    using System;
    using System.Collections.Generic;
    using System.Globalization;
    using System.IO;
    using System.Linq;
    using System.Linq.Expressions;
    using System.Reflection;
    using System.Text.RegularExpressions;
    using System.Xml;
    using Microsoft.VisualStudio.TestTools.UnitTesting;

    [TestClass]
    [DoNotParallelize]
    public class TestResultsXmlTests
    {
        private const string AssembliesElement = @"/assemblies";
        private const string AssemblyElement = @"/assemblies/assembly";
        private const string CollectionElement = @"/assemblies/assembly/collection";
        private const string TotalTestsCount = "9";
        private const string TotalPassingTestsCount = "6";
        private const int TotalTestClassesCount = 5;

        // Fixtures spawn dotnet test subprocesses that share mutable state
        // (test/assets/global.json selects the test runner per leg), so legs
        // run once here up front and all tests in this class stay sequential.
        [ClassInitialize]
        public static void SuiteInitialize(TestContext context)
        {
            // Run VSTest tests
            var vstestLoggerArgs = "xunit;LogFilePath=test-results-vstest.xml";
            var vstestResultsFile = global::TestLogger.Fixtures.DotnetTestFixture
                .Create()
                .Execute("Xunit.Xml.TestLogger.NetCore.Tests", vstestLoggerArgs, collectCoverage: false, resultsFileName: "test-results-vstest.xml", isMTP: false);

            // Run MTP tests
            var mtpLoggerArgs = "--report-spekt-xunit --report-spekt-xunit-filename test-results-mtp.xml";
            var mtpResultsFile = global::TestLogger.Fixtures.DotnetTestFixture
                .Create()
                .WithNoBuild()
                .Execute("Xunit.Xml.TestLogger.NetCore.Tests", mtpLoggerArgs, collectCoverage: false, resultsFileName: "test-results-mtp.xml", isMTP: true);

            Assert.IsFalse(string.IsNullOrEmpty(vstestResultsFile), "VSTest results file cannot be null");
            Assert.IsFalse(string.IsNullOrEmpty(mtpResultsFile), "MTP results file cannot be null");
        }

        [TestMethod]
        [DataRow("test-results-vstest.xml")]
        [DataRow("test-results-mtp.xml")]
        public void OnlyOneAssembliesElementShouldExists(string resultFileName)
        {
            var testResultsXmlDocument = this.LoadTestResultsXml(resultFileName);
            var assembliesNodes = testResultsXmlDocument.SelectNodes(TestResultsXmlTests.AssembliesElement);

            Assert.IsTrue(assembliesNodes.Count == 1);
        }

        [TestMethod]
        [DataRow("test-results-vstest.xml")]
        [DataRow("test-results-mtp.xml")]
        public void AssembliesElementShouldHaveTimestampAttribute(string resultFileName)
        {
            var testResultsXmlDocument = this.LoadTestResultsXml(resultFileName);
            var assembliesNodes = testResultsXmlDocument.SelectSingleNode(TestResultsXmlTests.AssembliesElement);

            Assert.IsNotNull(assembliesNodes.Attributes["timestamp"]);
        }

        [TestMethod]
        [DataRow("test-results-vstest.xml")]
        [DataRow("test-results-mtp.xml")]
        public void AssembliesElementTimestampAttributeShouldHaveValidTimestamp(string resultFileName)
        {
            var testResultsXmlDocument = this.LoadTestResultsXml(resultFileName);
            var assembliesNodes = testResultsXmlDocument.SelectSingleNode(TestResultsXmlTests.AssembliesElement);

            // Should not throw FormatException.
            var timestamp = assembliesNodes.Attributes["timestamp"].Value;
            Convert.ToDateTime(timestamp, CultureInfo.InvariantCulture);
        }

        [TestMethod]
        [DataRow("test-results-vstest.xml")]
        [DataRow("test-results-mtp.xml")]
        public void AssembliesElementTimestampAttributeValueShouldHaveCertainFormat(string resultFileName)
        {
            var testResultsXmlDocument = this.LoadTestResultsXml(resultFileName);
            XmlNode assembliesNodes = testResultsXmlDocument.SelectSingleNode(TestResultsXmlTests.AssembliesElement);

            string timestampString = assembliesNodes.Attributes["timestamp"].Value;
            Regex regex = new Regex(@"^\d{2,2}/\d{2,2}/\d{4,4} \d{2,2}:\d{2,2}:\d{2,2}$");

            StringAssert.Matches(timestampString, regex);
        }

        [TestMethod]
        [DataRow("test-results-vstest.xml")]
        [DataRow("test-results-mtp.xml")]
        public void AssemblyElementShouldPresent(string resultFileName)
        {
            var testResultsXmlDocument = this.LoadTestResultsXml(resultFileName);
            var assemblyNodes = testResultsXmlDocument.SelectNodes(TestResultsXmlTests.AssemblyElement);

            Assert.IsTrue(assemblyNodes.Count == 1);
        }

        [TestMethod]
        [DataRow("test-results-vstest.xml")]
        [DataRow("test-results-mtp.xml")]
        public void AssemblyElementNameAttributeShouldHaveValueRootedPathToAssembly(string resultFileName)
        {
            var testResultsXmlDocument = this.LoadTestResultsXml(resultFileName);
            XmlNode assemblyNode = testResultsXmlDocument.SelectSingleNode(TestResultsXmlTests.AssemblyElement);

            XmlAttribute nameAttribute = assemblyNode.Attributes["name"];
            Assert.IsNotNull(nameAttribute);

            string nameValue = nameAttribute.Value;

            // We cannot assert the file exists because the tests cleanup previous build outputs.
            // Assert.True(File.Exists(nameValue), "File does not exist: " + nameValue);
            Assert.IsTrue(Path.IsPathRooted(nameValue), "Path is not rooted: " + nameValue);

            Assert.AreEqual("Xunit.Xml.TestLogger.NetCore.Tests.dll", Path.GetFileName(nameValue));
        }

        [TestMethod]
        [DataRow("test-results-vstest.xml")]
        [DataRow("test-results-mtp.xml")]
        public void AssemblyElementRunDateAttributeShouldHaveValidFormatDate(string resultFileName)
        {
            var testResultsXmlDocument = this.LoadTestResultsXml(resultFileName);
            XmlNode assemblyNode = testResultsXmlDocument.SelectSingleNode(TestResultsXmlTests.AssemblyElement);

            XmlAttribute runDateAttribute = assemblyNode.Attributes["run-date"];
            Assert.IsNotNull(runDateAttribute);

            string runDateValue = runDateAttribute.Value;
            Regex regex = new Regex(@"^\d{4,4}-\d{2,2}-\d{2,2}$");

            StringAssert.Matches(runDateValue, regex);
        }

        [TestMethod]
        [DataRow("test-results-vstest.xml")]
        [DataRow("test-results-mtp.xml")]
        public void AssemblyElementRunDateAttributeShouldHaveValidDateValue(string resultFileName)
        {
            var testResultsXmlDocument = this.LoadTestResultsXml(resultFileName);
            XmlNode assemblyNode = testResultsXmlDocument.SelectSingleNode(TestResultsXmlTests.AssemblyElement);

            XmlAttribute runTimeAttribute = assemblyNode.Attributes["run-time"];
            Assert.IsNotNull(runTimeAttribute);

            string runTimeValue = runTimeAttribute.Value;
            Regex regex = new Regex(@"^\d{2,2}:\d{2,2}:\d{2,2}$");

            StringAssert.Matches(runTimeValue, regex);
        }

        [TestMethod]
        [DataRow("test-results-vstest.xml")]
        [DataRow("test-results-mtp.xml")]
        public void AssemblyElementTotalAttributeShouldValueEqualToNumberOfTotalTests(string resultFileName)
        {
            var testResultsXmlDocument = this.LoadTestResultsXml(resultFileName);
            XmlNode assemblyNode = testResultsXmlDocument.SelectSingleNode(TestResultsXmlTests.AssemblyElement);

            Assert.AreEqual(TotalTestsCount, assemblyNode.Attributes["total"].Value);
        }

        [TestMethod]
        [DataRow("test-results-vstest.xml")]
        [DataRow("test-results-mtp.xml")]
        public void AssemblyElementPassedAttributeShouldValueEqualToNumberOfPassedTests(string resultFileName)
        {
            var testResultsXmlDocument = this.LoadTestResultsXml(resultFileName);
            XmlNode assemblyNode = testResultsXmlDocument.SelectSingleNode(TestResultsXmlTests.AssemblyElement);

            Assert.AreEqual(TotalPassingTestsCount, assemblyNode.Attributes["passed"].Value);
        }

        [TestMethod]
        [DataRow("test-results-vstest.xml")]
        [DataRow("test-results-mtp.xml")]
        public void AssemblyElementFailedAttributeShouldHaveValueEqualToNumberOfFailedTests(string resultFileName)
        {
            var testResultsXmlDocument = this.LoadTestResultsXml(resultFileName);
            XmlNode assemblyNode = testResultsXmlDocument.SelectSingleNode(TestResultsXmlTests.AssemblyElement);

            Assert.AreEqual("2", assemblyNode.Attributes["failed"].Value);
        }

        [TestMethod]
        [DataRow("test-results-vstest.xml")]
        [DataRow("test-results-mtp.xml")]
        public void AssemblyElementSkippedAttributeShouldHaveValueEqualToNumberOfSkippedTests(string resultFileName)
        {
            var testResultsXmlDocument = this.LoadTestResultsXml(resultFileName);
            XmlNode assemblyNode = testResultsXmlDocument.SelectSingleNode(TestResultsXmlTests.AssemblyElement);

            Assert.AreEqual("1", assemblyNode.Attributes["skipped"].Value);
        }

        [TestMethod]
        [DataRow("test-results-vstest.xml")]
        [DataRow("test-results-mtp.xml")]
        public void AssemblyElementErrorsAttributeShouldHaveValueEqualToNumberOfErrors(string resultFileName)
        {
            var testResultsXmlDocument = this.LoadTestResultsXml(resultFileName);
            XmlNode assemblyNode = testResultsXmlDocument.SelectSingleNode(TestResultsXmlTests.AssemblyElement);

            Assert.AreEqual("0", assemblyNode.Attributes["errors"].Value);
        }

        [TestMethod]
        [DataRow("test-results-vstest.xml")]
        [DataRow("test-results-mtp.xml")]
        public void AssemblyElementTimeAttributeShouldHaveValidFormatValue(string resultFileName)
        {
            var testResultsXmlDocument = this.LoadTestResultsXml(resultFileName);
            XmlNode assemblyNode = testResultsXmlDocument.SelectSingleNode(TestResultsXmlTests.AssemblyElement);

            Regex regex = new Regex(@"^\d{1,}\.\d{3,3}$");
            StringAssert.Matches(assemblyNode.Attributes["time"].Value, regex);
        }

        [TestMethod]
        [DataRow("test-results-vstest.xml")]
        [DataRow("test-results-mtp.xml")]
        public void ErrorsElementShouldHaveNoError(string resultFileName)
        {
            var testResultsXmlDocument = this.LoadTestResultsXml(resultFileName);
            XmlNode assemblyNode = testResultsXmlDocument.SelectSingleNode(TestResultsXmlTests.AssemblyElement);

            XmlNode errorsNode = assemblyNode.SelectSingleNode("errors");

            Assert.AreEqual(string.Empty, errorsNode.InnerText);
        }

        [TestMethod]
        [DataRow("test-results-vstest.xml")]
        [DataRow("test-results-mtp.xml")]
        public void CollectionElementsCountShouldBeTwo(string resultFileName)
        {
            var testResultsXmlDocument = this.LoadTestResultsXml(resultFileName);
            XmlNodeList collectionElementNodeList = testResultsXmlDocument.SelectNodes(TestResultsXmlTests.CollectionElement);

            Assert.AreEqual(TotalTestClassesCount, collectionElementNodeList.Count);
        }

        [TestMethod]
        [DataRow("test-results-vstest.xml")]
        [DataRow("test-results-mtp.xml")]
        public void CollectionElementTotalAttributeShouldHaveValueEqualToTotalNumberOfTestsInAClass(string resultFileName)
        {
            var testResultsXmlDocument = this.LoadTestResultsXml(resultFileName);
            XmlNode unitTest1Collection = this.GetUnitTest1Collection(testResultsXmlDocument);

            Assert.AreEqual("3", unitTest1Collection.Attributes["total"].Value);
        }

        [TestMethod]
        [DataRow("test-results-vstest.xml")]
        [DataRow("test-results-mtp.xml")]
        public void CollectionElementPassedAttributeShouldHaveValueEqualToPassedTestsInAClass(string resultFileName)
        {
            var testResultsXmlDocument = this.LoadTestResultsXml(resultFileName);
            XmlNode unitTest1Collection = this.GetUnitTest1Collection(testResultsXmlDocument);

            Assert.AreEqual("1", unitTest1Collection.Attributes["passed"].Value);
        }

        [TestMethod]
        [DataRow("test-results-vstest.xml")]
        [DataRow("test-results-mtp.xml")]
        public void CollectionElementFailedAttributeShouldHaveValueEqualToFailedTestsInAClass(string resultFileName)
        {
            var testResultsXmlDocument = this.LoadTestResultsXml(resultFileName);
            XmlNode unitTest1Collection = this.GetUnitTest1Collection(testResultsXmlDocument);

            Assert.AreEqual("1", unitTest1Collection.Attributes["failed"].Value);
        }

        [TestMethod]
        [DataRow("test-results-vstest.xml")]
        [DataRow("test-results-mtp.xml")]
        public void CollectionElementSkippedAttributeShouldHaveValueEqualToSkippedTestsInAClass(string resultFileName)
        {
            var testResultsXmlDocument = this.LoadTestResultsXml(resultFileName);
            XmlNode unitTest1Collection = this.GetUnitTest1Collection(testResultsXmlDocument);

            Assert.AreEqual("1", unitTest1Collection.Attributes["skipped"].Value);
        }

        [TestMethod]
        [DataRow("test-results-vstest.xml")]
        [DataRow("test-results-mtp.xml")]
        public void CollectionElementTimeAttributeShouldHaveValidFormatValue(string resultFileName)
        {
            var testResultsXmlDocument = this.LoadTestResultsXml(resultFileName);
            XmlNode unitTest1Collection = this.GetUnitTest1Collection(testResultsXmlDocument);

            Regex regex = new Regex(@"^\d{1,}\.\d{3,3}$");
            StringAssert.Matches(unitTest1Collection.Attributes["time"].Value, regex);
        }

        [TestMethod]
        [DataRow("test-results-vstest.xml")]
        [DataRow("test-results-mtp.xml")]
        public void CollectionElementShouldContainThreeTestsElements(string resultFileName)
        {
            var testResultsXmlDocument = this.LoadTestResultsXml(resultFileName);
            XmlNode unitTest1Collection = this.GetUnitTest1Collection(testResultsXmlDocument);

            Assert.IsTrue(unitTest1Collection.SelectNodes("test").Count == 3);
        }

        [TestMethod]
        [DataRow("test-results-vstest.xml")]
        [DataRow("test-results-mtp.xml")]
        public void TestElementNameAttributeShouldBeEscaped(string resultFileName)
        {
            var testResultsXmlDocument = this.LoadTestResultsXml(resultFileName);
            var testNodes = this.GetTestXmlNodePartial(
                testResultsXmlDocument,
                "UnitTest3",
                @"Xunit.Xml.TestLogger.NetCore.Tests.UnitTest3.TestInvalidName");

            Assert.AreEqual(
                "Xunit.Xml.TestLogger.NetCore.Tests.UnitTest3.TestInvalidName(input: \"Head\\u0080r\")",
                testNodes.Item(0).Attributes["name"].Value);
        }

        [TestMethod]
        [DataRow("test-results-vstest.xml")]
        [DataRow("test-results-mtp.xml")]
        public void TestElementTypeAttributeShouldHaveCorrectValue(string resultFileName)
        {
            var testResultsXmlDocument = this.LoadTestResultsXml(resultFileName);
            XmlNode failedTestXmlNode = this.GetATestXmlNode(testResultsXmlDocument);

            Assert.AreEqual("Xunit.Xml.TestLogger.NetCore.Tests.UnitTest1", failedTestXmlNode.Attributes["type"].Value);
        }

        [TestMethod]
        [DataRow("test-results-vstest.xml")]
        [DataRow("test-results-mtp.xml")]
        public void TestElementMethodAttributeShouldHaveCorrectValue(string resultFileName)
        {
            var testResultsXmlDocument = this.LoadTestResultsXml(resultFileName);
            XmlNode failedTestXmlNode = this.GetATestXmlNode(testResultsXmlDocument);

            Assert.AreEqual("FailTest11", failedTestXmlNode.Attributes["method"].Value);
        }

        [TestMethod]
        [DataRow("test-results-vstest.xml")]
        [DataRow("test-results-mtp.xml")]
        public void TestElementTimeAttributeShouldHaveValidFormatValue(string resultFileName)
        {
            var testResultsXmlDocument = this.LoadTestResultsXml(resultFileName);
            XmlNode failedTestXmlNode = this.GetATestXmlNode(testResultsXmlDocument);

            Regex regex = new Regex(@"^\d{1,}\.\d{7,7}$");

            StringAssert.Matches(failedTestXmlNode.Attributes["time"].Value, regex);
        }

        [TestMethod]
        [DataRow("test-results-vstest.xml")]
        [DataRow("test-results-mtp.xml")]
        public void TestElementShouldHaveTraits(string resultFileName)
        {
            var testResultsXmlDocument = this.LoadTestResultsXml(resultFileName);
            XmlNode failedTestXmlNode = this.GetATestXmlNode(testResultsXmlDocument);

            var traits = failedTestXmlNode.SelectSingleNode("traits")?.ChildNodes;
            Assert.IsNotNull(traits);
            Assert.AreEqual(1, traits.Count);
            Assert.AreEqual("Category", traits[0].Attributes["name"].Value);
            Assert.AreEqual("DummyCategory", traits[0].Attributes["value"].Value);
        }

        [TestMethod]
        [DataRow("test-results-vstest.xml")]
        [DataRow("test-results-mtp.xml")]
        public void FailedTestElementResultAttributeShouldHaveValueFail(string resultFileName)
        {
            var testResultsXmlDocument = this.LoadTestResultsXml(resultFileName);
            XmlNode failedTestXmlNode = this.GetATestXmlNode(testResultsXmlDocument);

            Assert.AreEqual("Fail", failedTestXmlNode.Attributes["result"].Value);
        }

        [TestMethod]
        [DataRow("test-results-vstest.xml")]
        [DataRow("test-results-mtp.xml")]
        public void PassedTestElementResultAttributeShouldHaveValuePass(string resultFileName)
        {
            var testResultsXmlDocument = this.LoadTestResultsXml(resultFileName);
            XmlNode passedTestXmlNode = this.GetATestXmlNode(
                testResultsXmlDocument,
                "UnitTest1",
                "Xunit.Xml.TestLogger.NetCore.Tests.UnitTest1.PassTest11");

            Assert.AreEqual("Pass", passedTestXmlNode.Attributes["result"].Value);
        }

        [TestMethod]
        [DataRow("test-results-vstest.xml")]
        [DataRow("test-results-mtp.xml")]
        public void FailedTestElementShouldContainsFailureDetails(string resultFileName)
        {
            var testResultsXmlDocument = this.LoadTestResultsXml(resultFileName);
            XmlNode failedTestXmlNode = this.GetATestXmlNode(testResultsXmlDocument);

            var failureNodeList = failedTestXmlNode.SelectNodes("failure");

            Assert.IsTrue(failureNodeList.Count == 1);

            var failureXmlNode = failureNodeList[0];

            var expectedFailureMessage = "Assert.False() Failure" + Environment.NewLine + "Expected: False" +
                                         Environment.NewLine + "Actual:   True";
            Assert.AreEqual(expectedFailureMessage, failureXmlNode.SelectSingleNode("message").InnerText);

            // Assert.NotEmpty(failureXmlNode.SelectSingleNode("stack-trace").InnerText);
        }

        // [InlineData("test-results-mtp.xml")] Run level messages not supported in MTP
        [TestMethod]
        [DataRow("test-results-vstest.xml")]
        public void SkippedTestElementShouldContainSkippingReason(string resultFileName)
        {
            var testResultsXmlDocument = this.LoadTestResultsXml(resultFileName);
            XmlNode skippedTestNode = this.GetATestXmlNode(
                testResultsXmlDocument,
                "UnitTest1",
                "Xunit.Xml.TestLogger.NetCore.Tests.UnitTest1.SkipTest11");
            var reasonNodes = skippedTestNode.SelectNodes("reason");

            Assert.AreEqual(1, reasonNodes.Count);

            var reasonNode = reasonNodes[0].FirstChild;
            Assert.IsInstanceOfType(reasonNode, typeof(XmlText));

            XmlText reasonData = (XmlText)reasonNode;

            string expectedReason = "Skipped";
            Assert.AreEqual(expectedReason, reasonData.Value);
        }

        [TestMethod]
        [DataRow("test-results-vstest.xml")]
        [DataRow("test-results-mtp.xml")]
        public void NestedTestClassesShouldBePresent(string resultFileName)
        {
            var testResultsXmlDocument = this.LoadTestResultsXml(resultFileName);
            XmlNode nestedTestNode = this.GetATestXmlNode(
                    testResultsXmlDocument,
                    "ChildUnitNestedTest3332",
                    "Xunit.Xml.TestLogger.NetCore.Tests.ParentUnitNestedTest3332+ChildUnitNestedTest3332.PassTest33321");
            var result = nestedTestNode.Attributes["result"];

            Assert.AreEqual("Pass", result.Value);
        }

        private XmlNode GetATestXmlNode(
            XmlDocument testResultsXmlDocument,
            string collectionName = "UnitTest1",
            string queryTestName = "Xunit.Xml.TestLogger.NetCore.Tests.UnitTest1.FailTest11")
        {
            var unitTest1Collection = this.GetUnitTestCollection(testResultsXmlDocument, collectionName);

            var testNodes = unitTest1Collection.SelectNodes($"test[@name=\"{queryTestName}\"]");
            return testNodes.Item(0);
        }

        private XmlNodeList GetTestXmlNodePartial(
            XmlDocument testResultsXmlDocument,
            string collectionName,
            string testName)
        {
            var unitTest1Collection = this.GetUnitTestCollection(testResultsXmlDocument, collectionName);

            var testNodes = unitTest1Collection.SelectNodes($"test[contains(@name, \"{testName}\")]");
            return testNodes;
        }

        private XmlNode GetUnitTestCollection(XmlDocument testResultsXmlDocument, string name)
        {
            var testNodes = testResultsXmlDocument.SelectNodes(
                $"//assemblies/assembly/collection[contains(@name, \"{name}\")]");

            Assert.AreEqual(1, testNodes.Count);
            return testNodes.Item(0);
        }

        private XmlNode GetUnitTest1Collection(XmlDocument testResultsXmlDocument)
        {
            return this.GetUnitTestCollection(testResultsXmlDocument, "UnitTest1");
        }

        private XmlDocument LoadTestResultsXml(string resultFileName)
        {
            var currentAssemblyLocation = typeof(TestResultsXmlTests).GetTypeInfo().Assembly.Location;
            var testResultsFilePath = Path.Combine(
                currentAssemblyLocation,
                "..",
                "..",
                "..",
                "..",
                "..",
                "assets",
                "Xunit.Xml.TestLogger.NetCore.Tests",
                resultFileName);
            var xmlDocument = new XmlDocument();
            xmlDocument.Load(testResultsFilePath);
            return xmlDocument;
        }
    }
}
