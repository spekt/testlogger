// Copyright (c) Spekt Contributors. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

namespace Spekt.TestLogger.UnitTests
{
    using System;
    using System.Collections.Generic;
    using Microsoft.VisualStudio.TestPlatform.ObjectModel;
    using Microsoft.VisualStudio.TestTools.UnitTesting;
    using Spekt.TestLogger.Core;

    [TestClass]
    public class LoggerConfigurationTests
    {
        [TestMethod]
        [DataRow(null)]
        [DataRow("")]
        public void LoggerConfigurationShouldThrowIfLogFilePathIsNullOrEmpty(string logFilePath)
        {
            var config = new Dictionary<string, string>
            {
                { LoggerConfiguration.LogFilePathKey, logFilePath }
            };

            Assert.ThrowsExactly<ArgumentNullException>(new Action(() =>
                new LoggerConfiguration(config)));
        }

        [TestMethod]
        public void LoggerConfigurationShouldThrowIfLogFileNameIsNotSet()
        {
            var config = new Dictionary<string, string>
            {
                { DefaultLoggerParameterNames.TestRunDirectory, "/tmp" }
            };

            Assert.ThrowsExactly<ArgumentException>(new Action(() =>
                new LoggerConfiguration(config)));
        }

        [TestMethod]
        [DataRow(null)]
        [DataRow("")]
        public void LoggerConfigurationShouldThrowIfLogFileNameIsEmptyOrNull(string logFileName)
        {
            var config = new Dictionary<string, string>
            {
                { LoggerConfiguration.LogFileNameKey, logFileName },
                { DefaultLoggerParameterNames.TestRunDirectory, "/tmp" }
            };

            Assert.ThrowsExactly<ArgumentNullException>(new Action(() =>
                new LoggerConfiguration(config)));
        }

        [TestMethod]
        public void LoggerConfigurationShouldThrowIfTestRunDirectoryIsNotSet()
        {
            var config = new Dictionary<string, string>
            {
                { LoggerConfiguration.LogFileNameKey, "result.json" },
            };

            Assert.ThrowsExactly<ArgumentException>(new Action(() =>
                new LoggerConfiguration(config)));
        }

        [TestMethod]
        [DataRow(null)]
        [DataRow("")]
        public void LoggerConfigurationShouldThrowIfTestRunDirectoryIsEmptyOrNull(string dir)
        {
            var config = new Dictionary<string, string>
            {
                { LoggerConfiguration.LogFileNameKey, "result.json" },
                { DefaultLoggerParameterNames.TestRunDirectory, dir }
            };

            Assert.ThrowsExactly<ArgumentNullException>(new Action(() =>
                new LoggerConfiguration(config)));
        }

        [TestMethod]
        public void LoggerConfigurationShouldSetLogFilePath()
        {
            var config = new Dictionary<string, string>
            {
                { LoggerConfiguration.LogFilePathKey, "/tmp/results/result.json" },
            };

            var loggerConfiguration = new LoggerConfiguration(config);

            Assert.AreEqual("/tmp/results/result.json", loggerConfiguration.LogFilePath);
        }

        [TestMethod]
        public void LoggerConfigurationShouldSetLogFilePathWithLogFileName()
        {
            var config = new Dictionary<string, string>
            {
                { LoggerConfiguration.LogFileNameKey, "result.json" },
                { DefaultLoggerParameterNames.TestRunDirectory, "/tmp/results" }
            };

            var loggerConfiguration = new LoggerConfiguration(config);

            // Prevents test from failing on windows.
            var forwardSlashPath = loggerConfiguration.LogFilePath.Replace('\\', '/');

            Assert.AreEqual("/tmp/results/result.json", forwardSlashPath);
        }

        [TestMethod]
        public void GetFormattedLogFilePathShouldReplaceAssemblyToken()
        {
            var runConfig = new TestRunConfiguration { AssemblyPath = "/tmp/foo.dll" };
            var config = new Dictionary<string, string>
            {
                { LoggerConfiguration.LogFilePathKey, "/tmp/results/{assembly}.json" },
            };
            var loggerConfiguration = new LoggerConfiguration(config);

            var logFilePath = loggerConfiguration.GetFormattedLogFilePath(runConfig);

            Assert.AreEqual("/tmp/results/foo.json", logFilePath);
        }

        [TestMethod]
        public void GetFormattedLogFilePathShouldReplaceFrameworkToken()
        {
            var runConfig = new TestRunConfiguration { TargetFramework = ".NETCoreApp,Version=v5.0" };
            var config = new Dictionary<string, string>
            {
                { LoggerConfiguration.LogFilePathKey, "/tmp/results/{framework}.json" },
            };
            var loggerConfiguration = new LoggerConfiguration(config);

            var logFilePath = loggerConfiguration.GetFormattedLogFilePath(runConfig);

            Assert.AreEqual("/tmp/results/NETCoreApp50.json", logFilePath);
        }

        [TestMethod]
        public void UseRelativeAttachmentPathsShouldBeFalseByDefault()
        {
            var config = new Dictionary<string, string>
            {
                { LoggerConfiguration.LogFilePathKey, "/tmp/results/result.json" },
            };

            var loggerConfiguration = new LoggerConfiguration(config);

            Assert.IsFalse(loggerConfiguration.UseRelativeAttachmentPaths);
        }

        [TestMethod]
        [DataRow("true")]
        [DataRow("True")]
        public void UseRelativeAttachmentPathsShouldBeTrueWhenSet(string value)
        {
            var config = new Dictionary<string, string>
            {
                { LoggerConfiguration.LogFilePathKey, "/tmp/results/result.json" },
                { LoggerConfiguration.UseRelativeAttachmentPathKey, value }
            };

            var loggerConfiguration = new LoggerConfiguration(config);

            Assert.IsTrue(loggerConfiguration.UseRelativeAttachmentPaths);
        }
    }
}