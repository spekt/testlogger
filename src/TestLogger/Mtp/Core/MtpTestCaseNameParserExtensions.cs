// Copyright (c) Spekt Contributors. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

namespace Spekt.TestLogger.Mtp.Core
{
    using Microsoft.Testing.Platform.Extensions.Messages;
    using Spekt.TestLogger.Core;

    public static class MtpTestCaseNameParserExtensions
    {
        public static ParsedName Parse(this TestCaseNameParser parser, TestMethodIdentifierProperty methodIdentifier)
        {
            if (methodIdentifier == null)
            {
                return new ParsedName(
                    TestCaseNameParser.TestCaseParserUnknownNamespace,
                    TestCaseNameParser.TestCaseParserUnknownType,
                    string.Empty);
            }

            string @namespace = methodIdentifier.Namespace;
            string type = methodIdentifier.TypeName;
            string method = methodIdentifier.MethodName;

            if (!string.IsNullOrEmpty(@namespace) && !string.IsNullOrEmpty(type))
            {
                if (type.StartsWith(@namespace + "."))
                {
                    type = type.Substring(@namespace.Length + 1);
                }
                else if (@namespace.EndsWith("." + type))
                {
                    @namespace = @namespace.Substring(0, @namespace.Length - type.Length - 1);
                }
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

        public static (string Namespace, string Type, string Method, string FullyQualifiedName) Parse(this TestCaseNameParser parser, TestMethodIdentifierProperty methodIdentifier, TestNode testNode)
        {
            if (methodIdentifier != null)
            {
                var parsedName = parser.Parse(methodIdentifier);
                var fqn = string.IsNullOrEmpty(parsedName.Namespace)
                    ? $"{parsedName.Type}.{parsedName.Method}"
                    : $"{parsedName.Namespace}.{parsedName.Type}.{parsedName.Method}";

                return (parsedName.Namespace, parsedName.Type, parsedName.Method, fqn);
            }
            else if (testNode != null)
            {
                var displayName = testNode.Uid;
                var parsedName = parser.Parse(displayName);

                if (parsedName.Namespace != TestCaseNameParser.TestCaseParserUnknownNamespace)
                {
                    return (parsedName.Namespace, parsedName.Type, parsedName.Method, displayName);
                }
                else
                {
                    return ("UnknownNamespace", "UnknownType", "UnknownMethod", "UnknownFullyQualifiedName");
                }
            }
            else
            {
                return ("UnknownNamespace", "UnknownType", "UnknownMethod", "UnknownFullyQualifiedName");
            }
        }
    }
}
