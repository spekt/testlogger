// Copyright (c) Spekt Contributors. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

namespace Spekt.TestReporter
{
    using System.Collections.Generic;
    using System.Linq;
    using System.Threading.Tasks;
    using Microsoft.Testing.Platform.CommandLine;
    using Microsoft.Testing.Platform.Extensions;
    using Microsoft.Testing.Platform.Extensions.CommandLine;

    public class TestReporterCommandLineProvider : ICommandLineOptionsProvider
    {
        private readonly IExtension extension;
        private readonly string loggerName;

        public TestReporterCommandLineProvider(IExtension extension, string loggerName)
        {
            this.extension = extension;
            this.loggerName = loggerName;
        }

        public string Uid => this.extension.Uid;

        public string Version => this.extension.Version;

        public string DisplayName => this.extension.DisplayName;

        public string Description => this.extension.Description;

        public string ReportOption => $"report-{this.loggerName}";

        public string ReportFileNameOption => $"report-{this.loggerName}-filename";

        public string ReportConfigOption => $"report-{this.loggerName}-config";

        public IReadOnlyCollection<CommandLineOption> GetCommandLineOptions()
        {
            return
            [
                new CommandLineOption(this.ReportOption, $"Enable generating {this.loggerName} test report", ArgumentArity.Zero, isHidden: false),
                new CommandLineOption(this.ReportFileNameOption, $"The name of the generated {this.loggerName} test report", ArgumentArity.ExactlyOne, isHidden: false),
                new CommandLineOption(this.ReportConfigOption, $"Configuration key-value pairs for {this.loggerName} reporter (format: key1=value1;key2=value2)", ArgumentArity.ExactlyOne, isHidden: false),
            ];
        }

        public Task<bool> IsEnabledAsync()
            => Task.FromResult(true);

        public Task<ValidationResult> ValidateCommandLineOptionsAsync(ICommandLineOptions commandLineOptions)
        {
            // Validate that filename and config options are only used when report option is enabled
            if (commandLineOptions.IsOptionSet(this.ReportFileNameOption) && !commandLineOptions.IsOptionSet(this.ReportOption))
            {
                return Task.FromResult(ValidationResult.Invalid($"--{this.ReportFileNameOption} requires --{this.ReportOption}"));
            }

            if (commandLineOptions.IsOptionSet(this.ReportConfigOption) && !commandLineOptions.IsOptionSet(this.ReportOption))
            {
                return Task.FromResult(ValidationResult.Invalid($"--{this.ReportConfigOption} requires --{this.ReportOption}"));
            }

            return ValidationResult.ValidTask;
        }

        public Task<ValidationResult> ValidateOptionArgumentsAsync(CommandLineOption commandOption, string[] arguments)
        {
            if (commandOption.Name == this.ReportConfigOption && arguments.Length > 0)
            {
                // Validate config option format (key=value pairs separated by semicolons)
                var configValue = arguments[0];
                if (!string.IsNullOrEmpty(configValue))
                {
                    var pairs = configValue.Split(';');
                    foreach (var pair in pairs)
                    {
                        if (string.IsNullOrWhiteSpace(pair))
                        {
                            continue;
                        }

                        if (!pair.Contains('=') || pair.Split('=').Length != 2)
                        {
                            return Task.FromResult(ValidationResult.Invalid(
                                $"Invalid config format for --{this.ReportConfigOption}. Use key1=value1;key2=value2"));
                        }
                    }
                }
            }

            return ValidationResult.ValidTask;
        }
    }
}
