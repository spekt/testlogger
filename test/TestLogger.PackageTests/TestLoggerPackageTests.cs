// Copyright (c) Spekt Contributors. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

namespace TestLogger.PackageTests
{
    using System;
    using System.IO;
    using System.Text.RegularExpressions;
    using System.Threading.Tasks;
    using Microsoft.VisualStudio.TestTools.UnitTesting;
    using Newtonsoft.Json;
    using TestLogger.Fixtures;
    using VerifyMSTest;
    using VerifyTests;

    [TestClass]
    public class TestLoggerPackageTests
    {
        [TestMethod]
        [DataRow("JUnit.Xml.PackageTest", "junit", "")]
        [DataRow("NUnit.Xml.PackageTest", "nunit", "")]
        [DataRow("Xunit.Xml.PackageTest", "xunit", "")]
        public void VerifyTestRunOutput(string testAssembly, string loggerName, string comment)
        {
            // Logger arguments are passed as it is for the test process: dotnet test --logger:<loggerArgs>
            var loggerArgs = $"{loggerName};LogFilePath=test-results.xml";

            // Collect coverage will attach a runlevel attachment.
            var collectCoverage = testAssembly.Contains("XUnit.NetCore");

            var resultsFile = DotnetTestFixture.Create().WithBuild().Execute(testAssembly, loggerArgs, collectCoverage, "test-results.xml");

            var testReport = File.ReadAllText(resultsFile);
            Assert.IsNotNull(testReport);
        }
    }
}