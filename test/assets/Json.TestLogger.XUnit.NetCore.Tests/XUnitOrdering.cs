using System.Collections.Generic;
using System.Linq;
using Xunit;
using Xunit.Abstractions;
using Xunit.Sdk;

// Added to ensure we get deterministic log message order for verification testing.
// Example code from https://docs.microsoft.com/en-us/dotnet/core/testing/order-unit-tests?pivots=xunit
[assembly: CollectionBehavior(DisableTestParallelization = true)]
[assembly: TestCaseOrderer("Json.TestLogger.XUnit.Orderers.AlphabeticalOrderer", "Json.TestLogger.XUnit")]
[assembly: TestCollectionOrderer("Json.TestLogger.XUnit.Orderers.DisplayNameOrderer", "Json.TestLogger.XUnit")]

namespace Json.TestLogger.XUnit.Orderers
{
    public class DisplayNameOrderer : ITestCollectionOrderer
    {
        public IEnumerable<ITestCollection> OrderTestCollections(
            IEnumerable<ITestCollection> testCollections) =>
            testCollections.OrderBy(collection => collection.DisplayName);
    }

    public class AlphabeticalOrderer : ITestCaseOrderer
    {
        public IEnumerable<TTestCase> OrderTestCases<TTestCase>(
            IEnumerable<TTestCase> testCases) where TTestCase : ITestCase =>
            testCases.OrderBy(testCase => testCase.TestMethod.Method.Name);
    }
}