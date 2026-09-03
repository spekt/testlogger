// Copyright (c) Spekt Contributors. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

namespace Xunit.Xml.TestLogger.AcceptanceTests
{
    using System.IO;
    using System.Xml;
    using Xunit;

    /// <summary>
    /// Validates running the MTP logger on a test project without a
    /// Microsoft.NET.Test.Sdk reference (see issue #229).
    /// </summary>
    [Collection("Acceptance")]
    public class NoTestSdkAcceptanceTests
    {
        private const string TargetFrameworkVersion = "net8.0";
        private const string ObjectModelAssembly = "Microsoft.VisualStudio.TestPlatform.ObjectModel.dll";

        private readonly NoTestSdkFixture fixture;

        public NoTestSdkAcceptanceTests(NoTestSdkFixture fixture)
        {
            this.fixture = fixture;
        }

        [Fact]
        public void MtpRunWithoutTestSdkShouldProduceResultsFile()
        {
            Assert.True(File.Exists(this.fixture.ResultsFile));
        }

        [Fact]
        public void MtpRunWithoutTestSdkShouldReportTestCounts()
        {
            var resultsXml = new XmlDocument();
            resultsXml.Load(this.fixture.ResultsFile);
            var assemblyNode = resultsXml.SelectSingleNode("/assemblies/assembly");

            Assert.NotNull(assemblyNode);
            Assert.Equal("3", assemblyNode.Attributes["total"].Value);
            Assert.Equal("2", assemblyNode.Attributes["passed"].Value);
            Assert.Equal("1", assemblyNode.Attributes["failed"].Value);
        }

        [Fact]
        public void MtpRunWithoutTestSdkShouldNotDeployTestPlatformObjectModel()
        {
#if DEBUG
            var config = "Debug";
#else
            var config = "Release";
#endif
            var objectModelPath = Path.Combine(
                this.fixture.AssetDirectory,
                "bin",
                config,
                "mtp",
                TargetFrameworkVersion,
                ObjectModelAssembly);

            Assert.False(File.Exists(objectModelPath), "Microsoft.VisualStudio.TestPlatform.ObjectModel must not be deployed for MTP-only runs (issue #229).");
        }
    }
}
