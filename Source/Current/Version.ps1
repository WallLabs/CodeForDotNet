Write-Output "Version"
Write-Output "======="
Write-Output "Increments the version number stored in the Version.txt file,"
Write-Output "then applies it to all relevant source files in the solution."
Write-Output "Build is set to the UTC year and month in ""yyMM"" format."
Write-Output "Revision is set to the UTC day * 1000 plus a three digit incrementing number." 
Write-Output ""

# Error handler
trap
{
    Write-Error $_
    exit 1
}

# Initialize
$scriptDirectory =  [System.IO.Path]::GetDirectoryName($MyInvocation.MyCommand.Definition)
Import-Module ($scriptDirectory + "\Dependencies\Version.psm1")

# Load current version file
$versionFilePath = $scriptDirectory + "\Version.txt"
$version = Get-VersionFile -File $versionFilePath
Write-Host ("Old Version: " + $version.ToString())

# Update version and save
$newVersion = Update-Version -Version $version
Write-Host ("New Version: " + $newVersion.ToString())
Set-VersionFile -File $versionFilePath -Version $newVersion

# Set version in Visual Studio project and source files...
Set-VersionInAssemblyInfo -File ($scriptDirectory + "\CodeForDotNet\Properties\AssemblyInfo.cs") -Version $newVersion
Set-VersionInAssemblyInfo -File ($scriptDirectory + "\CodeForDotNet.Full\Properties\AssemblyInfo.cs") -Version $newVersion
Set-VersionInAssemblyInfo -File ($scriptDirectory + "\CodeForDotNet.Tests\Properties\AssemblyInfo.cs") -Version $newVersion
Set-VersionInAssemblyInfo -File ($scriptDirectory + "\CodeForDotNet.Windows\Properties\AssemblyInfo.cs") -Version $newVersion

# Exit successful
exit 0
