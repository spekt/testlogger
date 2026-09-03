// Copyright (c) Spekt Contributors. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

namespace Spekt.TestLogger.VSTest.Core
{
    using System.Linq;
    using System.Xml;
    using Microsoft.VisualStudio.TestPlatform.ObjectModel.Client;
    using Spekt.TestLogger.Core;

    public static class VSTestTestRunStartWorkflow
    {
        public static TestRunConfiguration Start(this ITestRun testRun, TestRunStartEventArgs startedEvent)
        {
            var assemblyPath = startedEvent.TestRunCriteria.Sources.First();
            var runSettings = new XmlDocument();
            runSettings.LoadXml(startedEvent.TestRunCriteria.TestRunSettings);
            var framework = runSettings
                .GetElementsByTagName("TargetFrameworkVersion")[0]
                .InnerText;

            return testRun.Start(assemblyPath, framework);
        }
    }
}
