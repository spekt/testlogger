using Xunit;

namespace Xunit.Xml.TestLogger.NoTestSdk.Tests
{
    public class UnitTest1
    {
        [Fact]
        public void PassTest11()
        {
        }

        [Fact]
        public void PassTest12()
        {
            Assert.Equal(2, 2);
        }

        [Fact]
        public void FailTest11()
        {
            Assert.False(true);
        }
    }
}
