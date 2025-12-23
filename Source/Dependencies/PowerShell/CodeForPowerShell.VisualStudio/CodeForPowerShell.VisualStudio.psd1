@{
	RootModule = ''
	ModuleVersion = '1.0.2512.23003'
	GUID = '5042A6BA-039F-4C87-97C9-7B12CCA1D532'
	Author = 'Wall Labs'
	CompanyName = 'Wall Labs'
	Copyright = 'Copyright Wall Labs. All rights reserved.'
	Description = 'PowerShell module to assist with Visual Studio development.'
	PowerShellVersion = '5.1'
	NestedModules = @('CodeForPowerShell.VisualStudio.Version.psm1')
	CmdletsToExport = @(
        'Get-VersionFile',
        'Set-VersionFile',
        'Set-VersionInAppXManifest',
        'Set-VersionInCppResourceFile',
        'Set-VersionInPowerShellManufest',
        'Set-VersionInPowerShellScript',
        'Set-VersionInXmlProject'
    )
}
