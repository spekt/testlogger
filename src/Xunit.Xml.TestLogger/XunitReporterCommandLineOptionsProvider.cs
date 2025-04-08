// Copyright (c) Spekt Contributors. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

namespace Spekt.TestReporter.Xunit
{
    internal sealed class XunitReporterCommandLineOptionsProvider : AbstractReporterCommandLineOptionsProvider
    {
        public const string ReportXunitOption = "report-xunit";
        public const string ReportXunitFileNameOption = "report-xunit-filename";

        public XunitReporterCommandLineOptionsProvider(XunitTestReporterExtension extension)
            : base(extension)
        {
        }

        public override string ReportOption => ReportXunitOption;

        public override string ReportFileNameOption => ReportXunitFileNameOption;
    }
}
