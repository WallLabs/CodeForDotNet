@echo off
setlocal
echo Update Dependencies
echo ===================
echo.
echo Updates dependencies with the current checked-in build version.

echo.
echo Close Visual Studio now to avoid errors with locked files.
pause

echo.
echo Initializing Visual Studio environment...
call "%~dp0Variables.cmd"
if %errorlevel% neq 0 goto Error

echo.
echo Updating build outputs from source control...
if exist "%~dp0Temp" (
    rmdir "%~dp0Temp" /s /q
    if %errorlevel% gtr 1 goto Error
)
git clone --branch=master https://github.com/WallLabs/CodeForPowerShell.git "%~dp0Temp\CodeForPowerShell"
if %errorlevel% neq 0 goto Error
git clone --branch=master https://github.com/WallLabs/CodeForWindows.git "%~dp0Temp\CodeForWindows"
if %errorlevel% neq 0 goto Error

echo.
echo Removing any read-only attributes.
cd "%~dp0"

echo.
echo Copying dependencies...

echo * CodeForWindows...
robocopy "%~dp0Temp\CodeForWindows\Build\Visual Studio" "%~dp0." Variables.cmd
if %errorlevel% gtr 7 goto Error
rem TODO: Update EditorConfg in master repository. Remove or re-enable CodeMaid or other tool to fill gap of proactive formatting.
rem robocopy "%~dp0Temp\CodeForWindows\Build\Visual Studio" "%~dp0.." .editorconfig CodeMaid.config
rem if %errorlevel% gtr 7 goto Error
copy "%~dp0Temp\CodeForWindows\Build\Documentation\Release Notes.md" "%~dp0Documentation\Code for Windows Release Notes.md" /y
if %errorlevel% neq 0 goto Error
robocopy "%~dp0Temp\CodeForWindows\Build\Version" "%~dp0Version" *.Version.txt
if %errorlevel% gtr 7 goto Error

echo * CodeForPowerShell...
robocopy "%~dp0Temp\CodeForPowerShell\Build\Modules\CodeForPowerShell.VisualStudio" "%~dp0PowerShell\CodeForPowerShell.VisualStudio" /s /purge
if %errorlevel% gtr 7 goto Error
copy "%~dp0Temp\CodeForPowerShell\Build\Documentation\Release Notes.md" "%~dp0Documentation\Code for PowerShell Release Notes.md" /y
if %errorlevel% neq 0 goto Error
robocopy "%~dp0Temp\CodeForPowerShell\Build\Version" "%~dp0Version" *.Version.txt
if %errorlevel% gtr 7 goto Error
attrib "%~dp0*" -r /s
if %errorlevel% neq 0 goto Error

echo.
echo Calling version script to update references...
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
