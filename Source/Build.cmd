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
robocopy "%~dp0CodeForDotNet\bin\%ConfigurationName%\netstandard2.0" "%~dp0Temp\Build\%ConfigurationName%\Components" CodeForDotNet*.dll CodeForDotNet*.pdb CodeForDotNet*.xml /xf *CodeAnalysisLog.xml
if %errorlevel% gtr 7 goto Error
robocopy "%~dp0CodeForDotNet.Full\bin\%ConfigurationName%" "%~dp0Temp\Build\%ConfigurationName%\Components" CodeForDotNet.Full*.dll CodeForDotNet.Full*.pdb CodeForDotNet.Full*.xml /xf *CodeAnalysisLog.xml
if %errorlevel% gtr 7 goto Error
robocopy "%~dp0CodeForDotNet.Windows\bin\%ConfigurationName%" "%~dp0Temp\Build\%ConfigurationName%\Components" CodeForDotNet.Windows*.dll CodeForDotNet.Windows*.pdb CodeForDotNet.Windows*.xml /xf *CodeAnalysisLog.xml
if %errorlevel% gtr 7 goto Error
robocopy "%~dp0CodeForDotNet.WindowsUniversal\bin\%ConfigurationName%" "%~dp0Temp\Build\%ConfigurationName%\Components" CodeForDotNet.WindowsUniversal*.dll CodeForDotNet.WindowsUniversal*.pdb CodeForDotNet.WindowsUniversal*.xml CodeForDotNet.WindowsUniversal*.pri *.xbf /xf *CodeAnalysisLog.xml /s
if %errorlevel% gtr 7 goto Error

echo.
echo Copying documentation...
robocopy "%~dp0Documentation" "%~dp0Temp\Build\%ConfigurationName%\Documentation"
if %errorlevel% gtr 7 goto Error

echo.
echo Copying version references...
md "%~dp0Temp\Build\%ConfigurationName%\Version"
if %errorlevel% neq 0 goto Error
copy "%~dp0Version.txt" "%~dp0Temp\Build\%ConfigurationName%\Version\CodeForDotNet.Version.txt"
if %errorlevel% neq 0 goto Error
robocopy "%~dp0Dependencies" "%~dp0Temp\Build\%ConfigurationName%\Version" *Version.txt
if %errorlevel% gtr 7 goto Error

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