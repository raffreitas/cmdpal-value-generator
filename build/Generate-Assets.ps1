# Generates Windows assets from the project's Fluent-inspired icon master.
$ErrorActionPreference = 'Stop'
Add-Type -AssemblyName System.Drawing
$assetsDirectory = Join-Path $PSScriptRoot '..\ValueGenerator\Assets'
$source = [Drawing.Image]::FromFile((Join-Path $assetsDirectory 'Source\ValueGenerator.png'))
$sizes = @{
    'StoreLogo.png' = @(50, 50)
    'Square44x44Logo.png' = @(44, 44)
    'Square44x44Logo.scale-200.png' = @(88, 88)
    'Square44x44Logo.targetsize-24_altform-unplated.png' = @(24, 24)
    'Square150x150Logo.png' = @(150, 150)
    'Square150x150Logo.scale-200.png' = @(300, 300)
    'SmallTile.png' = @(71, 71)
    'LargeTile.png' = @(310, 310)
    'Wide310x150Logo.png' = @(310, 150)
    'Wide310x150Logo.scale-200.png' = @(620, 300)
    'SplashScreen.png' = @(620, 300)
    'SplashScreen.scale-200.png' = @(1240, 600)
    'LockScreenLogo.scale-200.png' = @(48, 48)
}
try {
    foreach ($asset in $sizes.GetEnumerator()) {
        $width, $height = $asset.Value
        $result = [Drawing.Bitmap]::new($width, $height)
        $graphics = [Drawing.Graphics]::FromImage($result)
        try {
            $graphics.Clear([Drawing.Color]::Transparent)
            $graphics.InterpolationMode = [Drawing.Drawing2D.InterpolationMode]::HighQualityBicubic
            $graphics.PixelOffsetMode = [Drawing.Drawing2D.PixelOffsetMode]::HighQuality
            $side = [Math]::Min($width, $height)
            $graphics.DrawImage($source, ($width - $side) / 2, ($height - $side) / 2, $side, $side)
            $result.Save((Join-Path $assetsDirectory $asset.Key), [Drawing.Imaging.ImageFormat]::Png)
        }
        finally {
            $graphics.Dispose()
            $result.Dispose()
        }
    }
}
finally { $source.Dispose() }
