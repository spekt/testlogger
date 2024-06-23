// Copyright (c) Spekt Contributors. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

namespace NUnit.Xml.TestLogger.AcceptanceTests
{
    using System;
    using System.IO;
    using System.Linq;
    using global::TestLogger.Fixtures;
    using Microsoft.VisualStudio.TestTools.UnitTesting;

    [TestClass]
    public class NUnitTestLoggerPathTests
    {
        private static readonly string[] ExpectedResultsFiles = new string[]
        {
            "NUnit.Xml.TestLogger.NetMulti.Tests.NETFramework461.test-results.xml",
            "NUnit.Xml.TestLogger.NetMulti.Tests.NETCoreApp31.test-results.xml"
        };

        [TestMethod]
        public void TestRunWithLoggerAndFilePathShouldCreateResultsFile()
        {
            var assetDir = "NUnit.Xml.TestLogger.NetMulti.Tests".ToAssetDirectoryPath();
            var testResultFiles = ExpectedResultsFiles.Select(x => Path.Combine(assetDir, x)).ToArray();
            var loggerArgs = "nunit;LogFilePath={assembly}.{framework}.test-results.xml";
            foreach (var f in testResultFiles.Where(File.Exists))
            {
                File.Delete(f);
            }

            _ = DotnetTestFixture
                    .Create()
                    .WithBuild()
                    .Execute("NUnit.Xml.TestLogger.NetMulti.Tests", loggerArgs, collectCoverage: false, "test-results.xml");

            foreach (string resultFile in testResultFiles)
            {
                Assert.IsTrue(File.Exists(resultFile), $"{resultFile} does not exist.");
            }
        }
    }
}
