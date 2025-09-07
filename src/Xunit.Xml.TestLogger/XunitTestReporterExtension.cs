// Copyright (c) Spekt Contributors. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

namespace Spekt.TestReporter.Xunit
{
    using System.Threading.Tasks;
    using Microsoft.Testing.Platform.Extensions;

    internal sealed class XunitTestReporterExtension : IExtension
    {
        public string Uid => nameof(XunitTestReporterExtension);

        // TODO:
        public string Version => "1.0.0";

        public string DisplayName => "Xunit test reporter extension";

        public string Description => "Xunit test reporter extension";

        public Task<bool> IsEnabledAsync()
            => Task.FromResult(true);
    }
}
