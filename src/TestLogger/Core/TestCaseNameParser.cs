// Copyright (c) Spekt Contributors. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

namespace Spekt.TestLogger.Core
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Text.RegularExpressions;

    public static class TestCaseNameParser
    {
        public const string TestCaseParserUnknownNamespace = "UnknownNamespace";
        public const string TestCaseParserUnknownType = "UnknownType";

        public const string TestCaseParserErrorTemplate = "Xml Logger: Unable to parse the test name '{0}' into a namespace type and method. " +
            "Using Namespace='{1}', Type='{2}' and Method='{3}'";

        private enum NameParseStep
        {
            FindMethod,
            FindType,
            FindNamespace
        }

        private enum NameParseState
        {
            Default,
            Parenthesis,
            String,
            Char,
        }

        /// <summary>
        /// This method attempts to parse out a Namespace, Type and Method name from a given string.
        /// When a clearly invalid output is encountered, a message is written to the console.
        /// </summary>
        /// <remarks>
        /// This is fragile, because the fully qualified name is constructed by a test adapter and
        /// there is no enforcement that the FQN starts with the namespace, or is of the expected
        /// format. Because the possible input space is very large and this parser is relatively
        /// simple there are some invalid strings, such as "#.#.#" will 'successfully' parse.
        /// </remarks>
        /// <param name="fullyQualifiedName">
        /// String like 'namespace.type.method', where type and or method may be followed by
        /// parenthesis containing parameter values.
        /// </param>
        /// <returns>
        /// An instance of ParsedName containing the parsed results. A result is always returned,
        /// even in the case when the input could not be full parsed.
        /// </returns>
        public static ParsedName Parse(string fullyQualifiedName)
        {
            var metadataNamespaceName = string.Empty;
            var metadataTypeName = string.Empty;
            var metadataMethodName = string.Empty;

            if (fullyQualifiedName != null)
            {
                // Occassionally we get multi line results. Flatten those.
                fullyQualifiedName = fullyQualifiedName.Replace("\r", string.Empty);
                fullyQualifiedName = fullyQualifiedName.Replace("\n", string.Empty);

                var regexOptions = RegexOptions.Compiled | RegexOptions.IgnoreCase;

                var method = new Regex(@"^([a-z1-9_.]{1,})\.([a-z1-9_.]{1,})\.(.{1,})$", regexOptions); // This one picks up anything with just method parameters or no method params.
                if (method.IsMatch(fullyQualifiedName))
                {
                    var matches = method.Matches(fullyQualifiedName);
                    metadataNamespaceName = matches[0].Groups[1].Value;
                    metadataTypeName = matches[0].Groups[2].Value;
                    metadataMethodName = matches[0].Groups[3].Value;
                }

                var classData = new Regex(@"^([a-z1-9_.]{1,})\.([a-z1-9_.]{1,}\(.{0,}\))\.(.{1,})$", regexOptions); // This one picks up anything with class data and maybe method data
                if (classData.IsMatch(fullyQualifiedName))
                {
                    var matches = classData.Matches(fullyQualifiedName);
                    metadataNamespaceName = matches[0].Groups[1].Value;
                    metadataTypeName = matches[0].Groups[2].Value;
                    metadataMethodName = matches[0].Groups[3].Value;
                }
            }

            if (string.IsNullOrWhiteSpace(metadataNamespaceName)
                || string.IsNullOrWhiteSpace(metadataTypeName)
                || string.IsNullOrWhiteSpace(metadataMethodName))
            {
                metadataNamespaceName = TestCaseParserUnknownNamespace;
                metadataTypeName = TestCaseParserUnknownType;
                metadataMethodName = fullyQualifiedName;
                Console.WriteLine(TestCaseParserErrorTemplate, fullyQualifiedName, metadataNamespaceName, metadataTypeName, metadataMethodName);
            }

            return new ParsedName(metadataNamespaceName, metadataTypeName, metadataMethodName);
        }

        public class ParsedName
        {
            public ParsedName(string namespaceName, string typeName, string methodName)
            {
                this.NamespaceName = namespaceName;
                this.TypeName = typeName;
                this.MethodName = methodName;
            }

            public string NamespaceName { get; }

            public string TypeName { get; }

            public string MethodName { get; }
        }
    }
}
