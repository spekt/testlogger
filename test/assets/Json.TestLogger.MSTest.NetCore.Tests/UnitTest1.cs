using System;
using System.Collections.Generic;
using System.Globalization;
using System.Reflection;
using System.Threading.Tasks;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace NUnit.Xml.TestLogger.NetFull.Tests
{
    [TestClass]
    public class UnitTest1
    {
        [TestMethod]
        public void ExampleFailure()
        {
            Assert.Fail();
        }

        [DataTestMethod]
        [CustomDataSource]
        public void Test_Add(int a, int b, int expected)
        {
            Assert.AreEqual(expected, a);
            Assert.AreEqual(expected, b);
        }
    }

    public class CustomDataSourceAttribute : Attribute, ITestDataSource
    {
        public IEnumerable<object[]> GetData(MethodInfo methodInfo)
        {
            yield return new object[] { 1, 1, 2 };
            yield return new object[] { 12, 30, 42 };
            yield return new object[] { 14, 1, 15 };
        }

        public string GetDisplayName(MethodInfo methodInfo, object[] data)
        {
            if (data != null)
                return string.Format(CultureInfo.InvariantCulture, "Custom - {0} ({1})", methodInfo.Name, string.Join(",", data));

            return null;
        }
    }

    [TestClass]
    public class MathTests
    {
        [DataTestMethod]
        [DynamicData(nameof(Data), DynamicDataSourceType.Property)]
        public void Test_Add_DynamicData_Property(int a, int b, int expected)
        {
            Assert.AreEqual(expected, a);
            Assert.AreEqual(expected, b);
        }

        public static IEnumerable<object[]> Data
        {
            get
            {
                yield return new object[] { 1, 1, 2 };
                yield return new object[] { 12, 30, 42 };
                yield return new object[] { 14, 1, 15 };
            }
        }
    }
}