using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Xunit;
using Xunit.Sdk;
using Xunit.Abstractions;

namespace NUnit.Xml.TestLogger.Tests2
{
    public class ApiTest
    {
        [Fact]
        [ApiTest]
        [Trait("SomeProp", "SomeVal")]
        public void ExampleTest()
        {
            Assert.False(false);
        }
    }
}
