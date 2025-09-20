// Copyright (c) Spekt Contributors. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

namespace Spekt.TestLogger.Core
{
    using System;
    using System.Text.RegularExpressions;
    using Microsoft.Testing.Platform.Extensions.Messages;
    using Spekt.TestLogger.Platform;

    public class TestCaseNameParser
    {
        public const string TestCaseParserUnknownNamespace = "UnknownNamespace";
        public const string TestCaseParserUnknownType = "UnknownType";

        public const string TestCaseParserError =
            "Test Logger: Unable to parse one or more test names provided by your test runner. " +
            "These will be logged using Namespace='UnknownNamespace', Type='UnknownType'. " +
            "The entire test name will be used as the Method name. Please open a ticket so we can " +
            "review this issue";

        private static readonly RegexOptions RegexOptions = RegexOptions.Compiled | RegexOptions.IgnoreCase;

        /// <summary>
        /// This one can handle standard formatting with or without method data.
        /// </summary>
        private static readonly Regex MethodRegex = new(@"^([a-z0-9_.]{1,})\.([a-z0-9_.+]{1,})\.(.{1,})$", RegexOptions);

        /// <summary>
        /// Can handle standard formatting with class and method data.
        /// </summary>
        private static readonly Regex ClassDataRegex = new(@"^([a-z0-9_.]{1,})\.([a-z0-9_.]{1,}\(.{0,}\))\.(.{1,})$", RegexOptions);

        private readonly IConsoleOutput consoleOutput;
        private bool parserErrorReported;

        /// <summary>
        /// Initializes a new instance of the <see cref="TestCaseNameParser"/> class.
        /// </summary>
        /// <param name="consoleOutput">The console output interface.</param>
        public TestCaseNameParser(IConsoleOutput consoleOutput)
        {
            this.consoleOutput = consoleOutput ?? throw new ArgumentNullException(nameof(consoleOutput));
        }

        /// <summary>
        /// This method parses the TestMethodIdentifierProperty from MTP to extract Namespace, Type and Method name.
        /// MTP sometimes provides the full type name in the Namespace property, so this method handles that case.
        /// </summary>
        /// <param name="methodIdentifier">The TestMethodIdentifierProperty from MTP.</param>
        /// <returns>
        /// An instance of ParsedName containing the parsed results.
        /// </returns>
        public ParsedName Parse(TestMethodIdentifierProperty methodIdentifier)
        {
            if (methodIdentifier == null)
            {
                return new ParsedName(
                    TestCaseParserUnknownNamespace,
                    TestCaseParserUnknownType,
                    string.Empty);
            }

            string @namespace = methodIdentifier.Namespace;
            string type = methodIdentifier.TypeName;
            string method = methodIdentifier.MethodName;

            // Check if the namespace contains the type name (the problematic case e.g. XUnit)
            // This happens when MTP's Namespace property contains the full type name
            if (!string.IsNullOrEmpty(@namespace) && !string.IsNullOrEmpty(type))
            {
                // NEW CASE: Type contains the full namespace + class name
                // If type contains the namespace, extract the actual class name
                if (type.StartsWith(@namespace + "."))
                {
                    type = type.Substring(@namespace.Length + 1);
                }

                // If namespace ends with the type name, extract the actual namespace
                else if (@namespace.EndsWith("." + type))
                {
                    @namespace = @namespace.Substring(0, @namespace.Length - type.Length - 1);
                }

                // If namespace equals the type name, it's a global namespace
                else if (@namespace == type)
                {
                    @namespace = string.Empty;
                }
            }

            return new ParsedName(
                @namespace ?? string.Empty,
                type ?? string.Empty,
                method ?? string.Empty);
        }

        /// <summary>
        /// This method parses test information from either a TestMethodIdentifierProperty or TestNode.
        /// When TestMethodIdentifierProperty is provided, it handles MTP-specific parsing where the Type field
        /// may contain the full namespace + class name. When TestMethodIdentifierProperty is null, it falls back
        /// to parsing the TestNode's UID for test frameworks like NUnit that don't emit TestMethodIdentifierProperty.
        /// </summary>
        /// <param name="methodIdentifier">The TestMethodIdentifierProperty from MTP (can be null).</param>
        /// <param name="testNode">The TestNode to use for fallback parsing (can be null).</param>
        /// <returns>
        /// A tuple containing the parsed namespace, type, method, and fully qualified name.
        /// </returns>
        public (string Namespace, string Type, string Method, string FullyQualifiedName) Parse(TestMethodIdentifierProperty methodIdentifier, TestNode testNode)
        {
            if (methodIdentifier != null)
            {
                var parsedName = this.Parse(methodIdentifier);
                var fqn = string.IsNullOrEmpty(parsedName.Namespace)
                    ? $"{parsedName.Type}.{parsedName.Method}"
                    : $"{parsedName.Namespace}.{parsedName.Type}.{parsedName.Method}";

                return (parsedName.Namespace, parsedName.Type, parsedName.Method, fqn);
            }
            else if (testNode != null)
            {
                // Fallback to parsing the TestNode's UID (e.g. for NUnit since it doesn't emit TestMethodIdentifierProperty)
                var displayName = testNode.Uid;
                var parsedName = this.Parse(displayName);

                if (parsedName.Namespace != TestCaseParserUnknownNamespace)
                {
                    // Successfully parsed the display name
                    return (parsedName.Namespace, parsedName.Type, parsedName.Method, displayName);
                }
                else
                {
                    // Could not parse the display name, use Unknown values
                    return ("UnknownNamespace", "UnknownType", "UnknownMethod", "UnknownFullyQualifiedName");
                }
            }
            else
            {
                // Neither methodIdentifier nor testNode provided
                return ("UnknownNamespace", "UnknownType", "UnknownMethod", "UnknownFullyQualifiedName");
            }
        }

        /// <summary>
        /// This method attempts to parse out a Namespace, Type and Method name from a given string.
        /// When a clearly invalid output is encountered, a message is written to the console.
        /// </summary>
        /// <remarks>
        /// This can be fragile because qualified name is constructed by a test adapter and
        /// there is no enforcement that the FQN starts with the namespace, or is of the expected
        /// format.
        /// </remarks>
        /// <param name="fullyQualifiedName">
        /// String like 'namespace.type.method', where type and or method may be followed by
        /// parenthesis containing parameter values.
        /// </param>
        /// <returns>
        /// An instance of ParsedName containing the parsed results. A result is always returned,
        /// even in the case when the input could not be full parsed.
        /// </returns>
        public ParsedName Parse(string fullyQualifiedName)
        {
            if (!string.IsNullOrWhiteSpace(fullyQualifiedName))
            {
                // Occassionally we get multi line results. Flatten those.
                fullyQualifiedName = Regex.Replace(fullyQualifiedName, @"\r\n?|\n", string.Empty);

                // This matches a subset of cases which the method regex below also
                // matches, but gets wrong. So this goes first.
                var cdMatches = ClassDataRegex.Matches(fullyQualifiedName);
                if (cdMatches.Count > 0)
                {
                    return new ParsedName(
                        cdMatches[0].Groups[1].Value,
                        cdMatches[0].Groups[2].Value,
                        cdMatches[0].Groups[3].Value);
                }

                var matches = MethodRegex.Matches(fullyQualifiedName);
                if (matches.Count > 0)
                {
                    return new ParsedName(
                          matches[0].Groups[1].Value,
                          matches[0].Groups[2].Value,
                          matches[0].Groups[3].Value);
                }
            }

            var pn = new ParsedName(
                    TestCaseParserUnknownNamespace,
                    TestCaseParserUnknownType,
                    fullyQualifiedName ?? string.Empty);

            if (!this.parserErrorReported)
            {
                this.parserErrorReported = true;
                this.consoleOutput.WriteMessage(TestCaseParserError);
            }

            return pn;
        }
    }
}
