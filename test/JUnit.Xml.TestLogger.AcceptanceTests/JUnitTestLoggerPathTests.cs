// Copyright (c) Spekt Contributors. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

namespace JUnit.Xml.TestLogger.AcceptanceTests
{
    using System.IO;
    using System.Linq;
    using global::TestLogger.Fixtures;
    using Microsoft.VisualStudio.TestTools.UnitTesting;

    /// <summary>
    /// Acceptance tests evaluate the most recent output of the build.ps1 script, NOT the most
    /// recent build performed by visual studio or dotnet.build
    ///
    /// These acceptance tests look at the path parameter and tokens.
    /// </summary>
    [TestClass]
    public class JUnitTestLoggerPathTests
    {
        private static readonly string[] ExpectedResultsFiles = new string[]
        {
            "JUnit.Xml.TestLogger.NetMulti.Tests.NETFramework461.test-results.xml",
            "JUnit.Xml.TestLogger.NetMulti.Tests.NETCoreApp31.test-results.xml"
        };

        [TestMethod]
        public void TestRunWithLoggerAndFilePathShouldCreateResultsFile()
        {
            var assetDir = "JUnit.Xml.TestLogger.NetMulti.Tests".ToAssetDirectoryPath();
            var testResultFiles = ExpectedResultsFiles.Select(x => Path.Combine(assetDir, x)).ToArray();
            var loggerArgs = "junit;LogFilePath={assembly}.{framework}.test-results.xml";
            foreach (var f in testResultFiles.Where(File.Exists))
            {
                File.Delete(f);
            }

            _ = DotnetTestFixture
                    .Create()
                    .WithBuild()
                    .Execute("JUnit.Xml.TestLogger.NetMulti.Tests", loggerArgs, collectCoverage: false, "test-results.xml");

            foreach (string resultFile in testResultFiles)
            {
                Assert.IsTrue(File.Exists(resultFile), $"{resultFile} does not exist.");
            }
        }
    }
}
