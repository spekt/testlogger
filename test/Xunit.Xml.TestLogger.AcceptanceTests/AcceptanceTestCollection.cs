// Copyright (c) Spekt Contributors. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

namespace Xunit.Xml.TestLogger.AcceptanceTests
{
    using Xunit;

    // Fixtures spawn dotnet test subprocesses that share mutable state
    // (test/assets/global.json selects the test runner per leg), so all
    // fixture-bound test classes must run sequentially in one collection.
    [CollectionDefinition("Acceptance")]
    public class AcceptanceTestCollection : ICollectionFixture<TestRunFixture>, ICollectionFixture<NoTestSdkFixture>
    {
    }
}
