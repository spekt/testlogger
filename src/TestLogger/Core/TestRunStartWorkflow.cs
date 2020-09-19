// Copyright (c) Spekt Contributors. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

namespace Spekt.TestLogger.Core
{
    using System;
    using Microsoft.VisualStudio.TestPlatform.ObjectModel.Client;

    public static class TestRunStartWorkflow
    {
        public static void Start(this ITestRun testRun, TestRunStartEventArgs startedEvent)
        {
#if NONE
            if (this.outputFilePath.Contains(AssemblyToken))
            {
                string assemblyPath = e.TestRunCriteria.AdapterSourceMap["_none_"].First();
                string assemblyName = Path.GetFileNameWithoutExtension(assemblyPath);
                this.outputFilePath = this.outputFilePath.Replace(AssemblyToken, assemblyName);
            }

            if (this.outputFilePath.Contains(FrameworkToken))
            {
                XmlDocument runSettings = new XmlDocument();
                runSettings.LoadXml(e.TestRunCriteria.TestRunSettings);
                XmlNode x = runSettings.GetElementsByTagName("TargetFrameworkVersion")[0];
                string framework = x.InnerText;
                framework = framework.Replace(",Version=v", string.Empty).Replace(".", string.Empty);
                this.outputFilePath = this.outputFilePath.Replace(FrameworkToken, framework);
            }
#endif
            throw new NotImplementedException();
        }
    }
}