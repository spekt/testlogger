// Copyright (c) Spekt Contributors. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

namespace Json.TestLogger
{
    using Microsoft.VisualStudio.TestPlatform.ObjectModel;
    using Spekt.TestLogger.UnitTests.TestDoubles;

    [FriendlyName(FriendlyName)]
    [ExtensionUri(ExtensionUri)]
    public class JsonTestLogger : Spekt.TestLogger.TestLogger
    {
        /// <summary>
        /// Uri used to uniquely identify the logger.
        /// </summary>
        private const string ExtensionUri = "logger://Microsoft/TestPlatform/JsonLogger/v1";

        /// <summary>
        /// Alternate user friendly string to uniquely identify the console logger.
        /// </summary>
        private const string FriendlyName = "json";

        public JsonTestLogger()
            : base(new JsonTestResultSerializer())
        {
        }

        protected override string DefaultTestResultFile => "TestResults.json";
    }
}
