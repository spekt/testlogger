// Copyright (c) Spekt Contributors. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

namespace JUnit.Xml.TestLogger.AcceptanceTests
{
    using System.Xml.Linq;
    using System.Xml.XPath;
    using global::TestLogger.Fixtures;
    using Microsoft.VisualStudio.TestTools.UnitTesting;

    /// <summary>
    /// Acceptance tests evaluate the most recent output of the build.ps1 script, NOT the most
    /// recent build performed by visual studio or dotnet.build
    ///
    /// These acceptance tests look at the specific places output is expected to change because of
    /// the format option specified. Accordingly, these tests cannot protect against other changes
    /// occurring due to the formatting option.
    /// </summary>
    [TestClass]
    public class JUnitTestLoggerStoreConsoleOutputOptionsAcceptanceTests
    {
        [TestMethod]
        public void StoreConsoleOutput_Default_ContainsConsoleOut()
        {
            var loggerArgs = "junit;LogFilePath=output-default-test-results.xml";

            var resultsFile = DotnetTestFixture
                                .Create()
                                .WithBuild()
                                .Execute("JUnit.Xml.TestLogger.NetCore.Tests", loggerArgs, collectCoverage: false, "output-default-test-results.xml");

            XDocument resultsXml = XDocument.Load(resultsFile);

            var passedTestCaseStdOutNode = resultsXml.XPathSelectElement("/testsuites/testsuite/*[@name='PassTest11']/system-out");
            Assert.Contains("{2010CAE3-7BC0-4841-A5A3-7D5F947BB9FB}", passedTestCaseStdOutNode.Value);
            Assert.Contains("{998AC9EC-7429-42CD-AD55-72037E7AF3D8}", passedTestCaseStdOutNode.Value);

            var testSuiteStdOutNode = resultsXml.XPathSelectElement("/testsuites/testsuite/system-out");
            Assert.Contains("{2010CAE3-7BC0-4841-A5A3-7D5F947BB9FB}", testSuiteStdOutNode.Value);
            Assert.Contains("{998AC9EC-7429-42CD-AD55-72037E7AF3D8}", testSuiteStdOutNode.Value);
            Assert.Contains("{EEEE1DA6-6296-4486-BDA5-A50A19672F0F}", testSuiteStdOutNode.Value);
            Assert.Contains("{C33FF4B5-75E1-4882-B968-DF9608BFE7C2}", testSuiteStdOutNode.Value);

            TestCaseShouldHaveAttachmentInStandardOut(resultsXml, "PassTest11");
        }

        [TestMethod]
        public void StoreConsoleOutput_Default_ContainsConsoleErr()
        {
            var loggerArgs = "junit;LogFilePath=output-default-test-results.xml";

            var resultsFile = DotnetTestFixture
                                .Create()
                                .WithBuild()
                                .Execute("JUnit.Xml.TestLogger.NetCore.Tests", loggerArgs, collectCoverage: false, "output-default-test-results.xml");

            XDocument resultsXml = XDocument.Load(resultsFile);

            var failedTestCaseStdErrNode = resultsXml.XPathSelectElement("/testsuites/testsuite/*[@name='FailTest11']/system-err");
            Assert.Contains("{D46DFA10-EEDD-49E5-804D-FE43051331A7}", failedTestCaseStdErrNode.Value);
            Assert.Contains("{33F5FD22-6F40-499D-98E4-481D87FAEAA1}", failedTestCaseStdErrNode.Value);

            var testSuiteStdErrNode = resultsXml.XPathSelectElement("/testsuites/testsuite/system-err");
            Assert.Contains("{D46DFA10-EEDD-49E5-804D-FE43051331A7}", testSuiteStdErrNode.Value);
            Assert.Contains("{33F5FD22-6F40-499D-98E4-481D87FAEAA1}", testSuiteStdErrNode.Value);
        }

        [TestMethod]
        public void StoreConsoleOutput_True_ContainsConsoleOut()
        {
            var loggerArgs = "junit;LogFilePath=output-true-test-results.xml;StoreConsoleOutput=true";

            var resultsFile = DotnetTestFixture
                                .Create()
                                .WithBuild()
                                .Execute("JUnit.Xml.TestLogger.NetCore.Tests", loggerArgs, collectCoverage: false, "output-true-test-results.xml");

            XDocument resultsXml = XDocument.Load(resultsFile);

            var passedTestCaseStdOutNode = resultsXml.XPathSelectElement("/testsuites/testsuite/*[@name='PassTest11']/system-out");
            Assert.Contains("{2010CAE3-7BC0-4841-A5A3-7D5F947BB9FB}", passedTestCaseStdOutNode.Value);
            Assert.Contains("{998AC9EC-7429-42CD-AD55-72037E7AF3D8}", passedTestCaseStdOutNode.Value);

            var testSuiteStdOutNode = resultsXml.XPathSelectElement("/testsuites/testsuite/system-out");
            Assert.Contains("{2010CAE3-7BC0-4841-A5A3-7D5F947BB9FB}", testSuiteStdOutNode.Value);
            Assert.Contains("{998AC9EC-7429-42CD-AD55-72037E7AF3D8}", testSuiteStdOutNode.Value);
            Assert.Contains("{EEEE1DA6-6296-4486-BDA5-A50A19672F0F}", testSuiteStdOutNode.Value);
            Assert.Contains("{C33FF4B5-75E1-4882-B968-DF9608BFE7C2}", testSuiteStdOutNode.Value);

            TestCaseShouldHaveAttachmentInStandardOut(resultsXml, "PassTest11");
        }

        [TestMethod]
        public void StoreConsoleOutput_True_ContainsConsoleErr()
        {
            var loggerArgs = "junit;LogFilePath=output-true-test-results.xml;StoreConsoleOutput=true";

            var resultsFile = DotnetTestFixture
                                .Create()
                                .WithBuild()
                                .Execute("JUnit.Xml.TestLogger.NetCore.Tests", loggerArgs, collectCoverage: false, "output-true-test-results.xml");

            XDocument resultsXml = XDocument.Load(resultsFile);

            var failedTestCaseStdErrNode = resultsXml.XPathSelectElement("/testsuites/testsuite/*[@name='FailTest11']/system-err");
            Assert.Contains("{D46DFA10-EEDD-49E5-804D-FE43051331A7}", failedTestCaseStdErrNode.Value);
            Assert.Contains("{33F5FD22-6F40-499D-98E4-481D87FAEAA1}", failedTestCaseStdErrNode.Value);

            var testSuiteStdErrNode = resultsXml.XPathSelectElement("/testsuites/testsuite/system-err");
            Assert.Contains("{D46DFA10-EEDD-49E5-804D-FE43051331A7}", testSuiteStdErrNode.Value);
            Assert.Contains("{33F5FD22-6F40-499D-98E4-481D87FAEAA1}", testSuiteStdErrNode.Value);
        }

        [TestMethod]
        public void StoreConsoleOutput_False_DoesNotContainConsoleOut()
        {
            var loggerArgs = "junit;LogFilePath=output-false-test-results.xml;StoreConsoleOutput=false";

            var resultsFile = DotnetTestFixture
                                .Create()
                                .WithBuild()
                                .Execute("JUnit.Xml.TestLogger.NetCore.Tests", loggerArgs, collectCoverage: false, "output-false-test-results.xml");

            XDocument resultsXml = XDocument.Load(resultsFile);

            var passedTestCaseStdOutNode = resultsXml.XPathSelectElement("/testsuites/testsuite/*[@name='PassTest11']/system-out");
            Assert.DoesNotContain("{2010CAE3-7BC0-4841-A5A3-7D5F947BB9FB}", passedTestCaseStdOutNode.Value);
            Assert.DoesNotContain("{998AC9EC-7429-42CD-AD55-72037E7AF3D8}", passedTestCaseStdOutNode.Value);

            var testSuiteStdOutNode = resultsXml.XPathSelectElement("/testsuites/testsuite/system-out");
            Assert.IsTrue(testSuiteStdOutNode.Value.Equals(string.Empty));

            TestCaseShouldHaveAttachmentInStandardOut(resultsXml, "PassTest11");
        }

        [TestMethod]
        public void StoreConsoleOutput_False_DoesNotContainConsoleErr()
        {
            var loggerArgs = "junit;LogFilePath=output-false-test-results.xml;StoreConsoleOutput=false";

            var resultsFile = DotnetTestFixture
                                .Create()
                                .WithBuild()
                                .Execute("JUnit.Xml.TestLogger.NetCore.Tests", loggerArgs, collectCoverage: false, "output-false-test-results.xml");

            XDocument resultsXml = XDocument.Load(resultsFile);

            var failedTestCaseStdErrNode = resultsXml.XPathSelectElement("/testsuites/testsuite/*[@name='FailTest11']/system-err");
            Assert.IsNull(failedTestCaseStdErrNode);

            var testSuiteStdErrNode = resultsXml.XPathSelectElement("/testsuites/testsuite/system-err");
            Assert.IsTrue(testSuiteStdErrNode.Value.Equals(string.Empty));
        }

        [TestMethod]
        public void StoreConsoleOutput_TestSuite_ContainsConsoleOutOnlyForTestSuite()
        {
            var loggerArgs = "junit;LogFilePath=output-testsuite-test-results.xml;StoreConsoleOutput=testsuite";

            var resultsFile = DotnetTestFixture
                                .Create()
                                .WithBuild()
                                .Execute("JUnit.Xml.TestLogger.NetCore.Tests", loggerArgs, collectCoverage: false, "output-testsuite-test-results.xml");

            XDocument resultsXml = XDocument.Load(resultsFile);

            var passedTestCaseStdOutNode = resultsXml.XPathSelectElement("/testsuites/testsuite/*[@name='PassTest11']/system-out");
            Assert.DoesNotContain("{2010CAE3-7BC0-4841-A5A3-7D5F947BB9FB}", passedTestCaseStdOutNode.Value);
            Assert.DoesNotContain("{998AC9EC-7429-42CD-AD55-72037E7AF3D8}", passedTestCaseStdOutNode.Value);

            var testSuiteStdOutNode = resultsXml.XPathSelectElement("/testsuites/testsuite/system-out");
            Assert.Contains("{2010CAE3-7BC0-4841-A5A3-7D5F947BB9FB}", testSuiteStdOutNode.Value);
            Assert.Contains("{998AC9EC-7429-42CD-AD55-72037E7AF3D8}", testSuiteStdOutNode.Value);
            Assert.Contains("{EEEE1DA6-6296-4486-BDA5-A50A19672F0F}", testSuiteStdOutNode.Value);
            Assert.Contains("{C33FF4B5-75E1-4882-B968-DF9608BFE7C2}", testSuiteStdOutNode.Value);

            TestCaseShouldHaveAttachmentInStandardOut(resultsXml, "PassTest11");
        }

        [TestMethod]
        public void StoreConsoleOutput_TestSuite_ContainsConsoleErrOnlyForTestSuite()
        {
            var loggerArgs = "junit;LogFilePath=output-testsuite-test-results.xml;StoreConsoleOutput=testsuite";

            var resultsFile = DotnetTestFixture
                                .Create()
                                .WithBuild()
                                .Execute("JUnit.Xml.TestLogger.NetCore.Tests", loggerArgs, collectCoverage: false, "output-testsuite-test-results.xml");

            XDocument resultsXml = XDocument.Load(resultsFile);

            var failedTestCaseStdErrNode = resultsXml.XPathSelectElement("/testsuites/testsuite/*[@name='FailTest11']/system-err");
            Assert.IsNull(failedTestCaseStdErrNode);

            var testSuiteStdErrNode = resultsXml.XPathSelectElement("/testsuites/testsuite/system-err");
            Assert.Contains("{D46DFA10-EEDD-49E5-804D-FE43051331A7}", testSuiteStdErrNode.Value);
            Assert.Contains("{33F5FD22-6F40-499D-98E4-481D87FAEAA1}", testSuiteStdErrNode.Value);
        }

        [TestMethod]
        public void StoreConsoleOutput_TestCase_ContainsConsoleOutOnlyForTestCase()
        {
            var loggerArgs = "junit;LogFilePath=output-testcase-test-results.xml;StoreConsoleOutput=testcase";

            var resultsFile = DotnetTestFixture
                                .Create()
                                .WithBuild()
                                .Execute("JUnit.Xml.TestLogger.NetCore.Tests", loggerArgs, collectCoverage: false, "output-testcase-test-results.xml");

            XDocument resultsXml = XDocument.Load(resultsFile);

            var passedTestCaseStdOutNode = resultsXml.XPathSelectElement("/testsuites/testsuite/*[@name='PassTest11']/system-out");
            Assert.Contains("{2010CAE3-7BC0-4841-A5A3-7D5F947BB9FB}", passedTestCaseStdOutNode.Value);
            Assert.Contains("{998AC9EC-7429-42CD-AD55-72037E7AF3D8}", passedTestCaseStdOutNode.Value);

            var testSuiteStdOutNode = resultsXml.XPathSelectElement("/testsuites/testsuite/system-out");
            Assert.IsTrue(testSuiteStdOutNode.Value.Equals(string.Empty));

            TestCaseShouldHaveAttachmentInStandardOut(resultsXml, "PassTest11");
        }

        [TestMethod]
        public void StoreConsoleOutput_TestCase_ContainsConsoleErrOnlyForTestCase()
        {
            var loggerArgs = "junit;LogFilePath=output-testcase-test-results.xml;StoreConsoleOutput=testcase";

            var resultsFile = DotnetTestFixture
                                .Create()
                                .WithBuild()
                                .Execute("JUnit.Xml.TestLogger.NetCore.Tests", loggerArgs, collectCoverage: false, "output-testcase-test-results.xml");

            XDocument resultsXml = XDocument.Load(resultsFile);

            var failedTestCaseStdErrNode = resultsXml.XPathSelectElement("/testsuites/testsuite/*[@name='FailTest11']/system-err");
            Assert.Contains("{D46DFA10-EEDD-49E5-804D-FE43051331A7}", failedTestCaseStdErrNode.Value);
            Assert.Contains("{33F5FD22-6F40-499D-98E4-481D87FAEAA1}", failedTestCaseStdErrNode.Value);

            var testSuiteStdErrNode = resultsXml.XPathSelectElement("/testsuites/testsuite/system-err");
            Assert.IsTrue(testSuiteStdErrNode.Value.Equals(string.Empty));
        }

        private static void TestCaseShouldHaveAttachmentInStandardOut(XDocument resultsXml, string testcaseName)
        {
            var node = resultsXml.XPathSelectElement($"/testsuites/testsuite/*[@name='{testcaseName}']/system-out");

            Assert.Contains("[[ATTACHMENT|", node.Value);
        }
    }
}
