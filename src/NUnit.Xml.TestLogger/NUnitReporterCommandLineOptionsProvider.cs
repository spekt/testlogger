// Copyright (c) Spekt Contributors. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

namespace Spekt.TestReporter.NUnit
{
    internal sealed class NUnitReporterCommandLineOptionsProvider : AbstractReporterCommandLineOptionsProvider
    {
        public const string ReportNUnitOption = "report-nunit";
        public const string ReportNUnitFileNameOption = "report-nunit-filename";

        public NUnitReporterCommandLineOptionsProvider(NUnitTestReporterExtension extension)
            : base(extension)
        {
        }

        public override string ReportOption => ReportNUnitOption;

        public override string ReportFileNameOption => ReportNUnitFileNameOption;
    }
}
