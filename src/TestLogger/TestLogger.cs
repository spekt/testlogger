// Copyright (c) Spekt Contributors. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

namespace Spekt.TestLogger
{
    using System;
    using System.Collections.Generic;
    using System.Globalization;
    using System.IO;
    using System.Linq;
    using System.Text;
    using System.Xml;
    using System.Xml.Linq;
    using Microsoft.VisualStudio.TestPlatform.ObjectModel;
    using Microsoft.VisualStudio.TestPlatform.ObjectModel.Client;
    using Microsoft.VisualStudio.TestPlatform.ObjectModel.Logging;

    /// <summary>
    /// Base test logger implementation.
    /// </summary>
    public abstract class TestLogger : ITestLoggerWithParameters
    {
        // Dicionary keys for command line arguments.
        public const string LogFilePathKey = "LogFilePath";
        public const string LogFileNameKey = "LogFileName";
        public const string ResultDirectoryKey = "TestRunDirectory";
        public const string FileEncodingKey = "FileEncoding";

        // Other public strings
        public const string ResultStatusPassed = "Passed";
        public const string ResultStatusFailed = "Failed";

        // Tokens to allow user to manipulate output file or directory names.
        private const string AssemblyToken = "{assembly}";
        private const string FrameworkToken = "{framework}";

        private readonly object resultsGuard = new object();
        private string outputFilePath;

        private List<TestResultInfo> results;

        public enum FileEncoding
        {
            /// <summary>
            /// UTF8
            /// </summary>
            UTF8,

            /// <summary>
            /// UTF8 Bom
            /// </summary>
            UTF8Bom
        }

        public FileEncoding FileEncodingOption { get; private set; } = FileEncoding.UTF8;

        public DateTime RunStartTimeUtc { get; set; }

        /// <summary>
        /// Gets uri used to uniquely identify the logger.
        /// </summary>
        /// <returns>Returns the extension URI for the implementation.</returns>
        public abstract string GetExtensionUri();

        /// <summary>
        /// Gets Alternate user friendly string to uniquely identify the console logger.
        /// </summary>
        /// <returns>Returns the friendly name for the implementation.</returns>
        public abstract string GetFriendlyName();

        public abstract XDocument BuildLog(List<TestResultInfo> resultList);

        public abstract void Initialize(Dictionary<string, string> parameters);

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

            if (parameters.TryGetValue(FileEncodingKey, out string fileEncoding))
            {
                if (Enum.TryParse(fileEncoding, true, out FileEncoding encoding))
                {
                    this.FileEncodingOption = encoding;
                }
                else
                {
                    Console.WriteLine($"JunitXML Logger: The provided File Encoding '{fileEncoding}' is not a recognized option. Using default");
                }
            }

            this.Initialize(parameters);
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
                this.results = new List<TestResultInfo>();
            }

            this.RunStartTimeUtc = DateTime.UtcNow;
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
        }

        /// <summary>
        /// Called when a test result is received.
        /// </summary>
        private void TestResultHandler(object sender, TestResultEventArgs e)
        {
            TestResult result = e.Result;

            var parsedName = TestCaseNameParser.Parse(result.TestCase.FullyQualifiedName, this.GetFriendlyName());

            lock (this.resultsGuard)
            {
                this.results.Add(new TestResultInfo(
                    result,
                    parsedName.NamespaceName,
                    parsedName.TypeName,
                    parsedName.MethodName));
            }
        }

        /// <summary>
        /// Called when a test run is completed.
        /// </summary>
        private void TestRunCompleteHandler(object sender, TestRunCompleteEventArgs e)
        {
            try
            {
                List<TestResultInfo> resultList;
                lock (this.resultsGuard)
                {
                    resultList = this.results;
                    this.results = new List<TestResultInfo>();
                }

                var doc = this.BuildLog(resultList);

                // Create directory if not exist
                var loggerFileDirPath = Path.GetDirectoryName(this.outputFilePath);
                if (!Directory.Exists(loggerFileDirPath))
                {
                    Directory.CreateDirectory(loggerFileDirPath);
                }

                var settings = new XmlWriterSettings()
                {
                    Encoding = new UTF8Encoding(this.FileEncodingOption == FileEncoding.UTF8Bom),
                    Indent = true,
                };

                using (var f = File.Create(this.outputFilePath))
                {
                    using (var w = XmlWriter.Create(f, settings))
                    {
                        doc.Save(w);
                    }
                }

                var resultsFileMessage = string.Format(CultureInfo.CurrentCulture, "{0} Logger - Results File: {1}", this.GetFriendlyName(), this.outputFilePath);
                Console.WriteLine(Environment.NewLine + resultsFileMessage);
            }
            catch (Exception ex)
            {
                Console.WriteLine($"{this.GetFriendlyName()} Logger: Threw an unhandeled exception. ");
                Console.WriteLine(ex.Message);
                Console.WriteLine(ex.Source);
                throw;
            }
        }
    }
}
