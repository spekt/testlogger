// Copyright (c) Spekt Contributors. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

namespace NUnit.Xml.TestLogger.AcceptanceTests
{
    using System;
    using System.IO;
    using Microsoft.VisualStudio.TestTools.UnitTesting;

    [TestClass]
    public class NUnitTestLoggerPathTests
    {
        private static readonly string[] ExpectedResultsFiles = new string[]
        {
            "NUnit.Xml.TestLogger.NetMulti.Tests.NETFramework461.test-results.xml",
            "NUnit.Xml.TestLogger.NetMulti.Tests.NETCoreApp31.test-results.xml"
        };

        public NUnitTestLoggerPathTests()
        {
        }

        [ClassInitialize]
        public static void SuiteInitialize(TestContext context)
        {
            DotnetTestFixture.RootDirectory = Path.GetFullPath(
                Path.Combine(
                    Environment.CurrentDirectory,
                    "..",
                    "..",
                    "..",
                    "..",
                    "assets",
                    "NUnit.Xml.TestLogger.NetMulti.Tests"));
            DotnetTestFixture.TestAssemblyName = "NUnit.Xml.TestLogger.NetMulti.Tests.dll";
            DotnetTestFixture.Execute("{assembly}.{framework}.test-results.xml");
        }

        [TestMethod]
        public void TestRunWithLoggerAndFilePathShouldCreateResultsFile()
        {
            foreach (string resultsFile in ExpectedResultsFiles)
            {
                Assert.IsTrue(File.Exists(Path.Combine(DotnetTestFixture.RootDirectory, resultsFile)), $"{resultsFile} does not exist.");
            }
        }
    }
}
