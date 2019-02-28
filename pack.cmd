@echo off
cls

del "PollyProxy\bin\debug\*.nupkg"

dotnet build "PollyProxy.sln" 

dotnet pack "PollyProxy/"
