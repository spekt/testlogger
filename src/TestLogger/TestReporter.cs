// Copyright (c) Spekt Contributors. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

namespace Spekt.TestReporter
{
    using System;
    using System.Collections.Generic;
    using System.Reflection;
    using System.Runtime.Versioning;
    using System.Threading;
    using System.Threading.Tasks;
    using Microsoft.Testing.Platform.Extensions;
    using Microsoft.Testing.Platform.Extensions.Messages;
    using Microsoft.Testing.Platform.Extensions.TestFramework;
    using Microsoft.Testing.Platform.Extensions.TestHost;
    using Microsoft.Testing.Platform.Services;
    using Microsoft.Testing.Platform.TestHost;
    using Microsoft.VisualStudio.TestPlatform.ObjectModel;
    using Spekt.TestLogger.Core;
    using Spekt.TestLogger.Platform;

    /// <summary>
    /// Base test reporter implementation for Microsoft.Testing.Platform.
    /// </summary>
    public abstract class TestReporter : IDataConsumer, ITestSessionLifetimeHandler
    {
        private readonly IServiceProvider serviceProvider;
        private readonly IExtension extension;
        private readonly List<TestAttachmentInfo> testAttachmentInfos = new List<TestAttachmentInfo>();

        private ITestRun testRun;

        protected TestReporter(IServiceProvider serviceProvider, IExtension extension, string loggerName)
        {
            this.serviceProvider = serviceProvider;
            this.extension = extension;
            this.Name = loggerName;
        }

        public Type[] DataTypesConsumed { get; } = new[] { typeof(TestNodeUpdateMessage), typeof(SessionFileArtifact) };

        public string Uid => this.extension.Uid;

        public string Version => this.extension.Version;

        public string DisplayName => this.extension.DisplayName;

        public string Description => this.extension.Description;

        protected string Name { get; }

        protected abstract string DefaultFileName { get; }

        public Task ConsumeAsync(IDataProducer dataProducer, IData value, CancellationToken cancellationToken)
        {
            switch (value)
            {
                case SessionFileArtifact sessionFileArtifact:
                    this.testAttachmentInfos.Add(new TestAttachmentInfo(sessionFileArtifact.FileInfo.FullName, sessionFileArtifact.Description));
                    break;

                case TestNodeUpdateMessage testNodeUpdateMessage:
                    // Capture standard output and error messages at test run level
                    this.testRun.Message(testNodeUpdateMessage);

                    // Process the test node update message for other updates (e.g., test results)
                    this.testRun.Result(testNodeUpdateMessage, this.serviceProvider.GetService<ITestFramework>());
                    break;
            }

            return Task.CompletedTask;
        }

        public Task<bool> IsEnabledAsync()
        {
            var isEnabled = this.serviceProvider.GetCommandLineOptions().IsOptionSet($"report-{this.Name}");
            if (isEnabled)
            {
                this.testRun = this.CreateTestRun(this.serviceProvider);
            }

            return Task.FromResult(isEnabled);
        }

        public Task OnTestSessionStartingAsync(SessionUid sessionUid, CancellationToken cancellationToken)
        {
            var assembly = Assembly.GetEntryAssembly();
            ((TestRun)this.testRun).RunConfiguration = this.testRun.Start(assembly.Location, assembly.GetCustomAttribute<TargetFrameworkAttribute>()?.FrameworkName ?? "unknown-targetframework");
            return Task.CompletedTask;
        }

        public Task OnTestSessionFinishingAsync(SessionUid sessionUid, CancellationToken cancellationToken)
        {
            this.testRun.Complete(this.testAttachmentInfos);
            return Task.CompletedTask;
        }

        protected abstract ITestResultSerializer CreateTestResultSerializer();

        protected ITestRun CreateTestRun(IServiceProvider serviceProvider)
        {
            var commandLineOptions = serviceProvider.GetCommandLineOptions();
            var configDictionary = new Dictionary<string, string>
            {
                // See `PlatformConfigurationConstants.PlatformResultDirectory` in MTP source.
                { DefaultLoggerParameterNames.TestRunDirectory, serviceProvider.GetConfiguration()["platformOptions:resultDirectory"] }
            };

            // Handle config option - parse key-value pairs
            if (commandLineOptions.TryGetOptionArgumentList($"report-{this.Name}-config", out var configArguments))
            {
                var configValue = configArguments[0];
                if (!string.IsNullOrEmpty(configValue))
                {
                    var pairs = configValue.Split(';');
                    foreach (var pair in pairs)
                    {
                        var keyValue = pair.Split(new[] { '=' }, 2);
                        if (keyValue.Length == 2)
                        {
                            var key = keyValue[0].Trim();
                            var value = keyValue[1].Trim();
                            if (!string.IsNullOrEmpty(key) && !configDictionary.ContainsKey(key))
                            {
                                configDictionary.Add(key, value);
                            }
                        }
                    }
                }
            }

            // Handle log file path option
            if (commandLineOptions.TryGetOptionArgumentList($"report-{this.Name}-filename", out var fileNameArguments))
            {
                configDictionary.Add(LoggerConfiguration.LogFileNameKey, fileNameArguments[0]);
            }

            // Set the default log file name if not provided by user
            if (!configDictionary.ContainsKey(LoggerConfiguration.LogFilePathKey) &&
                !configDictionary.ContainsKey(LoggerConfiguration.LogFileNameKey))
            {
                configDictionary[LoggerConfiguration.LogFileNameKey] = this.DefaultFileName;
            }

            var loggerConfiguration = new LoggerConfiguration(configDictionary);

            return new TestRunBuilder()
                .WithLoggerConfiguration(loggerConfiguration)
                .WithFileSystem(new FileSystem())
                .WithConsoleOutput(new ConsoleOutput())
                .WithStore(new TestResultStore())
                .WithSerializer(this.CreateTestResultSerializer())
                .Build();
        }
    }
}
