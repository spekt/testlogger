// Copyright (c) Spekt Contributors. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

namespace Spekt.TestLogger.UnitTests.TestDoubles
{
    using System;
    using Microsoft.Testing.Platform.CommandLine;
    using Microsoft.Testing.Platform.Configurations;
    using Moq;

    /// <summary>
    /// Extension methods for creating mock objects in unit tests.
    /// </summary>
    internal static class MockExtensions
    {
        public static Mock<IServiceProvider> SetupWithMockCommandLineOptions(this Mock<IServiceProvider> mockServiceProvider, MockCommandLineOptions mockCommandLineOptions)
        {
            var mockConfiguration = new Mock<IConfiguration>();
            mockConfiguration.Setup(c => c["platformOptions:resultDirectory"]).Returns("/test/results");

            mockServiceProvider.Setup(s => s.GetService(It.IsAny<Type>()))
                .Returns<Type>(t =>
                {
                    if (t == typeof(ICommandLineOptions))
                    {
                        return mockCommandLineOptions;
                    }

                    if (t == typeof(IConfiguration))
                    {
                        return mockConfiguration.Object;
                    }

                    return null;
                });

            return mockServiceProvider;
        }
    }
}