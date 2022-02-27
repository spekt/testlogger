dotnet nuget locals global-packages --clear
Remove-Item .\test\package\bin\ -Recurse
Remove-Item .\test\package\obj\ -Recurse

dotnet pack
if ($?) {
    dotnet test test/TestLogger.UnitTests/TestLogger.UnitTests.csproj -p:CollectCoverage=true -p:CoverletOutputFormat=opencover
}
if ($?) {
    dotnet test test/TestLogger.AcceptanceTests/TestLogger.AcceptanceTests.csproj
}
