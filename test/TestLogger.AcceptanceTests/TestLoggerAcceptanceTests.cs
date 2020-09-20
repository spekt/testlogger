// Copyright (c) Spekt Contributors. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

namespace TestLogger.AcceptanceTests
{
    using System.IO;
    using Microsoft.VisualStudio.TestTools.UnitTesting;

    [TestClass]
    public class TestLoggerAcceptanceTests
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
    }
}
