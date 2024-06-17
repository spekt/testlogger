﻿// Copyright (c) Spekt Contributors. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

namespace JUnit.Xml.TestLogger.AcceptanceTests
{
    using System;
    using System.IO;
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
                        "JUnit.Xml.TestLogger.NetCore.Tests"));
            DotnetTestFixture.TestAssemblyName = "JUnit.Xml.TestLogger.NetCore.Tests.dll";
            var testResultsPath = Path.Combine(DotnetTestFixture.RootDirectory, "artifacts");
            DotnetTestFixture.Execute("test-results.xml", testResultsPath);
        }

        [TestMethod]
        public void TestRunWithResultDirectoryAndFileNameShouldCreateResultsFile()
        {
            var expectedResultsPath = Path.Combine(DotnetTestFixture.RootDirectory, "artifacts", "test-results.xml");
            Assert.IsTrue(File.Exists(expectedResultsPath), $"Results file at '{expectedResultsPath}' not found.");
        }
    }
}
