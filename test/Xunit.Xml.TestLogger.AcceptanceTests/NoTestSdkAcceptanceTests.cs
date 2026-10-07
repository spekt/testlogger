// Copyright (c) Spekt Contributors. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

namespace Xunit.Xml.TestLogger.AcceptanceTests
{
    using System.IO;
    using System.Xml;
    using global::TestLogger.Fixtures;
    using Microsoft.VisualStudio.TestTools.UnitTesting;

    /// <summary>
    /// Validates running the MTP logger on a test project without a
    /// Microsoft.NET.Test.Sdk reference (see issue #229).
    /// </summary>
    [TestClass]
    [DoNotParallelize]
    public class NoTestSdkAcceptanceTests
    {
        private const string AssetName = "Xunit.Xml.TestLogger.NoTestSdk.Tests";
        private const string ResultsFileName = "test-results-mtp.xml";
        private const string TargetFrameworkVersion = "net8.0";
        private const string ObjectModelAssembly = "Microsoft.VisualStudio.TestPlatform.ObjectModel.dll";

        private static string resultsFile;
        private static string assetDirectory;

        // Fixture legs spawn dotnet test subprocesses that share mutable state
        // (test/assets/global.json selects the test runner per leg), so legs
        // run once here up front and all tests in this class stay sequential.
        [ClassInitialize]
        public static void SuiteInitialize(TestContext context)
        {
            // MTP-only asset without Microsoft.NET.Test.Sdk (see issue #229).
            var mtpLoggerArgs = $"--report-spekt-xunit --report-spekt-xunit-filename {ResultsFileName}";
            resultsFile = DotnetTestFixture
                .Create()
                .WithNoBuild()
                .Execute(AssetName, mtpLoggerArgs, collectCoverage: false, resultsFileName: ResultsFileName, isMTP: true);

            Assert.IsFalse(string.IsNullOrEmpty(resultsFile), "MTP results file cannot be null");
            assetDirectory = AssetName.ToAssetDirectoryPath();
        }

        [TestMethod]
        public void MtpRunWithoutTestSdkShouldProduceResultsFile()
        {
            Assert.IsTrue(File.Exists(resultsFile));
        }

        [TestMethod]
        public void MtpRunWithoutTestSdkShouldReportTestCounts()
        {
            var resultsXml = new XmlDocument();
            resultsXml.Load(resultsFile);
            var assemblyNode = resultsXml.SelectSingleNode("/assemblies/assembly");

            Assert.IsNotNull(assemblyNode);
            Assert.AreEqual("3", assemblyNode.Attributes["total"].Value);
            Assert.AreEqual("2", assemblyNode.Attributes["passed"].Value);
            Assert.AreEqual("1", assemblyNode.Attributes["failed"].Value);
        }

        [TestMethod]
        public void MtpRunWithoutTestSdkShouldNotDeployTestPlatformObjectModel()
        {
#if DEBUG
            var config = "Debug";
#else
            var config = "Release";
#endif
            var objectModelPath = Path.Combine(
                assetDirectory,
                "bin",
                config,
                "mtp",
                TargetFrameworkVersion,
                ObjectModelAssembly);

            Assert.IsFalse(File.Exists(objectModelPath), "Microsoft.VisualStudio.TestPlatform.ObjectModel must not be deployed for MTP-only runs (issue #229).");
        }
    }
}
