# Bundled license sources

`Windows-SDK-LICENSE.txt` is a plain-text copy of the Microsoft Windows Software
Development Kit license retrieved on 2026-10-27 from
https://aka.ms/WinSDKLicenseURL (the license URL of the stable Windows SDK package).
The original document is available at
https://download.microsoft.com/download/0/F/F/0FF2B061-47DD-4F55-89B6-FD1D8C44F14D/sdk_license.rtf.

Other dependency licenses and the .NET runtime third-party notices are copied from
the restored packages by `build/Prepare-PackageLicenses.ps1`. The build writes an
exact version inventory to `Licenses/COMPONENTS.md` in the MSIX. The CmdPal SDK
declares MIT and is distributed as part of Microsoft PowerToys; its MIT notice is
copied from `licenses/PowerToys-MIT.txt`, which preserves the original Microsoft
notice separately from the project's additional `raffreitas` copyright.
CsWinRT's `LICENSE` and `NOTICE.txt` are copied without editing their contents.
The package includes this source note as `Licenses/SOURCES.md`; the generated
third-party document uses links relative to that directory.
