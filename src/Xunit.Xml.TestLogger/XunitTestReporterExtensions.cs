// Copyright (c) Spekt Contributors. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

namespace Spekt.TestReporter.Xunit
{
    using Microsoft.Testing.Platform.Builder;
    using Microsoft.Testing.Platform.Extensions;

    public static class XunitTestReporterExtensions
    {
        public static void AddXunitReportProvider(this ITestApplicationBuilder testApplicationBuilder)
        {
            var extension = new XunitTestReporterExtension();
            var compositeExtension = new CompositeExtensionFactory<XunitTestReporter>(serviceProvider =>
                new XunitTestReporter(extension, serviceProvider));
            testApplicationBuilder.TestHost.AddDataConsumer(compositeExtension);
            testApplicationBuilder.TestHost.AddTestSessionLifetimeHandle(compositeExtension);

            testApplicationBuilder.CommandLine.AddProvider(() => new XunitReporterCommandLineOptionsProvider(extension));
        }
    }
}
