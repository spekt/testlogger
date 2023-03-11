// Copyright (c) Spekt Contributors. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

namespace Spekt.TestLogger.Core
{
    using System.Text;

    public class InputSanitizerJson : IInputSanitizer
    {
        private static readonly char[] EscapeTable;
        private static readonly char[] EscapeCharacters = { '"', '\\', '\b', '\f', '\n', '\r', '\t' };

        static InputSanitizerJson()
        {
            EscapeTable = new char[93];
            EscapeTable['"'] = '"';
            EscapeTable['\\'] = '\\';
            EscapeTable['\b'] = 'b';
            EscapeTable['\f'] = 'f';
            EscapeTable['\n'] = 'n';
            EscapeTable['\r'] = 'r';
            EscapeTable['\t'] = 't';
        }

        public string Sanitize(string input)
        {
            var sb = new StringBuilder();

            // Happy path if there's nothing to be escaped. IndexOfAny is highly optimized (and unmanaged)
            if (input.IndexOfAny(EscapeCharacters) == -1)
            {
                return input;
            }

            int safeCharacterCount = 0;
            char[] charArray = input.ToCharArray();

            for (int i = 0; i < charArray.Length; i++)
            {
                char c = charArray[i];

                // Non ascii characters are fine, buffer them up and send them to the builder
                // in larger chunks if possible. The escape table is a 1:1 translation table
                // with \0 [default(char)] denoting a safe character.
                if (c >= EscapeTable.Length || EscapeTable[c] == default(char))
                {
                    safeCharacterCount++;
                }
                else
                {
                    if (safeCharacterCount > 0)
                    {
                        sb.Append(charArray, i - safeCharacterCount, safeCharacterCount);
                        safeCharacterCount = 0;
                    }

                    sb.Append('\\');
                    sb.Append((char)EscapeTable[c]);
                }
            }

            if (safeCharacterCount > 0)
            {
                sb.Append(charArray, charArray.Length - safeCharacterCount, safeCharacterCount);
            }

            return sb.ToString();
        }
    }
}
