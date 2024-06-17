# Xunit Test Logger
Xunit logger extensions for [Visual Studio Test Platform](https://github.com/microsoft/vstest).

[![NuGet Downloads](https://img.shields.io/nuget/dt/XunitXml.TestLogger)](https://www.nuget.org/packages/XunitXml.TestLogger/)

## Packages
| Logger | Stable Package | Pre-release Package |
| ------ | -------------- | ------------------- |
| Xunit | [![NuGet](https://img.shields.io/nuget/v/XunitXml.TestLogger.svg)](https://www.nuget.org/packages/XunitXml.TestLogger/) | [![MyGet Pre Release](https://img.shields.io/myget/spekt/vpre/xunitxml.testlogger.svg)](https://www.myget.org/feed/spekt/package/nuget/XunitXml.TestLogger) |

If you're looking for `nunit` or `junit`, please see <https://github.com/spekt/testlogger>.

## Usage
Xunit logger can generate xml reports in the xunit v2 format (https://xunit.net/docs/format-xml-v2).

1. Add a reference to the [Xunit Logger](https://www.nuget.org/packages/XunitXml.TestLogger) nuget package in test project
2. Use the following command line in tests
```
> dotnet test --logger:xunit
```
3. Test results are generated in the `TestResults` directory relative to the `test.csproj`

A path for the report file can be specified as follows:
```
> dotnet test --logger:"xunit;LogFilePath=test_result.xml"
```

`test_result.xml` will be generated in the same directory as `test.csproj`.

**Note:** the arguments to `--logger` should be in quotes since `;` is treated as a command delimiter in shell.

All common options to the logger is documented [in the wiki][config-wiki].

[config-wiki]: https://github.com/spekt/testlogger/wiki/Logger-Configuration

## License
MIT
