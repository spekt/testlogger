// Copyright (c) Spekt Contributors. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

namespace NUnit.Xml.TestLogger.AcceptanceTests
{
    using System.IO;
    using global::TestLogger.Fixtures;
    using Microsoft.VisualStudio.TestTools.UnitTesting;

    [TestClass]
    public class NUnitTestLoggerResultDirectoryAcceptanceTests
    {
        [TestMethod]
        public void TestRunWithResultDirectoryAndFileNameShouldCreateResultsFile()
        {
            var loggerArgs = "nunit;LogFileName=test-results.xml";

            var resultsFile = DotnetTestFixture
                                .Create()
                                .WithBuild()
                                .WithResultsDirectory("artifacts")
                                .Execute("NUnit.Xml.TestLogger.NetCore.Tests", loggerArgs, collectCoverage: false, "test-results.xml");

            Assert.IsTrue(File.Exists(resultsFile), $"Results file at '{resultsFile}' not found.");
        }
    }
}
