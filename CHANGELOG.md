# Changelog

Starting with v4.0.x, all the testloggers use a unified changelog for
simplicity. For v3.x changelogs, refer to the end of this document.

## [6.1.0](https://github.com/spekt/testlogger/compare/v6.0.0...v6.1.0) (2025-03-15)


### Features

* Allow console output to be stored for either/both/none of testsuite and testcase ([#164](https://github.com/spekt/testlogger/issues/164)) ([#165](https://github.com/spekt/testlogger/issues/165)) ([fc15f84](https://github.com/spekt/testlogger/commit/fc15f84e2b80630f469b5dfb54a0fc023e4c357a))
* move to netstandard2.0 for full framework support ([#168](https://github.com/spekt/testlogger/issues/168)) ([84266a6](https://github.com/spekt/testlogger/commit/84266a6f8f9dd00bc2965019326d7f1d7f35c00c))

## [6.0.0](https://github.com/spekt/testlogger/compare/v5.0.0...v6.0.0) (2025-02-13)


### ⚠ BREAKING CHANGES

* move to netstandard2.1 and drop .NET full support ([#158](https://github.com/spekt/testlogger/issues/158))
* [junit] #153 Add properties support on test case level for test logger ([#154](https://github.com/spekt/testlogger/issues/154))

### Features

* [junit] [#153](https://github.com/spekt/testlogger/issues/153) Add properties support on test case level for test logger ([#154](https://github.com/spekt/testlogger/issues/154)) ([f555322](https://github.com/spekt/testlogger/commit/f555322cdb8c593a633b9707c289957b80110fab))
* move to netstandard2.1 and drop .NET full support ([#158](https://github.com/spekt/testlogger/issues/158)) ([4a00352](https://github.com/spekt/testlogger/commit/4a0035288ccddd02551b88ef3fd68124841f2116))

## [5.0.0](https://github.com/spekt/testlogger/compare/v4.1.0...v5.0.0) (2024-12-19)


### ⚠ BREAKING CHANGES

* [junit] add testcase system-out and system-err along with attachment support. ([#136](https://github.com/spekt/testlogger/issues/136))

### Features

* [junit] add testcase system-out and system-err along with attachment support. ([#136](https://github.com/spekt/testlogger/issues/136)) ([dff18d6](https://github.com/spekt/testlogger/commit/dff18d6379009656fc622fe15e9a5f2708e72f33))
* update junit xsd to allow test case level outputs. ([#134](https://github.com/spekt/testlogger/issues/134)) ([1a37110](https://github.com/spekt/testlogger/commit/1a37110a8be5e4e20896826b2ed5db28b5dd4a06))

## [4.1.0](https://github.com/spekt/testlogger/compare/v4.0.254...v4.1.0) (2024-10-12)


### Features

* [JUnit] added StoreConsoleOutput option ([#115](https://github.com/spekt/testlogger/issues/115)) ([63fc58f](https://github.com/spekt/testlogger/commit/63fc58fecb2f48d60335b85b190e30fb7450e443))

## v4.0.254 - 2024/07/28

- Fix: [nunit logger]: standardize start and end times. See #88 and https://github.com/spekt/nunit.testlogger/issues/105.
- Fix: [nunit logger]: make attachment description optional. See #87 and https://github.com/spekt/nunit.testlogger/issues/106.
- Infra: [all loggers]: remove dependency on System.ValueTuple. See #82.
- Infra: [all loggers]: consolidate build infra, test infra and make E2E tests now run on library dependencies. Makes running tests faster and possible from the test explorers.
- Infra: [all loggers]: port various loggers to testlogger repo, add sample gitlab and circle CIs for junit.

## v3.x and earlier

- **JUnitXml.TestLogger**: See changelog on the releases page of the [JUnit Test Logger GitHub repository](https://github.com/spekt/junit.testlogger/).
- **NUnitXml.TestLogger**: See
  [CHANGELOG](https://github.com/spekt/nunit.testlogger/blob/master/CHANGELOG.md).
- **XunitXml.TestLogger**: See
  [CHANGELOG](https://github.com/spekt/xunit.testlogger/blob/master/CHANGELOG.md).
