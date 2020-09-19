// Copyright (c) Spekt Contributors. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

namespace Spekt.TestLogger.Platform
{
    using System.IO;

    public class FileSystem : IFileSystem
    {
        public void Write(string path)
        {
            throw new System.NotImplementedException();
        }

        public string GetFullPath(string path)
        {
            return Path.GetFullPath(path);
        }
    }
}