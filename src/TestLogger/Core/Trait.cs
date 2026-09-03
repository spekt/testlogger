// Copyright (c) Spekt Contributors. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

namespace Spekt.TestLogger.Core
{
    /// <summary>
    /// Represents a trait (category) for a test.
    /// </summary>
    public sealed class Trait
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="Trait"/> class.
        /// </summary>
        /// <param name="name">The trait name.</param>
        /// <param name="value">The trait value.</param>
        public Trait(string name, string value)
        {
            this.Name = name;
            this.Value = value;
        }

        /// <summary>
        /// Gets the trait name.
        /// </summary>
        public string Name { get; }

        /// <summary>
        /// Gets the trait value.
        /// </summary>
        public string Value { get; }
    }
}
