// Copyright (c) Spekt Contributors. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

namespace Spekt.TestLogger.Platform
{
    /// <summary>
    /// Abstraction for the file system IO primitives.
    /// </summary>
    public interface IFileSystem
    {
        /// <summary>
        /// Read the contents of given file path.
        /// </summary>
        /// <param name="path">Valid file path.</param>
        /// <returns>Content of the file.</returns>
        string Read(string path);

        /// <summary>
        /// Writes the content to a path. Creates the path if it doesn't exist.
        /// </summary>
        /// <param name="path">Path of the file.</param>
        /// <param name="content">Content of the file.</param>
        void Write(string path, string content);

        /// <summary>
        /// Delete a file if it exists.
        /// </summary>
        /// <param name="path">File path.</param>
        void Delete(string path);
    }
}