using System;
using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;
using NUnit.Framework;

namespace NUnit.Xml.TestLogger.NetFull.Tests
{
    [TestFixture]
    public class UnitTest1
    {
        [Test]
        [Description("Passing test description")]
        public async Task PassTest11()
        {
            await Task.Delay(TimeSpan.FromMilliseconds(400));
        }

        [Test]
        public void FailTest11()
        {
            Assert.False(true);
        }

        [Test]
        public void Inconclusive()
        {
            Assert.Inconclusive("test inconclusive");
        }

        [Test]
        [Ignore("ignore reason")]
        public void Ignored()
        {
        }

        [Test]
        [Property("Property name", "Property value")]
        public void WithProperty()
        {
        }

        [Test]
        public void NoProperty()
        {
        }

        [Test]
        [Category("Nunit Test Category")]
        public void WithCategory()
        {
        }

        [Test]
        [Category("Category2")]
        [Category("Category1")]
        public void MultipleCategories()
        {
        }

        [Test]
        [Category("NUnit Test Category")]
        [Property("Property name", "Property value")]
        public void WithCategoryAndProperty()
        {
        }

        [Test]
        [Property("Property name", "Property value 1")]
        [Property("Property name", "Property value 2")]
        public void WithProperties()
        {
        }
    }

    public class UnitTest2
    {
        [Test]
        [Category("passing category")]
        public void PassTest21()
        {
            Assert.That(2, Is.EqualTo(2));
        }

        [Test]
        [Category("failing category")]
        public void FailTest22()
        {
            Assert.False(true);
        }

        [Test]
        public void Inconclusive()
        {
            Assert.Inconclusive();
        }

        [Test]
        [Ignore("ignore reason")]
        public void IgnoredTest()
        {
        }

        [Test]
        public void WarningTest()
        {
            Assert.Warn("Warning");
        }

        [Test]
        [Explicit]
        public void ExplicitTest()
        {
        }
    }

    [TestFixture]
    public class SuccessFixture
    {
        [Test]
        public void SuccessTest()
        {
        }
    }

    [TestFixture]
    public class SuccessAndInconclusiveFixture
    {
        [Test]
        public void SuccessTest()
        {
        }

        [Test]
        public void InconclusiveTest()
        {
            Assert.Inconclusive();
        }
    }

    [TestFixture]
    public class FailingOneTimeSetUp
    {
        [OneTimeSetUp]
        public void OneTimeSetUp()
        {
            throw new InvalidOperationException();
        }

        [Test]
        public void Test()
        {
        }
    }

    [TestFixture]
    public class FailingTestSetup
    {
        [SetUp]
        public void SetUp()
        {
            throw new InvalidOperationException();
        }

        [Test]
        public void Test()
        {
        }
    }

    [TestFixture]
    public class FailingTearDown
    {
        [TearDown]
        public void TearDown()
        {
            throw new InvalidOperationException();
        }

        [Test]
        public void Test()
        {
        }
    }

    [TestFixture]
    public class FailingOneTimeTearDown
    {
        [OneTimeTearDown]
        public void OneTimeTearDown()
        {
            throw new InvalidOperationException();
        }

        [Test]
        public void Test()
        {
        }
    }

    [TestFixture]
    [TestFixtureSource("FixtureArgs")]
    public class ParametrizedFixture
    {
        public ParametrizedFixture(string word, int num)
        {
        }

        [Test]
        public void Test()
        {
        }

        static object[] FixtureArgs =
        {
            new object[] {"Question", 1},
            new object[] {"Answer", 42}
        };
    }

    [TestFixture]
    public class ParametrizedTestCases
    {
        [Test]
        public void TestData([Values(1, 2)] int x, [Values("A", "B")] string s)
        {
            Assert.That(x, Is.Not.EqualTo(2), "failing for second case");
            Assert.That(s, Is.Not.Null);
        }
    }

    public class Issue28_Examples
    {
        [TestCase('0')]
        [TestCase('a')]
        [TestCase('A')]
        [TestCase('!')]
        [TestCase('-')]
        [TestCase('_')]
        [TestCase('.')]
        [TestCase('*')]
        [TestCase('\'')]
        [TestCase('(')]
        [TestCase(')')]
        [TestCase('/')]
        public void ExampleTest3(char c)
        {
            Assert.IsNotNull(c);
        }

        [TestCaseSource(nameof(ExceptionTestCases))]
        public void ExampleTest4(Exception e) => Assert.Pass();

        private static readonly Exception[] ExceptionTestCases = 
        {
            new MyException1(),
            new MyException2(),
            new AggregateException(new MyException1()),
            new AggregateException(new MyException2())
        };

        public class MyException1 : Exception { }
        public class MyException2 : Exception { }

        [TestCaseSource(nameof(MyTestCases))]
        public void ExampleTest4(string b) => Assert.Pass();

        private static IEnumerable<TestCaseData> MyTestCases()
        {
            yield return new TestCaseData("a").SetName("Testing. Input.");
            yield return new TestCaseData("a").SetName("Testing. Input");
            yield return new TestCaseData("a").SetName("Testing.");
            yield return new TestCaseData("a").SetName("Testing");
        }
    }
}