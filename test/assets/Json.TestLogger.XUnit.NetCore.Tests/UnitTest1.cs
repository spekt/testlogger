using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Xunit;

namespace NUnit.Xml.TestLogger.NetFull.Tests
{
    public class UnitTest1
    {
        [Fact]
        public void Example_Success()
        {
            Assert.True(true);
        }

        [Fact]
        public void Example_Failure()
        {
            Assert.False(true);
        }

        [Theory]
        [InlineData("foo")]
        [InlineData(null)]
        public void MyTheory(string input)
        {
            Assert.NotNull(input);
        }
    }
}