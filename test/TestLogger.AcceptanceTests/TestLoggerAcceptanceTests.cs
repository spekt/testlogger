// Copyright (c) Spekt Contributors. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

namespace TestLogger.AcceptanceTests
{
    using System;
    using System.IO;
    using System.Threading.Tasks;
    using Microsoft.VisualStudio.TestTools.UnitTesting;
    using Newtonsoft.Json;
    using VerifyMSTest;
    using static Spekt.TestLogger.UnitTests.TestDoubles.JsonTestResultSerializer;

    [TestClass]
    public class TestLoggerAcceptanceTests : VerifyBase
    {
        [TestMethod]
        ////[DataRow()]
        public Task VerifyTestRunOutput() ////string testAssembly)
        {
            var testAssembly = "Json.TestLogger.NetCore.Tests";
            DotnetTestFixture.Execute(testAssembly, "test-results.json");
            var resultsFile = Path.Combine(DotnetTestFixture.RootDirectory, "test-results.json");

            var testReport = JsonConvert.DeserializeObject<TestReport>(File.ReadAllText(resultsFile));
            var settings = new VerifyTests.VerifySettings();
            settings.UseDirectory("Snapshots");

            return this.Verify(testReport, settings);
        }
    }
}
