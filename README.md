# Test Logger
Common test logger abstractions for [Visual Studio Test Platform](https://gtihub.com/microsoft/vstest).

[![Build Status](https://github.com/spekt/testlogger/workflows/.NET/badge.svg)](https://github.com/spekt/testlogger/actions?query=workflow%3A.NET)
[![Code coverage](https://codecov.io/gh/spekt/testlogger/branch/master/graph/badge.svg)](https://codecov.io/gh/spekt/testlogger)

## Packages

| Logger | Stable Package |
| ------ | -------------- |
| Spekt.TestLogger | https://github.com/spekt/testlogger/packages/580072 |

## Contribution Guide

Run the `build.ps1` or `build.sh` scripts in Windows or Linux to build and run the tests.

This repo always requires the latest LTS release of dotnet and dotnet runtime 3.1 for development.

If acceptance tests are failing, try running this command to see detailed output:

```sh
# Run from root of repo
> dotnet test --no-build --logger:"json;LogFilePath=test-results.json" test/assets/Json.TestLogger.MSTest.NetCore.Tests/Json.TestLogger.MSTest.NetCore.Tests.csproj
```

## License
MIT
