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
    using Xunit;

    public class TestResultsXmlTests : IClassFixture<TestRunFixture>
    {
        private const string AssembliesElement = @"/assemblies";
        private const string AssemblyElement = @"/assemblies/assembly";
        private const string CollectionElement = @"/assemblies/assembly/collection";
        private const string TotalTestsCount = "9";
        private const string TotalPassingTestsCount = "6";
        private const int TotalTestClassesCount = 5;

        [Theory]
        [InlineData("test-results-vstest.xml")]
        [InlineData("test-results-mtp.xml")]
        public void OnlyOneAssembliesElementShouldExists(string resultFileName)
        {
            var testResultsXmlDocument = this.LoadTestResultsXml(resultFileName);
            var assembliesNodes = testResultsXmlDocument.SelectNodes(TestResultsXmlTests.AssembliesElement);

            Assert.True(assembliesNodes.Count == 1);
        }

        [Theory]
        [InlineData("test-results-vstest.xml")]
        [InlineData("test-results-mtp.xml")]
        public void AssembliesElementShouldHaveTimestampAttribute(string resultFileName)
        {
            var testResultsXmlDocument = this.LoadTestResultsXml(resultFileName);
            var assembliesNodes = testResultsXmlDocument.SelectSingleNode(TestResultsXmlTests.AssembliesElement);

            Assert.NotNull(assembliesNodes.Attributes["timestamp"]);
        }

        [Theory]
        [InlineData("test-results-vstest.xml")]
        [InlineData("test-results-mtp.xml")]
        public void AssembliesElementTimestampAttributeShouldHaveValidTimestamp(string resultFileName)
        {
            var testResultsXmlDocument = this.LoadTestResultsXml(resultFileName);
            var assembliesNodes = testResultsXmlDocument.SelectSingleNode(TestResultsXmlTests.AssembliesElement);

            // Should not throw FormatException.
            var timestamp = assembliesNodes.Attributes["timestamp"].Value;
            Convert.ToDateTime(timestamp, CultureInfo.InvariantCulture);
        }

        [Theory]
        [InlineData("test-results-vstest.xml")]
        [InlineData("test-results-mtp.xml")]
        public void AssembliesElementTimestampAttributeValueShouldHaveCertainFormat(string resultFileName)
        {
            var testResultsXmlDocument = this.LoadTestResultsXml(resultFileName);
            XmlNode assembliesNodes = testResultsXmlDocument.SelectSingleNode(TestResultsXmlTests.AssembliesElement);

            string timestampString = assembliesNodes.Attributes["timestamp"].Value;
            Regex regex = new Regex(@"^\d{2,2}/\d{2,2}/\d{4,4} \d{2,2}:\d{2,2}:\d{2,2}$");

            Assert.Matches(regex, timestampString);
        }

        [Theory]
        [InlineData("test-results-vstest.xml")]
        [InlineData("test-results-mtp.xml")]
        public void AssemblyElementShouldPresent(string resultFileName)
        {
            var testResultsXmlDocument = this.LoadTestResultsXml(resultFileName);
            var assemblyNodes = testResultsXmlDocument.SelectNodes(TestResultsXmlTests.AssemblyElement);

            Assert.True(assemblyNodes.Count == 1);
        }

        [Theory]
        [InlineData("test-results-vstest.xml")]
        [InlineData("test-results-mtp.xml")]
        public void AssemblyElementNameAttributeShouldHaveValueRootedPathToAssembly(string resultFileName)
        {
            var testResultsXmlDocument = this.LoadTestResultsXml(resultFileName);
            XmlNode assemblyNode = testResultsXmlDocument.SelectSingleNode(TestResultsXmlTests.AssemblyElement);

            XmlAttribute nameAttribute = assemblyNode.Attributes["name"];
            Assert.NotNull(nameAttribute);

            string nameValue = nameAttribute.Value;

            // We cannot assert the file exists because the tests cleanup previous build outputs.
            // Assert.True(File.Exists(nameValue), "File does not exist: " + nameValue);
            Assert.True(Path.IsPathRooted(nameValue), "Path is not rooted: " + nameValue);

            Assert.Equal("Xunit.Xml.TestLogger.NetCore.Tests.dll", Path.GetFileName(nameValue));
        }

        [Theory]
        [InlineData("test-results-vstest.xml")]
        [InlineData("test-results-mtp.xml")]
        public void AssemblyElementRunDateAttributeShouldHaveValidFormatDate(string resultFileName)
        {
            var testResultsXmlDocument = this.LoadTestResultsXml(resultFileName);
            XmlNode assemblyNode = testResultsXmlDocument.SelectSingleNode(TestResultsXmlTests.AssemblyElement);

            XmlAttribute runDateAttribute = assemblyNode.Attributes["run-date"];
            Assert.NotNull(runDateAttribute);

            string runDateValue = runDateAttribute.Value;
            Regex regex = new Regex(@"^\d{4,4}-\d{2,2}-\d{2,2}$");

            Assert.Matches(regex, runDateValue);
        }

        [Theory]
        [InlineData("test-results-vstest.xml")]
        [InlineData("test-results-mtp.xml")]
        public void AssemblyElementRunDateAttributeShouldHaveValidDateValue(string resultFileName)
        {
            var testResultsXmlDocument = this.LoadTestResultsXml(resultFileName);
            XmlNode assemblyNode = testResultsXmlDocument.SelectSingleNode(TestResultsXmlTests.AssemblyElement);

            XmlAttribute runTimeAttribute = assemblyNode.Attributes["run-time"];
            Assert.NotNull(runTimeAttribute);

            string runTimeValue = runTimeAttribute.Value;
            Regex regex = new Regex(@"^\d{2,2}:\d{2,2}:\d{2,2}$");

            Assert.Matches(regex, runTimeValue);
        }

        [Theory]
        [InlineData("test-results-vstest.xml")]
        [InlineData("test-results-mtp.xml")]
        public void AssemblyElementTotalAttributeShouldValueEqualToNumberOfTotalTests(string resultFileName)
        {
            var testResultsXmlDocument = this.LoadTestResultsXml(resultFileName);
            XmlNode assemblyNode = testResultsXmlDocument.SelectSingleNode(TestResultsXmlTests.AssemblyElement);

            Assert.Equal(TotalTestsCount, assemblyNode.Attributes["total"].Value);
        }

        [Theory]
        [InlineData("test-results-vstest.xml")]
        [InlineData("test-results-mtp.xml")]
        public void AssemblyElementPassedAttributeShouldValueEqualToNumberOfPassedTests(string resultFileName)
        {
            var testResultsXmlDocument = this.LoadTestResultsXml(resultFileName);
            XmlNode assemblyNode = testResultsXmlDocument.SelectSingleNode(TestResultsXmlTests.AssemblyElement);

            Assert.Equal(TotalPassingTestsCount, assemblyNode.Attributes["passed"].Value);
        }

        [Theory]
        [InlineData("test-results-vstest.xml")]
        [InlineData("test-results-mtp.xml")]
        public void AssemblyElementFailedAttributeShouldHaveValueEqualToNumberOfFailedTests(string resultFileName)
        {
            var testResultsXmlDocument = this.LoadTestResultsXml(resultFileName);
            XmlNode assemblyNode = testResultsXmlDocument.SelectSingleNode(TestResultsXmlTests.AssemblyElement);

            Assert.Equal("2", assemblyNode.Attributes["failed"].Value);
        }

        [Theory]
        [InlineData("test-results-vstest.xml")]
        [InlineData("test-results-mtp.xml")]
        public void AssemblyElementSkippedAttributeShouldHaveValueEqualToNumberOfSkippedTests(string resultFileName)
        {
            var testResultsXmlDocument = this.LoadTestResultsXml(resultFileName);
            XmlNode assemblyNode = testResultsXmlDocument.SelectSingleNode(TestResultsXmlTests.AssemblyElement);

            Assert.Equal("1", assemblyNode.Attributes["skipped"].Value);
        }

        [Theory]
        [InlineData("test-results-vstest.xml")]
        [InlineData("test-results-mtp.xml")]
        public void AssemblyElementErrorsAttributeShouldHaveValueEqualToNumberOfErrors(string resultFileName)
        {
            var testResultsXmlDocument = this.LoadTestResultsXml(resultFileName);
            XmlNode assemblyNode = testResultsXmlDocument.SelectSingleNode(TestResultsXmlTests.AssemblyElement);

            Assert.Equal("0", assemblyNode.Attributes["errors"].Value);
        }

        [Theory]
        [InlineData("test-results-vstest.xml")]
        [InlineData("test-results-mtp.xml")]
        public void AssemblyElementTimeAttributeShouldHaveValidFormatValue(string resultFileName)
        {
            var testResultsXmlDocument = this.LoadTestResultsXml(resultFileName);
            XmlNode assemblyNode = testResultsXmlDocument.SelectSingleNode(TestResultsXmlTests.AssemblyElement);

            Regex regex = new Regex(@"^\d{1,}\.\d{3,3}$");
            Assert.Matches(regex, assemblyNode.Attributes["time"].Value);
        }

        [Theory]
        [InlineData("test-results-vstest.xml")]
        [InlineData("test-results-mtp.xml")]
        public void ErrorsElementShouldHaveNoError(string resultFileName)
        {
            var testResultsXmlDocument = this.LoadTestResultsXml(resultFileName);
            XmlNode assemblyNode = testResultsXmlDocument.SelectSingleNode(TestResultsXmlTests.AssemblyElement);

            XmlNode errorsNode = assemblyNode.SelectSingleNode("errors");

            Assert.Equal(string.Empty, errorsNode.InnerText);
        }

        [Theory]
        [InlineData("test-results-vstest.xml")]
        [InlineData("test-results-mtp.xml")]
        public void CollectionElementsCountShouldBeTwo(string resultFileName)
        {
            var testResultsXmlDocument = this.LoadTestResultsXml(resultFileName);
            XmlNodeList collectionElementNodeList = testResultsXmlDocument.SelectNodes(TestResultsXmlTests.CollectionElement);

            Assert.Equal(TotalTestClassesCount, collectionElementNodeList.Count);
        }

        [Theory]
        [InlineData("test-results-vstest.xml")]
        [InlineData("test-results-mtp.xml")]
        public void CollectionElementTotalAttributeShouldHaveValueEqualToTotalNumberOfTestsInAClass(string resultFileName)
        {
            var testResultsXmlDocument = this.LoadTestResultsXml(resultFileName);
            XmlNode unitTest1Collection = this.GetUnitTest1Collection(testResultsXmlDocument);

            Assert.Equal("3", unitTest1Collection.Attributes["total"].Value);
        }

        [Theory]
        [InlineData("test-results-vstest.xml")]
        [InlineData("test-results-mtp.xml")]
        public void CollectionElementPassedAttributeShouldHaveValueEqualToPassedTestsInAClass(string resultFileName)
        {
            var testResultsXmlDocument = this.LoadTestResultsXml(resultFileName);
            XmlNode unitTest1Collection = this.GetUnitTest1Collection(testResultsXmlDocument);

            Assert.Equal("1", unitTest1Collection.Attributes["passed"].Value);
        }

        [Theory]
        [InlineData("test-results-vstest.xml")]
        [InlineData("test-results-mtp.xml")]
        public void CollectionElementFailedAttributeShouldHaveValueEqualToFailedTestsInAClass(string resultFileName)
        {
            var testResultsXmlDocument = this.LoadTestResultsXml(resultFileName);
            XmlNode unitTest1Collection = this.GetUnitTest1Collection(testResultsXmlDocument);

            Assert.Equal("1", unitTest1Collection.Attributes["failed"].Value);
        }

        [Theory]
        [InlineData("test-results-vstest.xml")]
        [InlineData("test-results-mtp.xml")]
        public void CollectionElementSkippedAttributeShouldHaveValueEqualToSkippedTestsInAClass(string resultFileName)
        {
            var testResultsXmlDocument = this.LoadTestResultsXml(resultFileName);
            XmlNode unitTest1Collection = this.GetUnitTest1Collection(testResultsXmlDocument);

            Assert.Equal("1", unitTest1Collection.Attributes["skipped"].Value);
        }

        [Theory]
        [InlineData("test-results-vstest.xml")]
        [InlineData("test-results-mtp.xml")]
        public void CollectionElementTimeAttributeShouldHaveValidFormatValue(string resultFileName)
        {
            var testResultsXmlDocument = this.LoadTestResultsXml(resultFileName);
            XmlNode unitTest1Collection = this.GetUnitTest1Collection(testResultsXmlDocument);

            Regex regex = new Regex(@"^\d{1,}\.\d{3,3}$");
            Assert.Matches(regex, unitTest1Collection.Attributes["time"].Value);
        }

        [Theory]
        [InlineData("test-results-vstest.xml")]
        [InlineData("test-results-mtp.xml")]
        public void CollectionElementShouldContainThreeTestsElements(string resultFileName)
        {
            var testResultsXmlDocument = this.LoadTestResultsXml(resultFileName);
            XmlNode unitTest1Collection = this.GetUnitTest1Collection(testResultsXmlDocument);

            Assert.True(unitTest1Collection.SelectNodes("test").Count == 3);
        }

        [Theory]
        [InlineData("test-results-vstest.xml")]
        [InlineData("test-results-mtp.xml")]
        public void TestElementNameAttributeShouldBeEscaped(string resultFileName)
        {
            var testResultsXmlDocument = this.LoadTestResultsXml(resultFileName);
            var testNodes = this.GetTestXmlNodePartial(
                testResultsXmlDocument,
                "UnitTest3",
                @"Xunit.Xml.TestLogger.NetCore.Tests.UnitTest3.TestInvalidName");

            Assert.Equal(
                "Xunit.Xml.TestLogger.NetCore.Tests.UnitTest3.TestInvalidName(input: \"Head\\u0080r\")",
                testNodes.Item(0).Attributes["name"].Value);
        }

        [Theory]
        [InlineData("test-results-vstest.xml")]
        [InlineData("test-results-mtp.xml")]
        public void TestElementTypeAttributeShouldHaveCorrectValue(string resultFileName)
        {
            var testResultsXmlDocument = this.LoadTestResultsXml(resultFileName);
            XmlNode failedTestXmlNode = this.GetATestXmlNode(testResultsXmlDocument);

            Assert.Equal("Xunit.Xml.TestLogger.NetCore.Tests.UnitTest1", failedTestXmlNode.Attributes["type"].Value);
        }

        [Theory]
        [InlineData("test-results-vstest.xml")]
        [InlineData("test-results-mtp.xml")]
        public void TestElementMethodAttributeShouldHaveCorrectValue(string resultFileName)
        {
            var testResultsXmlDocument = this.LoadTestResultsXml(resultFileName);
            XmlNode failedTestXmlNode = this.GetATestXmlNode(testResultsXmlDocument);

            Assert.Equal("FailTest11", failedTestXmlNode.Attributes["method"].Value);
        }

        [Theory]
        [InlineData("test-results-vstest.xml")]
        [InlineData("test-results-mtp.xml")]
        public void TestElementTimeAttributeShouldHaveValidFormatValue(string resultFileName)
        {
            var testResultsXmlDocument = this.LoadTestResultsXml(resultFileName);
            XmlNode failedTestXmlNode = this.GetATestXmlNode(testResultsXmlDocument);

            Regex regex = new Regex(@"^\d{1,}\.\d{7,7}$");

            Assert.Matches(regex, failedTestXmlNode.Attributes["time"].Value);
        }

        [Theory]
        [InlineData("test-results-vstest.xml")]
        [InlineData("test-results-mtp.xml")]
        public void TestElementShouldHaveTraits(string resultFileName)
        {
            var testResultsXmlDocument = this.LoadTestResultsXml(resultFileName);
            XmlNode failedTestXmlNode = this.GetATestXmlNode(testResultsXmlDocument);

            var traits = failedTestXmlNode.SelectSingleNode("traits")?.ChildNodes;
            Assert.NotNull(traits);
            Assert.Equal(1, traits.Count);
            Assert.Equal("Category", traits[0].Attributes["name"].Value);
            Assert.Equal("DummyCategory", traits[0].Attributes["value"].Value);
        }

        [Theory]
        [InlineData("test-results-vstest.xml")]
        [InlineData("test-results-mtp.xml")]
        public void FailedTestElementResultAttributeShouldHaveValueFail(string resultFileName)
        {
            var testResultsXmlDocument = this.LoadTestResultsXml(resultFileName);
            XmlNode failedTestXmlNode = this.GetATestXmlNode(testResultsXmlDocument);

            Assert.Equal("Fail", failedTestXmlNode.Attributes["result"].Value);
        }

        [Theory]
        [InlineData("test-results-vstest.xml")]
        [InlineData("test-results-mtp.xml")]
        public void PassedTestElementResultAttributeShouldHaveValuePass(string resultFileName)
        {
            var testResultsXmlDocument = this.LoadTestResultsXml(resultFileName);
            XmlNode passedTestXmlNode = this.GetATestXmlNode(
                testResultsXmlDocument,
                "UnitTest1",
                "Xunit.Xml.TestLogger.NetCore.Tests.UnitTest1.PassTest11");

            Assert.Equal("Pass", passedTestXmlNode.Attributes["result"].Value);
        }

        [Theory]
        [InlineData("test-results-vstest.xml")]
        [InlineData("test-results-mtp.xml")]
        public void FailedTestElementShouldContainsFailureDetails(string resultFileName)
        {
            var testResultsXmlDocument = this.LoadTestResultsXml(resultFileName);
            XmlNode failedTestXmlNode = this.GetATestXmlNode(testResultsXmlDocument);

            var failureNodeList = failedTestXmlNode.SelectNodes("failure");

            Assert.True(failureNodeList.Count == 1);

            var failureXmlNode = failureNodeList[0];

            var expectedFailureMessage = "Assert.False() Failure" + Environment.NewLine + "Expected: False" +
                                         Environment.NewLine + "Actual:   True";
            Assert.Equal(expectedFailureMessage, failureXmlNode.SelectSingleNode("message").InnerText);

            // Assert.NotEmpty(failureXmlNode.SelectSingleNode("stack-trace").InnerText);
        }

        // [InlineData("test-results-mtp.xml")] Run level messages not supported in MTP
        [Theory]
        [InlineData("test-results-vstest.xml")]
        public void SkippedTestElementShouldContainSkippingReason(string resultFileName)
        {
            var testResultsXmlDocument = this.LoadTestResultsXml(resultFileName);
            XmlNode skippedTestNode = this.GetATestXmlNode(
                testResultsXmlDocument,
                "UnitTest1",
                "Xunit.Xml.TestLogger.NetCore.Tests.UnitTest1.SkipTest11");
            var reasonNodes = skippedTestNode.SelectNodes("reason");

            Assert.Equal(1, reasonNodes.Count);

            var reasonNode = reasonNodes[0].FirstChild;
            Assert.IsType<XmlCDataSection>(reasonNode);

            XmlCDataSection reasonData = (XmlCDataSection)reasonNode;

            string expectedReason = "Skipped";
            Assert.Equal(expectedReason, reasonData.Value);
        }

        [Theory]
        [InlineData("test-results-vstest.xml")]
        [InlineData("test-results-mtp.xml")]
        public void NestedTestClassesShouldBePresent(string resultFileName)
        {
            var testResultsXmlDocument = this.LoadTestResultsXml(resultFileName);
            XmlNode nestedTestNode = this.GetATestXmlNode(
                    testResultsXmlDocument,
                    "ChildUnitNestedTest3332",
                    "Xunit.Xml.TestLogger.NetCore.Tests.ParentUnitNestedTest3332+ChildUnitNestedTest3332.PassTest33321");
            var result = nestedTestNode.Attributes["result"];

            Assert.Equal("Pass", result.Value);
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

            Assert.Equal(1, testNodes.Count);
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
