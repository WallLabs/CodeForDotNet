<#
	.Synopsis
	Development Setup Script

	.Description
	Prepares a developer workstation to test, debug or edit this solution.
#>

#region Globals

# Options.
Set-StrictMode -Version Latest;   # Proactively avoid errors and inconsistency.
$error.Clear();                   # Clear any errors from previous script runs.
$ErrorActionPreference = "Stop";  # All unhandled errors stop program.
$WarningPreference = "Stop";      # All warnings stop program.

#endregion

#region Main script.

# Display banner.
Write-Host 'Development Setup';
Write-Host '=================';
Write-Host 'Installs and configures the system ready for development.';

# Set web proxy default credential (in case necessary).
$proxy = [System.Net.WebRequest]::GetSystemWebProxy();
$proxy.Credentials = [System.Net.CredentialCache]::DefaultNetworkCredentials;
[System.Net.WebRequest]::DefaultWebProxy = $proxy;

# Ensure default .NET development certificates are installed.
Write-Host "Installing default .NET development certificates...";
dotnet.exe dev-certs https --trust 2>&1;
Write-Host;

# Exit successfully.
Write-Host "Development setup completed successfully.";
Exit 0;

#endregion
