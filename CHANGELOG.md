# Changelog

Starting with v4.0.x, all the testloggers use a unified changelog for
simplicity. For v3.x changelogs, refer to the end of this document.

## Unreleased (v4.1.x)

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
