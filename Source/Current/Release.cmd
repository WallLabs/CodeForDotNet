@echo off
echo Release
echo =======
echo.
echo Performs a full build of all configurations then copies the output
echo to the central build directory for check-in and use by other
echo components or release.

echo.
echo Update source (and delete extra files)...
tf undo "%~dp0..\..\Build\v4.51" /recursive /noprompt
if %errorlevel% gtr 1 goto error
tf scorch "%~dp0..\..\Build\v4.51" /recursive /diff /noprompt
if %errorlevel% gtr 1 goto error
tf get "%~dp0" /recursive /noprompt
if %errorlevel% gtr 1 goto error
tf scorch "%~dp0" /recursive /diff /noprompt
if %errorlevel% gtr 1 goto error

echo.
echo Versioning...
call "%~dp0Version.cmd"
if %errorlevel% neq 0 goto error

echo.
echo Building...
call "%~dp0Build.cmd" Debug
if %errorlevel% neq 0 goto error
call "%~dp0Build.cmd" Release
if %errorlevel% neq 0 goto error

echo.
echo Delete old build directory so that old or renamed items are cleaned
if not exist "%~dp0..\..\Build\v4.51" goto TargetClean
rmdir "%~dp0..\..\Build\v4.51" /s /q
if %errorlevel% neq 0 goto error
:TargetClean

echo.
echo Copying output to build directory...
robocopy "%~dp0Debug" "%~dp0..\..\Build\v4.51\Debug" /s
if %errorlevel% gtr 7 goto error
robocopy "%~dp0Release" "%~dp0..\..\Build\v4.51\Release" /s
if %errorlevel% gtr 7 goto error

echo.
echo Adding any new files in build directory...
tf add "%~dp0..\..\Build\" /recursive /noprompt /noignore
if %errorlevel% gtr 1 goto error

echo.
echo Build all successful.
exit /b 0

:error
set result=%errorlevel%
echo.
echo Error %result%
exit /b %result%