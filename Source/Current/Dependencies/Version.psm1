# Version Module
# ===============
#
# Provides functions for versioning of Visual Studio projects and source files.
# The version logic is as follows:
#
# Versions are stored in text files which can be read and written with Get/Set-VersionFile methods.
#
# Versions are updated/calculated via the Update-Version method, which calculates the version as follows:
#    Major and minor parts of the version are static, i.e. controlled by the developer.
#    Build is set to the UTC year and month in ""yyMM"" format.
#    Revision is set to the UTC day * 1000 plus a three digit incrementing number.
#
# This version format has the following benefits:
# 1) It is predictable, the date can be devised from any released module.
# 2) Storing the version in text files allows them to be redistributed and used to update dependency references,
#    or other simple file based logic used to scan and identify source/runtime versions.
# 3) Up to 999 builds per day are supported which should be enough for any environnment, even continous build.
# 4) Can be called during a build script for fully automate incremental versioning on each test or release build.
# 5) Unique versions solve limitations caused when updating code without changing the version number.
# 6) When the update and reference updates are automated there is no longer any risk changing the version.,
# 7) GAC deployment is no longer necessary just to consistently version dependencies, because binding redirects
#    are easy to maintain when they are updated automatically via this script.
#

function Update-Version ([Version]$Version)
{
    $date = (Get-Date).ToUniversalTime()
    $newBuild = $date.ToString("yyMM")
    $dayRevisionMin = $date.Day * 1000
	$dayRevisionMax = $dayRevisionMin + 999
    if ($Version.Revision -ge $dayRevisionMin -and $Version.Revision -le $dayRevisionMax)
	{
		# Increment revision in range
		$newRevision = $Version.Revision + 1 
	} else 
	{ 
		# Use minimum revision when out of range
		$newRevision = $dayRevisionMin
	}
    New-Object -TypeName System.Version -ArgumentList $Version.Major, $Version.Minor, $newBuild, $newRevision
}

function Get-VersionFile ([String]$File)
{
    Write-Host ("Reading version file " + $File)
    $versionString = [System.IO.File]::ReadAllText($File).Trim()
    New-Object -TypeName System.Version -ArgumentList $versionString
}

function Set-VersionFile ([String]$File, [Version]$Version)
{
    Write-Host ("Writing version file " + $File)
    [System.IO.File]::WriteAllText($File, $Version.ToString())
}

function Set-VersionInAssemblyInfo ([String]$File, [Version]$Version)
{
    Write-Host ("Setting version in assembly info file " + $File)
    $contents = [System.IO.File]::ReadAllText($File)
    $contents = [RegEx]::Replace($contents, "(AssemblyVersion\(\"")(?:\d+\.\d+\.\d+\.\d+)(\""\))", ("`${1}" + $Version.ToString() + "`${2}"))
    $contents = [RegEx]::Replace($contents, "(AssemblyFileVersion\(\"")(?:\d+\.\d+\.\d+\.\d+)(\""\))", ("`${1}" + $Version.ToString() + "`${2}"))
    [System.IO.File]::WriteAllText($File, $contents)
}

function Set-VersionInWixGlobal ([String]$File, [Version]$Version)
{
    Write-Host ("Setting version in WIX global file " + $File)
    $contents = [System.IO.File]::ReadAllText($File)
    $contents = [RegEx]::Replace($contents, "(\<\?define\s*ProductVersion\s*=\s*\"")(?:\d+\.\d+\.\d+\.\d+)(\""\s*\?\>)", ("`${1}" + $Version.ToString() + "`${2}"))
    [System.IO.File]::WriteAllText($File, $contents)
}

function Set-VersionInAssemblyReference ([String]$File, [String]$AssemblyName, [Version]$Version)
{
    Write-Host ("Setting version in assembly references of " + $File)
    $contents = [System.IO.File]::ReadAllText($File)
    $contents = [RegEx]::Replace($contents, "([\"">](?:\S+,\s+){0,1}" + $AssemblyName + ",\s+Version=)(?:\d+\.\d+\.\d+\.\d+)([,\""<])", ("`${1}" + $Version.ToString() + "`${2}"))
    [System.IO.File]::WriteAllText($File, $contents)
}

function Set-VersionInBindingRedirect ([String]$File, [String]$AssemblyName, [Version]$Version)
{
    Write-Host ("Setting version in binding redirects of " + $File)
    $contents = [System.IO.File]::ReadAllText($File)
    $oldVersionMax = New-Object -TypeName "System.Version" -ArgumentList $Version.Major, $Version.Minor, $Version.Build, ($Version.Revision - 1)
    $pattern = "(<dependentAssembly>[\s\S]*?<assemblyIdentity\s+name=\""" + $AssemblyName + "\""[\s\S]+?/>[\s\S]*?<bindingRedirect\s+oldVersion=\""\d+\.\d+\.\d+\.\d+-)(?:\d+\.\d+\.\d+\.\d+)(\""\s+newVersion=\"")(?:\d+\.\d+\.\d+\.\d+)(\""[\s\S]*?/>)"
    $contents = [RegEx]::Replace($contents, $pattern, ("`${1}" + $oldVersionMax.ToString() + "`${2}" + $Version.ToString() + "`${3}"))
    [System.IO.File]::WriteAllText($File, $contents)
}

function Set-VersionInCppResourceFile ([String]$File, [Version]$Version)
{
    Write-Host ("Setting version in C++ resource file " + $File)
    $contents = [System.IO.File]::ReadAllText($File)
    $contents = [RegEx]::Replace($contents, "(FILEVERSION\s+)(?:\d+\,\d+\,\d+\,\d+)", ("`${1}" + $Version.Major.ToString() + "," + $Version.Minor.ToString() + "," + $Version.Build.ToString() + "," + $Version.Revision.ToString()))
    $contents = [RegEx]::Replace($contents, "(PRODUCTVERSION\s+)(?:\d+\,\d+\,\d+\,\d+)", ("`${1}" + $Version.Major.ToString() + "," + $Version.Minor.ToString() + "," + $Version.Build.ToString() + "," + $Version.Revision.ToString()))
    $contents = [RegEx]::Replace($contents, "(VALUE\s+\""FileVersion\"",\s*\"")(?:\d+\.\d+\.\d+\.\d+)(\"")", ("`${1}" + $Version.ToString() + "`${2}"))
    $contents = [RegEx]::Replace($contents, "(VALUE\s+\""ProductVersion\"",\s*\"")(?:\d+\.\d+\.\d+\.\d+)(\"")", ("`${1}" + $Version.ToString() + "`${2}"))
    [System.IO.File]::WriteAllText($File, $contents)
}

function Set-VersionInCppModuleDefinitionFile ([String]$File, [Version]$Version)
{
    Write-Host ("Setting version in C++ module definition file " + $File)
    $contents = [System.IO.File]::ReadAllText($File)
    $contents = [RegEx]::Replace($contents, "(VERSION\s+)(?:\d+\,\d+\,\d+\,\d+)", ("`${1}" + $Version.ToString(2)))
    [System.IO.File]::WriteAllText($File, $contents)
}
