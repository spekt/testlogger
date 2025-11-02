// Copyright (c) Spekt Contributors. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

namespace Spekt.TestLogger.Utilities
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using Microsoft.VisualStudio.TestPlatform.ObjectModel;
    using Spekt.TestLogger.Core;

    public static class AttachmentSetExtensions
    {
        public static IEnumerable<TestAttachmentInfo> ToAttachments(this AttachmentSet attachmentSet, string baseDirectory = null, bool makeRelativePaths = false)
        {
            if (makeRelativePaths && !string.IsNullOrEmpty(baseDirectory))
            {
                return attachmentSet.Attachments.Select(a =>
                {
                    var attachmentPath = GetPathFromUri(a.Uri);
                    var relativePath = ArtifactExtensions.MakeRelativePath(baseDirectory, attachmentPath);
                    return new TestAttachmentInfo(relativePath, a.Description);
                });
            }

            return attachmentSet.Attachments.Select(a => new
                    TestAttachmentInfo(GetPathFromUri(a.Uri), a.Description));
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