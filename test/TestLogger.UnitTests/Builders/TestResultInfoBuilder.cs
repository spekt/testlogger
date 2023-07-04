﻿// Copyright (c) Spekt Contributors. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

namespace Spekt.TestLogger.UnitTests.Builders
{
    using System;
    using System.Collections.Generic;
    using Microsoft.VisualStudio.TestPlatform.ObjectModel;
    using Spekt.TestLogger.Core;

    internal class TestResultInfoBuilder
    {
        private readonly string @namespace = string.Empty;
        private readonly string type = string.Empty;
        private readonly string method = string.Empty;
        private TestOutcome outcome = TestOutcome.Passed;
        private IReadOnlyCollection<Trait> traits = new List<Trait>();
        private IReadOnlyCollection<KeyValuePair<string, object>> properties = new List<KeyValuePair<string, object>>();
        private string errorMessage = string.Empty;
        private string displayName = string.Empty;

        internal TestResultInfoBuilder()
        {
        }

        internal TestResultInfoBuilder(
            string @namespace,
            string type,
            string method)
        {
            this.@namespace = @namespace;
            this.type = type;
            this.method = method;
        }

        internal TestResultInfoBuilder WithOutcome(TestOutcome outcome)
        {
            this.outcome = outcome;
            return this;
        }

        internal TestResultInfoBuilder WithTraits(IReadOnlyCollection<Trait> traits)
        {
            this.traits = traits;
            return this;
        }

        internal TestResultInfoBuilder WithProperties(IReadOnlyCollection<KeyValuePair<string, object>> properties)
        {
            this.properties = properties;
            return this;
        }

        internal TestResultInfoBuilder WithErrorMessage(string errorMessage)
        {
            this.errorMessage = errorMessage;
            return this;
        }

        internal TestResultInfoBuilder WithDisplayName(string displayName)
        {
            this.displayName = displayName;
            return this;
        }

        internal TestResultInfo Build()
        {
            return new TestResultInfo(
                this.@namespace,
                this.type,
                this.method,
                $"{this.@namespace}.{this.type}.{this.method}",
                this.outcome,
                this.displayName,
                this.displayName,
                "/tmp/test.dll",
                "/tmp/code.cs",
                42,
                DateTime.Parse("2023-01-02 03:04:05"),
                DateTime.Parse("2023-01-02 03:04:06"),
                TimeSpan.FromSeconds(1),
                this.errorMessage,
                string.Empty,
                new (),
                this.traits,
                this.properties,
                "executor://dummy");
        }
    }
}
