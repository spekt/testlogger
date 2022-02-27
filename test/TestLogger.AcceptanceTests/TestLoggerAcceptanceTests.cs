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
    using static Spekt.TestLogger.UnitTests.TestDoubles.JsonTestResultSerializer;

    [TestClass]
    public class TestLoggerAcceptanceTests : VerifyBase
    {
        [TestMethod]
        [DataRow("Json.TestLogger.MStest.NetCore.Tests")]
        [DataRow("Json.TestLogger.MStest.NetFull.Tests")]
        [DataRow("Json.TestLogger.MStest.NetMulti.Tests")]
        [DataRow("Json.TestLogger.NUnit.NetCore.Tests")]
        [DataRow("Json.TestLogger.NUnit.NetFull.Tests")]
        [DataRow("Json.TestLogger.NUnit.NetMulti.Tests")]
        [DataRow("Json.TestLogger.XUnit.NetCore.Tests")]
        [DataRow("Json.TestLogger.XUnit.NetFull.Tests")]
        [DataRow("Json.TestLogger.XUnit.NetMulti.Tests")]
        public Task VerifyTestRunOutput(string testAssembly)
        {
            var settings = new VerifyTests.VerifySettings();
            settings.UseDirectory("Snapshots");
            settings.UseParameters(testAssembly);

            // Make any paths uniform regardless of OS.
            settings.ScrubLinesWithReplace(x =>
            {
                var options = RegexOptions.IgnoreCase | RegexOptions.Compiled;
                var pathMatch = new Regex(@"^(.{0,}: )(.{0,}test[\/\\]assets[\/\\]Json\.TestLogger)(.{0,})$", options);
                if (pathMatch.IsMatch(x))
                {
                    var m = pathMatch.Match(x);
                    var prefix = m.Groups[1].Captures[0].Value.Replace('\\', '/');
                    var pathForwardSlashes = m.Groups[3].Captures[0].Value.Replace('\\', '/');
                    x = prefix + "test/assets/Json.TestLogger" + pathForwardSlashes;
                }

                return x;
            });

            if (testAssembly.Contains(".XUnit.", StringComparison.OrdinalIgnoreCase))
            {
                settings.ScrubLinesWithReplace(x => Regex.Replace(x, @"\[xUnit\.net [0-9:.]{11,}\]", "[xUnit.net _Timestamp_]"));
            }

            DotnetTestFixture.Execute(testAssembly, "test-results.json");
            var resultsFile = Path.Combine(DotnetTestFixture.RootDirectory, "test-results.json");
            var testReport = JsonConvert.DeserializeObject<TestReport>(File.ReadAllText(resultsFile));

            return this.Verify(testReport, settings);
        }
    }
}
