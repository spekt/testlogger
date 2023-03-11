// Copyright (c) Spekt Contributors. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

namespace Spekt.TestLogger.Core
{
    using System.Text.RegularExpressions;

    internal static class XmlSanitizer
    {
        internal static string RemoveInvalidXmlChar(string str)
        {
            if (str == null)
            {
                return null;
            }

            // From xml spec (http://www.w3.org/TR/xml/#charsets) valid chars:
            // #x9 | #xA | #xD | [#x20-#xD7FF] | [#xE000-#xFFFD] | [#x10000-#x10FFFF]

            // we are handling only #x9 | #xA | #xD | [#x20-#xD7FF] | [#xE000-#xFFFD]
            // because C# support unicode character in range \u0000 to \uFFFF
            var evaluator = new MatchEvaluator(ReplaceInvalidCharacterWithUniCodeEscapeSequence);
            var invalidChar = @"[^\x09\x0A\x0D\x20-\uD7FF\uE000-\uFFFD]";
            return Regex.Replace(str, invalidChar, evaluator);
        }

        private static string ReplaceInvalidCharacterWithUniCodeEscapeSequence(Match match)
        {
            char x = match.Value[0];
            return $@"\u{(ushort)x:x4}";
        }
    }
}
