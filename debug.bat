@echo off
dotnet build src/Limbo.Umbraco.TwentyThree --configuration Debug /t:rebuild /t:pack -p:PackageOutputPath=c:/nuget