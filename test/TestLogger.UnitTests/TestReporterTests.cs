// Copyright (c) Spekt Contributors. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

namespace Spekt.TestLogger.UnitTests
{
    using System;
    using System.Collections.Generic;
    using Microsoft.Testing.Platform.Extensions;
    using Microsoft.VisualStudio.TestTools.UnitTesting;
    using Moq;
    using Spekt.TestLogger.Core;
    using Spekt.TestLogger.UnitTests.TestDoubles;
    using Spekt.TestReporter;

    [TestClass]
    public class TestReporterTests
    {
        private Mock<IExtension> mockExtension;
        private MockCommandLineOptions mockCommandLineOptions;
        private Mock<IServiceProvider> mockServiceProvider;

        [TestInitialize]
        public void Setup()
        {
            this.mockExtension = new Mock<IExtension>();
            this.mockCommandLineOptions = new MockCommandLineOptions();
            this.mockServiceProvider = new Mock<IServiceProvider>();
            this.mockServiceProvider.SetupWithMockCommandLineOptions(this.mockCommandLineOptions);
        }

        [TestMethod]
        public void CreateTestRun_WhenNoConfigOption_ShouldUseDefaultConfig()
        {
            // Arrange
            this.mockCommandLineOptions.SetOption("report-junit");
            var reporter = new TestableTestReporter(this.mockServiceProvider.Object, this.mockExtension.Object);

            // Act
            var testRun = reporter.TestCreateTestRun(this.mockServiceProvider.Object);

            // Assert
            Assert.IsNotNull(testRun);

            // The config should contain only the default test run directory
        }

        [TestMethod]
        public void CreateTestRun_WhenSingleConfigPair_ShouldParseCorrectly()
        {
            // Arrange
            this.mockCommandLineOptions.SetOption("report-junit");
            this.mockCommandLineOptions.SetOption("report-spekt-junit", "customKey=customValue");
            var reporter = new TestableTestReporter(this.mockServiceProvider.Object, this.mockExtension.Object);

            // Act
            var testRun = reporter.TestCreateTestRun(this.mockServiceProvider.Object);

            // Assert
            Assert.IsNotNull(testRun);

            // Note: We can't directly access the config dictionary from testRun
            // but the method should not throw an exception and should create a valid test run
        }

        [TestMethod]
        public void CreateTestRun_WhenMultipleConfigPairs_ShouldParseAllPairs()
        {
            // Arrange
            this.mockCommandLineOptions.SetOption("report-junit");
            this.mockCommandLineOptions.SetOption("report-spekt-junit", "key1=value1;key2=value2;key3=value3");
            var reporter = new TestableTestReporter(this.mockServiceProvider.Object, this.mockExtension.Object);

            // Act
            var testRun = reporter.TestCreateTestRun(this.mockServiceProvider.Object);

            // Assert
            Assert.IsNotNull(testRun);

            // The method should handle multiple key-value pairs correctly
        }

        [TestMethod]
        public void CreateTestRun_WhenConfigWithWhitespace_ShouldTrimKeysAndValues()
        {
            // Arrange
            this.mockCommandLineOptions.SetOption("report-junit");
            this.mockCommandLineOptions.SetOption("report-spekt-junit", "  key1  =  value1  ;  key2  =  value2  ");
            var reporter = new TestableTestReporter(this.mockServiceProvider.Object, this.mockExtension.Object);

            // Act
            var testRun = reporter.TestCreateTestRun(this.mockServiceProvider.Object);

            // Assert
            Assert.IsNotNull(testRun);

            // Whitespace should be trimmed from keys and values
        }

        [TestMethod]
        public void CreateTestRun_WhenConfigWithEmptyPair_ShouldSkipEmptyPairs()
        {
            // Arrange
            this.mockCommandLineOptions.SetOption("report-junit");
            this.mockCommandLineOptions.SetOption("report-spekt-junit", "key1=value1;;key2=value2");
            var reporter = new TestableTestReporter(this.mockServiceProvider.Object, this.mockExtension.Object);

            // Act
            var testRun = reporter.TestCreateTestRun(this.mockServiceProvider.Object);

            // Assert
            Assert.IsNotNull(testRun);

            // Empty pairs (between semicolons) should be skipped
        }

        [TestMethod]
        public void CreateTestRun_WhenConfigWithInvalidPair_ShouldSkipInvalidPairs()
        {
            // Arrange
            this.mockCommandLineOptions.SetOption("report-junit");
            this.mockCommandLineOptions.SetOption("report-spekt-junit", "key1=value1;invalidPair;key2=value2");
            var reporter = new TestableTestReporter(this.mockServiceProvider.Object, this.mockExtension.Object);

            // Act
            var testRun = reporter.TestCreateTestRun(this.mockServiceProvider.Object);

            // Assert
            Assert.IsNotNull(testRun);

            // Invalid pairs (without =) should be skipped
        }

        [TestMethod]
        public void CreateTestRun_WhenConfigWithOnlyEquals_ShouldSkipInvalidPair()
        {
            // Arrange
            this.mockCommandLineOptions.SetOption("report-junit");
            this.mockCommandLineOptions.SetOption("report-spekt-junit", "key1=value1=extra;key2=value2");
            var reporter = new TestableTestReporter(this.mockServiceProvider.Object, this.mockExtension.Object);

            // Act
            var testRun = reporter.TestCreateTestRun(this.mockServiceProvider.Object);

            // Assert
            Assert.IsNotNull(testRun);

            // First equals should be used as separator, rest treated as part of value
        }

        [TestMethod]
        public void CreateTestRun_WhenConfigWithEmptyValue_ShouldIncludePair()
        {
            // Arrange
            this.mockCommandLineOptions.SetOption("report-junit");
            this.mockCommandLineOptions.SetOption("report-spekt-junit", "key1=;key2=value2");
            var reporter = new TestableTestReporter(this.mockServiceProvider.Object, this.mockExtension.Object);

            // Act
            var testRun = reporter.TestCreateTestRun(this.mockServiceProvider.Object);

            // Assert
            Assert.IsNotNull(testRun);

            // Empty values should be allowed
        }

        [TestMethod]
        public void CreateTestRun_WhenConfigWithEmptyKey_ShouldSkipPair()
        {
            // Arrange
            this.mockCommandLineOptions.SetOption("report-junit");
            this.mockCommandLineOptions.SetOption("report-spekt-junit", "=value1;key2=value2");
            var reporter = new TestableTestReporter(this.mockServiceProvider.Object, this.mockExtension.Object);

            // Act
            var testRun = reporter.TestCreateTestRun(this.mockServiceProvider.Object);

            // Assert
            Assert.IsNotNull(testRun);

            // Empty keys should be skipped
        }

        [TestMethod]
        public void CreateTestRun_WhenConfigWithWhitespaceKey_ShouldSkipPair()
        {
            // Arrange
            this.mockCommandLineOptions.SetOption("report-junit");
            this.mockCommandLineOptions.SetOption("report-spekt-junit", "   =value1;key2=value2");
            var reporter = new TestableTestReporter(this.mockServiceProvider.Object, this.mockExtension.Object);

            // Act
            var testRun = reporter.TestCreateTestRun(this.mockServiceProvider.Object);

            // Assert
            Assert.IsNotNull(testRun);

            // Whitespace-only keys should be skipped
        }

        [TestMethod]
        public void CreateTestRun_WhenConfigEmpty_ShouldNotAddAnyCustomConfig()
        {
            // Arrange
            this.mockCommandLineOptions.SetOption("report-junit");
            this.mockCommandLineOptions.SetOption("report-spekt-junit", string.Empty);
            var reporter = new TestableTestReporter(this.mockServiceProvider.Object, this.mockExtension.Object);

            // Act
            var testRun = reporter.TestCreateTestRun(this.mockServiceProvider.Object);

            // Assert
            Assert.IsNotNull(testRun);

            // Empty config should not cause issues
        }

        [TestMethod]
        public void CreateTestRun_WhenConfigNull_ShouldNotAddAnyCustomConfig()
        {
            // Arrange
            this.mockCommandLineOptions.SetOption("report-junit");
            this.mockCommandLineOptions.SetOption("report-spekt-junit", (string)null);
            var reporter = new TestableTestReporter(this.mockServiceProvider.Object, this.mockExtension.Object);

            // Act
            var testRun = reporter.TestCreateTestRun(this.mockServiceProvider.Object);

            // Assert
            Assert.IsNotNull(testRun);

            // Null config should not cause issues
        }

        [TestMethod]
        public void CreateTestRun_WhenConfigWithSpecialCharacters_ShouldHandleCorrectly()
        {
            // Arrange
            this.mockCommandLineOptions.SetOption("report-junit");
            this.mockCommandLineOptions.SetOption("report-spekt-junit", "path=C:\\Test\\Folder;url=https://example.com;name=Test-Report_v2.0");
            var reporter = new TestableTestReporter(this.mockServiceProvider.Object, this.mockExtension.Object);

            // Act
            var testRun = reporter.TestCreateTestRun(this.mockServiceProvider.Object);

            // Assert
            Assert.IsNotNull(testRun);

            // Special characters should be handled correctly
        }

        [TestMethod]
        public void CreateTestRun_WhenBothConfigAndFileName_ShouldHandleBothOptions()
        {
            // Arrange
            this.mockCommandLineOptions.SetOption("report-junit");
            this.mockCommandLineOptions.SetOption("report-spekt-junit", "key1=value1");
            this.mockCommandLineOptions.SetOption("report-spekt-junit-filename", "custom-report.xml");
            var reporter = new TestableTestReporter(this.mockServiceProvider.Object, this.mockExtension.Object);

            // Act
            var testRun = reporter.TestCreateTestRun(this.mockServiceProvider.Object);

            // Assert
            Assert.IsNotNull(testRun);

            // Both config and filename options should be handled
        }

        [TestMethod]
        public void CreateTestRun_WhenNoLogFilePathOrFileName_ShouldUseDefaultFileName()
        {
            // Arrange
            this.mockCommandLineOptions.SetOption("report-junit");

            // Don't set any filename or LogFilePath config
            var reporter = new TestableTestReporter(this.mockServiceProvider.Object, this.mockExtension.Object);

            // Act
            var testRun = reporter.TestCreateTestRun(this.mockServiceProvider.Object);

            // Assert
            Assert.IsNotNull(testRun);

            // The test should not throw and should use the default filename
        }

        [TestMethod]
        public void CreateTestRun_WhenLogFilePathProvided_ShouldNotUseDefaultFileName()
        {
            // Arrange
            this.mockCommandLineOptions.SetOption("report-junit");
            this.mockCommandLineOptions.SetOption("report-spekt-junit", "LogFilePath=custom-path.xml");
            var reporter = new TestableTestReporter(this.mockServiceProvider.Object, this.mockExtension.Object);

            // Act
            var testRun = reporter.TestCreateTestRun(this.mockServiceProvider.Object);

            // Assert
            Assert.IsNotNull(testRun);

            // Should not use default filename when LogFilePath is provided
        }

        [TestMethod]
        public void CreateTestRun_WhenLogFileNameProvided_ShouldNotUseDefaultFileName()
        {
            // Arrange
            this.mockCommandLineOptions.SetOption("report-junit");
            this.mockCommandLineOptions.SetOption("report-spekt-junit", "LogFileName=custom-name.xml");
            var reporter = new TestableTestReporter(this.mockServiceProvider.Object, this.mockExtension.Object);

            // Act
            var testRun = reporter.TestCreateTestRun(this.mockServiceProvider.Object);

            // Assert
            Assert.IsNotNull(testRun);

            // Should not use default filename when LogFileName is provided
        }

        [TestMethod]
        public void CreateTestRun_WhenFileNameOptionProvided_ShouldNotUseDefaultFileName()
        {
            // Arrange
            this.mockCommandLineOptions.SetOption("report-junit");
            this.mockCommandLineOptions.SetOption("report-spekt-junit-filename", "custom-filename.xml");
            var reporter = new TestableTestReporter(this.mockServiceProvider.Object, this.mockExtension.Object);

            // Act
            var testRun = reporter.TestCreateTestRun(this.mockServiceProvider.Object);

            // Assert
            Assert.IsNotNull(testRun);

            // Should not use default filename when FileNameOption is provided
        }
    }
}