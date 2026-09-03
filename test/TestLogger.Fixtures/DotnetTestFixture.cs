// Copyright (c) Spekt Contributors. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

namespace TestLogger.Fixtures
{
    using System;
    using System.Diagnostics;
    using System.IO;
    using System.Text;

    public class DotnetTestFixture
    {
        private const string NetcoreVersion = "net8.0";
        private bool cleanProject = false;
        private bool noBuild = false;
        private string relativeResultsDirectory = string.Empty;
        private string runSettingsSuffix = string.Empty;

        public static DotnetTestFixture Create() => new DotnetTestFixture();

        public DotnetTestFixture WithBuild(bool cleanProject = true)
        {
            this.cleanProject = cleanProject;
            this.noBuild = false;
            return this;
        }

        /// <summary>
        /// Runs the test leg with --no-build against pre-built outputs.
        /// Only use where --no-build is known to work (MTP legs); VSTest legs
        /// must build (incrementally) because vstest.console rejects --no-build runs.
        /// </summary>
        /// <returns>The current fixture instance.</returns>
        public DotnetTestFixture WithNoBuild()
        {
            this.cleanProject = false;
            this.noBuild = true;
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

        public string Execute(string assemblyName, string loggerArgs, bool collectCoverage, string resultsFileName, bool isMTP = false)
        {
            if (this.cleanProject)
            {
                using var cleanProcess = new Process
                {
                    StartInfo =
                    {
                        UseShellExecute = false,
                        RedirectStandardOutput = true,
                        FileName = "dotnet",
                        Arguments = $"clean \"{assemblyName.ToAssetDirectoryPath()}\\{assemblyName}.csproj\"{(isMTP ? " -p:IsMTP=true" : string.Empty)}"
                    }
                };
                cleanProcess.Start();
                cleanProcess.StandardOutput.ReadToEnd();
                cleanProcess.WaitForExit();
            }

            // Clean up global.json to allow running both VSTest and MTP tests in the same build
            var globalJsonTemplate = Path.Combine(assemblyName.ToAssetDirectoryPath(), "..", "global.json.template");
            var globalJsonPath = Path.Combine(assemblyName.ToAssetDirectoryPath(), "..", "global.json");
            if (File.Exists(globalJsonPath))
            {
                File.Delete(globalJsonPath);
            }

            var resultsDirectory = Path.Combine(assemblyName.ToAssetDirectoryPath(), this.relativeResultsDirectory);
            var resultsFile = Path.Combine(resultsDirectory, resultsFileName);
            if (File.Exists(resultsFile))
            {
                File.Delete(resultsFile);
            }

            // Run dotnet test with logger. --no-build is opt-in (WithNoBuild) because
            // vstest.console rejects --no-build runs; default is an incremental build.
            var buildArgs = this.noBuild ? "--no-build" : string.Empty;
            var resultDirectoryArgs = string.IsNullOrEmpty(this.relativeResultsDirectory) ? string.Empty : $"--results-directory \"{resultsDirectory}\"";

            if (isMTP)
            {
                buildArgs += " -p:IsMTP=true";
                if (resultDirectoryArgs.Length == 0)
                {
                    resultDirectoryArgs = $"--results-directory \"{resultsDirectory}\"";
                }

                File.Copy(globalJsonTemplate, globalJsonPath);
            }
            else
            {
                loggerArgs = $"--logger:\"{loggerArgs}\"";
            }

            var testProjectPath = Path.Combine(assemblyName.ToAssetDirectoryPath(), $"{assemblyName}.csproj");
            var commandlineSuffix = string.IsNullOrEmpty(this.runSettingsSuffix) ? string.Empty : $"--{(isMTP ? "test-parameter" : string.Empty)} {this.runSettingsSuffix}";
            var testCommand = isMTP ? $"test --project \"{testProjectPath}\"" : $"test \"{testProjectPath}\"";
            using var dotnet = new Process
            {
                StartInfo =
                {
                    WorkingDirectory = assemblyName.ToAssetDirectoryPath(),
                    UseShellExecute = false,
                    RedirectStandardOutput = true,
                    RedirectStandardError = true,
                    FileName = "dotnet",
                    Arguments = $"{testCommand} {buildArgs} {loggerArgs} {resultDirectoryArgs} {commandlineSuffix}"
                }
            };

            // Add coverage arg if required
            if (collectCoverage)
            {
                if (isMTP)
                {
                    // https://github.com/coverlet-coverage/coverlet/issues/1715
                    throw new NotSupportedException("Coverlet isn't supported with MTP yet.");
                }

                var coverletRunSettingsPath = Path.Combine(Environment.CurrentDirectory, "coverlet.runsettings");
                dotnet.StartInfo.Arguments += $" --collect:\"XPlat Code Coverage\" --settings \"{coverletRunSettingsPath}\"";
            }

            this.LogTestAssetOutDir(assemblyName, isMTP);

            Console.WriteLine("\n\n## Test run arguments: dotnet " + dotnet.StartInfo.Arguments);

            // Use async reads to avoid deadlock when child process writes heavily to stderr
            // while parent blocks on stdout ReadToEnd().
            var outputBuilder = new StringBuilder();
            var errorBuilder = new StringBuilder();
            dotnet.OutputDataReceived += (sender, e) =>
            {
                if (e.Data != null)
                {
                    outputBuilder.AppendLine(e.Data);
                }
            };
            dotnet.ErrorDataReceived += (sender, e) =>
            {
                if (e.Data != null)
                {
                    errorBuilder.AppendLine(e.Data);
                }
            };
            dotnet.Start();
            dotnet.BeginOutputReadLine();
            dotnet.BeginErrorReadLine();
            dotnet.WaitForExit();

            var output = outputBuilder.ToString();
            var error = errorBuilder.ToString();

            Console.WriteLine("\n\n ## Test run output\n" + output);
            if (!string.IsNullOrEmpty(error))
            {
                Console.WriteLine("\n\n ## Test run error\n" + error);
            }

            return resultsFile;
        }

        private void LogTestAssetOutDir(string assemblyName, bool isMTP)
        {
            // Log the contents of test output directory. Useful to verify if the logger is copied
            Console.WriteLine("\n\n## Contents of test output directory:");

            var flavor = isMTP ? "mtp" : "vstest";

            // Create directory so test does not fail under windows.
            Directory.CreateDirectory(Path.Combine(assemblyName, $"bin/Debug/{flavor}/{NetcoreVersion}"));
            foreach (var f in Directory.GetFiles(Path.Combine(assemblyName, $"bin/Debug/{flavor}/{NetcoreVersion}")))
            {
                Console.WriteLine("  " + f);
            }

            Console.WriteLine();
        }
    }
}
