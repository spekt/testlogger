// Copyright (c) Spekt Contributors. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

namespace Spekt.TestLogger
{
    using System;
    using System.Collections.Generic;
    using System.Globalization;
    using System.IO;

    using Microsoft.VisualStudio.TestPlatform.ObjectModel;
    using Microsoft.VisualStudio.TestPlatform.ObjectModel.Client;
    using Microsoft.VisualStudio.TestPlatform.ObjectModel.Logging;

    /// <summary>
    /// Base test logger implementation.
    /// </summary>
    public class TestLogger : ITestLoggerWithParameters
    {
        public const string LogFilePathKey = "LogFilePath";

        private readonly object resultsGuard = new object();
        private string outputFilePath;
        private List<string> results;
        private DateTime localStartTime;

        public void Initialize(TestLoggerEvents events, string testResultsDirPath)
        {
            if (events == null)
            {
                throw new ArgumentNullException(nameof(events));
            }

            if (testResultsDirPath == null)
            {
                throw new ArgumentNullException(nameof(testResultsDirPath));
            }

            var outputPath = Path.Combine(testResultsDirPath, "TestResults.xml");
            this.InitializeImpl(events, outputPath);
        }

        public void Initialize(TestLoggerEvents events, Dictionary<string, string> parameters)
        {
            if (events == null)
            {
                throw new ArgumentNullException(nameof(events));
            }

            if (parameters == null)
            {
                throw new ArgumentNullException(nameof(parameters));
            }

            if (parameters.TryGetValue(LogFilePathKey, out string outputPath))
            {
                this.InitializeImpl(events, outputPath);
            }
            else if (parameters.TryGetValue(DefaultLoggerParameterNames.TestRunDirectory, out string outputDir))
            {
                this.Initialize(events, outputDir);
            }
            else
            {
                throw new ArgumentException($"Expected {LogFilePathKey} or {DefaultLoggerParameterNames.TestRunDirectory} parameter", nameof(parameters));
            }
        }

        private void InitializeImpl(TestLoggerEvents events, string outputPath)
        {
            events.TestRunMessage += this.TestMessageHandler;
            events.TestRunStart += this.TestRunStartHandler;
            events.TestResult += this.TestResultHandler;
            events.TestRunComplete += this.TestRunCompleteHandler;

            this.outputFilePath = Path.GetFullPath(outputPath);

            lock (this.resultsGuard)
            {
                this.results = new List<string>();
            }

            this.localStartTime = DateTime.UtcNow;
        }

        /// <summary>
        /// Called when a test message is received.
        /// </summary>
        private void TestMessageHandler(object sender, TestRunMessageEventArgs e)
        {
        }

        /// <summary>
        /// Called when a test starts.
        /// </summary>
        private void TestRunStartHandler(object sender, TestRunStartEventArgs e)
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
        }

        /// <summary>
        /// Called when a test result is received.
        /// </summary>
        private void TestResultHandler(object sender, TestResultEventArgs e)
        {
            TestResult result = e.Result;

            this.results.Add(result.TestCase.FullyQualifiedName);
#if NONE
            if (TryParseName(result.TestCase.FullyQualifiedName, out var typeName, out var methodName, out _))
            {
                lock (this.resultsGuard)
                {
                    this.results.Add(new TestResultInfo(
                        result,
                        typeName,
                        methodName));
                }
            }
#endif
        }

        /// <summary>
        /// Called when a test run is completed.
        /// </summary>
        private void TestRunCompleteHandler(object sender, TestRunCompleteEventArgs e)
        {
            List<string> resultList;
            lock (this.resultsGuard)
            {
                resultList = this.results;
                this.results = new List<string>();
            }

            // Create directory if not exist
            var loggerFileDirPath = Path.GetDirectoryName(this.outputFilePath);
            if (!Directory.Exists(loggerFileDirPath))
            {
                Directory.CreateDirectory(loggerFileDirPath);
            }

            using (var f = File.Create(this.outputFilePath))
            {
            }

            var resultsFileMessage = string.Format(CultureInfo.CurrentCulture, "Results File: {0}", this.outputFilePath);
            Console.WriteLine(resultsFileMessage);
        }
    }
}
