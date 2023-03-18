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
            string testResultDisplayName,
            string testCaseDisplayName,
            string assemblyPath,
            string codeFilePath,
            int lineNumber,
            DateTime startTime,
            DateTime endTime,
            TimeSpan duration,
            string errorMessage,
            string errorStackTrace,
            List<TestResultMessage> messages,
            IReadOnlyCollection<Trait> traits,
            string executorUri)
        {
            this.Namespace = @namespace;
            this.Type = type;
            this.Method = method;
            this.Outcome = outcome;
            this.TestResultDisplayName = testResultDisplayName;
            this.TestCaseDisplayName = testCaseDisplayName;
            this.AssemblyPath = assemblyPath;
            this.CodeFilePath = codeFilePath;
            this.LineNumber = lineNumber;
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

        public string AssemblyPath { get; }

        public string CodeFilePath { get; }

        public int LineNumber { get; }

        public DateTime StartTime { get; }

        public DateTime EndTime { get; }

        public TimeSpan Duration { get; }

        public string ErrorMessage { get; }

        public string ErrorStackTrace { get; }

        public List<TestResultMessage> Messages { get; }

        public IReadOnlyCollection<Trait> Traits { get; }

        public string ExecutorUri { get; }

        public string FullTypeName => this.Namespace + "." + this.Type;

        /// <summary>
        /// Gets value that originated at <see cref="TestResult.DisplayName"/>. Intended for use within
        /// this library by framework specific adapters, to ensure that <see cref="Method"/> has the
        /// proper value.
        /// </summary>
        internal string TestResultDisplayName { get; }

        /// <summary>
        /// Gets value that originated at <see cref="TestCase.DisplayName"/>. Intended for use within
        /// this library by framework specific adapters, to ensure that <see cref="Method"/> has the
        /// proper value.
        /// </summary>
        internal string TestCaseDisplayName { get; }

        public override bool Equals(object obj)
        {
            return obj is TestResultInfo info &&
                   this.Namespace == info.Namespace &&
                   this.Type == info.Type &&
                   this.Method == info.Method &&
                   this.Outcome == info.Outcome &&
                   this.TestResultDisplayName == info.TestResultDisplayName &&
                   this.TestCaseDisplayName == info.TestCaseDisplayName &&
                   this.AssemblyPath == info.AssemblyPath &&
                   this.CodeFilePath == info.CodeFilePath &&
                   this.LineNumber == info.LineNumber &&
                   this.StartTime == info.StartTime &&
                   this.EndTime == info.EndTime &&
                   this.Duration.Equals(info.Duration) &&
                   this.ErrorMessage == info.ErrorMessage &&
                   this.ErrorStackTrace == info.ErrorStackTrace &&
                   EqualityComparer<List<TestResultMessage>>.Default.Equals(this.Messages, info.Messages) &&
                   EqualityComparer<IReadOnlyCollection<Trait>>.Default.Equals(this.Traits, info.Traits) &&
                   this.ExecutorUri == info.ExecutorUri &&
                   this.FullTypeName == info.FullTypeName;
        }

        public override int GetHashCode()
        {
            int hashCode = -1082088776;
            hashCode = (hashCode * -1521134295) + EqualityComparer<string>.Default.GetHashCode(this.Namespace);
            hashCode = (hashCode * -1521134295) + EqualityComparer<string>.Default.GetHashCode(this.Type);
            hashCode = (hashCode * -1521134295) + EqualityComparer<string>.Default.GetHashCode(this.Method);
            hashCode = (hashCode * -1521134295) + this.Outcome.GetHashCode();
            hashCode = (hashCode * -1521134295) + EqualityComparer<string>.Default.GetHashCode(this.TestResultDisplayName);
            hashCode = (hashCode * -1521134295) + EqualityComparer<string>.Default.GetHashCode(this.TestCaseDisplayName);
            hashCode = (hashCode * -1521134295) + EqualityComparer<string>.Default.GetHashCode(this.AssemblyPath);
            hashCode = (hashCode * -1521134295) + EqualityComparer<string>.Default.GetHashCode(this.CodeFilePath);
            hashCode = (hashCode * -1521134295) + this.LineNumber.GetHashCode();
            hashCode = (hashCode * -1521134295) + this.StartTime.GetHashCode();
            hashCode = (hashCode * -1521134295) + this.EndTime.GetHashCode();
            hashCode = (hashCode * -1521134295) + this.Duration.GetHashCode();
            hashCode = (hashCode * -1521134295) + EqualityComparer<string>.Default.GetHashCode(this.ErrorMessage);
            hashCode = (hashCode * -1521134295) + EqualityComparer<string>.Default.GetHashCode(this.ErrorStackTrace);
            hashCode = (hashCode * -1521134295) + EqualityComparer<List<TestResultMessage>>.Default.GetHashCode(this.Messages);
            hashCode = (hashCode * -1521134295) + EqualityComparer<IReadOnlyCollection<Trait>>.Default.GetHashCode(this.Traits);
            hashCode = (hashCode * -1521134295) + EqualityComparer<string>.Default.GetHashCode(this.ExecutorUri);
            hashCode = (hashCode * -1521134295) + EqualityComparer<string>.Default.GetHashCode(this.FullTypeName);
            return hashCode;
        }
    }
}
