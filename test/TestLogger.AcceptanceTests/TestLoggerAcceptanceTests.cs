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
        private static readonly string ResultsFile = Path.Combine(DotnetTestFixture.RootDirectory, "test-results.json");

        [ClassInitialize]
        public static void SuiteInitialize(TestContext context)
        {
            if (File.Exists(ResultsFile))
            {
                File.Delete(ResultsFile);
            }

            DotnetTestFixture.Execute("test-results.json");
        }

        [TestMethod]
        public void TestRunWithLoggerAndFilePathShouldCreateResultsFile()
        {
            Assert.IsTrue(File.Exists(ResultsFile));
        }

        [TestMethod]
        public Task VerifyTestRunOutput()
        {
            var testReport = JsonConvert.DeserializeObject<TestReport>(File.ReadAllText(ResultsFile));
            var settings = new VerifyTests.VerifySettings();
            settings.UseDirectory("Snapshots");

            return this.Verify(testReport, settings);
        }
    }
}
