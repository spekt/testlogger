// Copyright (c) Spekt Contributors. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

namespace JUnit.Xml.TestLogger.AcceptanceTests
{
    using System.IO;
    using System.Linq;
    using System.Reflection;
    using System.Runtime.CompilerServices;
    using System.Xml.Linq;
    using System.Xml.XPath;
    using global::TestLogger.Fixtures;
    using Microsoft.VisualStudio.TestTools.UnitTesting;

    /// <summary>
    /// Acceptance tests evaluate the most recent output of the build.ps1 script, NOT the most
    /// recent build performed by visual studio or dotnet.build
    ///
    /// These acceptance tests look at the specific structure and contents of the produced Xml,
    /// when running using the xUnit vstest runner.
    /// </summary>
    [TestClass]
    public class JUnitTestLoggerXunitAcceptanceTests
    {
        private const string AssetName = "JUnit.Xml.TestLogger.XUnit.NetCore.Tests";
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
            var mtpLoggerArgs = $"--report-spekt-junit --report-spekt-junit-filename {MtpResultsFile}";
            _ = DotnetTestFixture
                .Create()
                .WithBuild()
                .Execute(AssetName, mtpLoggerArgs, collectCoverage: false, MtpResultsFile, isMTP: true);
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

        [TestMethod]
        [DataRow("test-results-vstest.xml")]
        [DataRow("test-results-mtp.xml")]
        public void TestResultFileShouldContainXUnitTraitAsProperty(string resultFileName)
        {
            var resultsFile = Path.Combine(AssetName.ToAssetDirectoryPath(), resultFileName);
            var resultsXml = XDocument.Load(resultsFile);
            var properties = resultsXml.XPathSelectElement(
                "/testsuites/testsuite//testcase[@classname=\"NUnit.Xml.TestLogger.Tests2.ApiTest\"]/properties");
            Assert.IsNotNull(properties);
            Assert.AreEqual(2, properties.Nodes().Count());
            foreach (XElement node in properties.Nodes())
            {
                Assert.IsNotNull(node);
                if (node.Attribute("name").Value == "SomeProp")
                {
                    Assert.AreEqual("SomeVal", node.Attribute("value").Value);
                }
                else if (node.Attribute("name").Value == "Category")
                {
                    Assert.AreEqual("ApiTest", node.Attribute("value").Value);
                }
                else
                {
                    Assert.Fail($"Unexpted property found");
                }
            }
        }
    }
}