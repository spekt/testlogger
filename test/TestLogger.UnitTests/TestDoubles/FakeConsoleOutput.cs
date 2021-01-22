// Copyright (c) Spekt Contributors. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

namespace Spekt.TestLogger.UnitTests.TestDoubles
{
    using System.Collections.Generic;
    using Spekt.TestLogger.Platform;

    public class FakeConsoleOutput : IConsoleOutput
    {
        private readonly List<(string, string)> messages;

        public FakeConsoleOutput() => this.messages = new List<(string, string)>();

        public void WriteMessage(string message)
        {
            this.messages.Add(("stdout", message));
        }

        public void WriteError(string message)
        {
            this.messages.Add(("stderr", message));
        }
    }
}