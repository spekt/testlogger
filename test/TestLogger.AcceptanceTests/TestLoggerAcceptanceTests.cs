// Copyright (c) Spekt Contributors. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

namespace TestLogger.AcceptanceTests
{
    using System.IO;
    using System.Text.RegularExpressions;
    using System.Threading.Tasks;
    using Microsoft.VisualStudio.TestTools.UnitTesting;
    using Newtonsoft.Json;
    using TestLogger.Fixtures;
    using VerifyMSTest;
    using VerifyTests;
    using static Spekt.TestLogger.UnitTests.TestDoubles.JsonTestResultSerializer;

    [TestClass]
    public class TestLoggerAcceptanceTests : VerifyBase
    {
        public TestLoggerAcceptanceTests()
        {
            VerifierSettings.OmitContentFromException();
        }

        [TestMethod]
        [DataRow("Json.TestLogger.MSTest.NetCore.Tests", "", "")]
        [DataRow("Json.TestLogger.NUnit.NetCore.Tests", "", "")]
        [DataRow("Json.TestLogger.NUnit.NetCore.Tests", ";Parser=Legacy", "IncludesParserFailures")]
        [DataRow("Json.TestLogger.XUnit.NetCore.Tests", "", "")]
        public Task VerifyTestRunOutput(string testAssembly, string additionalArgs, string comment)
        {
            // Logger arguments are passed as it is for the test process: dotnet test --logger:<loggerArgs>
            var loggerArgs = $"json;LogFilePath=test-results.json{additionalArgs}";
            return this.VerifyAssembly(testAssembly, loggerArgs, additionalArgs, comment);
        }

        private Task VerifyAssembly(string testAssembly, string loggerArgs, string additionalArgs, string comment)
        {
            var settings = new VerifySettings();
            settings.UseDirectory(Path.Combine("Snapshots", "TestLoggerAcceptanceTests", "VerifyTestRunOutput"));
            settings.UseFileName(
                $"{testAssembly}" +
                $"{(additionalArgs.Length > 0 ? "-" + additionalArgs : string.Empty)}" +
                $"{(comment.Length > 0 ? "-" + comment : string.Empty)}");

            // Make any paths uniform regardless of OS.
            settings.ScrubLinesWithReplace(line => SnapshotScrubber.ScrubLine(line, "Json.TestLogger"));

            // Collect coverage will attach a runlevel attachment.
            var collectCoverage = testAssembly.Contains("XUnit.NetCore");
            var resultsFile = DotnetTestFixture.Create().Execute(testAssembly, loggerArgs, collectCoverage, "test-results.json");
            var testReport = JsonConvert.DeserializeObject<TestReport>(File.ReadAllText(resultsFile));

            // Using VerifyJson with serialized JSON to avoid incompatibility in object serialization
            // between NewtonSoft.Json and Argon (the JSON serializer used by Verify)
            return this.VerifyJson(JsonConvert.SerializeObject(testReport.TestAssemblies), settings);
        }
    }
}
