#!/usr/bin/env sh
# vi: set tw=0

dotnet pack &&\
dotnet test test/TestLogger.UnitTests/TestLogger.UnitTests.csproj -p:CollectCoverage=true -p:CoverletOutputFormat=opencover &&\
dotnet test test/NUnit.Xml.TestLogger.UnitTests/NUnit.Xml.TestLogger.UnitTests.csproj -p:CollectCoverage=true -p:CoverletOutputFormat=opencover &&\
dotnet test test/JUnit.Xml.TestLogger.UnitTests/JUnit.Xml.TestLogger.UnitTests.csproj -p:CollectCoverage=true -p:CoverletOutputFormat=opencover &&\
dotnet test test/TestLogger.AcceptanceTests/TestLogger.AcceptanceTests.csproj &&\
dotnet test test/TestLogger.PackageTests/TestLogger.PackageTests.csproj &&\
dotnet test test/Xunit.Xml.TestLogger.AcceptanceTests/Xunit.Xml.TestLogger.AcceptanceTests.csproj &&\
dotnet test test/NUnit.Xml.TestLogger.AcceptanceTests/NUnit.Xml.TestLogger.AcceptanceTests.csproj &&\
dotnet test test/JUnit.Xml.TestLogger.AcceptanceTests/JUnit.Xml.TestLogger.AcceptanceTests.csproj
