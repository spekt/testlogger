// Copyright (c) Spekt Contributors. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

namespace Spekt.TestLogger.UnitTests.TestDoubles
{
    using System.Collections.Generic;
    using Spekt.TestLogger.Platform;

    public class FakeConsoleOutput : IConsoleOutput
    {
        public FakeConsoleOutput() => this.Messages = new List<(string, string)>();

#pragma warning disable SA1414 // Tuple types in signatures should have element names
        public List<(string, string)> Messages { get; private set; }
#pragma warning restore SA1414 // Tuple types in signatures should have element names

        public void WriteMessage(string message)
        {
            this.Messages.Add(("stdout", message));
        }

        public void WriteError(string message)
        {
            this.Messages.Add(("stderr", message));
        }
    }
}