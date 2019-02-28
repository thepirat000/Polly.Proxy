@echo off
cls

del "PollyProxy\bin\debug\*.symbols.nupkg"

nuget push "PollyProxy\bin\debug\*.nupkg" -source %1
