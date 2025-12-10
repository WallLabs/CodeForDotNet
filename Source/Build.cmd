@echo off
setlocal
echo Build
echo =====
echo.
echo Performs a build of one configuration then copies the output
echo to a solution local build directory, ready for release.
echo.

rem * Check syntax
if "%~1" == "" (
	echo Configuration name parameter is required.
	endlocal
	exit /b 1
)
set ConfigurationName=%~1

echo.
echo %ConfigurationName% Build...

echo.
echo Initializing Visual Studio environment...
call "%~dp0Dependencies\Variables.cmd"
if %errorlevel% neq 0 goto Error

echo.
echo Delete old files...
if exist "%~dp0Temp\Build\%ConfigurationName%" (
	rmdir "%~dp0Temp\Build\%ConfigurationName%" /s /q
	if %errorlevel% gtr 1 goto Error
)

echo.
echo Compiling %ConfigurationName% build...
msbuild "%~dp0Code for .NET.slnx" /p:Configuration=%ConfigurationName%
if %errorlevel% neq 0 goto Error

echo.
echo Copying components...
robocopy "%~dp0CodeForDotNet\bin\%ConfigurationName%\net10.0" "%~dp0Temp\Build\%ConfigurationName%\Components" CodeForDotNet.*
if %errorlevel% gtr 7 goto Error
robocopy "%~dp0CodeForDotNet.Data.Sql\bin\%ConfigurationName%\net10.0" "%~dp0Temp\Build\%ConfigurationName%\Components" CodeForDotNet.Data.Sql.*
if %errorlevel% gtr 7 goto Error
robocopy "%~dp0CodeForDotNet.UI\bin\%ConfigurationName%\net10.0" "%~dp0Temp\Build\%ConfigurationName%\Components" CodeForDotNet.UI.*
if %errorlevel% gtr 7 goto Error
robocopy "%~dp0CodeForDotNet.Windows\bin\%ConfigurationName%\net10.0-windows10.0.26100.0" "%~dp0Temp\Build\%ConfigurationName%\Components" CodeForDotNet.Windows.*
if %errorlevel% gtr 7 goto Error
robocopy "%~dp0CodeForDotNet.WindowsUniversal\bin\%ConfigurationName%\net10.0-windows10.0.26100.0" "%~dp0Temp\Build\%ConfigurationName%\Components" CodeForDotNet.WindowsUniversal.*
if %errorlevel% gtr 7 goto Error
robocopy "%~dp0CodeForDotNet.WindowsUniversal\bin\%ConfigurationName%\net10.0-windows10.0.26100.0\CodeForDotNet.WindowsUniversal" "%~dp0Temp\Build\%ConfigurationName%\Components\CodeForDotNet.WindowsUniversal" /s
if %errorlevel% gtr 7 goto Error

echo.
echo Copying documentation...
robocopy "%~dp0Documentation" "%~dp0Temp\Build\%ConfigurationName%\Documentation"
if %errorlevel% gtr 7 goto Error

echo.
echo Copying version reference...
md "%~dp0Temp\Build\%ConfigurationName%\Version"
if %errorlevel% neq 0 goto Error
copy "%~dp0Version.txt" "%~dp0Temp\Build\%ConfigurationName%\Version\CodeForDotNet.Version.txt"
if %errorlevel% neq 0 goto Error

echo.
echo %ConfigurationName% build successful.
endlocal
exit /b 0

:Error
echo Error %errorlevel%!
endlocal
exit /b 1
