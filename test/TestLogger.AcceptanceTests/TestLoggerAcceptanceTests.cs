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
        [DataRow("Json.TestLogger.MSTest.NetCore.Tests", "")]
        [DataRow("Json.TestLogger.NUnit.NetCore.Tests", "")]
        [DataRow("Json.TestLogger.XUnit.NetCore.Tests", "")]
        [DataRow("Json.TestLogger.MSTest.NetMulti.Tests", "")]
        [DataRow("Json.TestLogger.NUnit.NetMulti.Tests", "")]
        [DataRow("Json.TestLogger.XUnit.NetMulti.Tests", "")]
#if WINDOWS_OS
        [DataRow("Json.TestLogger.MSTest.NetFull.Tests", "WindowsOnly")]
        [DataRow("Json.TestLogger.NUnit.NetFull.Tests", "WindowsOnly")]
        [DataRow("Json.TestLogger.XUnit.NetFull.Tests", "WindowsOnly")]
#endif
        public Task VerifyTestRunOutput(string testAssembly, string comment)
        {
            return this.VerifyAssembly(testAssembly, comment);
        }

        private Task VerifyAssembly(string testAssembly, string comment)
        {
            var settings = new VerifyTests.VerifySettings();
            settings.UseDirectory(@"Snapshots\TestLoggerAcceptanceTests\VerifyTestRunOutput");
            settings.UseFileName($"{testAssembly}{(comment.Length > 0 ? "-" + comment : string.Empty)}");

            // Make any paths uniform regardless of OS.
            settings.ScrubLinesWithReplace(x =>
            {
                var options = RegexOptions.IgnoreCase | RegexOptions.Compiled;
                var prefixedMatch = new Regex(@"^(.{0,}: )(.{0,}test[\/\\]assets[\/\\]Json\.TestLogger)(.{0,})$", options);
                var pathMatch = new Regex(@"^(.{0,}test[\/\\]assets[\/\\]Json\.TestLogger)(.{0,})$", options);
                if (prefixedMatch.IsMatch(x))
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

                return x;
            });

            if (testAssembly.Contains(".XUnit.", StringComparison.OrdinalIgnoreCase))
            {
                settings.ScrubLinesWithReplace(x => Regex.Replace(x, @"\[xUnit\.net [0-9:.]{11,}\]", "[xUnit.net _Timestamp_]"));
            }

            DotnetTestFixture.Execute(testAssembly, out var resultsFile);
            var testReport = JsonConvert.DeserializeObject<TestReport>(File.ReadAllText(resultsFile));

            return this.Verify(testReport, settings);
        }
    }
}
