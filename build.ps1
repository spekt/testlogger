dotnet pack
if ($?) {
    dotnet test test/TestLogger.UnitTests/TestLogger.UnitTests.csproj -p:CollectCoverage=true
}
if ($?) {
    dotnet test test/TestLogger.AcceptanceTests/TestLogger.AcceptanceTests.csproj
}
