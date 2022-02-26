using System;
using System.Threading.Tasks;
using Xunit;

namespace NUnit.Xml.TestLogger.NetFull.Tests
{
    public class UnitTest1
    {
        [Fact]
        public void Example_Failure()
        {
            Assert.False(true);
        }
    }
}