param(
    [Parameter(Mandatory)][string]$AssetsFile,
    [Parameter(Mandatory)][string]$TargetFramework,
    [Parameter(Mandatory)][string]$RuntimeIdentifier,
    [Parameter(Mandatory)][string]$WindowsSdkPackageVersion,
    [Parameter(Mandatory)][string]$OutputDirectory
)

$ErrorActionPreference = 'Stop'
$repository = [IO.Path]::GetFullPath((Join-Path $PSScriptRoot '..'))
$assets = Get-Content -LiteralPath $AssetsFile -Raw | ConvertFrom-Json -AsHashtable
$packageFolders = @($assets.packageFolders.Keys)
$framework = $assets.project.frameworks[$TargetFramework]
if (!$framework) { throw "Framework $TargetFramework is missing from restored assets." }
$OutputDirectory = [IO.Path]::GetFullPath($OutputDirectory)
[IO.Directory]::CreateDirectory($OutputDirectory) | Out-Null
$licenseFiles = [Collections.Generic.List[string]]::new()

function Get-PackageDirectory([string]$RelativePath) {
    foreach ($folder in $packageFolders) {
        $candidate = Join-Path $folder $RelativePath
        if (Test-Path -LiteralPath $candidate -PathType Container) { return $candidate }
    }
    throw "Restored package not found: $RelativePath"
}

function Copy-License([string]$Source, [string]$Name) {
    if (!(Test-Path -LiteralPath $Source -PathType Leaf)) { throw "Required license is missing: $Source" }
    Copy-Item -LiteralPath $Source -Destination (Join-Path $OutputDirectory $Name) -Force
    $licenseFiles.Add((Join-Path $OutputDirectory $Name))
}

Copy-License (Join-Path $repository 'LICENSE') 'LICENSE.txt'
Copy-License (Join-Path $repository 'THIRD-PARTY-NOTICES.md') 'THIRD-PARTY-NOTICES.md'
Copy-License (Join-Path $repository 'PRIVACY.md') 'PRIVACY.md'
Copy-License (Join-Path $repository 'licenses\Windows-SDK-LICENSE.txt') 'Windows-SDK-LICENSE.txt'
Copy-License (Join-Path $repository 'licenses\README.md') 'SOURCES.md'
# These notices also ship outside the repository, with different relative paths.
$noticesPath = Join-Path $OutputDirectory 'THIRD-PARTY-NOTICES.md'
$notices = [IO.File]::ReadAllText($noticesPath).Replace('(LICENSE)', '(LICENSE.txt)').Replace('(licenses/README.md)', '(SOURCES.md)')
[IO.File]::WriteAllText($noticesPath, $notices, [Text.UTF8Encoding]::new($false))

$inventory = [Collections.Generic.List[string]]::new()
$inventory.Add('# Packaged component licenses')
$inventory.Add('')
$inventory.Add('Generated from the restored package versions for this build.')
$inventory.Add('The extension and ported PowerToys code use the MIT license in LICENSE.txt.')
$inventory.Add('')
foreach ($name in @('Microsoft.CommandPalette.Extensions', 'Microsoft.Windows.CsWinRT', 'Shmuelie.WinRTServer')) {
    $licenseName = switch ($name) {
        'Microsoft.CommandPalette.Extensions' { 'CmdPal-LICENSE.txt' }
        'Microsoft.Windows.CsWinRT' { 'CsWinRT-LICENSE.txt' }
        'Shmuelie.WinRTServer' { 'WinRTServer-LICENSE.txt' }
    }
    $matchingPackages = @($assets.libraries.GetEnumerator() | Where-Object {
        $_.Key.StartsWith($name + '/', [StringComparison]::OrdinalIgnoreCase)
    })
    if ($matchingPackages.Count -ne 1) { throw "Cannot resolve a unique restored version of $name." }
    $package = $matchingPackages[0]
    $directory = Get-PackageDirectory $package.Value.path
    [xml]$metadata = Get-Content -LiteralPath (Join-Path $directory ($name.ToLowerInvariant() + '.nuspec')) -Raw
    if ($name -eq 'Microsoft.CommandPalette.Extensions') {
        if ($metadata.package.metadata.license.InnerText -ne 'MIT') { throw 'Review the changed CmdPal SDK license.' }
        # The SDK declares MIT and is part of the same Microsoft PowerToys project.
        Copy-License (Join-Path $repository 'licenses\PowerToys-MIT.txt') $licenseName
    }
    else {
        Copy-License (Join-Path $directory 'LICENSE') $licenseName
    }
    if ($name -eq 'Microsoft.Windows.CsWinRT') {
        Copy-License (Join-Path $directory 'NOTICE.txt') 'CsWinRT-NOTICE.txt'
        $inventory.Add("- $($package.Key): MIT; $licenseName and CsWinRT-NOTICE.txt")
    }
    else {
        $inventory.Add("- $($package.Key): MIT; $licenseName")
    }
}

$runtimeName = "Microsoft.NETCore.App.Runtime.$RuntimeIdentifier"
$runtime = @($framework.downloadDependencies | Where-Object name -EQ $runtimeName)
if ($runtime.Count -ne 1 -or $runtime[0].version -notmatch '^\[([^,]+),\s*\1\]$') {
    throw "Cannot resolve the exact $runtimeName version from restored assets."
}
$runtimeVersion = $Matches[1]
$runtimeDirectory = Get-PackageDirectory ($runtimeName.ToLowerInvariant() + '/' + $runtimeVersion)
Copy-License (Join-Path $runtimeDirectory 'LICENSE.TXT') 'NET-Runtime-LICENSE.txt'
Copy-License (Join-Path $runtimeDirectory 'THIRD-PARTY-NOTICES.TXT') 'NET-Runtime-THIRD-PARTY-NOTICES.txt'
$inventory.Add("- ${runtimeName}/${runtimeVersion}: NET-Runtime-LICENSE.txt and NET-Runtime-THIRD-PARTY-NOTICES.txt")
$inventory.Add("- Microsoft.Windows.SDK.NET.Ref/${WindowsSdkPackageVersion}: Windows-SDK-LICENSE.txt; https://aka.ms/WinSDKLicenseURL")
$inventory.Add('')
$inventory.Add('The runtime notices cover third-party components included with the self-contained .NET runtime.')
$inventory.Add('Build tools and test packages are not distributed as extension dependencies.')
[IO.File]::WriteAllLines((Join-Path $OutputDirectory 'COMPONENTS.md'), $inventory, [Text.UTF8Encoding]::new($false))
$licenseFiles.Add((Join-Path $OutputDirectory 'COMPONENTS.md'))
[IO.File]::WriteAllLines((Join-Path $OutputDirectory 'files.list'), $licenseFiles, [Text.UTF8Encoding]::new($false))
