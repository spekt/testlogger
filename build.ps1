$locals = dotnet nuget locals all -l
ForEach ($local in $($locals -split "`r`n"))
{
    $local = $local.Substring($local.IndexOf(":") + 2)
    "Deleting from $local"
    dotnet nuget delete Json.TestLogger 3.0.0-dev --force-english-output --non-interactive -s $local
}

Remove-Item .\test\package\bin\ -Recurse
Remove-Item .\test\package\obj\ -Recurse

dotnet pack

if ($?) {
    dotnet test test/TestLogger.UnitTests/TestLogger.UnitTests.csproj -p:CollectCoverage=true -p:CoverletOutputFormat=opencover
}
if ($?) {
    # On Windows, if this step is failing to build because the test logger is missing, or
    # is using an out of date logger, you may need to manually copy the test logger
    # nuget package into one of the sources such as C:\Program Files\dotnet\library-packs
    # which msbuild is using for local nuget packages.
    dotnet test test/TestLogger.AcceptanceTests/TestLogger.AcceptanceTests.csproj
}
