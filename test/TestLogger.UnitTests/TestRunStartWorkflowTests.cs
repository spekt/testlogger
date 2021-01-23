// Copyright (c) Spekt Contributors. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

namespace Spekt.TestLogger.UnitTests
{
    using System;
    using System.Collections.Generic;
    using Microsoft.VisualStudio.TestPlatform.ObjectModel.Client;
    using Microsoft.VisualStudio.TestTools.UnitTesting;
    using Spekt.TestLogger.Core;
    using Spekt.TestLogger.UnitTests.TestDoubles;

    [TestClass]
    public class TestRunStartWorkflowTests
    {
        private const string DummySettings = @"<RunSettings>
        <RunConfiguration>
            <ResultsDirectory>/tmp/trial/TestResults</ResultsDirectory>
            <TargetPlatform>X64</TargetPlatform>
            <TargetFrameworkVersion>.NETCoreApp,Version=v5.0</TargetFrameworkVersion>
            <TestAdaptersPaths>/home/test/.nuget/packages/coverlet.collector/1.3.0/build/netstandard1.0/</TestAdaptersPaths>
            <DesignMode>False</DesignMode>
            <CollectSourceInformation>False</CollectSourceInformation>
        </RunConfiguration>
        <LoggerRunSettings>
            <Loggers>
                <Logger friendlyName=""Console"" uri=""logger://microsoft/TestPlatform/ConsoleLogger/v1"" assemblyQualifiedName=""Microsoft.VisualStudio.TestPlatform.CommandLine.Internal.ConsoleLogger, vstest.console, Version=15.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a"" codeBase=""/usr/share/dotnet/sdk/5.0.101/vstest.console.dll"" enabled=""True"" />
            </Loggers>
        </LoggerRunSettings>
        </RunSettings>";

        private readonly List<string> sources;
        private readonly TestRun testRun;
        private readonly TestRunCriteria testRunCriteria;

        public TestRunStartWorkflowTests()
        {
            this.testRun = new TestRun();
            this.sources = new List<string> { "/tmp/test.dll" };
            this.testRunCriteria = new TestRunCriteria(this.sources, 10, false, DummySettings);
        }

        [TestMethod]
        public void StartShouldExtractAssemblyPathForTestRun()
        {
            var startedEvent = new TestRunStartEventArgs(this.testRunCriteria);

            var config = this.testRun.Start(startedEvent);

            Assert.IsNotNull(config);
            Assert.AreEqual("/tmp/test.dll", config.AssemblyPath);
        }

        [TestMethod]
        public void StartShouldExtractFrameworkForTestRun()
        {
            var startedEvent = new TestRunStartEventArgs(this.testRunCriteria);

            var config = this.testRun.Start(startedEvent);

            Assert.AreEqual(".NETCoreApp,Version=v5.0", config.TargetFramework);
        }

        [TestMethod]
        public void StartShouldRecordStartTimeOfTestRun()
        {
            var startedEvent = new TestRunStartEventArgs(this.testRunCriteria);

            var config = this.testRun.Start(startedEvent);

            Assert.AreEqual(
                DateTime.Now.Date,
                config.StartTime.ToLocalTime().Date);
        }

        [TestMethod]
        public void StartShouldUpdateTestRunConfiguration()
        {
            var loggerEvents = new MockTestLoggerEvents();
            var run = new TestRunBuilder().Subscribe(loggerEvents).Build();

            loggerEvents.RaiseTestRunStart(this.testRunCriteria);

            Assert.IsNotNull(run.RunConfiguration);
            Assert.AreEqual("/tmp/test.dll", run.RunConfiguration.AssemblyPath);
            Assert.AreEqual(".NETCoreApp,Version=v5.0", run.RunConfiguration.TargetFramework);
            Assert.IsNotNull(run.RunConfiguration.StartTime);
        }
    }
}