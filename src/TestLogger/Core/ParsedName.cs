// Copyright (c) Spekt Contributors. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

namespace Spekt.TestLogger.Core
{
    public class ParsedName
    {
        public ParsedName(string namespaceName, string typeName, string methodName)
        {
            this.Namespace = namespaceName;
            this.Type = typeName;
            this.Method = methodName;
        }

        public string Namespace { get; }

        public string Type { get; }

        public string Method { get; }
    }
}
