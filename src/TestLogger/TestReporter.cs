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

        protected TestReporter(IServiceProvider serviceProvider, IExtension extension)
        {
            this.serviceProvider = serviceProvider;
            this.extension = extension;
        }

        public Type[] DataTypesConsumed { get; } = new[] { typeof(TestNodeUpdateMessage), typeof(SessionFileArtifact) };

        public string Uid => this.extension.Uid;

        public string Version => this.extension.Version;

        public string DisplayName => this.extension.DisplayName;

        public string Description => this.extension.Description;

        protected abstract string FileNameOption { get; }

        protected abstract string ReportOption { get; }

        public Task ConsumeAsync(IDataProducer dataProducer, IData value, CancellationToken cancellationToken)
        {
            switch (value)
            {
                case SessionFileArtifact sessionFileArtifact:
                    this.testAttachmentInfos.Add(new TestAttachmentInfo(sessionFileArtifact.FileInfo.FullName, sessionFileArtifact.Description));
                    break;

                // TODO: When to call this.testRun.Message ?
                case TestNodeUpdateMessage testNodeUpdateMessage:
                    this.testRun.Result(testNodeUpdateMessage, this.serviceProvider.GetService<ITestFramework>());

                    break;
            }

            return Task.CompletedTask;
        }

        public Task<bool> IsEnabledAsync()
        {
            var isEnabled = this.serviceProvider.GetCommandLineOptions().IsOptionSet(this.ReportOption);
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

        private ITestRun CreateTestRun(IServiceProvider serviceProvider)
        {
            var commandLineOptions = serviceProvider.GetCommandLineOptions();
            var configDictionary = new Dictionary<string, string>
            {
                { DefaultLoggerParameterNames.TestRunDirectory, serviceProvider.GetConfiguration()["platformOptions:resultDirectory"] }
            };

            if (commandLineOptions.TryGetOptionArgumentList(this.FileNameOption, out var arguments))
            {
                configDictionary.Add(LoggerConfiguration.LogFileNameKey, arguments[0]);
            }

            // TODO: Results directory?
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
