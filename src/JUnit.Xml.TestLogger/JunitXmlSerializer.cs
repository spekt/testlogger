﻿// Copyright (c) Spekt Contributors. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

namespace Microsoft.VisualStudio.TestPlatform.Extension.Junit.Xml.TestLogger
{
    using System;
    using System.Collections.Generic;
    using System.Globalization;
    using System.IO;
    using System.Linq;
    using System.Text;
    using System.Xml.Linq;
    using Microsoft.VisualStudio.TestPlatform.ObjectModel;
    using Microsoft.VisualStudio.TestPlatform.ObjectModel.Logging;
    using Spekt.TestLogger.Core;
    using Spekt.TestLogger.Utilities;

    public class JunitXmlSerializer : ITestResultSerializer
    {
        // Dicionary keys for command line arguments.
        private const string MethodFormatKey = "MethodFormat";
        private const string FailureBodyFormatKey = "FailureBodyFormat";
        private const string StoreConsoleOutputKey = "StoreConsoleOutput";

        private const string ResultStatusPassed = "Passed";
        private const string ResultStatusFailed = "Failed";

        public enum MethodFormat
        {
            /// <summary>
            /// The method format will be the method only (i.e. Class.Method())
            /// </summary>
            Default,

            /// <summary>
            /// The method format will include the class and method name (i.e. Class.Method())
            /// </summary>
            Class,

            /// <summary>
            /// The method format will include the namespace, class and method (i.e. Namespace.Class.Method())
            /// </summary>
            Full,
        }

        public enum FailureBodyFormat
        {
            /// <summary>
            /// The failure body will incldue only the error stack trace.
            /// </summary>
            Default,

            /// <summary>
            /// The failure body will incldue the Expected/Actual messages.
            /// </summary>
            Verbose,
        }

        public enum StoreConsoleOutputConfiguration
        {
            /// <summary>
            /// Console output will be stored at both test suite and test case level.
            /// </summary>
            Both,

            /// <summary>
            /// No console output will be stored.
            /// </summary>
            None,

            /// <summary>
            /// Console output will be stored at test case level only.
            /// </summary>
            TestCase,

            /// <summary>
            /// Console output will be stored at test suite level only.
            /// </summary>
            TestSuite
        }

        public IInputSanitizer InputSanitizer { get; } = new InputSanitizerXml();

        public MethodFormat MethodFormatOption { get; private set; } = MethodFormat.Default;

        public FailureBodyFormat FailureBodyFormatOption { get; private set; } = FailureBodyFormat.Default;

        public StoreConsoleOutputConfiguration StoreConsoleOutputOption { get; private set; } = StoreConsoleOutputConfiguration.Both;

        public static IEnumerable<TestSuite> GroupTestSuites(IEnumerable<TestSuite> suites)
        {
            var groups = suites;
            var roots = new List<TestSuite>();
            while (groups.Any())
            {
                groups = groups.GroupBy(r =>
                {
                    var name = r.FullName.SubstringBeforeDot();
                    if (string.IsNullOrEmpty(name))
                    {
                        roots.Add(r);
                    }

                    return name;
                })
                                .OrderBy(g => g.Key)
                                .Where(g => !string.IsNullOrEmpty(g.Key))
                                .Select(g => AggregateTestSuites(g, "TestSuite", g.Key.SubstringAfterDot(), g.Key))
                                .ToList();
            }

            return roots;
        }

        public string Serialize(
            LoggerConfiguration loggerConfiguration,
            TestRunConfiguration runConfiguration,
            List<TestResultInfo> results,
            List<TestMessageInfo> messages)
        {
            this.Configure(loggerConfiguration);
            var doc = new XDocument(this.CreateTestSuitesElement(results, runConfiguration, messages));
            return doc.ToString();
        }

        private static TestSuite AggregateTestSuites(
            IEnumerable<TestSuite> suites,
            string testSuiteType,
            string name,
            string fullName)
        {
            var element = new XElement("test-suite");

            int total = 0;
            int passed = 0;
            int failed = 0;
            int skipped = 0;
            int inconclusive = 0;
            int error = 0;
            var time = TimeSpan.Zero;

            foreach (var result in suites)
            {
                total += result.Total;
                passed += result.Passed;
                failed += result.Failed;
                skipped += result.Skipped;
                inconclusive += result.Inconclusive;
                error += result.Error;
                time += result.Time;

                element.Add(result.Element);
            }

            element.SetAttributeValue("type", testSuiteType);
            element.SetAttributeValue("name", name);
            element.SetAttributeValue("fullname", fullName);
            element.SetAttributeValue("total", total);
            element.SetAttributeValue("passed", passed);
            element.SetAttributeValue("failed", failed);
            element.SetAttributeValue("inconclusive", inconclusive);
            element.SetAttributeValue("skipped", skipped);

            var resultString = failed > 0 ? ResultStatusFailed : ResultStatusPassed;
            element.SetAttributeValue("result", resultString);
            element.SetAttributeValue("duration", time.TotalSeconds);

            return new TestSuite
            {
                Element = element,
                Name = name,
                FullName = fullName,
                Total = total,
                Passed = passed,
                Failed = failed,
                Inconclusive = inconclusive,
                Skipped = skipped,
                Error = error,
                Time = time
            };
        }

        /// <summary>
        /// Produces a consistently indented output, taking into account that incoming messages
        /// often have new lines within a message.
        /// </summary>
        private static string Indent(IReadOnlyCollection<TestResultMessage> messages)
        {
            var indent = "    ";

            // Splitting on any line feed or carrage return because a message may include new lines
            // that are inconsistent with the Environment.NewLine. We then remove all blank lines so
            // it shouldn't cause an issue that this generates extra line breaks.
            return
                indent +
                string.Join(
                    $"{Environment.NewLine}{indent}",
                    messages.SelectMany(m =>
                        m.Text.Split(new string[] { "\r", "\n" }, StringSplitOptions.None)
                            .Where(x => !string.IsNullOrWhiteSpace(x))
                            .Select(x => x.Trim())));
        }

        private static IEnumerable<XElement> CreatePropertyElement(string name, params string[] values)
        {
            if (string.IsNullOrWhiteSpace(name))
            {
                throw new ArgumentException("message", nameof(name));
            }

            foreach (var value in values)
            {
                yield return new XElement(
                    "property",
                    new XAttribute("name", name),
                    new XAttribute("value", value));
            }
        }

        private static XElement CreatePropertyElement(Trait trait)
        {
            return CreatePropertyElement(trait.Name, trait.Value).Single();
        }

        private static XElement CreatePropertiesElement(TestResultInfo result)
        {
            var propertyElements = new HashSet<XNode>(result.Traits.Select(CreatePropertyElement));

#pragma warning disable CS0618 // Type or member is obsolete

            // Required since TestCase.Properties is a superset of TestCase.Traits
            // Unfortunately not all NUnit properties are available as traits
            var traitProperties = result.Properties;

#pragma warning restore CS0618 // Type or member is obsolete

            foreach (var traitProperty in traitProperties)
            {
                if (traitProperty.Key != "CustomProperty")
                {
                    continue;
                }

                var propertyDef = traitProperty.Value as string[];
                if (propertyDef == null || propertyDef.Length != 2)
                {
                    continue;
                }

                var propertyElement = CreatePropertyElement(propertyDef[0], propertyDef[1]);
                foreach (var element in propertyElement)
                {
                    propertyElements.Add(element);
                }
            }

            return propertyElements.Any() ? new XElement("properties", propertyElements.Distinct()) : null;
        }

        private XElement CreateTestSuitesElement(List<TestResultInfo> results, TestRunConfiguration runConfiguration, List<TestMessageInfo> messages)
        {
            var assemblies = results.Select(x => x.AssemblyPath).Distinct().ToList();
            var testsuiteElements = assemblies
                .Select(a => this.CreateTestSuiteElement(
                    results.Where(x => x.AssemblyPath == a).ToList(),
                    runConfiguration,
                    messages));

            return new XElement("testsuites", testsuiteElements);
        }

        private XElement CreateTestSuiteElement(List<TestResultInfo> results, TestRunConfiguration runConfiguration, List<TestMessageInfo> messages)
        {
            var testCaseElements = results.Select(a => this.CreateTestCaseElement(a));

            var stdOut = new StringBuilder();
            var stdErr = new StringBuilder();
            if (this.StoreConsoleOutputOption is StoreConsoleOutputConfiguration.Both or StoreConsoleOutputConfiguration.TestSuite)
            {
                var frameworkInfo = messages.Where(x => x.Level == TestMessageLevel.Informational);
                if (frameworkInfo.Any())
                {
                    stdOut.AppendLine(string.Empty);
                    stdOut.AppendLine("Test Framework Informational Messages:");

                    foreach (var m in frameworkInfo)
                    {
                        stdOut.AppendLine(m.Message);
                    }
                }

                foreach (var m in messages.Where(x => x.Level != TestMessageLevel.Informational))
                {
                    stdErr.AppendLine($"{m.Level} - {m.Message}");
                }
            }

            // Adding required properties, system-out, and system-err elements in the correct
            // positions as required by the xsd. In system-out collapse consecutive newlines to a
            // single newline.
            var element = new XElement(
                "testsuite",
                new XElement("properties"),
                testCaseElements,
                new XElement("system-out", stdOut.ToString()),
                new XElement("system-err", stdErr.ToString()));

            element.SetAttributeValue("name", Path.GetFileName(results.First().AssemblyPath));

            element.SetAttributeValue("tests", results.Count);
            element.SetAttributeValue("skipped", results.Where(x => x.Outcome == TestOutcome.Skipped).Count());
            element.SetAttributeValue("failures", results.Where(x => x.Outcome == TestOutcome.Failed).Count());
            element.SetAttributeValue("errors", 0); // looks like this isn't supported by .net?
            element.SetAttributeValue("time", results.Sum(x => x.Duration.TotalSeconds).ToString(CultureInfo.InvariantCulture));
            element.SetAttributeValue("timestamp", runConfiguration.StartTime.ToString("s"));
            element.SetAttributeValue("hostname", Environment.MachineName);
            element.SetAttributeValue("id", 0); // we never output multiple, so this is always zero.
            element.SetAttributeValue("package", Path.GetFileName(results.First().AssemblyPath));

            return element;
        }

        private XElement CreateTestCaseElement(TestResultInfo result)
        {
            var testcaseElement = new XElement("testcase");

            var namespaceClass = result.Namespace + "." + result.Type;

            testcaseElement.SetAttributeValue("classname", namespaceClass);

            if (this.MethodFormatOption == MethodFormat.Full)
            {
                testcaseElement.SetAttributeValue("name", namespaceClass + "." + result.Method);
            }
            else if (this.MethodFormatOption == MethodFormat.Class)
            {
                testcaseElement.SetAttributeValue("name", result.Type + "." + result.Method);
            }
            else
            {
                testcaseElement.SetAttributeValue("name", result.Method);
            }

            // Ensure time value is never zero because gitlab treats 0 like its null. 0.1 micro
            // seconds should be low enough it won't interfere with anyone monitoring test duration.
            testcaseElement.SetAttributeValue("time", Math.Max(0.0000001f, result.Duration.TotalSeconds).ToString("0.0000000", CultureInfo.InvariantCulture));

            if (result.Outcome == TestOutcome.Failed)
            {
                var failureBodySB = new StringBuilder();

                if (this.FailureBodyFormatOption == FailureBodyFormat.Verbose)
                {
                    failureBodySB.AppendLine(result.ErrorMessage);

                    // Stack trace label included to mimic the normal test output
                    failureBodySB.AppendLine("Stack Trace:");
                }

                failureBodySB.AppendLine(result.ErrorStackTrace);

                if (this.FailureBodyFormatOption == FailureBodyFormat.Verbose &&
                    result.Messages.Count > 0)
                {
                    failureBodySB.AppendLine("Standard Output:");

                    failureBodySB.AppendLine(Indent(result.Messages));
                }

                var failureElement = new XElement("failure", failureBodySB.ToString().Trim());

                failureElement.SetAttributeValue("type", "failure"); // TODO are there failure types?
                failureElement.SetAttributeValue("message", result.ErrorMessage);

                testcaseElement.Add(failureElement);
            }
            else if (result.Outcome == TestOutcome.Skipped)
            {
                var skippedElement = new XElement("skipped");

                testcaseElement.Add(skippedElement);
            }

            // Add stdout and stderr to the testcase element
            var stdOut = new StringBuilder();
            var stdErr = new StringBuilder();
            if (this.StoreConsoleOutputOption is StoreConsoleOutputConfiguration.Both or StoreConsoleOutputConfiguration.TestCase)
            {
                // Store the system-out and system-err only if store console output is enabled
                foreach (var m in result.Messages)
                {
                    if (TestResultMessage.StandardOutCategory.Equals(m.Category, StringComparison.OrdinalIgnoreCase))
                    {
                        stdOut.AppendLine(m.Text);
                    }
                    else if (TestResultMessage.StandardErrorCategory.Equals(m.Category, StringComparison.OrdinalIgnoreCase))
                    {
                        stdErr.AppendLine(m.Text);
                    }
                }
            }

            // Attachments are always included in system-out irrespective of the StoreConsoleOutput option.
            // See attachments spec here: https://plugins.jenkins.io/junit-attachments/
            foreach (var attachment in result.Attachments)
            {
                // Ignore Attachment descriptions, it is not clear if CI systems use it.
                stdOut.AppendLine($"[[ATTACHMENT|{attachment.FilePath}]]");
            }

            if (!string.IsNullOrWhiteSpace(stdOut.ToString()))
            {
                testcaseElement.Add(new XElement("system-out", new XCData(stdOut.ToString())));
            }

            if (!string.IsNullOrWhiteSpace(stdErr.ToString()))
            {
                testcaseElement.Add(new XElement("system-err", new XCData(stdErr.ToString())));
            }

            testcaseElement.Add(CreatePropertiesElement(result));

            return testcaseElement;
        }

        /// <summary>
        /// Performs logger specific configuration based the user's CLI flags, which are provided
        /// through <see cref="LoggerConfiguration"/>.
        /// </summary>
        private void Configure(LoggerConfiguration loggerConfiguration)
        {
            if (loggerConfiguration.Values.TryGetValue(MethodFormatKey, out string methodFormat))
            {
                if (string.Equals(methodFormat.Trim(), "Class", StringComparison.OrdinalIgnoreCase))
                {
                    this.MethodFormatOption = MethodFormat.Class;
                }
                else if (string.Equals(methodFormat.Trim(), "Full", StringComparison.OrdinalIgnoreCase))
                {
                    this.MethodFormatOption = MethodFormat.Full;
                }
                else if (string.Equals(methodFormat.Trim(), "Default", StringComparison.OrdinalIgnoreCase))
                {
                    this.MethodFormatOption = MethodFormat.Default;
                }
                else
                {
                    Console.WriteLine($"JunitXML Logger: The provided Method Format '{methodFormat}' is not a recognized option. Using default");
                }
            }

            if (loggerConfiguration.Values.TryGetValue(FailureBodyFormatKey, out string failureFormat))
            {
                if (string.Equals(failureFormat.Trim(), "Verbose", StringComparison.OrdinalIgnoreCase))
                {
                    this.FailureBodyFormatOption = FailureBodyFormat.Verbose;
                }
                else if (string.Equals(failureFormat.Trim(), "Default", StringComparison.OrdinalIgnoreCase))
                {
                    this.FailureBodyFormatOption = FailureBodyFormat.Default;
                }
                else
                {
                    Console.WriteLine($"JunitXML Logger: The provided Failure Body Format '{failureFormat}' is not a recognized option. Using default");
                }
            }

            if (loggerConfiguration.Values.TryGetValue(StoreConsoleOutputKey, out string storeOutputValue))
            {
                storeOutputValue = storeOutputValue.Trim();
                if (string.Equals(storeOutputValue, "true", StringComparison.OrdinalIgnoreCase))
                {
                    this.StoreConsoleOutputOption = StoreConsoleOutputConfiguration.Both;
                }
                else if (string.Equals(storeOutputValue, "false", StringComparison.OrdinalIgnoreCase))
                {
                    this.StoreConsoleOutputOption = StoreConsoleOutputConfiguration.None;
                }
                else if (string.Equals(storeOutputValue, "testcase", StringComparison.OrdinalIgnoreCase))
                {
                    this.StoreConsoleOutputOption = StoreConsoleOutputConfiguration.TestCase;
                }
                else if (string.Equals(storeOutputValue, "testsuite", StringComparison.OrdinalIgnoreCase))
                {
                    this.StoreConsoleOutputOption = StoreConsoleOutputConfiguration.TestSuite;
                }
                else
                {
                    Console.WriteLine($"JunitXML Logger: The provided Store Console Output '{storeOutputValue}' is not a recognized option. Using default");
                }
            }
        }

        public class TestSuite
        {
            public XElement Element { get; set; }

            public string Name { get; set; }

            public string FullName { get; set; }

            public int Total { get; set; }

            public int Passed { get; set; }

            public int Failed { get; set; }

            public int Inconclusive { get; set; }

            public int Skipped { get; set; }

            public int Error { get; set; }

            public TimeSpan Time { get; set; }
        }
    }
}
