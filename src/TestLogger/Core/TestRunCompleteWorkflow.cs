// Copyright (c) Spekt Contributors. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

namespace Spekt.TestLogger.Core
{
    using System;
    using Microsoft.VisualStudio.TestPlatform.ObjectModel.Client;

    public static class TestRunCompleteWorkflow
    {
        public static void Complete(this ITestRun testRun, TestRunCompleteEventArgs completeEvent)
        {
            // Workflow - serialize and persist the test results
            //    Input - result store, persisted file
            //    Output - void - side effect results are persisted
            //
            //    if result store is empty -> return
            //    serialize store into relevant string
            //    write the serialized string to file
            //    show a message on console
            testRun.FileSystem.Write(testRun.ResultFile, testRun.Serializer.Serialize(testRun.Store));
    #if NONE
            List<string> resultList;
            lock (this.resultsGuard)
            {
                resultList = this.results;
                this.results = new List<string>();
            }

            // Workflow
            // var completeRunWorkflow = TestRunCompleteWorkflow()
            //     .withStore(testStore)
            //     .withSerializer(resultSerializer)
            //     .serialize()
            //     .write(resultPath);

            // Create directory if not exist
            var loggerFileDirPath = Path.GetDirectoryName(this.outputFilePath);
            if (!Directory.Exists(loggerFileDirPath))
            {
                Directory.CreateDirectory(loggerFileDirPath);
            }

            using (var f = File.Create(this.outputFilePath))
            {
            }

            var resultsFileMessage = string.Format(CultureInfo.CurrentCulture, "Results File: {0}", this.outputFilePath);
            Console.WriteLine(resultsFileMessage);
#endif
        }
    }
}