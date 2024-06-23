// Copyright (c) Spekt Contributors. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

namespace TestLogger.AcceptanceTests
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
        [DataRow("Json.TestLogger.MSTest.NetMulti.Tests", "", "")]
        [DataRow("Json.TestLogger.NUnit.NetMulti.Tests", "", "")]
        [DataRow("Json.TestLogger.XUnit.NetMulti.Tests", "", "")]
#if WINDOWS_OS
        [DataRow("Json.TestLogger.MSTest.NetFull.Tests", "", "WindowsOnly")]
        [DataRow("Json.TestLogger.NUnit.NetFull.Tests", "", "WindowsOnly")]
        [DataRow("Json.TestLogger.XUnit.NetFull.Tests", "", "WindowsOnly")]
#endif
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
            settings.ScrubLinesWithReplace(x =>
            {
                var options = RegexOptions.IgnoreCase | RegexOptions.Compiled;
                var nameInDebugFolderMatch = new Regex(@".*([\/\\]*bin[\/\\]*Debug[\/\\]*.*)$", options);
                var prefixedMatch = new Regex(@"^(.{0,}: )(.{0,}test[\/\\]assets[\/\\]Json\.TestLogger)(.{0,})$", options);
                var pathMatch = new Regex(@"^(.{0,}test[\/\\]assets[\/\\]Json\.TestLogger)(.{0,})$", options);

                if (nameInDebugFolderMatch.IsMatch(x))
                {
                    // Used to take something like 'C:\\lsdkjf\sdf\bin\Debug\a\b\c.txt' => '/bin/Debug/a/b/c.txt' which helps with cross dev/platform comparison
                    var m = nameInDebugFolderMatch.Match(x);
                    var pathForwardSlashes = m.Groups[1].Captures[0].Value.Replace('\\', '/');
                    x = pathForwardSlashes;
                    x = x.Replace("//", "/");
                }
                else if (prefixedMatch.IsMatch(x))
                {
                    var m = prefixedMatch.Match(x);
                    var prefix = m.Groups[1].Captures[0].Value.Replace('\\', '/');
                    var pathForwardSlashes = m.Groups[3].Captures[0].Value.Replace('\\', '/');
                    x = prefix + "test/assets/Json.TestLogger" + pathForwardSlashes;
                }
                else if (pathMatch.IsMatch(x))
                {
                    var m = pathMatch.Match(x);
                    var pathForwardSlashes = m.Groups[2].Captures[0].Value.Replace('\\', '/');
                    x = "test/assets/Json.TestLogger" + pathForwardSlashes;
                }

                x = x.Replace(@"\r\n", @"\n"); // Fix cross plat failures.
                return x;
            });

            // Collect coverage will attach a runlevel attachment.
            var collectCoverage = testAssembly.Contains("XUnit.NetCore");
            var resultsFile = DotnetTestFixture.Create().WithBuild().Execute(testAssembly, loggerArgs, collectCoverage, "test-results.json");
            var testReport = JsonConvert.DeserializeObject<TestReport>(File.ReadAllText(resultsFile));

            // Using VerifyJson with serialized JSON to avoid incompatibility in object serialization
            // between NewtonSoft.Json and Argon (the JSON serializer used by Verify)
            return this.VerifyJson(JsonConvert.SerializeObject(testReport.TestAssemblies), settings);
        }
    }
}
