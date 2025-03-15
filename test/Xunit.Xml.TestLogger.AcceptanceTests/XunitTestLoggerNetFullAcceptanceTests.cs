// Copyright (c) Spekt Contributors. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

namespace Xunit.Xml.TestLogger.AcceptanceTests
{
    using System;
    using System.IO;
    using System.Runtime.InteropServices;
    using System.Xml.Linq;
    using System.Xml.XPath;
    using global::TestLogger.Fixtures;
    using Microsoft.VisualStudio.TestTools.UnitTesting;

    /// <summary>
    /// Acceptance tests evaluate the most recent output of the build.ps1 script, NOT the most
    /// recent build performed by visual studio or dotnet.build
    ///
    /// These acceptance tests look at the specific structure and contents of the produced Xml,
    /// when running using the xUnit vstest runner.
    /// </summary>
    [TestClass]
    public class XunitTestLoggerNetFullAcceptanceTests
    {
        private const string AssetName = "Xunit.Xml.TestLogger.NetFull.Tests";
        private readonly string resultsFile;

        public XunitTestLoggerNetFullAcceptanceTests()
        {
            this.resultsFile = Path.Combine(AssetName.ToAssetDirectoryPath(), "test-results.xml");
        }

        [ClassInitialize]
        public static void SuiteInitialize(TestContext context)
        {
            if (!RuntimeInformation.IsOSPlatform(OSPlatform.Windows))
            {
                return;
            }

            var loggerArgs = "xunit;LogFilePath=test-results.xml";

            // Enable reporting of internal properties in the adapter using runsettings
            _ = DotnetTestFixture
                .Create()
                .WithBuild()
                .Execute(AssetName, loggerArgs, collectCoverage: false, "test-results.xml");
        }

        [TestMethod]
        public void NetFullTestsAreRun()
        {
            if (!RuntimeInformation.IsOSPlatform(OSPlatform.Windows))
            {
                return;
            }

            var resultsXml = XDocument.Load(this.resultsFile);

            var node = resultsXml.XPathSelectElement(@"/assemblies/assembly");
            Assert.IsNotNull(node);
            Assert.IsTrue(Convert.ToInt32(node.Attribute(XName.Get("total")).Value) > 0);
        }
    }
}
