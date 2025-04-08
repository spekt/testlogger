# Jenkins Recommendation

**TODO** If you are a Jenkins user who is willing to help, please see Issue [#38](https://github.com/spekt/junit.testlogger/issues/38)

Test results should be collected in a post / always stage, so jenkins can properly track failed tests.

## JUnit Logger

Jenkins needs [`junit`](https://plugins.jenkins.io/junit/) plugin.

`_.runsettings`
```xml
<?xml version="1.0" encoding="utf-8" ?>
<RunSettings>
  <LoggerRunSettings>
    <Loggers>
      <Logger friendlyName="console" enabled="True">
        <Configuration>
          <Verbosity>minimal</Verbosity>
        </Configuration>
      </Logger>
      <Logger friendlyName="junit" enabled="True">
        <Configuration>
          <LogFileName>{assembly}.{framework}.junit.xml</LogFileName>
        </Configuration>
      </Logger>
    </Loggers>
  </LoggerRunSettings>
</RunSettings>
```
`Jenkinsfile`
```groovy
junit allowEmptyResults: false, testResults: '**/TestResults/*.junit.xml'
```
## NUnit Logger

Jenkins needs [`nunit`](https://plugins.jenkins.io/nunit/) plugin.

`_.runsettings`
```xml
<?xml version="1.0" encoding="utf-8" ?>
<RunSettings>
  <LoggerRunSettings>
    <Loggers>
      <Logger friendlyName="console" enabled="True">
        <Configuration>
          <Verbosity>minimal</Verbosity>
        </Configuration>
      </Logger>
      <Logger friendlyName="nunit" enabled="True">
        <Configuration>
          <LogFileName>{assembly}.{framework}.nunit.xml</LogFileName>
        </Configuration>
      </Logger>
    </Loggers>
  </LoggerRunSettings>
</RunSettings>
```
`Jenkinsfile`
```groovy
nunit testResultsPattern: '**/TestResults/*.nunit.xml'
```
