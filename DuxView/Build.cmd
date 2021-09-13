@echo off
MSBuild DuxView.csproj /t:Rebuild /p:Configuration=Debug
MSBuild DuxView.csproj /t:Rebuild /p:Configuration=Release