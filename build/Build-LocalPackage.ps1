#Requires -Version 7.0

param(
    [string]$OutputDirectory
)

$ErrorActionPreference = 'Stop'
$repository = [IO.Path]::GetFullPath((Join-Path $PSScriptRoot '..'))
$project = Join-Path $repository 'src\ValueGenerator\ValueGenerator.csproj'
if (!$OutputDirectory) {
    $OutputDirectory = Join-Path $repository 'src\ValueGenerator\AppPackages\LocalSignedTest'
}
$OutputDirectory = [IO.Path]::GetFullPath($OutputDirectory)

# Restore before touching the certificate store or generating the local manifest.
& dotnet restore (Join-Path $repository 'ValueGenerator.slnx') -p:Platform=x64
if ($LASTEXITCODE -ne 0) { throw "Restore failed ($LASTEXITCODE)." }

$certificate = Get-ChildItem Cert:\CurrentUser\My -CodeSigningCert | Where-Object {
    $_.Subject -eq 'CN=raffreitas' -and
    $_.FriendlyName -eq 'ValueGenerator local testing' -and
    $_.HasPrivateKey -and $_.NotAfter -gt (Get-Date).AddDays(1)
} | Sort-Object NotAfter -Descending | Select-Object -First 1

if (!$certificate) {
    $certificate = New-SelfSignedCertificate -Type Custom -Subject 'CN=raffreitas' `
        -FriendlyName 'ValueGenerator local testing' -KeyUsage DigitalSignature `
        -KeyAlgorithm RSA -KeyLength 2048 -HashAlgorithm SHA256 `
        -CertStoreLocation 'Cert:\CurrentUser\My' -NotAfter (Get-Date).AddMonths(3) `
        -TextExtension @('2.5.29.37={text}1.3.6.1.5.5.7.3.3', '2.5.29.19={text}')
}

# Keep the versioned manifest and the Store build identity unchanged.
$manifestDirectory = Join-Path $repository 'src\ValueGenerator\obj\LocalPackage'
[IO.Directory]::CreateDirectory($manifestDirectory) | Out-Null
[xml]$manifest = Get-Content -LiteralPath (Join-Path $repository 'src\ValueGenerator\Package.appxmanifest') -Raw
$manifest.Package.Identity.Publisher = $certificate.Subject
$manifest.Package.Properties.PublisherDisplayName = 'raffreitas'
$localManifest = Join-Path $manifestDirectory 'Package.appxmanifest'
$manifest.Save($localManifest)

& dotnet build $project -c Release -p:Platform=x64 --no-restore `
    -p:GenerateAppxPackageOnBuild=true -p:UapAppxPackageBuildMode=SideloadOnly `
    -p:GenerateTemporaryStoreCertificate=false -p:AppxPackageSigningEnabled=true `
    "-p:PackageCertificateThumbprint=$($certificate.Thumbprint)" `
    "-p:StoreManifestPath=$localManifest" -p:AppxBundle=Never `
    "-p:AppxPackageDir=$OutputDirectory\"
if ($LASTEXITCODE -ne 0) { throw "Local package build failed ($LASTEXITCODE)." }

$packageName = "ValueGenerator_$($manifest.Package.Identity.Version)_x64"
$packageDirectory = Join-Path $OutputDirectory "${packageName}_Test"
$packagePath = Join-Path $packageDirectory "$packageName.msix"
$certificatePath = Join-Path $packageDirectory "$packageName.cer"
foreach ($path in @($packagePath, $certificatePath, (Join-Path $packageDirectory 'Add-AppDevPackage.ps1'))) {
    if (!(Test-Path -LiteralPath $path -PathType Leaf)) { throw "Missing installer file: $path" }
}

Add-Type -AssemblyName System.IO.Compression.FileSystem
$archive = [IO.Compression.ZipFile]::OpenRead($packagePath)
try {
    if (!$archive.GetEntry('AppxSignature.p7x')) { throw 'Local MSIX is not signed.' }
}
finally { $archive.Dispose() }

Write-Host "Copy this entire folder to the other x64 PC: $packageDirectory"
Write-Host 'On that PC, run .\Add-AppDevPackage.ps1 and accept the test certificate installation.'
Write-Host 'PowerToys with Command Palette enabled is required. Reload its extensions after installing.'
Write-Host 'This package is for local testing, not Microsoft Store submission. The private key stays on this PC.'
