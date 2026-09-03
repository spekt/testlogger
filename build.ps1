dotnet pack

if ($?) {
    dotnet run --project test/TestLogger.VSTest.UnitTests/TestLogger.VSTest.UnitTests.csproj -- --coverlet --coverlet-output-format cobertura --coverlet-include "[Spekt*]*"
    dotnet run --project test/TestLogger.Mtp.UnitTests/TestLogger.Mtp.UnitTests.csproj -- --coverlet --coverlet-output-format cobertura --coverlet-include "[Spekt*]*"
    dotnet run --project test/NUnit.Xml.TestLogger.UnitTests/NUnit.Xml.TestLogger.UnitTests.csproj -- --coverlet --coverlet-output-format cobertura --coverlet-include "[Spekt*]*" --coverlet-include "[Microsoft.VisualStudio.TestPlatform.Extension*]*"
    dotnet run --project test/JUnit.Xml.TestLogger.UnitTests/JUnit.Xml.TestLogger.UnitTests.csproj -- --coverlet --coverlet-output-format cobertura --coverlet-include "[Spekt*]*" --coverlet-include "[Microsoft.VisualStudio.TestPlatform.Extension*]*"
}
if ($?) {
    dotnet tool restore
    dotnet reportgenerator -reports:"test/TestLogger.VSTest.UnitTests/bin/Debug/net10.0/TestResults/coverage.cobertura.xml;test/TestLogger.Mtp.UnitTests/bin/Debug/net10.0/TestResults/coverage.cobertura.xml;test/NUnit.Xml.TestLogger.UnitTests/bin/Debug/net10.0/TestResults/coverage.cobertura.xml;test/JUnit.Xml.TestLogger.UnitTests/bin/Debug/net10.0/TestResults/coverage.cobertura.xml" -targetdir:coveragereport -reporttypes:"TextSummary;Html"
    Get-Content coveragereport/Summary.txt
}
if ($?) {
    dotnet test test/TestLogger.AcceptanceTests/TestLogger.AcceptanceTests.csproj
    dotnet test test/TestLogger.PackageTests/TestLogger.PackageTests.csproj
}
if ($?) {
    dotnet test test/Xunit.Xml.TestLogger.AcceptanceTests/Xunit.Xml.TestLogger.AcceptanceTests.csproj
    dotnet test test/NUnit.Xml.TestLogger.AcceptanceTests/NUnit.Xml.TestLogger.AcceptanceTests.csproj
    dotnet test test/JUnit.Xml.TestLogger.AcceptanceTests/JUnit.Xml.TestLogger.AcceptanceTests.csproj
}
