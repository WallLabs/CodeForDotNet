@echo off
setlocal
echo Update Dependencies
echo ===================
echo.
echo Updates dependencies with the current checked-in build version.
echo.

echo.
echo Initializing Visual Studio environment...
call "%~dp0Variables.cmd"
if %errorlevel% neq 0 goto Error

echo.
echo Updating build outputs from source control...
if exist "%~dp0Temp" rmdir "%~dp0Temp" /s /q
if %errorlevel% neq 0 goto Error
git clone --depth=1 --branch=master git://github.com/WallLabs/CodeForWindows "%~dp0Temp\CodeForWindows"
if %errorlevel% neq 0 goto Error

echo.
echo Copying dependencies...
robocopy "%~dp0Temp\CodeForWindows\Build\v1\Release\PowerShell" "%~dp0PowerShell" /s /purge
if %errorlevel% gtr 7 goto Error
robocopy "%~dp0Temp\CodeForWindows\Build\v1\Release\Scripts" "%~dp0." Variables.cmd
if %errorlevel% gtr 7 goto Error
robocopy "%~dp0Temp\CodeForWindows\Build\v1\Release\Documentation" "%~dp0." "*Release Notes*"
if %errorlevel% gtr 7 goto Error
robocopy "%~dp0Temp\CodeForWindows\Build\v1\Release\Version" "%~dp0." *.Version.txt
if %errorlevel% gtr 7 goto Error
attrib "%~dp0*" -r /s
if %errorlevel% neq 0 goto Error

echo.
echo Calling version script to update references...
echo..
call "%~dp0..\Version.cmd"
if %errorlevel% neq 0 goto Error

echo.
echo Clean temporary files...
rmdir "%~dp0Temp" /s /q
if %errorlevel% gtr 1 goto Error

echo.
echo Update successful.
endlocal
exit /b 0

:Error
echo Error %errorlevel%!
endlocal
exit /b 1