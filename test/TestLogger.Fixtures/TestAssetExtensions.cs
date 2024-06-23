// Copyright (c) Spekt Contributors. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

namespace TestLogger.Fixtures
{
    public static class TestAssetExtensions
    {
        /// <summary>
        /// Gets the full path to root directory for an test asset.
        /// </summary>
        /// <param name="assetName">Name of the test asset.</param>
        /// <returns>Full path to the test asset directory.</returns>
        public static string ToAssetDirectoryPath(this string assetName)
        {
            return Path.GetFullPath(
                Path.Combine(
                    Environment.CurrentDirectory,
                    "..",
                    "..",
                    "..",
                    "..",
                    "assets",
                    assetName));
        }

        /// <summary>
        /// Gets the full path to test asset assembly file.
        /// </summary>
        /// <param name="assetName">Name of the test asset.</param>
        /// <returns>Full path to test assembly.</returns>
        public static string ToAssetAssemblyPath(this string assetName, string targetFrameworkVersion)
        {
#if DEBUG
            var config = "Debug";
#else
            var config = "Release";
#endif
            return Path.Combine(assetName.ToAssetDirectoryPath(), "bin", config, targetFrameworkVersion, $"{assetName}.dll");
        }
    }
}