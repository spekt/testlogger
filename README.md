# .NET test loggers
Junit, NUnit and Xunit logger extensions for [Visual Studio Test Platform](https://github.com/microsoft/vstest). Connect your test reports to Circle CI, Gitlab and others from `dotnet test`.

[![Build Status](https://github.com/spekt/testlogger/workflows/.NET/badge.svg)](https://github.com/spekt/testlogger/actions?query=workflow%3A.NET)
[![Code coverage](https://codecov.io/gh/spekt/testlogger/branch/master/graph/badge.svg)](https://codecov.io/gh/spekt/testlogger)

## Packages

| Logger | Stable Package | Pre-release Package | Usage | Release Notes |
| ------ | -------------- | ------------------- | ------------- | ------------- |
| JUnit | [![NuGet](https://img.shields.io/nuget/v/JUnitXml.TestLogger.svg)](https://www.nuget.org/packages/JUnitXml.TestLogger/) | [![MyGet Pre Release](https://img.shields.io/myget/spekt/vpre/junitxml.testlogger.svg)](https://www.myget.org/feed/spekt/package/nuget/JunitXml.TestLogger) | [README](src/JUnit.Xml.Package/README.md) | [CHANGELOG](src/JUnit.Xml.Package/CHANGELOG.md) |
| NUnit | [![NuGet](https://img.shields.io/nuget/v/NUnitXml.TestLogger.svg)](https://www.nuget.org/packages/NUnitXml.TestLogger/) | [![MyGet Pre Release](https://img.shields.io/myget/spekt/vpre/nunitxml.testlogger.svg)](https://www.myget.org/feed/spekt/package/nuget/NunitXml.TestLogger) | [README](src/NUnit.Xml.Package/README.md) | [CHANGELOG](src/NUnit.Xml.Package/CHANGELOG.md) |
| Xunit | [![NuGet](https://img.shields.io/nuget/v/XunitXml.TestLogger.svg)](https://www.nuget.org/packages/XunitXml.TestLogger/) | [![MyGet Pre Release](https://img.shields.io/myget/spekt/vpre/xunitxml.testlogger.svg)](https://www.myget.org/feed/spekt/package/nuget/XunitXml.TestLogger) | [README](src/Xunit.Xml.Package/README.md) | [CHANGELOG](src/Xunit.Xml.Package/CHANGELOG.md) |

## Contribution Guide

Run the `build.ps1` or `build.sh` scripts in Windows or Linux to build and run the tests.

This repo always requires the latest LTS release of dotnet and dotnet runtime 3.1 for development.

If acceptance tests are failing, try running this command to see detailed output:

```sh
# Run from root of repo
> dotnet test --no-build --logger:"json;LogFilePath=test-results.json" test/assets/Json.TestLogger.MSTest.NetCore.Tests/Json.TestLogger.MSTest.NetCore.Tests.csproj
```

### CI/CD Integration Tests

- [Sample Gitlab pipeline](https://gitlab.com/codito/sample-junit-test/-/pipelines) picks the latest pre-release Junit logger in Gitlab CI.
- [Sample Circle CI pipeline](https://app.circleci.com/pipelines/circleci/GA6zAWSpZy4izQcaCFyvJP/CRQra9Zg7NR4ZYZk2vsmEX) runs on every PR via webhook.

## Wiki

See <https://github.com/spekt/testlogger/wiki> for [Logger Configuration](https://github.com/spekt/testlogger/wiki/Logger-Configuration) and troubleshooting.
  
## License
MIT
