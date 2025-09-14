// Copyright (c) Spekt Contributors. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

namespace Spekt.TestReporter.JUnit
{
    using System;
    using Microsoft.VisualStudio.TestPlatform.Extension.Junit.Xml.TestLogger;
    using Spekt.TestLogger.Core;

    internal sealed class JUnitTestReporter : TestReporter
    {
        public JUnitTestReporter(JUnitTestReporterExtension extension, IServiceProvider serviceProvider)
            : base(serviceProvider, extension, "junit")
        {
        }

        protected override string DefaultFileName => "TestResults.xml";

        protected override ITestResultSerializer CreateTestResultSerializer()
            => new JunitXmlSerializer();
    }
}
