// Copyright (c) Spekt Contributors. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

namespace JUnit.Xml.TestLogger.AcceptanceTests
{
    using System.IO;
    using System.Xml.Linq;
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
    public class JUnitTestLoggerNetFullAcceptanceTests
    {
        private const string AssetName = "JUnit.Xml.TestLogger.NetFull.Tests";
        private readonly string resultsFile;
        private readonly XDocument resultsXml;

        public JUnitTestLoggerNetFullAcceptanceTests()
        {
            this.resultsFile = Path.Combine(AssetName.ToAssetDirectoryPath(), "test-results.xml");
            this.resultsXml = XDocument.Load(this.resultsFile);
        }

        [ClassInitialize]
        public static void SuiteInitialize(TestContext context)
        {
#if WINDOWS_OS
            var loggerArgs = "junit;LogFilePath=test-results.xml";

            // Enable reporting of internal properties in the adapter using runsettings
            _ = DotnetTestFixture
                .Create()
                .WithBuild()
                .Execute(AssetName, loggerArgs, collectCoverage: false, "test-results.xml");
#endif
        }

        [TestMethod]
        public void LoggedXmlValidatesAgainstXsdSchema()
        {
#if WINDOWS_OS
            var validator = new JunitXmlValidator();
            var result = validator.IsValid(File.ReadAllText(this.resultsFile));
            Assert.IsTrue(result);
#endif
        }
    }
}
