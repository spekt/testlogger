# CLAUDE.md

This file provides guidance to Claude Code (claude.ai/code) when working with code in this repository.

## Project Overview

This repository contains .NET test loggers for Visual Studio Test Platform (VSTest) and Microsoft.Testing.Platform (MTP) that generate JUnit, NUnit, and Xunit XML reports. The project enables integration with CI/CD systems like Circle CI and Gitlab.

## Development Commands

### Building and Testing

- **Full build and test**: `./build.sh` (Linux/macOS) or `.\build.ps1` (Windows)
- **Package only**: `dotnet pack`
- **Run specific unit test**: `dotnet test test/TestLogger.UnitTests/TestLogger.UnitTests.csproj`
- **Run specific acceptance test**: `dotnet test test/TestLogger.AcceptanceTests/TestLogger.AcceptanceTests.csproj`

### Debugging Acceptance Tests

If acceptance tests are failing, run the test asset with detailed output. Example below:

```sh
DOTNET_SYSTEM_GLOBALIZATION_INVARIANT=1 # prefix below commands with this on Linux

# Reproduce VSTest logger issue
dotnet test --logger:"json;LogFilePath=test-results-vstest.json" test/assets/Json.TestLogger.MSTest.NetCore.Tests/Json.TestLogger.MSTest.NetCore.Tests.csproj

# Reproduce MTP logger issue
dotnet test test/assets/Json.TestLogger.MSTest.NetCore.Tests/Json.TestLogger.MSTest.NetCore.Tests.csproj -p:IsMTP=true -- --report-spekt-json --report-spekt-json test-results-mtp.json
```

## Architecture

### Core Components

- **TestLogger**: Core library with interfaces and workflows in `src/TestLogger/Core/` supporting both VSTest and MTP
- **Format-specific loggers**: JUnit, NUnit, and Xunit implementations in their respective directories
- **Package projects**: Distributable NuGet packages in `src/*.Package/`

### Key Interfaces and Workflows

- `ITestRunBuilder`: Builds test run information
- `ITestResultSerializer`: Serializes test results to different formats
- `TestRunMessageWorkflow`: Handles test run messages
- `TestRunCompleteWorkflow`: Handles test run completion

### Testing Strategy

- **Unit tests**: Core functionality testing in `test/*UnitTests/`
- **Acceptance tests**: End-to-end testing with test projects in `test/*AcceptanceTests/`
- **Package tests**: NuGet package validation in `test/TestLogger.PackageTests/`

## Configuration

### Build Properties

- Version is managed via `version.txt` and CI workflow
- Uses .NET 9.0 for development, supports netstandard2.0+ for packages
- Code analysis with StyleCop enforced via `Directory.Build.props`

### Code Style

- C# latest language version
- XML documentation required for public APIs
- StyleCop analyzers enabled with custom ruleset
- Warnings treated as errors

## Project Structure

```
src/
├── TestLogger/               # Core library
├── JUnit.Xml.TestLogger/     # JUnit format implementation
├── NUnit.Xml.TestLogger/     # NUnit format implementation
├── Xunit.Xml.TestLogger/     # Xunit format implementation
└── *.Package/               # NuGet package projects

test/
├── *UnitTests/              # Unit tests for each logger
├── *AcceptanceTests/        # End-to-end tests
├── TestLogger.UnitTests/     # Core library tests
└── assets/                  # Test project assets
```

## CI/CD

- Uses GitHub Actions with multi-OS testing (Ubuntu, Windows)
- Automated versioning via release-please action
- Coverage reporting to Codecov
- Pre-release packages published to MyGet

## Development Guidelines

- Respect DRY, SOLID and similar clean code practices.
- Code must be usable, correct and performant.
- Add documentation for all public methods.
- Add unit tests for every change. Follow the existing test conventions in the project.
- Do not add unnecessary comments.
