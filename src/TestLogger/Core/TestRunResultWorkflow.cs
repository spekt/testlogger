// Copyright (c) Spekt Contributors. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

namespace Spekt.TestLogger.Core
{
    using System;
    using Microsoft.VisualStudio.TestPlatform.ObjectModel.Logging;

    public static class TestRunResultWorkflow
    {
        public static void Result(this ITestRun testRun, TestResultEventArgs resultEvent)
        {
            var fqn = resultEvent.Result.TestCase.FullyQualifiedName;
            testRun.LoggerConfiguration.Values.TryGetValue(LoggerConfiguration.ParserKey, out string parserVal);
            var parsedName = parserVal switch
            {
                string x when x.Equals("Legacy", StringComparison.OrdinalIgnoreCase) => LegacyTestCaseNameParser.Parse(fqn),
                _ => TestCaseNameParser.Parse(fqn),
            };

            testRun.Store.Add(new TestResultInfo(
                resultEvent.Result,
                parsedName.NamespaceName,
                parsedName.TypeName,
                parsedName.MethodName));
        }
    }
}