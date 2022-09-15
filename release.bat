@echo off
dotnet build src/Limbo.Umbraco.TwentyThree --configuration Release /t:rebuild /t:pack -p:PackageOutputPath=../../releases/nuget