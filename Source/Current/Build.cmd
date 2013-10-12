@echo off
setlocal

rem *** Syntax
if "%~1" == "" (
	echo Configuration name parameter is required.
	exit /b 1
)
set ConfigurationName=%~1

echo %ConfigurationName% Build
echo.
echo Performs a %ConfigurationName% build then copies the output
echo into a solution local "%ConfigurationName%" subdirectory.
echo.

echo.
echo Delete old files...
if exist "%~dp0%ConfigurationName%" (
	rmdir "%~dp0%ConfigurationName%" /s /q
	if %errorlevel% gtr 1 goto error
)

echo.
echo Compiling %ConfigurationName% build...
msbuild "%~dp0Code for .NET.sln" /p:Configuration=%ConfigurationName%
if %errorlevel% neq 0 goto error

echo.
echo Copying components...
robocopy "%~dp0CodeForDotNet\bin\%ConfigurationName%" "%~dp0%ConfigurationName%\Components" *.dll *.xml /xf *CodeAnalysisLog.xml
if %errorlevel% gtr 7 goto error
robocopy "%~dp0CodeForDotNet.Full\bin\%ConfigurationName%" "%~dp0%ConfigurationName%\Components" *.dll *.xml /xf *CodeAnalysisLog.xml
if %errorlevel% gtr 7 goto error

echo.
echo Copying documentation...
robocopy "%~dp0Documentation" "%~dp0%ConfigurationName%\Documentation"
if %errorlevel% gtr 7 goto error

echo.
echo Copying version references...
md "%~dp0%ConfigurationName%\Version"
if %errorlevel% neq 0 goto error
copy "%~dp0Version.txt" "%~dp0%ConfigurationName%\Version\CodeForDotNet.Version.txt"
if %errorlevel% neq 0 goto error
robocopy "%~dp0Dependencies" "%~dp0%ConfigurationName%\Version" *Version.txt
if %errorlevel% gtr 7 goto error

echo.
echo %ConfigurationName% build successful.
endlocal
exit /b 0

:error
set result=%errorlevel%
echo.
echo Error %result%
exit /b %result%