// Copyright (c) Spekt Contributors. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

namespace Spekt.TestLogger.Core
{
    using Spekt.TestLogger.Platform;

    public class TestRun : ITestRun
    {
        public LoggerConfiguration LoggerConfiguration { get; set; }

        public TestRunConfiguration RunConfiguration { get; set; }

        public ITestResultStore Store { get; set; }

        public ITestResultSerializer Serializer { get; set; }

        public IConsoleOutput ConsoleOutput { get; set; }

        public IFileSystem FileSystem { get; set; }
    }
}