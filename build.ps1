$locals = dotnet nuget locals all -l
ForEach ($local in $($locals -split "`r`n"))
{
    $local = $local.Substring($local.IndexOf(":") + 2)
    "Deleting from $local"
    dotnet nuget delete Json.TestLogger 4.0.0-dev --force-english-output --non-interactive -s $local
}

Remove-Item .\test\package\bin\ -Recurse
Remove-Item .\test\package\obj\ -Recurse

dotnet pack

if ($?) {
    dotnet test test/TestLogger.UnitTests/TestLogger.UnitTests.csproj -p:CollectCoverage=true -p:CoverletOutputFormat=opencover
}
if ($?) {
    dotnet test test/TestLogger.AcceptanceTests/TestLogger.AcceptanceTests.csproj
}
