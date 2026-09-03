// Copyright (c) Spekt Contributors. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

namespace Xunit.Xml.TestLogger.AcceptanceTests
{
    using global::TestLogger.Fixtures;
    using Xunit;

    public class NoTestSdkFixture
    {
        private const string AssetName = "Xunit.Xml.TestLogger.NoTestSdk.Tests";
        private const string ResultsFileName = "test-results-mtp.xml";

        public NoTestSdkFixture()
        {
            // MTP-only asset without Microsoft.NET.Test.Sdk (see issue #229).
            var mtpLoggerArgs = $"--report-spekt-xunit --report-spekt-xunit-filename {ResultsFileName}";
            this.ResultsFile = global::TestLogger.Fixtures.DotnetTestFixture
                .Create()
                .WithNoBuild()
                .Execute(AssetName, mtpLoggerArgs, collectCoverage: false, resultsFileName: ResultsFileName, isMTP: true);

            Assert.False(string.IsNullOrEmpty(this.ResultsFile), "MTP results file cannot be null");
            this.AssetDirectory = AssetName.ToAssetDirectoryPath();
        }

        public string ResultsFile { get; }

        public string AssetDirectory { get; }
    }
}
