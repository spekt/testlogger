// Copyright (c) Spekt Contributors. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

namespace Spekt.TestLogger.Core
{
    /// <summary>
    /// Represents a test result message.
    /// </summary>
    public sealed class TestResultMessage
    {
        /// <summary>
        /// Standard output category.
        /// </summary>
        public const string StandardOutCategory = "StdOutMsgs";

        /// <summary>
        /// Standard error category.
        /// </summary>
        public const string StandardErrorCategory = "StdErrMsgs";

        /// <summary>
        /// Initializes a new instance of the <see cref="TestResultMessage"/> class.
        /// </summary>
        /// <param name="category">The message category.</param>
        /// <param name="text">The message text.</param>
        public TestResultMessage(string category, string text)
        {
            this.Category = category;
            this.Text = text;
        }

        /// <summary>
        /// Gets the category.
        /// </summary>
        public string Category { get; }

        /// <summary>
        /// Gets the text.
        /// </summary>
        public string Text { get; }
    }
}
