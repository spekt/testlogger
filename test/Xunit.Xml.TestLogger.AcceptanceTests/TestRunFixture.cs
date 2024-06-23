// Copyright (c) Spekt Contributors. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

namespace Xunit.Xml.TestLogger.AcceptanceTests
{
    using System;
    using System.IO;

    public class TestRunFixture : IDisposable
    {
        private const string DotnetVersion = "netcoreapp3.1";

        public TestRunFixture()
        {
            var loggerArgs = "xunit;LogFilePath=test-results.xml";
            var resultsFile = global::TestLogger.Fixtures.DotnetTestFixture.Create().WithBuild().Execute("Xunit.Xml.TestLogger.NetCore.Tests", loggerArgs, collectCoverage: false, resultsFileName: "test-results.xml");

            Assert.False(string.IsNullOrEmpty(resultsFile), "Results file cannot be null");
        }

        public void Dispose()
        {
        }
    }
}
