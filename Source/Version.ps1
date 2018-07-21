# ==============================================================================
# Version Update Script
# ------------------------------------------------------------------------------
# Updates the solution version text file with the date based build number then
# applies the new version to all project files in the solution.
# ==============================================================================


# ==============================================================================
# Globals
# ------------------------------------------------------------------------------

# Options
Set-StrictMode -Version Latest    # Proactively avoid errors and inconsistency
$error.Clear()                    # Clear any errors from previous script runs
$ErrorActionPreference = "Stop"   # All unhandled errors stop program
$WarningPreference = "Stop"       # All warnings stop program


# ==============================================================================
# Modules
# ------------------------------------------------------------------------------

# Initialize module paths
$env:PSModulePath = [Environment]::GetEnvironmentVariable("PSModulePath", "Machine");
$env:PSModulePath = "$env:PSModulePath;$PSScriptRoot\Dependencies\PowerShell";

# Import modules
Import-Module CodeForWindows.PowerShell.VisualStudio;


# ==============================================================================
# Main Program
# ------------------------------------------------------------------------------

# Display banner
Write-Output "Version Update Script";
Write-Output "=====================";
Write-Output "Increments the version number stored in the Version.txt file,";
Write-Output "then applies it to all relevant source files in the solution.";
Write-Output "Build is set to the UTC year and month in ""yyMM"" format.";
Write-Output "Revision is set to the UTC day * 1000 plus a three digit incrementing number.";
Write-Output "";

# Load current version file
$versionFilePath = "$PSScriptRoot\Version.txt";
$version = Get-VersionFile -File $versionFilePath;
Write-Host ("Old Version: " + $version.ToString());

# Update version and save
$newVersion = Update-Version -Version $version;
Write-Host ("New Version: " + $newVersion.ToString());
Set-VersionFile -File $versionFilePath -Version $newVersion;

# Set version in Visual Studio project and source files...
Set-VersionInXmlProject -File "$PSScriptRoot\CodeForDotNet\CodeForDotNet.csproj" -Version $newVersion;
Set-VersionInAssemblyInfo -File "$PSScriptRoot\Common\AssemblyInfoCore.cs" -Version $newVersion;
Set-VersionInAppXManifest -File "$PSScriptRoot\CodeForDotNet.WindowsUniversal.Tests\Package.appxmanifest" -Version $newVersion;

# Exit successful
exit 0;