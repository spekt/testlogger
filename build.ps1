dotnet pack
if ($?) {
    dotnet test test/TestLogger.UnitTests/TestLogger.UnitTests.csproj
}
if ($?) {
    dotnet test test/TestLogger.AcceptanceTests/TestLogger.AcceptanceTests.csproj
}
