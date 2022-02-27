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

        public static IEnumerable<object[]> ValidationTests
        {
            get
            {
                List<object[]> tests = new List<object[]>();
                tests.Add(new object[] { new ValidationTest() });
                tests.Add(new object[] { new ValidationTest() });
                return tests;
            }
        }

        public class ValidationTest { }

        [Theory]
        [MemberData(nameof(ValidationTests))]
        public void When_ValidOrInvalidDataIsProvided_Then_ValidationErrorsOccurAccordingly(ValidationTest test)
        {
            Assert.NotNull(test);
        }
    }
}