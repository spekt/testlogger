Write-Host "Running package test..."
dotnet pack
dotnet test test/assets/JUnit.Xml.PackageTest/JUnit.Xml.PackageTest.csproj --logger:"junit;LogFilePath=TestResults/test-result.xml;FailureBodyFormat=Verbose;MethodFormat=Class"

Write-Host "-------------------"

Write-Host "Test result content"
Get-Content test/assets/JUnit.Xml.PackageTest/TestResults/test-result.xml
