// Copyright (c) Spekt Contributors. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

namespace JUnit.Xml.TestLogger.AcceptanceTests
{
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
    /// These acceptance tests look at the specific structure and contents of the produced Xml,
    /// when running using the xUnit vstest runner.
    /// </summary>
    [TestClass]
    public class JUnitTestLoggerXunitAcceptanceTests
    {
        private const string AssetName = "JUnit.Xml.TestLogger.XUnit.NetCore.Tests";
        private readonly string resultsFile;
        private readonly XDocument resultsXml;

        public JUnitTestLoggerXunitAcceptanceTests()
        {
            this.resultsFile = Path.Combine(AssetName.ToAssetDirectoryPath(), "test-results.xml");
            this.resultsXml = XDocument.Load(this.resultsFile);
        }

        [ClassInitialize]
        public static void SuiteInitialize(TestContext context)
        {
            var loggerArgs = "junit;LogFilePath=test-results.xml";

            // Enable reporting of internal properties in the adapter using runsettings
            _ = DotnetTestFixture
                .Create()
                .WithBuild()
                .Execute(AssetName, loggerArgs, collectCoverage: false, "test-results.xml");
        }

        [TestMethod]
        public void LoggedXmlValidatesAgainstXsdSchema()
        {
            var validator = new JunitXmlValidator();
            var result = validator.IsValid(File.ReadAllText(this.resultsFile));
            Assert.IsTrue(result);
        }

        [TestMethod]
        public void TestResultFileShouldContainXUnitTraitAsProperty()
        {
            var properties = this.resultsXml.XPathSelectElement(
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