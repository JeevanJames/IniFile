@echo off
setlocal

if ("%1"=="") goto error

set VERSION=%1
echo %VERSION%
dotnet nuget push .\src\IniFile\bin\Release\IniFile.NET.%VERSION%.nupkg -s https://api.nuget.org/v3/index.json
goto done

:error
echo Specify the version number

:done
