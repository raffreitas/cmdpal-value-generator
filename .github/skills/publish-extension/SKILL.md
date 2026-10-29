---
name: publish-extension
description: >-
  Publish your Command Palette extension to the Microsoft Store or WinGet.
  Use when asked to publish, distribute, release, deploy to store,
  create MSIX packages, submit to WinGet, set up CI/CD for releases,
  or automate builds with GitHub Actions.
---

# Publish Your Command Palette Extension

Guide for distributing your Command Palette extension through the Microsoft Store, WinGet, or both.

## When to Use This Skill

- Publishing your extension to the Microsoft Store
- Submitting your extension to WinGet for `winget install` discovery
- Setting up GitHub Actions to automate builds and releases
- Creating MSIX packages for Store submission
- Creating EXE installers for WinGet submission

## Publishing Options

| Channel | Package Format | Discovery | Auto-Updates |
|---------|---------------|-----------|--------------|
| Microsoft Store | MSIX upload/package or bundle | Store app, direct link; curated Gallery listing | Yes |
| WinGet | EXE installer | `winget install`, CmdPal browse | Yes (via manifest) |

For this repository, prepare the existing x64 Microsoft Store workflow first.
WinGet distribution is optional and is not required for a Gallery listing that
links to the Microsoft Store.

## Workflows

### Microsoft Store Publishing
See [store-publishing.md](references/store-publishing.md) for the complete step-by-step guide.

**Summary:**
1. Register for Partner Center
2. Supply Partner Center identity through the existing Store build variables
3. Build and validate the self-contained x64 `.msixupload`
4. Validate the package and its bundled license documents
5. Submit to Partner Center only when explicitly requested

### WinGet Publishing
See [winget-publishing.md](references/winget-publishing.md) for the complete step-by-step guide.

**Summary:**
1. Switch project to unpackaged mode
2. Create Inno Setup installer script
3. Build EXE installers
4. Submit manifest via `wingetcreate new`
5. Optionally automate with GitHub Actions

## Prerequisites

- [Visual Studio](https://visualstudio.microsoft.com/) with C# and WinUI workloads
- [Partner Center account](https://partner.microsoft.com/dashboard/home) (for Store publishing)
- [GitHub CLI](https://cli.github.com/) (for WinGet publishing)
- [WingetCreate](https://github.com/microsoft/winget-create) — `winget install Microsoft.WingetCreate`
- [Inno Setup](https://jrsoftware.org/isdl.php) (for WinGet EXE packaging)

## Important Notes

- Your extension's CLSID (the `[Guid("...")]` in your main .cs file) must be unique and consistent across all files
- WinGet manifests must include the `windows-commandpalette-extension` tag for CmdPal discovery
- A single-architecture MSIX submission is supported; this workflow targets x64.
- Preserve the local manifest and use the generated manifest for Store identity.
- WinGet prerequisites must describe actual runtime dependencies; this project
  does not require Windows App SDK merely because it is a CmdPal extension.
