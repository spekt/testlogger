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
            Assert.AreEqual(0, value);
        }

        [TestMethod("@RMS-TEST-3 RMS Test 3")]
        [DataRow("PostAsync", null)]
        [DataRow("GetAsync", typeof(string))]
        [DataRow("DeleteAsync", typeof(int))]
        public void ValidateTest3(string methodName, Type type)
        {
            Assert.Fail($"Failed on {methodName}");
        }

        [TestMethod("@RMS-TEST-4 RMS Test 4")]
        [DataRow("PostAsync")]
        [DataRow("GetAsync", typeof(string), typeof(string))]
        [DataRow("DeleteAsync", typeof(int))]
        public void ValidateTest4(string methodName, params Type[] extraParams)
        {
            Assert.Fail($"Failed on {methodName}");
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
