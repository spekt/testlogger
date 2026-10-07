// Copyright (c) Spekt Contributors. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

namespace TestLogger.AcceptanceTests
{
    using System;

    /// <summary>
    /// Rewrites Verify snapshot lines (absolute build paths) into a uniform,
    /// OS-independent form. All matching is done with linear scans: the previous
    /// implementation used per-line compiled regexes with nested quantifiers
    /// (e.g. <c>.*([\/\\]*bin[\/\\]*Debug[\/\\]*.*)$</c>) whose backtracking
    /// dominated snapshot verification time on large reports.
    /// The output is byte-identical to the regex implementation on all inputs.
    /// </summary>
    internal static class SnapshotScrubber
    {
        /// <summary>
        /// Scrubs a single snapshot line: paths under <c>bin/Debug</c> are reduced
        /// to their build-relative form (with the vstest/mtp flavor directory
        /// stripped), and paths under <c>test/assets/&lt;assetMarker&gt;</c> are
        /// reduced to repo-relative form. Anything else passes through, apart
        /// from the verbatim <c>\r\n</c> normalization applied to every line.
        /// </summary>
        /// <param name="line">The snapshot line to scrub.</param>
        /// <param name="assetMarker">The asset directory name, e.g. <c>Json.TestLogger</c>.</param>
        /// <returns>The scrubbed line.</returns>
        public static string ScrubLine(string line, string assetMarker)
        {
            var binIndex = LastBinDebugStart(line);
            string scrubbed;
            if (binIndex >= 0)
            {
                // Used to take something like 'C:\\lsdkjf\sdf\bin\Debug\a\b\c.txt' => '/bin/Debug/a/b/c.txt' which helps with cross dev/platform comparison
                scrubbed = line.Substring(binIndex).Replace('\\', '/');
                scrubbed = scrubbed.Replace("//", "/");

                // Test runner flavor (vstest/mtp) is an implementation detail of the
                // build layout; normalize it away so snapshots stay stable.
                scrubbed = scrubbed.Replace("bin/Debug/vstest/", "bin/Debug/");
                scrubbed = scrubbed.Replace("bin/Debug/mtp/", "bin/Debug/");
            }
            else
            {
                var markerStart = LastMarkerStart(line, assetMarker);
                if (markerStart < 0)
                {
                    scrubbed = line;
                }
                else
                {
                    var markerEnd = markerStart + MarkerLength(assetMarker);
                    var suffix = line.Substring(markerEnd).Replace('\\', '/');

                    // A "<prefix>: " header is preserved when one precedes the marker.
                    // NB: LastIndexOf(value, startIndex, ...) constrains the match END,
                    // so startIndex is markerStart - 1 to allow starts up to markerStart - 2.
                    var separator = markerStart >= 1 ? line.LastIndexOf(": ", markerStart - 1, StringComparison.Ordinal) : -1;
                    var prefix = separator >= 0
                        ? line.Substring(0, separator + 2).Replace('\\', '/')
                        : "test/assets/" + assetMarker;
                    scrubbed = separator >= 0
                        ? prefix + "test/assets/" + assetMarker + suffix
                        : prefix + suffix;
                }
            }

            scrubbed = scrubbed.Replace(@"\r\n", @"\n"); // Fix cross plat failures.
            return scrubbed;
        }

        private static int MarkerLength(string assetMarker)
        {
            // "test" + separator + "assets" + separator + marker.
            return 4 + 1 + 6 + 1 + assetMarker.Length;
        }

        private static bool IsSeparator(char c)
        {
            return c == '/' || c == '\\';
        }

        private static bool MatchesWord(string line, int index, string word)
        {
            return index >= 0
                && index + word.Length <= line.Length
                && string.Compare(line, index, word, 0, word.Length, StringComparison.OrdinalIgnoreCase) == 0;
        }

        private static bool MatchMarkerAt(string line, int index, string assetMarker)
        {
            // "test" + separator + "assets" + separator + marker (case-insensitive).
            if (!MatchesWord(line, index, "test"))
            {
                return false;
            }

            var i = index + 4;
            if (i >= line.Length || !IsSeparator(line[i]))
            {
                return false;
            }

            i++;
            if (!MatchesWord(line, i, "assets"))
            {
                return false;
            }

            i += 6;
            if (i >= line.Length || !IsSeparator(line[i]))
            {
                return false;
            }

            i++;
            return MatchesWord(line, i, assetMarker);
        }

        private static int LastMarkerStart(string line, string assetMarker)
        {
            var last = -1;
            var limit = line.Length - MarkerLength(assetMarker);
            for (var index = 0; index <= limit; index++)
            {
                if (MatchMarkerAt(line, index, assetMarker))
                {
                    last = index;
                }
            }

            return last;
        }

        private static int LastBinDebugStart(string line)
        {
            // Rightmost "bin" (case-insensitive) followed by separators + "Debug".
            // The greedy regex_group starts at the bin itself (leading separators
            // are matched by [\/\\]* but the rightmost viable start is the bin).
            var from = line.Length - 1;
            while (from >= 0)
            {
                var bin = line.LastIndexOf("bin", from, StringComparison.OrdinalIgnoreCase);
                if (bin < 0)
                {
                    return -1;
                }

                var i = bin + 3;
                while (i < line.Length && IsSeparator(line[i]))
                {
                    i++;
                }

                if (MatchesWord(line, i, "Debug"))
                {
                    return bin;
                }

                from = bin - 1;
            }

            return -1;
        }
    }
}
