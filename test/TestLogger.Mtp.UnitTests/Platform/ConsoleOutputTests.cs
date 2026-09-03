// Copyright (c) Spekt Contributors. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

namespace Spekt.TestLogger.UnitTests.Platform
{
    using System;
    using System.IO;
    using Microsoft.VisualStudio.TestTools.UnitTesting;
    using Spekt.TestLogger.Platform;

    [TestClass]
    public class ConsoleOutputTests
    {
        [TestMethod]
        public void WriteMessageShouldPrintMessageOnStandardOutput()
        {
            var consoleOutput = new ConsoleOutput();
            using var stdout = new MemoryStream();
            var actualStdout = Console.Out;
            try
            {
                var writer = new StreamWriter(stdout);
                Console.SetOut(writer);
                consoleOutput.WriteMessage("test message");
                writer.Flush();

                stdout.Seek(0, SeekOrigin.Begin);
                var reader = new StreamReader(stdout);
                Assert.AreEqual("test message", reader.ReadLine());
            }
            finally
            {
                Console.SetOut(actualStdout);
            }
        }

        [TestMethod]
        public void WriteErrorShouldPrintMessageOnStandardError()
        {
            var consoleOutput = new ConsoleOutput();
            using var stderr = new MemoryStream();
            var actualStdErr = Console.Error;
            try
            {
                var writer = new StreamWriter(stderr);
                Console.SetError(writer);
                consoleOutput.WriteError("test message");
                writer.Flush();

                stderr.Seek(0, SeekOrigin.Begin);
                var reader = new StreamReader(stderr);
                Assert.AreEqual("test message", reader.ReadLine());
            }
            finally
            {
                Console.SetError(actualStdErr);
            }
        }
    }
}