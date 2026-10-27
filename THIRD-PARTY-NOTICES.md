# Third-party notices

This extension ports Value Generator from Microsoft PowerToys:

- Repository: https://github.com/microsoft/PowerToys
- Source: `src/modules/launcher/Plugins/Community.PowerToys.Run.Plugin.ValueGenerator`
- Tests: `src/modules/launcher/Plugins/Community.PowerToys.Run.Plugin.ValueGenerator.UnitTests`
- Reference commit: `9e6593c34949d33fff03ba360846eca4be284f59`
- Copyright (c) Microsoft Corporation. Licensed under the MIT license; see [LICENSE](LICENSE).

Ported files include `Generators/` (except the new ULID generator), `IComputeRequest.cs`,
`InputParser.cs`, `GeneratorData.cs`, the suggestion catalog and strings from
`Helper/QueryHelper.cs` and `Properties/Resources.resx`, and the original unit tests.
Original copyright/license headers are retained. Adaptations remove Wox dependencies,
enable nullable, add clipboard support, use concurrent-safe hashing, and report invalid
hexadecimal escapes as errors.

CmdPal UI integration follows the PowerToys calculator extension at
`src/modules/cmdpal/ext/Microsoft.CmdPal.Ext.Calc`, under the same MIT license.

## Packaged dependencies

The MSIX includes a self-contained .NET runtime and Windows/WinRT components.
Their licenses are separate from this extension's MIT license. Every build copies
the applicable documents to `Licenses/` and generates `COMPONENTS.md` with the
restored versions:

- Microsoft.CommandPalette.Extensions (including Toolkit): MIT.
- Microsoft.Windows.CsWinRT: MIT; its original license is included.
- Shmuelie.WinRTServer: MIT; its original license is included.
- .NET runtime: its LICENSE and THIRD-PARTY-NOTICES files are both included.
- Windows SDK .NET projection: Microsoft Windows SDK license, included as
  `Windows-SDK-LICENSE.txt`; source: https://aka.ms/WinSDKLicenseURL.

Build tools and unit-test dependencies are not shipped with the extension.
See [licenses/README.md](licenses/README.md) for the stored license source.

The package icon is original artwork generated for this extension, inspired by
Microsoft Fluent app-icon guidance. It is not a Microsoft or PowerToys logo.
The same original artwork is used in Command Palette;
no Microsoft font file is redistributed. This project is independently maintained
and is not an official Microsoft product.
