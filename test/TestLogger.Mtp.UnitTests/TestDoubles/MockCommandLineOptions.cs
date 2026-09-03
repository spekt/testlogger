// Copyright (c) Spekt Contributors. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

namespace Spekt.TestLogger.UnitTests.TestDoubles
{
    using System.Collections.Generic;
    using System.ComponentModel;
    using Microsoft.Testing.Platform.CommandLine;
    using Microsoft.Testing.Platform.Extensions.CommandLine;

    /// <summary>
    /// Mock implementation of ICommandLineOptions for testing.
    /// </summary>
    public class MockCommandLineOptions : ICommandLineOptions
    {
        private readonly Dictionary<string, string[]> options = new Dictionary<string, string[]>();

        /// <summary>
        /// Sets a command line option with the specified arguments.
        /// </summary>
        /// <param name="optionName">The name of the option.</param>
        /// <param name="arguments">The arguments for the option.</param>
        public void SetOption(string optionName, params string[] arguments)
        {
            this.options[optionName] = arguments;
        }

        /// <summary>
        /// Checks if an option is set.
        /// </summary>
        /// <param name="optionName">The name of the option.</param>
        /// <returns>True if the option is set, false otherwise.</returns>
        public bool IsOptionSet(string optionName)
        {
            return this.options.ContainsKey(optionName);
        }

        /// <summary>
        /// Tries to get the argument list for an option.
        /// </summary>
        /// <param name="optionName">The name of the option.</param>
        /// <param name="arguments">The arguments for the option.</param>
        /// <returns>True if the option exists, false otherwise.</returns>
        public bool TryGetOptionArgumentList(string optionName, out string[] arguments)
        {
            return this.options.TryGetValue(optionName, out arguments);
        }

        // Required interface implementations - return empty/defaults
        public IEnumerable<CommandLineOption> GetCommandLineOptions() => [];

        public bool TryGetOptionArgument(string optionName, out string argument)
        {
            argument = null;
            return false;
        }

        public bool TryGetOptionArgumentList(string optionName, out IReadOnlyList<string> arguments)
        {
            arguments = null;
            return false;
        }
    }
}