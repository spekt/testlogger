// Copyright (c) Spekt Contributors. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

namespace Spekt.TestLogger.Utilities
{
    using System;
    using System.Collections.Generic;
    using System.IO;
    using System.Linq;
    using Microsoft.Testing.Platform.Extensions.Messages;
    using Spekt.TestLogger.Core;

    public static class ArtifactExtensions
    {
        public static TestAttachmentInfo ToAttachment(this SessionFileArtifact artifact, string baseDirectory = null, bool makeRelativePath = false)
        {
            var filePath = artifact.FileInfo.FullName;
            if (makeRelativePath && !string.IsNullOrEmpty(baseDirectory))
            {
                filePath = MakeRelativePath(baseDirectory, filePath);
            }

            return new TestAttachmentInfo(filePath, artifact.Description);
        }

        public static IEnumerable<TestAttachmentInfo> ToAttachments(this IEnumerable<FileArtifactProperty> artifacts, string baseDirectory = null, bool makeRelativePaths = false)
        {
            if (makeRelativePaths && !string.IsNullOrEmpty(baseDirectory))
            {
                return artifacts.Select(a =>
                {
                    var relativePath = MakeRelativePath(baseDirectory, a.FileInfo.FullName);
                    return new TestAttachmentInfo(relativePath, a.Description);
                });
            }

            return artifacts.Select(a => new TestAttachmentInfo(a.FileInfo.FullName, a.Description));
        }

        /// <summary>
        /// Makes a target path relative to a base directory path.
        /// </summary>
        /// <param name="baseDirectoryPath">Base directory path.</param>
        /// <param name="targetPath">Target file path.</param>
        /// <returns>File path relative to base directory path.</returns>
        public static string MakeRelativePath(string baseDirectoryPath, string targetPath)
        {
            // Example: basePath: C:\a\b\c, targetPath: C:\a\d\e.txt, Output: ..\..\d\e.txt
            // Assumes baseDirectoryPath is a directory path in any OS.
            if (!baseDirectoryPath.EndsWith(Path.DirectorySeparatorChar.ToString()) &&
                !baseDirectoryPath.EndsWith(Path.AltDirectorySeparatorChar.ToString()))
            {
                baseDirectoryPath += Path.DirectorySeparatorChar;
            }

            // Target path can be relative, or on a different drive, just return it as is.
            if (!Path.IsPathRooted(targetPath) ||
                !string.Equals(Path.GetPathRoot(baseDirectoryPath), Path.GetPathRoot(targetPath), StringComparison.OrdinalIgnoreCase))
            {
                return targetPath;
            }

            var baseUri = new Uri(baseDirectoryPath);
            var targetUri = new Uri(targetPath);

            var relativeUri = baseUri.MakeRelativeUri(targetUri);
            var relativePath = Uri.UnescapeDataString(relativeUri.ToString());

            // Convert URI slashes to platform-specific directory separators.
            return relativePath.Replace('/', Path.DirectorySeparatorChar);
        }
    }
}