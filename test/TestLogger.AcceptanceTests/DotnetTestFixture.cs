// Copyright (c) Spekt Contributors. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

namespace TestLogger.AcceptanceTests
{
    using System;
    using System.Diagnostics;
    using System.IO;

    public static class DotnetTestFixture
    {
        private const string NetcoreVersion = "netcoreapp3.1";
        private const string ResultFile = "test-results.json";

        public static void Execute(string assemblyName, out string resultsFile)
        {
            resultsFile = Path.Combine(GetAssemblyPath(assemblyName), "test-results.json");
            if (File.Exists(resultsFile))
            {
                File.Delete(resultsFile);
            }

            // Log the contents of test output directory. Useful to verify if the logger is copied
            Console.WriteLine("------------");
            Console.WriteLine("Contents of test output directory:");

            // Create directory so test does not fail under windows.
            Directory.CreateDirectory(Path.Combine(assemblyName, $"bin/Debug/{NetcoreVersion}"));

            foreach (var f in Directory.GetFiles(Path.Combine(assemblyName, $"bin/Debug/{NetcoreVersion}")))
            {
                Console.WriteLine("  " + f);
            }

            Console.WriteLine();

            // Run dotnet test with logger
            using var dotnet = new Process
            {
                StartInfo =
                {
                    UseShellExecute = false,
                    RedirectStandardOutput = true,
                    FileName = "dotnet",
                    Arguments = $"test --no-build --logger:\"json;LogFilePath={ResultFile}\" {GetAssemblyPath(assemblyName)}\\{assemblyName}.csproj"
                }
            };
            dotnet.Start();

            Console.WriteLine("dotnet arguments: " + dotnet.StartInfo.Arguments);

            // To avoid deadlocks, always read the output stream first and then wait.
            var output = dotnet.StandardOutput.ReadToEnd();
            dotnet.WaitForExit();
            Console.WriteLine("dotnet output: " + output);
            Console.WriteLine("------------");
        }

        private static string GetAssemblyPath(string assembly) =>
            Path.GetFullPath(
                Path.Combine(
                    Environment.CurrentDirectory,
                    "..",
                    "..",
                    "..",
                    "..",
                    "assets",
                    assembly));
    }
}
