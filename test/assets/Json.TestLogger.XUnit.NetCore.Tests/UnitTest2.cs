using System;
using System.Threading.Tasks;
using Xunit;

namespace NUnit.Xml.TestLogger.Tests2
{
    public class UnitTest2
    {
        [Fact]
        public void Example_Failure()
        {
            Assert.False(true);
        }
    }
}
