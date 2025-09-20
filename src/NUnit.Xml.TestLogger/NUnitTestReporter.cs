// Copyright (c) Spekt Contributors. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

namespace Spekt.TestReporter.NUnit
{
    using System;
    using Microsoft.VisualStudio.TestPlatform.Extension.NUnit.Xml.TestLogger;
    using Spekt.TestLogger.Core;

    internal sealed class NUnitTestReporter : TestReporter
    {
        public NUnitTestReporter(NUnitTestReporterExtension extension, IServiceProvider serviceProvider)
            : base(serviceProvider, extension, "nunit")
        {
        }

        protected override string DefaultFileName => "TestResults.xml";

        protected override ITestResultSerializer CreateTestResultSerializer()
            => new NUnitXmlSerializer();
    }
}
