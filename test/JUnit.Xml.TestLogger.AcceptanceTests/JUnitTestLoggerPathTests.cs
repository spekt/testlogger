// Copyright (c) Spekt Contributors. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

namespace JUnit.Xml.TestLogger.AcceptanceTests
{
    using System;
    using System.IO;
    using System.Linq;
    using System.Xml.Linq;
    using System.Xml.XPath;
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
        public JUnitTestLoggerPathTests()
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
                    "JUnit.Xml.TestLogger.NetMulti.Tests"));
            DotnetTestFixture.TestAssemblyName = "JUnit.Xml.TestLogger.NetMulti.Tests.dll";
            DotnetTestFixture.Execute("{assembly}.{framework}.test-results.xml");
        }

        [TestMethod]
        public void TestRunWithLoggerAndFilePathShouldCreateResultsFile()
        {
            string[] expectedResultsFiles = new string[]
            {
                Path.Combine(DotnetTestFixture.RootDirectory, "JUnit.Xml.TestLogger.NetMulti.Tests.NETFramework461.test-results.xml"),
                Path.Combine(DotnetTestFixture.RootDirectory, "JUnit.Xml.TestLogger.NetMulti.Tests.NETCoreApp31.test-results.xml")
            };
            foreach (string resultsFile in expectedResultsFiles)
            {
                Assert.IsTrue(File.Exists(resultsFile), $"{resultsFile} does not exist.");
            }
        }
    }
}
