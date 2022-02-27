using NUnit.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Json.TestLogger.NUnit.NetCore.Tests.NetCoreOnly
{

    public class Issue28_Examples
    {
        [TestCaseSource(nameof(ExampleTestCases))]
        public void ExampleTest1_Tuples((string a, string b) s) => Assert.Pass();

        private static readonly IReadOnlyList<(string, string)> ExampleTestCases =
            new List<(string, string)>
            {
                (null, ""),
                ("test", "test"),
                (@"\", @"\\"),
                (@"""", @" "), // parsing error
                (@"[", @" "),
                (@"]", @" ")
            };

        [TestCaseSource(nameof(ExampleTest2Cases))]
        public void ExampleTest2(bool a, decimal? b, decimal? c, (decimal?, bool) d) => Assert.Pass();
        private static IEnumerable<TestCaseData> ExampleTest2Cases()
        {
            yield return new TestCaseData(false, null, 4.5m, ((decimal?)null, false));
            yield return new TestCaseData(false, 4.5m, 4.6m, ((decimal?)4.5m, false)); // parsing error
            yield return new TestCaseData(false, 4.5m, null, ((decimal?)4.5m, false));
            yield return new TestCaseData(true, 4.3m, null, ((decimal?)4.3m, false));
            yield return new TestCaseData(true, 4.1m, 5.0m, ((decimal?)5.0m, true)); // parsing error
            yield return new TestCaseData(true, 4.8m, 4.5m, ((decimal?)4.8m, false)); // parsing error
            yield return new TestCaseData(true, 4.5m, 4.5m, ((decimal?)4.5m, false)); // parsing error
        }
    }
}
