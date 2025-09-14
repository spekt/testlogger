// Copyright (c) Spekt Contributors. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

namespace Spekt.TestLogger.UnitTests
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Threading.Tasks;
    using Microsoft.Testing.Platform.CommandLine;
    using Microsoft.Testing.Platform.Extensions;
    using Microsoft.Testing.Platform.Extensions.CommandLine;
    using Microsoft.VisualStudio.TestTools.UnitTesting;
    using Spekt.TestReporter;

    [TestClass]
    public class TestReporterCommandLineProviderTests
    {
        private MockExtension mockExtension;
        private TestReporterCommandLineProvider provider;

        [TestInitialize]
        public void Setup()
        {
            this.mockExtension = new MockExtension();
            this.provider = new TestReporterCommandLineProvider(this.mockExtension, "junit");
        }

        [TestMethod]
        public void Constructor_ShouldInitializeWithLoggerName()
        {
            // Arrange & Act
            var provider = new TestReporterCommandLineProvider(this.mockExtension, "nunit");

            // Assert
            Assert.AreEqual("report-nunit", provider.ReportOption);
            Assert.AreEqual("report-nunit-filename", provider.ReportFileNameOption);
            Assert.AreEqual("report-nunit-config", provider.ReportConfigOption);
        }

        [TestMethod]
        public void GetCommandLineOptions_ShouldReturnThreeOptions()
        {
            // Act
            var options = this.provider.GetCommandLineOptions().ToArray();

            // Assert
            Assert.AreEqual(3, options.Length);
            Assert.AreEqual("report-junit", options[0].Name);
            Assert.AreEqual("report-junit-filename", options[1].Name);
            Assert.AreEqual("report-junit-config", options[2].Name);
        }

        [TestMethod]
        public void GetCommandLineOptions_ShouldHaveCorrectDescriptions()
        {
            // Act
            var options = this.provider.GetCommandLineOptions().ToArray();

            // Assert
            StringAssert.Contains(options[0].Description, "junit");
            StringAssert.Contains(options[1].Description, "junit");
            StringAssert.Contains(options[2].Description, "junit");
            StringAssert.Contains(options[2].Description, "key-value");
        }

        [TestMethod]
        public async Task ValidateCommandLineOptionsAsync_ShouldFailWhenFilenameWithoutReport()
        {
            // Arrange
            var mockOptions = new MockCommandLineOptions();
            mockOptions.SetOption("report-junit-filename", "test.xml");

            // Act
            var result = await this.provider.ValidateCommandLineOptionsAsync(mockOptions);

            // Assert
            Assert.IsFalse(result.IsValid);
            StringAssert.Contains(result.ErrorMessage, "requires --report-junit");
        }

        [TestMethod]
        public async Task ValidateCommandLineOptionsAsync_ShouldFailWhenConfigWithoutReport()
        {
            // Arrange
            var mockOptions = new MockCommandLineOptions();
            mockOptions.SetOption("report-junit-config", "key=value");

            // Act
            var result = await this.provider.ValidateCommandLineOptionsAsync(mockOptions);

            // Assert
            Assert.IsFalse(result.IsValid);
            StringAssert.Contains(result.ErrorMessage, "requires --report-junit");
        }

        [TestMethod]
        public async Task ValidateCommandLineOptionsAsync_ShouldPassWhenValid()
        {
            // Arrange
            var mockOptions = new MockCommandLineOptions();
            mockOptions.SetOption("report-junit");

            // Act
            var result = await this.provider.ValidateCommandLineOptionsAsync(mockOptions);

            // Assert
            Assert.IsTrue(result.IsValid);
        }

        [TestMethod]
        public async Task ValidateOptionArgumentsAsync_ShouldFailInvalidConfigFormat()
        {
            // Arrange
            var commandOption = new CommandLineOption("report-junit-config", "Config", ArgumentArity.ExactlyOne, false);
            var invalidArguments = new[] { "invalid-format" };

            // Act
            var result = await this.provider.ValidateOptionArgumentsAsync(commandOption, invalidArguments);

            // Assert
            Assert.IsFalse(result.IsValid);
            StringAssert.Contains(result.ErrorMessage, "Invalid config format");
        }

        [TestMethod]
        public async Task ValidateOptionArgumentsAsync_ShouldPassValidConfigFormat()
        {
            // Arrange
            var commandOption = new CommandLineOption("report-junit-config", "Config", ArgumentArity.ExactlyOne, false);
            var validArguments = new[] { "key1=value1;key2=value2" };

            // Act
            var result = await this.provider.ValidateOptionArgumentsAsync(commandOption, validArguments);

            // Assert
            Assert.IsTrue(result.IsValid);
        }

        [TestMethod]
        public async Task ValidateOptionArgumentsAsync_ShouldIgnoreNonConfigOptions()
        {
            // Arrange
            var commandOption = new CommandLineOption("report-junit-filename", "Filename", ArgumentArity.ExactlyOne, false);
            var arguments = new[] { "test.xml" };

            // Act
            var result = await this.provider.ValidateOptionArgumentsAsync(commandOption, arguments);

            // Assert
            Assert.IsTrue(result.IsValid);
        }

        private class MockExtension : IExtension
        {
            public string Uid => "test-extension";

            public string Version => "1.0.0";

            public string DisplayName => "Test Extension";

            public string Description => "Mock extension for testing";

            public Task<bool> IsEnabledAsync()
            {
                return Task.FromResult(true);
            }
        }

        private class MockCommandLineOptions : ICommandLineOptions
        {
            private readonly Dictionary<string, string[]> options = new Dictionary<string, string[]>();

            public void SetOption(string name, params string[] arguments)
            {
                this.options[name] = arguments;
            }

            public bool TryGetOptionArgumentList(string optionName, out string[] arguments)
            {
                return this.options.TryGetValue(optionName, out arguments);
            }

            public bool IsOptionSet(string optionName)
            {
                return this.options.ContainsKey(optionName);
            }

            // Implement other required methods with minimal functionality
            public IEnumerable<CommandLineOption> GetCommandLineOptions() => new List<CommandLineOption>();

            public bool TryGetOptionArgument(string optionName, out string argument) => throw new NotImplementedException();

            public bool TryGetOptionArgumentList(string optionName, out IReadOnlyList<string> arguments)
            {
                arguments = null;
                return false;
            }
        }
    }
}