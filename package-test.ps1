Write-Host "Running package test..."
dotnet pack
dotnet build test/JUnit.Xml.TestLogger.AcceptanceTests/JUnit.Xml.TestLogger.AcceptanceTests.csproj
dotnet test --no-build test/assets/JUnit.Xml.PackageTest/JUnit.Xml.PackageTest.csproj --logger:"junit;LogFilePath=TestResults/test-result.xml;FailureBodyFormat=Verbose;MethodFormat=Class"

Write-Host "-------------------"

Write-Host "Test result content"
Get-Content test/assets/JUnit.Xml.PackageTest/TestResults/test-result.xml
