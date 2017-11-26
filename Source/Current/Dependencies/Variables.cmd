@echo off
rem ==========================================================================
rem Variables.cmd
rem --------------------------------------------------------------------------
rem Initializes Visual Studio environment variables to the most recent
rem version of Visual Studio installed. Since 2017 the VSWhere tool and
rem VSDevCmd.bat replaces the VS#COMNTOOLS environment variable and
rem VSVars32.bat.
rem --------------------------------------------------------------------------

rem Do nothing when already setup
if "%VSCMD_VER%" neq "" goto Done

rem Use the new VSWhere when present
if not exist "%ProgramFiles(x86)%\Microsoft Visual Studio\Installer\VSWhere.exe" goto OldMethod

rem * Call VSWhere to locate Visual Studio
for /f "usebackq tokens=1* delims=" %%i in (`"%ProgramFiles(x86)%\Microsoft Visual Studio\Installer\VSWhere.exe" -version 15.0^ -property installationPath`) do set VSInstallDir=%%i
if %errorlevel% neq 0 goto Error

rem Use old method when no 2017 or later installations found
if "%VSInstallDir%" neq "" goto NewMethod

rem Call old setup script
:OldMethod
echo * Calling VSVars32.bat to initialize environment...
call "%VS140ComnTools%VSVars32.bat"
if %errorlevel% neq 0 goto Error

goto Done

rem Call new setup script
:NewMethod
echo * Calling VSDevCmd.bat to initialize environment...
set VSCMD_START_DIR=%CD%
call "%VSInstallDir%\Common7\Tools\VSDevCmd.bat"
if %errorlevel% neq 0 goto Error

rem Successful
:Done
exit /b 0

rem Error handler
:Error
echo Error %errorlevel%!
exit /b 1