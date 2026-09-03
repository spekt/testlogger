// Copyright (c) Spekt Contributors. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

namespace Spekt.TestLogger.UnitTests.TestDoubles
{
    using System;
    using System.Collections.Generic;
    using System.IO;
    using System.Linq;
    using Spekt.TestLogger.Platform;

    public class FakeFileSystem : IFileSystem
    {
        private readonly Dictionary<string, string> files;
        private readonly HashSet<string> directories;

        public FakeFileSystem()
        {
            this.files = new Dictionary<string, string>();
            this.directories = new HashSet<string> { Path.GetTempPath() };
        }

        public void CreateDirectory(string path)
        {
            this.directories.Add(path);
        }

        public bool ExistsDirectory(string path)
        {
            return this.directories.Contains(path);
        }

        public void RemoveDirectory(string path)
        {
            // Remove all paths which could be children of provided path
            foreach (var p in this.directories.Where(p => p.StartsWith(path)).ToList())
            {
                this.directories.Remove(p);
            }
        }

        public string Read(string path)
        {
            if (this.files.TryGetValue(path, out var content))
            {
                return content;
            }

            throw new ArgumentException("File does not exist.", nameof(path));
        }

        public void Write(string path, string content)
        {
            this.files[path] = content;
        }

        public void Delete(string path)
        {
            if (this.files.ContainsKey(path))
            {
                this.files.Remove(path);
            }
        }
    }
}