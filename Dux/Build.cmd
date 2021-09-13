@echo off
MSBuild Dux.csproj /t:Rebuild /p:Configuration=Debug
MSBuild Dux.csproj /t:Rebuild /p:Configuration=Release