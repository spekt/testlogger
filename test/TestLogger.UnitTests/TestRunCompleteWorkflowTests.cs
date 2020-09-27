// Copyright (c) Spekt Contributors. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

namespace Spekt.TestLogger.UnitTests
{
    using System;
    using System.Collections.ObjectModel;
    using Microsoft.VisualStudio.TestPlatform.ObjectModel;
    using Microsoft.VisualStudio.TestPlatform.ObjectModel.Client;
    using Microsoft.VisualStudio.TestTools.UnitTesting;
    using Spekt.TestLogger.Core;
    using Spekt.TestLogger.UnitTests.TestDoubles;

    [TestClass]
    public class TestRunCompleteWorkflowTests
    {
        [TestMethod]
        public void CompleteShouldCreateAnEmptyFile()
        {
            var fileSystem = new FakeFileSystem();
            var testRun = new TestRunBuilder()
                .WithResultFile("dummyFile.json")
                .WithFileSystem(fileSystem)
                .WithStore(new TestResultStore())
                .WithSerializer(new JsonTestResultSerializer())
                .Build();
            var testRunCompleteEvent = new TestRunCompleteEventArgs(
                stats: new TestRunStatistics(),
                isCanceled: false,
                isAborted: false,
                error: null,
                attachmentSets: new Collection<AttachmentSet>(),
                elapsedTime: TimeSpan.Zero);

            testRun.Complete(testRunCompleteEvent);

            Assert.AreEqual(string.Empty, fileSystem.Read(testRun.ResultFile));
        }
    }
}