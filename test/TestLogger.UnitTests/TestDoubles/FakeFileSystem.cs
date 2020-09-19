// Copyright (c) Spekt Contributors. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

namespace Spekt.TestLogger.UnitTests.TestDoubles
{
    using System;
    using Spekt.TestLogger.Platform;

    public class FakeFileSystem : IFileSystem
    {
        public void Write(string path)
        {
            throw new NotImplementedException();
        }

        public string GetFullPath(string path)
        {
            return path;
        }
    }
}