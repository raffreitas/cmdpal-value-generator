# Third-party notices

This extension ports Value Generator from Microsoft PowerToys:

- Repository: https://github.com/microsoft/PowerToys
- Source: `src/modules/launcher/Plugins/Community.PowerToys.Run.Plugin.ValueGenerator`
- Reference commit: `9e6593c34949d33fff03ba360846eca4be284f59`
- Copyright (c) Microsoft Corporation. Licensed under the MIT license; see [LICENSE](LICENSE).

Ported files include `Generators/`, `IComputeRequest.cs`, `InputParser.cs`, and
`GeneratorData.cs`. Original copyright/license headers are retained. Adaptations
remove Wox dependencies, enable nullable, use concurrent-safe hashing, and report
invalid hexadecimal escapes as errors.
