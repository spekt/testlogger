// Copyright (c) Spekt Contributors. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

namespace Spekt.TestLogger.UnitTests.Platform
{
    using System;
    using Microsoft.VisualStudio.TestTools.UnitTesting;
    using Spekt.TestLogger.Platform;

    [TestClass]
    public class ConsoleOutputTests
    {
        [TestMethod]
        public void WriteMessageShouldThrowNotImplementedException()
        {
            var consoleOutput = new ConsoleOutput();
            Assert.ThrowsException<NotImplementedException>(() => consoleOutput.WriteMessage(string.Empty));
        }

        [TestMethod]
        public void WriteErrorShouldThrowNotImplementedException()
        {
            var consoleOutput = new ConsoleOutput();
            Assert.ThrowsException<NotImplementedException>(() => consoleOutput.WriteError(string.Empty));
        }
    }
}