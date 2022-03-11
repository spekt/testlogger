using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
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

    [Collection(nameof(TestCollection))]
    public class CollectionTest1
    {
        private readonly Fixture _fixture;

        public CollectionTest1(Fixture fixture)
        {
            _fixture = fixture;
        }

        [Fact]
        public void CollectionOnly() => Assert.True(true);

        [Theory]
        [InlineData(2)]
        [InlineData(4)]
        public void CollectionAndInline(int i) => Assert.NotEqual(0, i);
    }

    [CollectionDefinition(nameof(TestCollection))]
    public class TestCollection : ICollectionFixture<Fixture>
    {

    }

    public class TestedClass { }

    public class Fixture : IDisposable
    {
        public TestedClass Sut { get; private set; }

        public Fixture()
        {
            Sut = new TestedClass();
        }

        public void Dispose()
        {
        }
    }

    public class TheoryClassData
    {
        [Theory]
        [ClassData(typeof(CalculatorTestData))]
        public void CanAddTheoryClassData(int i, int j, int expected)
        {
            Assert.Equal(expected, i);
            Assert.Equal(expected, j); 
        }
    }

    public class CalculatorTestData : IEnumerable<object[]>
    {
        public IEnumerator<object[]> GetEnumerator()
        {
            yield return new object[] { 1, 2, 3 };
            yield return new object[] { -4, -6, -10 };
            yield return new object[] { -2, 2, 0 };
            yield return new object[] { int.MinValue, -1, int.MaxValue };
        }

        IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();
    }

    public class TheoryMemberData
    {
        [Theory]
        [MemberData(nameof(GetData), parameters: 3)]
        public void CanAddTheoryMemberDataMethod(int i, int j, int expected)
        {
            Assert.Equal(expected, i);
            Assert.Equal(expected, j);
        }

        public static IEnumerable<object[]> GetData(int numTests)
        {
            var allData = new List<object[]>
        {
            new object[] { 1, 2, 3 },
            new object[] { -4, -6, -10 },
            new object[] { -2, 2, 0 },
            new object[] { int.MinValue, -1, int.MaxValue },
        };

            return allData.Take(numTests);
        }
    }
}
