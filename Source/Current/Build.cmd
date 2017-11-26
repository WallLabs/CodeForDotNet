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
if exist "%~dp0%ConfigurationName%" (
	rmdir "%~dp0%ConfigurationName%" /s /q
	if %errorlevel% gtr 1 goto Error
)

echo.
echo Compiling %ConfigurationName% build...
msbuild "%~dp0Code for .NET.sln" /p:Configuration=%ConfigurationName%
if %errorlevel% neq 0 goto Error

echo.
echo Copying components...
robocopy "%~dp0CodeForDotNet\bin\%ConfigurationName%" "%~dp0%ConfigurationName%\Components" *.dll *.pdb *.xml /xf *CodeAnalysisLog.xml
if %errorlevel% gtr 7 goto Error
robocopy "%~dp0CodeForDotNet.Full\bin\%ConfigurationName%" "%~dp0%ConfigurationName%\Components" *.dll *.pdb *.xml /xf *CodeAnalysisLog.xml
if %errorlevel% gtr 7 goto Error
robocopy "%~dp0CodeForDotNet.Windows\bin\%ConfigurationName%" "%~dp0%ConfigurationName%\Components" *.dll *.pdb *.xml /xf *CodeAnalysisLog.xml
if %errorlevel% gtr 7 goto Error
robocopy "%~dp0CodeForDotNet.WindowsUniversal\bin\%ConfigurationName%" "%~dp0%ConfigurationName%\Components" *.dll *.pdb *.xml *.pri /xf *CodeAnalysisLog.xml
if %errorlevel% gtr 7 goto Error

echo.
echo Copying documentation...
robocopy "%~dp0Documentation" "%~dp0%ConfigurationName%\Documentation"
if %errorlevel% gtr 7 goto Error

echo.
echo Copying version references...
md "%~dp0%ConfigurationName%\Version"
if %errorlevel% neq 0 goto Error
copy "%~dp0Version.txt" "%~dp0%ConfigurationName%\Version\CodeForDotNet.Version.txt"
if %errorlevel% neq 0 goto Error
robocopy "%~dp0Dependencies" "%~dp0%ConfigurationName%\Version" *Version.txt
if %errorlevel% gtr 7 goto Error

echo.
echo %ConfigurationName% build successful.
endlocal
exit /b 0

:Error
echo Error %errorlevel%!
endlocal
exit /b 1