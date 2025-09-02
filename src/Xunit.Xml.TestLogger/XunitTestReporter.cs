// Copyright (c) Spekt Contributors. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

namespace Spekt.TestReporter.Xunit
{
    using System;
    using Microsoft.VisualStudio.TestPlatform.Extension.Xunit.Xml.TestLogger;
    using Spekt.TestLogger.Core;

    internal sealed class XunitTestReporter : TestReporter
    {
        public XunitTestReporter(XunitTestReporterExtension extension, IServiceProvider serviceProvider)
            : base(serviceProvider, extension)
        {
        }

        protected override string FileNameOption => XunitReporterCommandLineOptionsProvider.ReportXunitFileNameOption;

        protected override string ReportOption => XunitReporterCommandLineOptionsProvider.ReportXunitOption;

        protected override ITestResultSerializer CreateTestResultSerializer()
            => new XunitXmlSerializer();
    }
}
