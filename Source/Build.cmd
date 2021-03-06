@echo off
setlocal
echo Build
echo =======
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
msbuild "%~dp0Code for .NET.sln" /p:Configuration=%ConfigurationName%
if %errorlevel% neq 0 goto Error

echo.
echo Copying components...
robocopy "%~dp0CodeForDotNet\bin\%ConfigurationName%\netstandard2.1" "%~dp0Temp\Build\%ConfigurationName%\Components" CodeForDotNet.*
if %errorlevel% gtr 7 goto Error
robocopy "%~dp0CodeForDotNet.Legacy\bin\%ConfigurationName%\netstandard2.0" "%~dp0Temp\Build\%ConfigurationName%\Components" CodeForDotNet.Legacy.*
if %errorlevel% gtr 7 goto Error
robocopy "%~dp0CodeForDotNet.Data.Sql\bin\%ConfigurationName%\netstandard2.1" "%~dp0Temp\Build\%ConfigurationName%\Components" CodeForDotNet.Data.Sql.*
if %errorlevel% gtr 7 goto Error
robocopy "%~dp0CodeForDotNet.Data.Sql.Legacy\bin\%ConfigurationName%\netstandard2.0" "%~dp0Temp\Build\%ConfigurationName%\Components" CodeForDotNet.Data.Sql.Legacy.*
if %errorlevel% gtr 7 goto Error
robocopy "%~dp0CodeForDotNet.UI\bin\%ConfigurationName%\netstandard2.1" "%~dp0Temp\Build\%ConfigurationName%\Components" CodeForDotNet.UI.*
if %errorlevel% gtr 7 goto Error
robocopy "%~dp0CodeForDotNet.UI.Legacy\bin\%ConfigurationName%\netstandard2.0" "%~dp0Temp\Build\%ConfigurationName%\Components" CodeForDotNet.UI.Legacy.*
if %errorlevel% gtr 7 goto Error
robocopy "%~dp0CodeForDotNet.Windows\bin\%ConfigurationName%\netcoreapp3.1" "%~dp0Temp\Build\%ConfigurationName%\Components" CodeForDotNet.Windows.* /xf *CodeAnalysisLog.xml /xf *.lastcodeanalysissucceeded
if %errorlevel% gtr 7 goto Error
robocopy "%~dp0CodeForDotNet.WindowsUniversal\bin\%ConfigurationName%" "%~dp0Temp\Build\%ConfigurationName%\Components" CodeForDotNet.WindowsUniversal.* *.winmd /xf *.xr.xml /xf *CodeAnalysisLog.xml /xf *.lastcodeanalysissucceeded
if %errorlevel% gtr 7 goto Error
robocopy "%~dp0CodeForDotNet.WindowsUniversal\bin\%ConfigurationName%" "%~dp0Temp\Build\%ConfigurationName%\Components\CodeForDotNet.WindowsUniversal" CodeForDotNet.WindowsUniversal.xr.xml *.xbf /s /xf *CodeAnalysisLog.xml /xf *.lastcodeanalysissucceeded
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
echo.
echo Note: Visual Studio must be closed before running this script to prevent build errors from locked files and caches.
endlocal
exit /b 1
