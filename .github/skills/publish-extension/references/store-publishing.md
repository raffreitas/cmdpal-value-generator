# Microsoft Store package

This repository builds an unsigned, self-contained x64 `.msixupload` using
`build/Build-StorePackage.ps1` or the **Build Microsoft Store package** workflow.
Neither path publishes the app.

Supply the public Partner Center identity through `STORE_IDENTITY_NAME`,
`STORE_PUBLISHER` and `STORE_PUBLISHER_DISPLAY_NAME`. Keep credentials and
certificates out of the repository. Preserve the development manifest and COM
CLSID; the script generates a separate Store manifest.

Run from the repository root after restoring for x64:

```powershell
dotnet restore ValueGenerator.slnx -p:Platform=x64
./build/Build-StorePackage.ps1
```

The output directory must be empty. The script validates identity, version,
architecture, COM registration, capabilities and license documents. An unsigned
Store upload is not a local installer. Use development deployment or a properly
signed package for installation tests. Build validation does not replace license
review or Store certification. Never submit a package built with a test identity.

The current workflow targets x64. A single-architecture package is supported;
ARM64 and a multi-architecture bundle are optional future additions. For updates,
increment the source manifest version with the fourth component zero and retain
the reserved product identity.

The Command Palette Extension Gallery can link to a Microsoft Store product.
Gallery inclusion is separate from Store publication; WinGet is optional.

- [MSIX package requirements](https://learn.microsoft.com/windows/apps/publish/publish-your-app/msix/app-package-requirements)
- [Command Palette distribution](https://learn.microsoft.com/windows/powertoys/command-palette/publish-extension)
