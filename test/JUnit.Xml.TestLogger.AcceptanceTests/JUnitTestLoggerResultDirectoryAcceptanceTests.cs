// Copyright (c) Spekt Contributors. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

namespace JUnit.Xml.TestLogger.AcceptanceTests
{
    using System.IO;
    using global::TestLogger.Fixtures;
    using Microsoft.VisualStudio.TestTools.UnitTesting;

    /// <summary>
    /// Acceptance tests evaluate the most recent output of the build.ps1 script, NOT the most
    /// recent build performed by visual studio or dotnet.build
    ///
    /// These acceptance tests look at the directory name argument.
    /// </summary>
    [TestClass]
    public class JUnitTestLoggerResultDirectoryAcceptanceTests
    {
        [TestMethod]
        public void TestRunWithResultDirectoryAndFileNameShouldCreateResultsFile()
        {
            var loggerArgs = "junit;LogFileName=test-results.xml";

            var resultsFile = DotnetTestFixture
                                .Create()
                                .WithBuild()
                                .WithResultsDirectory("artifacts")
                                .Execute("JUnit.Xml.TestLogger.NetCore.Tests", loggerArgs, collectCoverage: false, "test-results.xml");

            Assert.IsTrue(File.Exists(resultsFile), $"Results file at '{resultsFile}' not found.");
        }
    }
}
