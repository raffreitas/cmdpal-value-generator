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
