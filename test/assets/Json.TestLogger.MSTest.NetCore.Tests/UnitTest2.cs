using System;
using System.Threading.Tasks;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace NUnit.Xml.TestLogger.Tests2
{
    [TestClass]
    public class UnitTest2
    {
        [TestMethod]
        public void ExampleFailure()
        {
            Assert.Fail();
        }

        [TestMethod]
        [DataRow(-1)]
        [DataRow(0)]
        [DataRow(1)]
        public void TestMethodDataRow(int value)
        {
            Assert.AreEqual(0,value);
        }

        [DataTestMethod]
        [DataRow(-1)]
        [DataRow(0)]
        [DataRow(1)]
        public void DataTestMethodDataRow(int value)
        {
            Assert.AreEqual(0, value);
        }
    }

    [TestClass]
    public class DataRowWithDisplayName
    {
        [TestMethod]
        [DataRow("ABCDE", 3, null, DisplayName = "When string is null")]
        [DataRow("", 3, "", DisplayName = "When string is empty")]
        public void Substring(string testString, int targetLength, string expectedValue)
        {
            testString = testString.Substring(targetLength);

            Assert.AreEqual(expectedValue, testString);
        }
    }
}
