// Copyright (c) Spekt Contributors. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

namespace TestLogger.Fixtures
{
    using System;
    using System.Diagnostics;
    using System.IO;

    public class DotnetTestFixture
    {
        private const string NetcoreVersion = "netcoreapp3.1";
        private bool buildProject = false;
        private string relativeResultsDirectory = string.Empty;
        private string runSettingsSuffix = string.Empty;

        public static DotnetTestFixture Create() => new DotnetTestFixture();

        public DotnetTestFixture WithBuild()
        {
            this.buildProject = true;
            return this;
        }

        public DotnetTestFixture WithResultsDirectory(string resultsDirectory)
        {
            this.relativeResultsDirectory = resultsDirectory;
            return this;
        }

        public DotnetTestFixture WithRunSettings(string runSettingsArgs)
        {
            // Appended to the dotnet test commandline like
            // dotnet test -- <runSettingsArgs>
            this.runSettingsSuffix = runSettingsArgs;
            return this;
        }

        public string Execute(string assemblyName, string loggerArgs, bool collectCoverage, string resultsFileName)
        {
            var resultsDirectory = Path.Combine(assemblyName.ToAssetDirectoryPath(), this.relativeResultsDirectory);
            var resultsFile = Path.Combine(resultsDirectory, resultsFileName);
            if (File.Exists(resultsFile))
            {
                File.Delete(resultsFile);
            }

            // Run dotnet test with logger
            var buildArgs = this.buildProject ? string.Empty : "--no-build";
            var resultDirectoryArgs = string.IsNullOrEmpty(this.relativeResultsDirectory) ? string.Empty : $"--results-directory \"{resultsDirectory}\"";
            var commandlineSuffix = string.IsNullOrEmpty(this.runSettingsSuffix) ? string.Empty : $"-- {this.runSettingsSuffix}";
            using var dotnet = new Process
            {
                StartInfo =
                {
                    UseShellExecute = false,
                    RedirectStandardOutput = true,
                    FileName = "dotnet",
                    Arguments = $"test {buildArgs} --logger:\"{loggerArgs}\" \"{assemblyName.ToAssetDirectoryPath()}\\{assemblyName}.csproj\" {resultDirectoryArgs} {commandlineSuffix}"
                }
            };

            // Add coverage arg if required
            if (collectCoverage)
            {
                dotnet.StartInfo.Arguments += " --collect:\"XPlat Code Coverage\" --settings coverlet.runsettings";
            }

            this.LogTestAssetOutDir(assemblyName);

            // Required to skip icu requirement for netcoreapp3.1 in linux
            dotnet.StartInfo.EnvironmentVariables["DOTNET_SYSTEM_GLOBALIZATION_INVARIANT"] = "1";
            dotnet.Start();

            Console.WriteLine("\n\n## Test run arguments: dotnet " + dotnet.StartInfo.Arguments);

            // To avoid deadlocks, always read the output stream first and then wait.
            var output = dotnet.StandardOutput.ReadToEnd();
            dotnet.WaitForExit();
            Console.WriteLine("\n\n ## Test run output\n" + output);

            return resultsFile;
        }

        private void LogTestAssetOutDir(string assemblyName)
        {
            // Log the contents of test output directory. Useful to verify if the logger is copied
            Console.WriteLine("\n\n## Contents of test output directory:");

            // Create directory so test does not fail under windows.
            Directory.CreateDirectory(Path.Combine(assemblyName, $"bin/Debug/{NetcoreVersion}"));
            foreach (var f in Directory.GetFiles(Path.Combine(assemblyName, $"bin/Debug/{NetcoreVersion}")))
            {
                Console.WriteLine("  " + f);
            }

            Console.WriteLine();
        }
    }
}
