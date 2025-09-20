// Copyright (c) Spekt Contributors. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

namespace Spekt.TestLogger.UnitTests.TestDoubles
{
    using System;
    using Microsoft.Testing.Platform.Extensions;
    using Moq;
    using Spekt.TestLogger.Core;

    /// <summary>
    /// Testable implementation of TestReporter for unit testing.
    /// </summary>
    public class TestableTestReporter : Spekt.TestReporter.TestReporter
    {
        public TestableTestReporter(IServiceProvider serviceProvider, IExtension extension)
            : base(serviceProvider, extension, "junit")
        {
        }

        protected override string DefaultFileName => "TestResults.xml";

        public ITestRun TestCreateTestRun(IServiceProvider serviceProvider)
        {
            return this.CreateTestRun(serviceProvider);
        }

        protected override ITestResultSerializer CreateTestResultSerializer()
        {
            return new Mock<ITestResultSerializer>().Object;
        }
    }
}