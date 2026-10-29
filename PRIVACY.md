# Value Generator privacy policy

Last updated: October 29, 2026

Value Generator is an independently maintained extension for Microsoft PowerToys
Command Palette. This policy describes the extension's own behavior.

## Text and clipboard

The extension processes the text you enter locally on your computer to generate
identifiers, calculate hashes, and encode or decode text. If you enter a hash
command without text, the extension reads the current clipboard text to calculate
the result as the query is evaluated during typing. This also applies to recognized
hash commands in the main-search fallback when that feature is enabled. You do not
need to press Enter for this read to happen. Selecting a copy action (for example,
pressing Enter on a result) writes the displayed result or the selected GUID format
to your clipboard.

Input, clipboard content and generated results are held in memory while the
extension processes and displays them. The extension does not create a history
file or log this content, and does not send it to a server or another service.
You control what you enter, whether you use clipboard-based commands, and when
you invoke copy actions. Copying a result replaces the previous clipboard text.

## Local settings

The extension's settings manager may read or write a local `settings.json` file
under the Command Palette extension settings directory for `ValueGenerator`.
It currently defines no custom user settings. Input text, clipboard content and
generated results are not saved in this settings file.

## Network, analytics and accounts

The extension does not implement network requests, advertising, analytics or
telemetry, and does not require an account of its own. Its processing takes place
locally. Installation and updates through the Microsoft Store are handled by
Microsoft, not by a network service operated by this extension's maintainer.

Windows, the Microsoft Store and PowerToys/Command Palette may independently
process data, retain interface state, synchronize clipboard data or provide
diagnostics according to their own settings and privacy policies. This extension
does not control those products or other applications that can access the clipboard.

## Contact and changes

For privacy questions or requests, contact the maintainer through
[the project's GitHub issues](https://github.com/raffreitas/cmdpal-value-generator/issues).
Do not include clipboard contents, secrets or other private input in public issues.
If the extension's data practices change, this document and its update date will
be revised before distributing that change.
