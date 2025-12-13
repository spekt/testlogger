// Copyright (c) Spekt Contributors. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

namespace Xunit.Xml.TestLogger.AcceptanceTests
{
    using System;
    using System.IO;

    public class TestRunFixture : IDisposable
    {
        private const string DotnetVersion = "net8.0";

        public TestRunFixture()
        {
            // Run VSTest tests
            var vstestLoggerArgs = "xunit;LogFilePath=test-results-vstest.xml";
            var vstestResultsFile = global::TestLogger.Fixtures.DotnetTestFixture
                .Create()
                .WithBuild()
                .Execute("Xunit.Xml.TestLogger.NetCore.Tests", vstestLoggerArgs, collectCoverage: false, resultsFileName: "test-results-vstest.xml", isMTP: false);

            // Run MTP tests
            var mtpLoggerArgs = "--report-spekt-xunit --report-spekt-xunit-filename test-results-mtp.xml";
            var mtpResultsFile = global::TestLogger.Fixtures.DotnetTestFixture
                .Create()
                .WithBuild()
                .Execute("Xunit.Xml.TestLogger.NetCore.Tests", mtpLoggerArgs, collectCoverage: false, resultsFileName: "test-results-mtp.xml", isMTP: true);

            Assert.False(string.IsNullOrEmpty(vstestResultsFile), "VSTest results file cannot be null");
            Assert.False(string.IsNullOrEmpty(mtpResultsFile), "MTP results file cannot be null");
        }

        public void Dispose()
        {
        }
    }
}
