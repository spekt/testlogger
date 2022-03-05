// Copyright (c) Spekt Contributors. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

namespace Spekt.TestLogger.Core
{
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
