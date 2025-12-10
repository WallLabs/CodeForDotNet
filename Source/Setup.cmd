@echo off
setlocal

rem Ensure PowerShell Core/7+ is installed.
winget install --id Microsoft.PowerShell --source winget
if %errorlevel% neq -1978335189 goto Error

rem Enable PowerShell execution for the current user.
pwsh.exe -Command "Set-ExecutionPolicy -Scope CurrentUser -ExecutionPolicy RemoteSigned"
if %errorlevel% neq 0 goto Error

rem Execute PowerShell script.
pwsh.exe -File "%~dp0Setup.ps1"
if %errorlevel% neq 0 goto Error

rem Exit successfully.
endlocal
exit /b 0

rem Error handler.
:Error
echo Error %errorlevel%!
endlocal
exit /b 1
