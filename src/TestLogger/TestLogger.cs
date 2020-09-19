// Copyright (c) Spekt Contributors. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

namespace Spekt.TestLogger
{
    using System;
    using System.Collections.Generic;
    using System.IO;
    using Microsoft.VisualStudio.TestPlatform.ObjectModel;
    using Microsoft.VisualStudio.TestPlatform.ObjectModel.Client;
    using Spekt.TestLogger.Core;
    using Spekt.TestLogger.Platform;

    /// <summary>
    /// Base test logger implementation.
    /// </summary>
    public abstract class TestLogger : ITestLoggerWithParameters
    {
        public const string LogFilePathKey = "LogFilePath";

        private readonly IFileSystem fileSystem;
        private readonly IConsoleOutput consoleOutput;
        private readonly ITestResultStore resultStore;
        private readonly ITestResultSerializer resultSerializer;
        private ITestRun testRun;

        protected TestLogger(ITestResultSerializer resultSerializer)
            : this(new FileSystem(), new ConsoleOutput(), new TestResultStore(), resultSerializer)
        {
        }

        protected TestLogger(
            IFileSystem fileSystem,
            IConsoleOutput consoleOutput,
            ITestResultStore resultStore,
            ITestResultSerializer resultSerializer)
        {
            this.fileSystem = fileSystem ?? throw new ArgumentNullException(nameof(fileSystem));
            this.consoleOutput = consoleOutput ?? throw new ArgumentNullException(nameof(consoleOutput));
            this.resultStore = resultStore ?? throw new ArgumentNullException(nameof(resultStore));
            this.resultSerializer = resultSerializer ?? throw new ArgumentNullException(nameof(resultSerializer));
        }

        protected abstract string DefaultTestResultFile { get; }

        /// <inheritdoc />
        /// <remarks>
        /// Overrides <see cref="ITestLogger.Initialize"/> method. Supports older runners.
        /// </remarks>
        public void Initialize(TestLoggerEvents events, string testResultsDirPath)
        {
            if (events == null)
            {
                throw new ArgumentNullException(nameof(events));
            }

            if (testResultsDirPath == null)
            {
                throw new ArgumentNullException(nameof(testResultsDirPath));
            }

            var outputPath = Path.Combine(testResultsDirPath, this.DefaultTestResultFile);

            this.CreateTestRun(events, outputPath);
        }

        /// <inheritdoc />
        /// <remarks>Overrides <c>ITestLoggerWithParameters.Initialize(TestLoggerEvents, Dictionary)</c> method.</remarks>
        public void Initialize(TestLoggerEvents events, Dictionary<string, string> parameters)
        {
            if (events == null)
            {
                throw new ArgumentNullException(nameof(events));
            }

            if (parameters == null)
            {
                throw new ArgumentNullException(nameof(parameters));
            }

            if (parameters.TryGetValue(LogFilePathKey, out var outputPath))
            {
                this.CreateTestRun(events, outputPath);
            }
            else if (parameters.TryGetValue(DefaultLoggerParameterNames.TestRunDirectory, out var outputDir))
            {
                this.Initialize(events, outputDir);
            }
            else
            {
                throw new ArgumentException($"Expected {LogFilePathKey} or {DefaultLoggerParameterNames.TestRunDirectory} parameter", nameof(parameters));
            }
        }

        private void CreateTestRun(TestLoggerEvents events, string outputPath)
        {
            this.testRun = new TestRunBuilder()
                .WithFileSystem(this.fileSystem)
                .WithConsoleOutput(this.consoleOutput)
                .WithStore(this.resultStore)
                .WithSerializer(this.resultSerializer)
                .WithResultFile(this.fileSystem.GetFullPath(outputPath))
                .Subscribe(events)
                .Build();
        }
    }
}
