@echo off
echo Update Dependencies
echo ===================
echo.
echo Updates dependencies with the current checked-in build version.
echo.

echo.
echo Updating build outputs from source control...
tf get "%~dp0..\..\..\..\CodeForWindows\Build\v1" /recursive /noprompt
if %errorlevel% gtr 1 goto error
tfpt scorch "%~dp0..\..\..\..\CodeForWindows\Build\v1" /recursive /diff /noprompt
if %errorlevel% gtr 1 goto error

echo.
echo Checking out dependencies...
tf checkout "%~dp0" /recursive
if %errorlevel% gtr 1 goto error

echo.
echo Copying dependencies...
robocopy "%~dp0..\..\..\..\CodeForWindows\Build\v1\Release\Documentation" "%~dp0." "*Release Notes*"
if %errorlevel% gtr 7 goto error
robocopy "%~dp0..\..\..\..\CodeForWindows\Build\v1\Release\PowerShell" "%~dp0." Version.psm1
if %errorlevel% gtr 7 goto error
attrib "%~dp0*" -r /s
if %errorlevel% neq 0 goto error

echo.
echo Calling version script to update references...
echo..
call "%~dp0..\Version.cmd"
if %errorlevel% neq 0 goto error

echo.
echo Update successful.
exit /b 0

:error
set result=%errorlevel%
echo.
echo Error %result%
exit /b %result%