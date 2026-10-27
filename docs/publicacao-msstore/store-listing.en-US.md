# Store listing draft — en-US

Review these fields in Partner Center before submitting. This document is not a
published listing. Product name remains subject to reservation availability.

## Product name

Value Generator

## Short description

Generate GUIDs, ULIDs, hashes and encoded text in PowerToys Command Palette.

## Description

Value Generator adds quick developer utilities to PowerToys Command Palette.
Generate GUIDs and ULIDs, calculate text hashes, and encode or decode Base64 and
URLs without leaving the launcher. Select a result to copy it to the clipboard.

- GUID versions 1, 3, 4, 5 and 7, with additional copy formats.
- ULIDs generated without an external service.
- MD5, SHA-1, SHA-256, SHA-384 and SHA-512 hashes of text.
- Base64 encoding and decoding, plus URL and URI escape utilities.
- Command suggestions, direct search fallback, and a configurable alias.

When a hash command has no input, it uses text from your clipboard. Processing is
local: the extension does not transmit your input or collect telemetry. MD5 and
SHA-1 are provided for compatibility, not for security-sensitive hashing.

Requires Microsoft PowerToys with Command Palette enabled. This is a Command
Palette extension, not a standalone desktop utility. Open Command Palette,
reload extensions if necessary, and search for Value Generator.

Based on the MIT-licensed Value Generator plugin from Microsoft PowerToys.
Independently maintained; not an official Microsoft product.

## URLs

- Support: https://github.com/raffreitas/cmdpal-value-generator/issues
- Privacy: https://github.com/raffreitas/cmdpal-value-generator/blob/main/PRIVACY.md
- Project: https://github.com/raffreitas/cmdpal-value-generator

Confirm the URLs resolve publicly after pushing, including the default branch name.

## Suggested keywords

GUID, UUID, ULID, hash, Base64, Command Palette, PowerToys

## Notes for certification

This MSIX contains an out-of-process COM server implementing the Microsoft Command
Palette extension SDK. The runFullTrust capability is required to host that COM
EXE and integrate with Command Palette. It does not request elevation. No Internet
capability or extension account is required. The .NET runtime is bundled.

Install current Microsoft PowerToys and enable Command Palette. Reload Command
Palette extensions, open Value Generator and test these examples:

1. Search `guid7`: select the result to copy a GUID. Additional formats are available
   in its actions menu.
2. Search `sha256 abc`: expected result is
   `BA7816BF8F01CFEA414140DE5DAE2223B00361A396177A9CB410FF61F20015AD`.
3. Search `base64 abc`: expected result is `YWJj`. Search `base64d YWJj`: `abc`.
4. Search `ulid`: a 26-character ULID is shown.
5. Search `base64d %%%`: an input error is shown without crashing.
6. Copy the text `abc`, search only `sha256`: the same hash from step 2 is shown.
7. Type `guid7` in Command Palette's main search: a fallback result should appear
   when extension fallback is enabled. `guidance` must not match this extension.

No credentials are needed for extension functionality. Store acquisition is limited
to the configured private audience; complete any account/access requirements in
Partner Center for certification separately.
