<p align="center">
  <img src="src/ValueGenerator/Assets/Square150x150Logo.scale-200.png" alt="Value Generator icon" width="160" />
</p>

<h1 align="center">
  Value Generator<br />
  for Command Palette
</h1>

<p align="center">
  Generate GUIDs, ULIDs, hashes and encoded text without leaving
  <a href="https://learn.microsoft.com/windows/powertoys/command-palette/overview">PowerToys Command Palette</a>.
</p>

---

> [!NOTE]
> This extension is a port of the **Value Generator** plugin from
> [PowerToys Run](https://learn.microsoft.com/windows/powertoys/run) to Command Palette.
> The original GUID, hash and encoding generators, command syntax and unit tests come from the
> [Microsoft PowerToys repository](https://github.com/microsoft/PowerToys/tree/9e6593c34949d33fff03ba360846eca4be284f59/src/modules/launcher/Plugins/Community.PowerToys.Run.Plugin.ValueGenerator)
> (MIT license). ULID support is an addition to this extension and was not part
> of the ported PowerToys Run plugin. This is an independent project, not an
> official Microsoft product.

Value Generator is a PowerToys Command Palette extension for the small values
developers need all the time. Type a command such as `guid7`, `sha256 abc` or
`base64 hello` and press Enter to copy the result to the clipboard.

- **GUID / UUID** versions 1, 3, 4, 5 and 7, with extra copy formats.
- **ULID** generation with a cryptographically secure random component.
- **Hashes**: MD5, SHA-1, SHA-256, SHA-384 and SHA-512, from typed text or the clipboard.
- **Encoding**: Base64, URL and URI data/hex escape and unescape.
- **Fast access**: command suggestions, a main-search fallback and an optional `#` alias.

Everything runs locally. The extension does not send your input anywhere and
collects no telemetry. See the [privacy policy](PRIVACY.md).

### What changed from the PowerToys Run plugin

The original generators were adapted to run as a Command Palette extension:

- Wox / PowerToys Run dependencies were replaced by the Command Palette extension SDK.
- Added **ULID** support and extra GUID copy formats in the result's action menu.
- Hash commands without input use the clipboard text.
- Nullable reference types are enabled and hashing is safe for concurrent use.
- Invalid hexadecimal escapes are now reported as errors.

The origin, reference commit and license of each ported file are listed in
[THIRD-PARTY-NOTICES.md](THIRD-PARTY-NOTICES.md).

## Install and open

Requires Microsoft PowerToys with **Command Palette** enabled. The Store build
currently targets **x64** and includes the .NET runtime; users do not need the
.NET SDK, Visual Studio or Developer Mode to install the Store version.

1. Install [PowerToys](https://learn.microsoft.com/windows/powertoys/install)
   and enable Command Palette in its settings.
2. Install Value Generator from its Store listing once released. This extension
   is not yet published; build artifacts are not a public Store installer.
3. Open Command Palette (default shortcut: **Win+Alt+Space**) and search for
   **Value Generator**. It has no separate Start menu entry.
4. If it is not listed, run **Reload Command Palette extensions** from Command
   Palette and search again.

To uninstall, open **Windows Settings > Apps > Installed apps**, find
**Value Generator**, and select **Uninstall**. Reload the Command Palette
extensions if its command remains visible afterward.

The package declares Windows build `19041` as its minimum. This declaration is
not a guarantee of compatibility with every Windows or PowerToys version.

## Commands

| Command | Example or behavior |
| --- | --- |
| `guid`, `uuid` | GUID v4 |
| `guid1`, `guid4`, `guid7` | GUID v1, v4, and .NET's native v7 |
| `guid3`, `guid5` | `guid5 ns:DNS abc`, or an explicit namespace GUID and a name |
| `md5`, `sha1`, `sha256`, `sha384`, `sha512` | `sha256 abc`; without input, hashes the clipboard text |
| `base64`, `base64d` | `base64 Hello world`, `base64d YWJj` |
| `url`, `urld` | `url a b+c`, `urld a+b%2bc` |
| `esc:data`, `uesc:data` | URI data escape/unescape |
| `esc:hex`, `uesc:hex` | `esc:hex z`, `uesc:hex %7A` |
| `ulid` | ULID with a timestamp and cryptographic randomness, in Crockford Base32 |

GUID versions also accept the original aliases `guidv1`, `uuid1`, `uuidv1` and
their v3/v4/v5/v7 equivalents. Predefined namespaces: `ns:DNS`, `ns:URL`, `ns:OID`
and `ns:X500`. GUID v3/v5 keep the original plugin's ASCII encoding and its
no-spaces name syntax.

An empty search shows suggestions; pressing Enter on a suggestion fills in the
command. Pressing Enter on a result copies the value. The GUID result's action
menu also copies the same GUID without hyphens, in uppercase or with braces.

Hash commands without text read the clipboard while the query is evaluated
during typing, including in the main-search fallback when enabled. Pressing
Enter is not required for that read; Enter on a result copies its value.

Commands are case-insensitive. Leading and trailing whitespace in the input is
trimmed; inner spaces are preserved. Clipboard text is used as-is. An empty or
non-text clipboard and invalid input show an error with no copy action. ULIDs sort
by timestamp; order within the same millisecond is not guaranteed.

> MD5 and SHA-1 are provided for compatibility, not for security-sensitive hashing.

## `#` alias

1. Open **Command Palette** settings and go to **Extensions**.
2. Find **Value Generator** and expand the **Value Generator** command.
3. Set **Alias** to `#` and enable **Direct** alias activation.
4. Back in the main search, type `#` to open the page, then type `guid7`,
   `sha256 abc` or any other command. The `#` is handled by Command Palette and is
   not part of the extension's command syntax.

When enabled in Command Palette settings, the fallback also recognizes commands
typed directly in the main search, without `#`.

## Build and test

Requirements: Windows, .NET SDK 10, PowerShell 7 (`pwsh`), Visual Studio with the
components in `.vsconfig`, Windows SDK 10.0.26100.0 and the MSIX packaging tools.

```text
src/ValueGenerator/              Command Palette extension (MSIX, COM server)
tests/ValueGenerator.UnitTests/  MSTest unit tests
build/                           Packaging and asset scripts
```

```powershell
dotnet restore ValueGenerator.slnx -p:Platform=x64
dotnet build ValueGenerator.slnx -p:Platform=x64 --no-restore
dotnet test tests/ValueGenerator.UnitTests/ValueGenerator.UnitTests.csproj -p:Platform=x64 --no-restore
```

The tests compile the extension's calculation sources directly, without starting
the COM server or touching the real clipboard. They include the original ported
tests, known hash and GUID vectors, round-trips, ULID checks and simulated
clipboard reads.

The project targets the stable Windows SDK `10.0.26100.57`. In Release, trimming
the `Microsoft.Windows.SDK.NET` and `WinRT.Runtime` dependencies can emit `IL2104`,
which groups `IL2081` warnings from those libraries' fallback code. These warnings
are not suppressed. The Debug build has no warnings. The project only excludes the
Command Palette interface WinMD from automatic activation registration, which avoids
`APPX1707`. The unused `internetClient` capability was removed; `runFullTrust` and
the COM server CLSID are kept.

## Deploy from Visual Studio

1. Open `ValueGenerator.slnx` and set `ValueGenerator` as the startup project.
2. Select `Debug`, platform `x64` (or `ARM64` on an ARM64 machine) and the
   **ValueGenerator (Package)** profile.
3. Enable Windows Developer Mode if Visual Studio asks for it.
4. Use **Build > Deploy Solution**, or right-click the `ValueGenerator` project and
   select **Deploy**. Building alone does not register the MSIX.
5. In Command Palette, search for `Reload` and run **Reload Command Palette extensions**.
6. Open **Value Generator**. To debug the extension process, press F5 with the
   package profile selected.

If you get `APPX3217` because `UAP.props` is missing, install or repair Windows SDK
10.0.26100 from the Visual Studio Installer; the target framework does not need to change.

### Checks after deploying

- Open the empty page, select a suggestion and confirm it fills in the search.
- Copy `sha256 abc` and check the hash; copy a GUID in the three extra formats.
- Run `guid7`, `guid5 ns:DNS abc`, `ulid`, Base64 and URL from the page and the fallback.
- Confirm that `guidance` and `sha256sum abc` do not show the Value Generator fallback.
- Confirm the errors for `base64d %%%`, `guid2` and `esc:hex ab` without the extension closing.
- Copy some text and run only `sha256`; repeat with a non-text clipboard.
- Confirm direct activation through `#`.

## Assets

The original artwork is `src/ValueGenerator/Assets/Source/ValueGenerator.png`.
To regenerate the package sizes on Windows, run `./build/Generate-Assets.ps1`. The
icon follows the Fluent app icon style with a transparent background and is also
used for the Command Palette commands. The package hides its Start menu entry to
avoid a duplicate app result in Command Palette search.

## References

- [Command Palette settings](https://learn.microsoft.com/windows/powertoys/command-palette/settings)
- [Creating a Command Palette extension](https://learn.microsoft.com/windows/powertoys/command-palette/creating-an-extension)
- [ULID specification](https://github.com/ulid/spec)

## License

[MIT](LICENSE), with original contributions by `raffreitas`. Portions are derived from Microsoft PowerToys; see
[THIRD-PARTY-NOTICES.md](THIRD-PARTY-NOTICES.md).
Dependency licenses and notices are included in the package under `Licenses/`.
