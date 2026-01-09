# NUnit Test Logger

NUnit xml report extension for [Visual Studio Test Platform](https://github.com/microsoft/vstest).
Supports MTP v2 and VSTest classic runners. Please use v7.x for MTP v1 runners.

[![NuGet Downloads](https://img.shields.io/nuget/dt/NunitXml.TestLogger)](https://www.nuget.org/packages/NunitXml.TestLogger/)

## Packages

| Logger | Stable Package                                                                                                          | Pre-release Package                                                                                                                                         |
| ------ | ----------------------------------------------------------------------------------------------------------------------- | ----------------------------------------------------------------------------------------------------------------------------------------------------------- |
| NUnit  | [![NuGet](https://img.shields.io/nuget/v/NUnitXml.TestLogger.svg)](https://www.nuget.org/packages/NUnitXml.TestLogger/) | [![MyGet Pre Release](https://img.shields.io/myget/spekt/vpre/nunitxml.testlogger.svg)](https://www.myget.org/feed/spekt/package/nuget/NunitXml.TestLogger) |

If you're looking for `xunit` or `junit`, please see <https://github.com/spekt/testlogger>.

## Usage

NUnit logger can generate xml reports in the [NUnit v3 format](https://docs.nunit.org/articles/nunit/technical-notes/usage/Test-Result-XML-Format.html).

1. Add a reference to the [NUnit Logger](https://www.nuget.org/packages/NUnitXml.TestLogger) NuGet package in test project
2. Use the following command line in tests

```
> dotnet test --logger:nunit
```

3. Test results are generated in the `TestResults` directory relative to the `test.csproj`

A path for the report file can be specified as follows:

```
> dotnet test --logger:"nunit;LogFilePath=test-result.xml"
```

`test-result.xml` will be generated in the same directory as `test.csproj`.

**Note:** the arguments to `--logger` should be in quotes since `;` is treated as a command delimiter in shell.

### Microsoft.Testing.Platform (MTP) Support

The logger also supports Microsoft.Testing.Platform (MTP) with the following command line options:

```
> dotnet test -- --report-spekt-nunit --report-spekt-nunit-filename test-result.xml
```

The `--report-spekt-nunit` option can also accept configuration arguments:

```
> dotnet test -- --report-spekt-nunit "key1=value1;key2=value2"
```

### Configuration

#### Available options

| Option name                 | Purpose                                                                    | Documentation     |
| --------------------------- | -------------------------------------------------------------------------- | ----------------- |
| LogFileName\*               | Customize test result file name with `{assembly}` or `{framework}` tokens  | See [config-wiki] |
| LogFilePath\*               | Test result file full path                                                 | See [config-wiki] |
| UseRelativeAttachmentPath\* | Use attachment paths relative to test result file. Boolean. Default: false | See [config-wiki] |

\*All common options to the logger is documented [in the wiki][config-wiki]. E.g.
token expansion for `{assembly}` or `{framework}` in result file.

[config-wiki]: https://github.com/spekt/testlogger/wiki/Logger-Configuration

**NUnit test framework settings**

- If your scenario requires test case properties like `Description` in the xml, please enable internal properties for the nunit adapter:

`dotnet test --logger:nunit -- NUnit.ShowInternalProperties=true`

- NUnit test adapter also provides a mechanism to emit test result xml from the NUnit engine. You may use following commandline for the same:

`dotnet test --logger:nunit -- NUnit.TestOutputXml=<foldername relative to test binary directory>`

## License

MIT
