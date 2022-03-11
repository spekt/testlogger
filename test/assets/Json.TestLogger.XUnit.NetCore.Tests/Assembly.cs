
// Added to ensure we get deterministic log message order for verification testing.
// Example code from https://docs.microsoft.com/en-us/dotnet/core/testing/order-unit-tests?pivots=xunit
[assembly: Xunit.CollectionBehavior(DisableTestParallelization = true)]
[assembly: Xunit.TestCaseOrderer("Json.TestLogger.XUnit.Orderers.AlphabeticalOrderer", "Json.TestLogger.XUnit.NetCore.Tests")]
[assembly: Xunit.TestCollectionOrderer("Json.TestLogger.XUnit.Orderers.DisplayNameOrderer", "Json.TestLogger.XUnit.NetCore.Tests")]
