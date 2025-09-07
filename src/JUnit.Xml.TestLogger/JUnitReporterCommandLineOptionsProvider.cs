// Copyright (c) Spekt Contributors. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

namespace Spekt.TestReporter.JUnit
{
    internal sealed class JUnitReporterCommandLineOptionsProvider : AbstractReporterCommandLineOptionsProvider
    {
        public const string ReportJUnitOption = "report-junit";
        public const string ReportJUnitFileNameOption = "report-junit-filename";

        public JUnitReporterCommandLineOptionsProvider(JUnitTestReporterExtension extension)
            : base(extension)
        {
        }

        public override string ReportOption => ReportJUnitOption;

        public override string ReportFileNameOption => ReportJUnitFileNameOption;
    }
}
