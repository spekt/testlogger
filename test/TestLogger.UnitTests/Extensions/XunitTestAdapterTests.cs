// Copyright (c) Spekt Contributors. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

namespace Spekt.TestLogger.UnitTests.Extensions
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using Microsoft.VisualStudio.TestTools.UnitTesting;
    using Spekt.TestLogger.Core;
    using Spekt.TestLogger.Extensions;

    [TestClass]
    public class XunitTestAdapterTests
    {
        [TestMethod]
        public void TransformShouldThrow()
        {
            var xunit = new XunitTestAdapter();
            Assert.ThrowsException<NotImplementedException>(() =>
                xunit.TransformResults(null, null));
        }
    }
}