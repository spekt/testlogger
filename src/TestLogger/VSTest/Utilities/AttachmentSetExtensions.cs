// Copyright (c) Spekt Contributors. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

namespace Spekt.TestLogger.VSTest.Utilities
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using Microsoft.VisualStudio.TestPlatform.ObjectModel;
    using Spekt.TestLogger.Core;

    public static class AttachmentSetExtensions
    {
        public static IEnumerable<TestAttachmentInfo> ToAttachments(this Microsoft.VisualStudio.TestPlatform.ObjectModel.AttachmentSet attachmentSet, string baseDirectory, bool makeRelativePaths)
        {
            if (makeRelativePaths && !string.IsNullOrEmpty(baseDirectory))
            {
                return attachmentSet.Attachments.Select(a =>
                {
                    var attachmentPath = GetPathFromUri(a.Uri);
                    var relativePath = MakeRelativePath(baseDirectory, attachmentPath);
                    return new TestAttachmentInfo(relativePath, a.Description);
                });
            }

            return attachmentSet.Attachments.Select(a => new
                    TestAttachmentInfo(GetPathFromUri(a.Uri), a.Description));
        }

        private static string MakeRelativePath(string baseDirectoryPath, string targetPath)
        {
            if (!baseDirectoryPath.EndsWith(System.IO.Path.DirectorySeparatorChar.ToString()) &&
                !baseDirectoryPath.EndsWith(System.IO.Path.AltDirectorySeparatorChar.ToString()))
            {
                baseDirectoryPath += System.IO.Path.DirectorySeparatorChar;
            }

            if (!System.IO.Path.IsPathRooted(targetPath) ||
                !string.Equals(System.IO.Path.GetPathRoot(baseDirectoryPath), System.IO.Path.GetPathRoot(targetPath), System.StringComparison.OrdinalIgnoreCase))
            {
                return targetPath;
            }

            var baseUri = new System.Uri(baseDirectoryPath);
            var targetUri = new System.Uri(targetPath);

            var relativeUri = baseUri.MakeRelativeUri(targetUri);
            var relativePath = System.Uri.UnescapeDataString(relativeUri.ToString());

            return relativePath.Replace('/', System.IO.Path.DirectorySeparatorChar);
        }

        private static string GetPathFromUri(Uri uri)
        {
            try
            {
                return uri.LocalPath;
            }
            catch (InvalidOperationException)
            {
                return uri.OriginalString;
            }
        }
    }
}