# Jenkins Recommendation

**TODO** If you are a Jenkins user who is willing to help, please see Issue [#38](https://github.com/spekt/junit.testlogger/issues/38)


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
