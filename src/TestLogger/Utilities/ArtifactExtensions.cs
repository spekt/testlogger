// Copyright (c) Spekt Contributors. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

namespace Spekt.TestLogger.Utilities
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using Microsoft.Testing.Platform.Extensions.Messages;
    using Microsoft.VisualStudio.TestPlatform.ObjectModel;
    using Spekt.TestLogger.Core;

    public static class ArtifactExtensions
    {
        public static TestAttachmentInfo ToAttachment(this SessionFileArtifact artifact)
        {
            return new TestAttachmentInfo(artifact.FileInfo.FullName, artifact.Description);
        }

        public static IEnumerable<TestAttachmentInfo> ToAttachments(this IEnumerable<FileArtifactProperty> artifacts)
        {
            return artifacts.Select(a => new TestAttachmentInfo(a.FileInfo.FullName, a.Description));
        }
    }
}