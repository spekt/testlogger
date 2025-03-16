// Copyright (c) Spekt Contributors. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

namespace Spekt.TestLogger.Core
{
    using Microsoft.VisualStudio.TestPlatform.ObjectModel.Client;
    using Spekt.TestLogger.Platform;

    public interface ITestRunBuilder
    {
        ITestRunBuilder WithLoggerConfiguration(LoggerConfiguration configuration);

        ITestRunBuilder WithStore(ITestResultStore store);

        ITestRunBuilder WithSerializer(ITestResultSerializer serializer);

        // TODO: Should ITestRunBuilder be platform-agnostic?
        // TestLoggerEvents is VSTest-specific.
        // It may be good to refactor this so that ITestRunBuilder is platform-agnostic, and move this somewhere else.
        ITestRunBuilder Subscribe(TestLoggerEvents loggerEvents);

        ITestRunBuilder WithFileSystem(IFileSystem fileSystem);

        ITestRunBuilder WithConsoleOutput(IConsoleOutput consoleOutput);

        ITestRun Build();
    }
}