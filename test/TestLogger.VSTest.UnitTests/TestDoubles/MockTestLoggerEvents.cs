// Copyright (c) Spekt Contributors. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

namespace Spekt.TestLogger.UnitTests.TestDoubles
{
    using System;

    using Microsoft.VisualStudio.TestPlatform.ObjectModel;
    using Microsoft.VisualStudio.TestPlatform.ObjectModel.Client;
    using Microsoft.VisualStudio.TestPlatform.ObjectModel.Logging;

    public class MockTestLoggerEvents : TestLoggerEvents
    {
        /// <inheritdoc/>
        public override event EventHandler<TestRunMessageEventArgs> TestRunMessage;

        /// <inheritdoc/>
        public override event EventHandler<TestRunStartEventArgs> TestRunStart;

        /// <inheritdoc/>
        public override event EventHandler<TestResultEventArgs> TestResult;

        /// <inheritdoc/>
        public override event EventHandler<TestRunCompleteEventArgs> TestRunComplete;

        /// <inheritdoc/>
        public override event EventHandler<DiscoveryStartEventArgs> DiscoveryStart;

        /// <inheritdoc/>
        public override event EventHandler<TestRunMessageEventArgs> DiscoveryMessage;

        /// <inheritdoc/>
        public override event EventHandler<DiscoveredTestsEventArgs> DiscoveredTests;

        /// <inheritdoc/>
        public override event EventHandler<DiscoveryCompleteEventArgs> DiscoveryComplete;

        public void RaiseTestRunStart(TestRunCriteria criteria)
        {
            this.TestRunStart(this, new TestRunStartEventArgs(criteria));
        }

        public void RaiseTestRunMessage(TestMessageLevel level, string message)
        {
            this.TestRunMessage(this, new TestRunMessageEventArgs(level, message));
        }

        public void RaiseTestResult(TestResult result)
        {
            this.TestResult(this, new TestResultEventArgs(result));
        }

        public void RaiseTestRunComplete(TestRunStatistics stats)
        {
            var completeEvent = new TestRunCompleteEventArgs(stats: stats, isCanceled: false, isAborted: false, error: null, attachmentSets: null, elapsedTime: TimeSpan.FromSeconds(30));
            this.TestRunComplete(this, completeEvent);
        }

        public bool TestRunEventsSubscribed()
        {
            return this.TestRunMessage != null && this.TestRunStart != null
                && this.TestResult != null && this.TestRunComplete != null;
        }

        public bool TestDiscoveryEventsSubscribed()
        {
            return this.DiscoveryStart != null && this.DiscoveryMessage != null
                && this.DiscoveredTests != null && this.DiscoveryComplete != null;
        }
    }
}
