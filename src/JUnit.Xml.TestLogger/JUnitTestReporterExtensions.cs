// Copyright (c) Spekt Contributors. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

namespace Spekt.TestReporter.JUnit
{
    using Microsoft.Testing.Platform.Builder;
    using Microsoft.Testing.Platform.Extensions;

    public static class JUnitTestReporterExtensions
    {
        public static void AddJUnitReportProvider(this ITestApplicationBuilder testApplicationBuilder)
        {
            var extension = new JUnitTestReporterExtension();
            var compositeExtension = new CompositeExtensionFactory<JUnitTestReporter>(serviceProvider =>
                new JUnitTestReporter(extension, serviceProvider));
            testApplicationBuilder.TestHost.AddDataConsumer(compositeExtension);
            testApplicationBuilder.TestHost.AddTestSessionLifetimeHandle(compositeExtension);

            testApplicationBuilder.CommandLine.AddProvider(() => new TestReporterCommandLineProvider(extension, "junit"));
        }
    }
}
