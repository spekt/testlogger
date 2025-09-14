// Copyright (c) Spekt Contributors. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

namespace Spekt.TestReporter.NUnit
{
    using Microsoft.Testing.Platform.Builder;
    using Microsoft.Testing.Platform.Extensions;

    public static class NUnitTestReporterExtensions
    {
        public static void AddNUnitReportProvider(this ITestApplicationBuilder testApplicationBuilder)
        {
            var extension = new NUnitTestReporterExtension();
            var compositeExtension = new CompositeExtensionFactory<NUnitTestReporter>(serviceProvider =>
                new NUnitTestReporter(extension, serviceProvider));
            testApplicationBuilder.TestHost.AddDataConsumer(compositeExtension);
            testApplicationBuilder.TestHost.AddTestSessionLifetimeHandle(compositeExtension);

            testApplicationBuilder.CommandLine.AddProvider(() => new TestReporterCommandLineProvider(extension, "nunit"));
        }
    }
}
