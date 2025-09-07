// Copyright (c) Spekt Contributors. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

namespace Spekt.TestReporter.Xunit;

using Microsoft.Testing.Platform.Builder;

/// <summary>
/// This class is used by Microsoft.Testing.Platform entrypoint generation to add Xunit XML report support.
/// </summary>
public static class TestingPlatformBuilderHook
{
    /// <summary>
    /// Adds Xunit XML report support.
    /// </summary>
    /// <param name="testApplicationBuilder">The test application builder.</param>
    /// <param name="args">The command line arguments.</param>
    public static void AddExtensions(ITestApplicationBuilder testApplicationBuilder, string[] args)
        => testApplicationBuilder.AddXunitReportProvider();
}