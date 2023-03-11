// Copyright (c) Spekt Contributors. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

namespace Spekt.TestLogger.Core
{
    using System;
    using System.Collections.Generic;
    using Microsoft.VisualStudio.TestPlatform.ObjectModel;

    public sealed class TestResultInfo
    {
        public TestResultInfo(
            string @namespace,
            string type,
            string method,
            TestOutcome outcome,
            string displayName,
            string assemblyPath,
            DateTime startTime,
            DateTime endTime,
            TimeSpan duration,
            string errorMessage,
            string errorStackTrace,
            List<TestResultMessage> messages,
            TraitCollection traits,
            string executorUri)
        {
            this.Namespace = @namespace;
            this.Type = type;
            this.Method = method;
            this.Outcome = outcome;
            this.DisplayName = displayName;
            this.AssemblyPath = assemblyPath;
            this.StartTime = startTime;
            this.EndTime = endTime;
            this.Duration = duration;
            this.ErrorMessage = errorMessage;
            this.ErrorStackTrace = errorStackTrace;
            this.Messages = messages;
            this.Traits = traits;
            this.ExecutorUri = executorUri;
        }

        public string Namespace { get; }

        public string Type { get; }

        /// <summary>
        /// Gets a string that contain the method name, along with any paramaterized data related to
        /// the method. For example, `SomeMethod` or `SomeParameterizedMethod(true)`.
        /// </summary>
        public string Method { get; internal set; }

        public TestOutcome Outcome { get; set; }

        public string DisplayName { get; }

        public string AssemblyPath { get; }

        public DateTime StartTime { get; }

        public DateTime EndTime { get; }

        public TimeSpan Duration { get; }

        public string ErrorMessage { get; }

        public string ErrorStackTrace { get; }

        public List<TestResultMessage> Messages { get; }

        public TraitCollection Traits { get; }

        public string ExecutorUri { get; }

        public string FullTypeName => this.Namespace + "." + this.Type;

        public override bool Equals(object obj)
        {
            if (obj is not TestResultInfo objectToCompare)
            {
                return false;
            }

            return string.Compare(this.ErrorMessage, objectToCompare.ErrorMessage, StringComparison.CurrentCulture) == 0
                   && string.Compare(this.ErrorStackTrace, objectToCompare.ErrorStackTrace, StringComparison.CurrentCulture) == 0;
        }

        public override int GetHashCode()
        {
            int hashCode = -33026708;
            hashCode = (hashCode * -1521134295) + EqualityComparer<string>.Default.GetHashCode(this.Namespace);
            hashCode = (hashCode * -1521134295) + EqualityComparer<string>.Default.GetHashCode(this.Type);
            hashCode = (hashCode * -1521134295) + EqualityComparer<string>.Default.GetHashCode(this.Method);
            hashCode = (hashCode * -1521134295) + this.Outcome.GetHashCode();
            hashCode = (hashCode * -1521134295) + EqualityComparer<string>.Default.GetHashCode(this.DisplayName);
            hashCode = (hashCode * -1521134295) + EqualityComparer<string>.Default.GetHashCode(this.AssemblyPath);
            hashCode = (hashCode * -1521134295) + this.StartTime.GetHashCode();
            hashCode = (hashCode * -1521134295) + this.EndTime.GetHashCode();
            hashCode = (hashCode * -1521134295) + this.Duration.GetHashCode();
            hashCode = (hashCode * -1521134295) + EqualityComparer<string>.Default.GetHashCode(this.ErrorMessage);
            hashCode = (hashCode * -1521134295) + EqualityComparer<string>.Default.GetHashCode(this.ErrorStackTrace);
            hashCode = (hashCode * -1521134295) + EqualityComparer<IReadOnlyCollection<TestResultMessage>>.Default.GetHashCode(this.Messages);
            hashCode = (hashCode * -1521134295) + EqualityComparer<TraitCollection>.Default.GetHashCode(this.Traits);
            hashCode = (hashCode * -1521134295) + EqualityComparer<string>.Default.GetHashCode(this.ExecutorUri);
            hashCode = (hashCode * -1521134295) + EqualityComparer<string>.Default.GetHashCode(this.FullTypeName);
            return hashCode;
        }
    }
}
