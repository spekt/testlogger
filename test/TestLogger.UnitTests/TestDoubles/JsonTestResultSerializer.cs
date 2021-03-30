// Copyright (c) Spekt Contributors. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

namespace Spekt.TestLogger.UnitTests.TestDoubles
{
    using System.Collections.Generic;
    using System.IO;
    using System.Linq;
    using System.Text;
    using Newtonsoft.Json;
    using Spekt.TestLogger.Core;

    /// <summary>
    /// Test logger to serialize results into json format.
    /// </summary>
    /// <remarks>
    /// Sample json output:
    /// <code>
    /// [{
    ///     name: "TestAssembly",
    ///     fixtures: [
    ///         {
    ///             name: "TestClass",
    ///             tests: [
    ///                 {
    ///                     name: "TestMethod",
    ///                     result: (pass|fail|skipped),
    ///                     traits: [
    ///                         { name: "key", value: "value" }
    ///                 }
    ///             ]
    ///         }
    ///     ]
    /// }]
    /// </code>
    /// </remarks>
    public class JsonTestResultSerializer : ITestResultSerializer
    {
        public string Serialize(
            LoggerConfiguration loggerConfiguration,
            TestRunConfiguration runConfiguration,
            List<TestResultInfo> results,
            List<TestMessageInfo> messages)
        {
            var res = from r in results
                group r by r.AssemblyPath
                into assemblies
                orderby assemblies.Key
                select this.CreateAssembly(assemblies);

            var content = new StringBuilder();
            new JsonSerializer().Serialize(new StringWriter(content), res);
            return content.ToString();
        }

        private TestAssembly CreateAssembly(
            IGrouping<string, TestResultInfo> resultsByAssembly)
        {
            return new ()
            {
                Name = resultsByAssembly.Key,
                Fixtures = resultsByAssembly.GroupBy(a => a.Type).Select(this.CreateFixture)
            };
        }

        private TestFixture CreateFixture(
            IGrouping<string, TestResultInfo> resultsByType)
        {
            return new ()
            {
                Name = resultsByType.Key,
                Tests = resultsByType.Select(this.CreateTest)
            };
        }

        private Test CreateTest(TestResultInfo result)
        {
            return new ()
            {
                Name = result.Method,
                Result = result.Outcome.ToString()
            };
        }

        internal class TestAssembly
        {
            public string Name { get; set; }

            public IEnumerable<TestFixture> Fixtures { get; set; }
        }

        internal class TestFixture
        {
            public string Name { get; set; }

            public IEnumerable<Test> Tests { get; set; }
        }

        internal class Test
        {
            public string Name { get; set; }

            public string Result { get; set; }
        }
    }
}
