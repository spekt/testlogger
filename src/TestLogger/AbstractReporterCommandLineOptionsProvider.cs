// Copyright (c) Spekt Contributors. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

namespace Spekt.TestReporter
{
    using System.Collections.Generic;
    using System.Threading.Tasks;
    using Microsoft.Testing.Platform.CommandLine;
    using Microsoft.Testing.Platform.Extensions;
    using Microsoft.Testing.Platform.Extensions.CommandLine;

    public abstract class AbstractReporterCommandLineOptionsProvider : ICommandLineOptionsProvider
    {
        private readonly IExtension extension;

        protected AbstractReporterCommandLineOptionsProvider(IExtension extension)
            => this.extension = extension;

        public string Uid => this.extension.Uid;

        public string Version => this.extension.Version;

        public string DisplayName => this.extension.DisplayName;

        public string Description => this.extension.Description;

        public abstract string ReportOption { get; }

        public abstract string ReportFileNameOption { get; }

        public IReadOnlyCollection<CommandLineOption> GetCommandLineOptions()
        {
            return
            [
                new CommandLineOption(this.ReportOption, "Enable generating JUnit test report", ArgumentArity.Zero, isHidden: false),
                new CommandLineOption(this.ReportFileNameOption, "The name of the generated JUnit test report", ArgumentArity.ExactlyOne, isHidden: false),
            ];
        }

        public Task<bool> IsEnabledAsync()
            => Task.FromResult(true);

        public Task<ValidationResult> ValidateCommandLineOptionsAsync(ICommandLineOptions commandLineOptions)
        {
            // TODO: Verify if some validation is needed.
            // For example, disallow --report-junit-filename if --report-junit isn't specified.
            return ValidationResult.ValidTask;
        }

        public Task<ValidationResult> ValidateOptionArgumentsAsync(CommandLineOption commandOption, string[] arguments)
        {
            // TODO: Verify if some validation is needed.
            return ValidationResult.ValidTask;
        }
    }
}
