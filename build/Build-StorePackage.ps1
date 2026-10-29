param(
    [string]$IdentityName = $env:STORE_IDENTITY_NAME,
    [string]$Publisher = $env:STORE_PUBLISHER,
    [string]$PublisherDisplayName = $env:STORE_PUBLISHER_DISPLAY_NAME,
    [string]$OutputDirectory
)

$ErrorActionPreference = 'Stop'
$repository = [IO.Path]::GetFullPath((Join-Path $PSScriptRoot '..'))
if ([string]::IsNullOrWhiteSpace($IdentityName) -or
    [string]::IsNullOrWhiteSpace($Publisher) -or
    [string]::IsNullOrWhiteSpace($PublisherDisplayName)) {
    throw 'Set STORE_IDENTITY_NAME, STORE_PUBLISHER and STORE_PUBLISHER_DISPLAY_NAME from Partner Center > Product identity.'
}
if ($Publisher -notmatch '^CN=' -or $Publisher -match 'CN=Microsoft Corporation' -or
    $PublisherDisplayName -eq 'A Lone Developer') {
    throw 'Use your own Partner Center identity, not the template publisher.'
}
if (!$OutputDirectory) { $OutputDirectory = Join-Path $repository 'src\ValueGenerator\AppPackages\Store' }
$OutputDirectory = [IO.Path]::GetFullPath($OutputDirectory)
# A fresh destination prevents a stale package from being reported as this build's output.
if ((Test-Path -LiteralPath $OutputDirectory) -and
    @(Get-ChildItem -LiteralPath $OutputDirectory -Force).Count -gt 0) {
    throw "Output directory must be empty: $OutputDirectory"
}
[IO.Directory]::CreateDirectory($OutputDirectory) | Out-Null
$sourceManifest = Join-Path $repository 'src\ValueGenerator\Package.appxmanifest'
[xml]$manifest = Get-Content -LiteralPath $sourceManifest -Raw
$version = $manifest.Package.Identity.Version
$manifest.Package.Identity.Name = $IdentityName
$manifest.Package.Identity.Publisher = $Publisher
$manifest.Package.Properties.PublisherDisplayName = $PublisherDisplayName
$manifestDirectory = Join-Path $repository 'src\ValueGenerator\obj\Store'
[IO.Directory]::CreateDirectory($manifestDirectory) | Out-Null
$storeManifest = Join-Path $manifestDirectory 'Package.appxmanifest'
$manifest.Save($storeManifest)

& dotnet build (Join-Path $repository 'src\ValueGenerator\ValueGenerator.csproj') `
    -c Release -p:Platform=x64 --no-restore "-p:StoreManifestPath=$storeManifest" `
    -p:GenerateAppxPackageOnBuild=true -p:UapAppxPackageBuildMode=StoreUpload `
    -p:AppxPackageSigningEnabled=false -p:AppxBundle=Never "-p:AppxPackageDir=$OutputDirectory\"
if ($LASTEXITCODE -ne 0) { throw "Store build failed ($LASTEXITCODE)." }

$uploads = @(Get-ChildItem -LiteralPath $OutputDirectory -Recurse -Filter '*.msixupload')
$packages = @(Get-ChildItem -LiteralPath $OutputDirectory -Recurse -Filter '*.msix')
if ($uploads.Count -ne 1 -or $packages.Count -ne 1) {
    throw 'Expected exactly one .msixupload and one .msix from StoreUpload.'
}
Add-Type -AssemblyName System.IO.Compression.FileSystem
$archive = [IO.Compression.ZipFile]::OpenRead($packages[0].FullName)
try {
    $entry = $archive.GetEntry('AppxManifest.xml')
    if (!$entry) { throw 'Package manifest is missing.' }
    $reader = [IO.StreamReader]::new($entry.Open())
    try { [xml]$packagedManifest = $reader.ReadToEnd() } finally { $reader.Dispose() }
    $identity = $packagedManifest.Package.Identity
    if ($identity.Name -cne $IdentityName -or $identity.Publisher -cne $Publisher -or
        $identity.Version -ne $version -or $identity.ProcessorArchitecture -ne 'x64' -or
        $packagedManifest.Package.Properties.PublisherDisplayName -cne $PublisherDisplayName) {
        throw 'Packaged identity, version or architecture does not match the requested build.'
    }
    $class = $packagedManifest.SelectSingleNode('//*[local-name()="Class" and @Id="cdce3db6-ffd6-46e9-864a-ff25ade45013"]')
    if (!$class) { throw 'COM server registration is missing.' }
    $capabilities = @($packagedManifest.Package.Capabilities.ChildNodes | ForEach-Object { $_.Name })
    if ('runFullTrust' -notin $capabilities -or 'internetClient' -in $capabilities) {
        throw 'Unexpected package capabilities.'
    }
    $required = @('ValueGenerator.exe', 'ValueGenerator.runtimeconfig.json', 'coreclr.dll',
        'Licenses/LICENSE.txt', 'Licenses/THIRD-PARTY-NOTICES.md', 'Licenses/PRIVACY.md',
        'Licenses/COMPONENTS.md', 'Licenses/SOURCES.md', 'Licenses/CmdPal-LICENSE.txt',
        'Licenses/CsWinRT-LICENSE.txt', 'Licenses/CsWinRT-NOTICE.txt',
        'Licenses/WinRTServer-LICENSE.txt', 'Licenses/Windows-SDK-LICENSE.txt',
        'Licenses/NET-Runtime-LICENSE.txt', 'Licenses/NET-Runtime-THIRD-PARTY-NOTICES.txt')
    foreach ($name in $required) {
        $file = $archive.GetEntry($name)
        if (!$file -or $file.Length -eq 0) { throw "Missing or empty package file: $name" }
    }
    if (@($archive.Entries | Where-Object {
        $_.FullName -match '(?i)(\.(pfx|cer|ps1)$|UnitTests|Assets/Source/|AppxSignature\.p7x$)'
    }).Count -gt 0) { throw 'Unexpected development or signing files in the unsigned Store package.' }
}
finally { $archive.Dispose() }
$uploadArchive = [IO.Compression.ZipFile]::OpenRead($uploads[0].FullName)
try {
    $uploadedPackage = @($uploadArchive.Entries | Where-Object FullName -Like '*.msix')
    if ($uploadedPackage.Count -ne 1 -or $uploadedPackage[0].Length -ne $packages[0].Length) {
        throw 'Store upload does not contain the expected MSIX.'
    }
}
finally { $uploadArchive.Dispose() }
Write-Host "Validated unsigned StoreUpload $version (x64): $($uploads[0].FullName)"
Write-Host 'This artifact is for Partner Center submission. It is not a signed local installer.'
