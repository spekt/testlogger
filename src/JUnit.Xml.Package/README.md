# JUnit Test Logger

JUnit xml report extension for [Visual Studio Test Platform](https://github.com/microsoft/vstest).

[![NuGet Downloads](https://img.shields.io/nuget/dt/JunitXml.TestLogger)](https://www.nuget.org/packages/JunitXml.TestLogger/)

## Packages

| Logger | Stable Package                                                                                                          | Pre-release Package                                                                                                                                         |
| ------ | ----------------------------------------------------------------------------------------------------------------------- | ----------------------------------------------------------------------------------------------------------------------------------------------------------- |
| JUnit  | [![NuGet](https://img.shields.io/nuget/v/JUnitXml.TestLogger.svg)](https://www.nuget.org/packages/JUnitXml.TestLogger/) | [![MyGet Pre Release](https://img.shields.io/myget/spekt/vpre/junitxml.testlogger.svg)](https://www.myget.org/feed/spekt/package/nuget/JunitXml.TestLogger) |

If you're looking for `Nunit` or `Xunit` loggers, please see <https://github.com/spekt/testlogger>.

## Usage

The JUnit Test Logger generates xml reports in the [Ant Junit Format](https://github.com/windyroad/JUnit-Schema), which the JUnit 5 repository refers to as the de-facto standard. While the generated xml complies with that schema, it does not contain values in every case. For example, the logger currently does not log any `properties`. Please [refer to a sample file](https://github.com/spekt/testlogger/blob/master/docs/assets/TestResults.xml) to see an example. If you find that the format is missing data required by your CI/CD system, please open an issue or PR.

To use the logger, follow these steps:

1. Add a reference to the [JUnit Logger](https://www.nuget.org/packages/JUnitXml.TestLogger) nuget package in test project
2. Use the following command line in tests

   ```none
   > dotnet test --logger:junit
   ```

3. Test results are generated in the `TestResults` directory relative to the `test.csproj`

A path for the report file can be specified as follows:

```none
> dotnet test --logger:"junit;LogFilePath=test-result.xml"
```

`test-result.xml` will be generated in the same directory as `test.csproj`.

**Note:** the arguments to `--logger` should be in quotes since `;` is treated as a command delimiter in shell.

All common options to the logger are documented [in the wiki][config-wiki]. E.g.
token expansion for `{assembly}` or `{framework}` in result file. If you are writing multiple
files to the same directory or testing multiple frameworks, these options can prevent
test logs from over-writing each other.

[config-wiki]: https://github.com/spekt/testlogger/wiki/Logger-Configuration

### Customizing Junit XML Contents

There are several options to customize how the junit xml is populated. These options exist to
provide additional control over the xml file so that the logged test results can be optimized for different CI/CD systems.

Platform Specific Recommendations:

- [GitLab CI/CD Recommendation](/docs/gitlab-recommendation.md)
- [Jenkins Recommendation](/docs/jenkins-recommendation.md)
- [CircleCI Recommendation](/docs/circleci-recommendation.md)

After the logger name, command line arguments are provided as key/value pairs with the following general format. **Note** the quotes are required, and key names are case-sensitive.

```none
> dotnet test --test-adapter-path:. --logger:"junit;key1=value1;key2=value2"
```

#### MethodFormat

This option alters the `testcase name` attribute. By default, this contains only the method. Class, will add the class to the name. Full, will add the assembly/namespace/class to the method.

We recommend this option for [GitLab](/docs/gitlab-recommendation.md) users.

##### Allowed Values

- MethodFormat=Default
- MethodFormat=Class
- MethodFormat=Full

#### FailureBodyFormat

When set to default, the body of a `failure` element will contain only the exception which is captured by vstest. Verbose will prepend the body with 'Expected X, Actual Y' similar to how it is displayed in the standard test output. 'Expected X, Actual Y' are normally only contained in the failure message. Additionally, Verbose will include standard output from the test in the failure message.

We recommend this option for [GitLab](/docs/gitlab-recommendation.md) and [CircleCI](/docs/circleci-recommendation.md) users.

##### Allowed Values

- FailureBodyFormat=Default
- FailureBodyFormat=Verbose

#### StoreConsoleOutput

You can use `StoreConsoleOutput` option to disable any `system-out` and `system-err` logs in either `testsuite` or `testcase` elements or both. By default, all console outputs are captured. Example usage:

`dotnet test --logger:"junit;StoreConsoleOutput=false"`

NOTE: test attachments are always emitted in `system-out` for tests cases even when above option is `false` or `testsuite`.

**v5.x and later behavior**

In v5.x and later, `system-out` and `system-err` logs are reported at a per `testcase` element in the report. Because each test
adapter determines to also emit the console output and errors at test suite level, there may be some duplication for the messages.

**v4.x and earlier behavior**

Prior to v5.x, console stdout and stderr was reported only in the `system-out` and `system-err` elements at `testsuite` level. This
would concatenate messages from all test results and information messages from adapter.

##### Allowed Values

- StoreConsoleOutput=true (default)
- StoreConsoleOutput=false
- StoreConsoleOutput=testsuite
- StoreConsoleOutput=testcase

## License

MIT
