// Copyright (c) Spekt Contributors. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

namespace Spekt.TestLogger.UnitTests.TestDoubles
{
    using Spekt.TestLogger.Platform;

    public class FakeConsoleOutput : IConsoleOutput
    {
        public void WriteMessage(string message)
        {
            throw new System.NotImplementedException();
        }

        public void WriteError(string message)
        {
            throw new System.NotImplementedException();
        }
    }
}