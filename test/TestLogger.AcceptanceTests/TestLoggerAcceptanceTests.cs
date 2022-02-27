// Copyright (c) Spekt Contributors. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

namespace TestLogger.AcceptanceTests
{
    using System;
    using System.IO;
    using System.Threading.Tasks;
    using Microsoft.VisualStudio.TestTools.UnitTesting;
    using Spekt.TestLogger.UnitTests.TestDoubles;
    using VerifyMSTest;
    using JsonSerializer = System.Text.Json.JsonSerializer;

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

            if (testAssembly.Contains(".XUnit.", StringComparison.OrdinalIgnoreCase))
            {
                settings.ScrubLinesWithReplace(x => System.Text.RegularExpressions.Regex.Replace(x, @"\[xUnit\.net [0-9:.]{11,}\]", "[xUnit.net _Timestamp_]"));
            }

            DotnetTestFixture.Execute(testAssembly, "test-results.json");
            var resultsFile = Path.Combine(DotnetTestFixture.RootDirectory, "test-results.json");
            var json = File.ReadAllText(resultsFile);
            var testReport = JsonSerializer.Deserialize<JsonTestResultSerializer.TestReport>(json, null);

            return this.Verify(testReport, settings);
        }
    }
}
