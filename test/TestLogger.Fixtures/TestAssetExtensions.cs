// Copyright (c) Spekt Contributors. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

namespace TestLogger.Fixtures
{
    using System.Reflection;
    using Microsoft.VisualStudio.TestTools.UnitTesting;

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
        /// <param name="targetFrameworkVersion">Target framework moniker, e.g. net8.0.</param>
        /// <param name="isMTP">Whether the MTP-flavored (rather than VSTest-flavored) output is wanted.</param>
        /// <returns>Full path to test assembly.</returns>
        public static string ToAssetAssemblyPath(this string assetName, string targetFrameworkVersion, bool isMTP)
        {
#if DEBUG
            var config = "Debug";
#else
            var config = "Release";
#endif
            var flavor = isMTP ? "mtp" : "vstest";
            return Path.Combine(assetName.ToAssetDirectoryPath(), "bin", config, flavor, targetFrameworkVersion, $"{assetName}.dll");
        }

        public static bool IsMTP(this TestContext context, Type type)
        {
            // https://github.com/microsoft/testfx/issues/5249
            // return context.Properties.Contains("IsMTP");
            var fullyQualifiedClassName = context.FullyQualifiedTestClassName
                ?? throw new InvalidOperationException("FullyQualifiedTestClassName is null.");

            var concreteTestType = type.Assembly.GetType(fullyQualifiedClassName)
                ?? throw new InvalidOperationException($"Could not find type {fullyQualifiedClassName} in assembly {type.Assembly.FullName}.");

            return concreteTestType
                .GetCustomAttributes<TestPropertyAttribute>(true)
                .Any(p => p.Name == "IsMTP" && bool.TryParse(p.Value, out var mtp) && mtp);
        }
    }
}