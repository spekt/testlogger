// Copyright (c) Spekt Contributors. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

namespace Spekt.TestLogger.UnitTests.TestDoubles
{
    using System;
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
    /// {
    ///     "TestAssemblies": [
    ///         {
    ///             "Name": "TestAssembly",
    ///             "Fixtures": [
    ///                 {
    ///                     "Name": "TestClass",
    ///                     "Tests": [
    ///                         {
    ///                             "Name": "TestMethod",
    ///                             "Result": (pass|fail|skipped),
    ///                         }
    ///                     ]
    ///                 }
    ///             ]
    ///         }
    ///     ],
    ///     "TestMessages": [
    ///         {
    ///             "Level": (0,1,2),
    ///             "Message": "MessageText"
    ///         }
    ///     ]
    /// }
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
            new JsonSerializer().Serialize(new StringWriter(content), new TestReport(res, messages));
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
                FullyQualifiedName = result.TestCase.FullyQualifiedName,
                DisplayName = result.TestCase.DisplayName,
                Namespace = result.Namespace,
                Type = result.Type,
                Method = result.Method,
                Result = result.Outcome.ToString(),
            };
        }

        public class TestReport
        {
            public TestReport(IEnumerable<TestAssembly> testAssemblies, IEnumerable<TestMessageInfo> testMessages)
            {
                this.TestAssemblies = testAssemblies ?? throw new ArgumentNullException(nameof(testAssemblies));
                this.TestMessages = testMessages ?? throw new ArgumentNullException(nameof(testMessages));
            }

            public IEnumerable<TestAssembly> TestAssemblies { get; set; }

            public IEnumerable<TestMessageInfo> TestMessages { get; set; }
        }

        public class TestAssembly
        {
            public string Name { get; set; }

            public IEnumerable<TestFixture> Fixtures { get; set; }
        }

        public class TestFixture
        {
            public string Name { get; set; }

            public IEnumerable<Test> Tests { get; set; }
        }

        public class Test
        {
            public string FullyQualifiedName { get; set; }

            public string DisplayName { get; set; }

            public string Namespace { get; set; }

            public string Type { get; set; }

            public string Method { get; set; }

            public string Result { get; set; }
        }
    }
}
