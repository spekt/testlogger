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

        private static readonly RegexOptions RegexOptions = RegexOptions.Compiled | RegexOptions.IgnoreCase;

        /// <summary>
        /// This one can handle standard formatting with or without method data.
        /// </summary>
        private static readonly Regex MethodRegex = new Regex(@"^([a-z1-9_.]{1,})\.([a-z1-9_.]{1,})\.(.{1,})$", RegexOptions);

        /// <summary>
        /// Can handle standard formatting with class and method data.
        /// </summary>
        private static readonly Regex ClassDataRegex = new Regex(@"^([a-z1-9_.]{1,})\.([a-z1-9_.]{1,}\(.{0,}\))\.(.{1,})$", RegexOptions);

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
        public static ParsedName Parse(string fullyQualifiedName)
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

            Console.WriteLine(TestCaseParserErrorTemplate, fullyQualifiedName, pn.NamespaceName, pn.TypeName, pn.MethodName);

            return pn;
        }
    }
}
