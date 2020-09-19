// Copyright (c) Spekt Contributors. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

namespace Spekt.TestLogger.Platform
{
    public interface IFileSystem
    {
        void Write(string path);

        string GetFullPath(string path);
    }
}