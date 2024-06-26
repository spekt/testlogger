// Copyright (c) Spekt Contributors. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

namespace Spekt.TestLogger.Core
{
    using System;
    using Microsoft.VisualStudio.TestPlatform.ObjectModel.Client;
    using Spekt.TestLogger.Extensions;
    using Spekt.TestLogger.Platform;

    public class TestRunBuilder : ITestRunBuilder
    {
        private readonly TestRun testRun;

        public TestRunBuilder()
        {
            this.testRun = new TestRun
            {
                RunConfiguration = new TestRunConfiguration(),
                AdapterFactory = new TestAdapterFactory(),
            };
        }

        public ITestRunBuilder WithLoggerConfiguration(LoggerConfiguration configuration)
        {
            this.testRun.LoggerConfiguration = configuration;
            return this;
        }

        public ITestRunBuilder WithStore(ITestResultStore store)
        {
            this.testRun.Store = store ?? throw new ArgumentNullException(nameof(store));
            return this;
        }

        public ITestRunBuilder WithSerializer(ITestResultSerializer serializer)
        {
            this.testRun.Serializer = serializer ?? throw new ArgumentNullException(nameof(serializer));
            return this;
        }

        public ITestRunBuilder Subscribe(TestLoggerEvents loggerEvents)
        {
            if (loggerEvents == null)
            {
                throw new ArgumentNullException(nameof(loggerEvents));
            }

            loggerEvents.TestRunStart += (_, eventArgs) =>
            {
                this.testRun.RunConfiguration = this.testRun.Start(eventArgs);
            };
            loggerEvents.TestRunMessage += (_, eventArgs) => this.TraceAndThrow(() => this.testRun.Message(eventArgs), "TestRunMessage");
            loggerEvents.TestResult += (_, eventArgs) => this.TraceAndThrow(() => this.testRun.Result(eventArgs), "TestResult");
            loggerEvents.TestRunComplete += (_, eventArgs) => this.TraceAndThrow(() => this.testRun.Complete(eventArgs), "TestRunComplete");

            return this;
        }

        public ITestRunBuilder WithFileSystem(IFileSystem fileSystem)
        {
            this.testRun.FileSystem = fileSystem ?? throw new ArgumentNullException(nameof(fileSystem));
            return this;
        }

        public ITestRunBuilder WithConsoleOutput(IConsoleOutput consoleOutput)
        {
            this.testRun.ConsoleOutput = consoleOutput ?? throw new ArgumentNullException(nameof(consoleOutput));
            return this;
        }

        public ITestRun Build()
        {
            return this.testRun;
        }

        private void TraceAndThrow(Action action, string source)
        {
            try
            {
                action();
            }
            catch (Exception)
            {
                this.testRun.ConsoleOutput?.WriteError($"Test Logger: Unexpected error in {source} workflow. Please rerun with `dotnet test --diag:log.txt` to see the stacktrace and report the issue at https://github.com/spekt/testlogger/issues/new.");
                throw;
            }
        }
    }
}