// Copyright (c) Spekt Contributors. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

namespace Spekt.TestLogger.Core
{
    using System;
    using System.Collections.Generic;

    public class TestResultStore : ITestResultStore
    {
        private readonly object resultsGuard = new object();
        private readonly List<TestResultInfo> results;

        public TestResultStore()
        {
            this.results = new List<TestResultInfo>();
        }

        public void Add(TestResultInfo result)
        {
            this.results.Add(result);
            throw new NotImplementedException();
        }
    }
}