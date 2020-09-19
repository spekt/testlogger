// Copyright (c) Spekt Contributors. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

namespace Spekt.TestLogger.UnitTests.TestDoubles
{
    using Spekt.TestLogger.Core;

    public class TestableTestLogger : TestLogger
    {
        public TestableTestLogger()
            : base(new FakeFileSystem(), new FakeConsoleOutput(), new TestResultStore(), new JsonTestResultSerializer())
        {
            this.DefaultTestResultFile = "TestResults.xml";
        }

        protected override string DefaultTestResultFile { get; }
    }
}