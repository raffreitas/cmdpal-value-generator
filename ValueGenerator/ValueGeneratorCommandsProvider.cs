// Copyright (c) Microsoft Corporation
// The Microsoft Corporation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using Microsoft.CommandPalette.Extensions;
using Microsoft.CommandPalette.Extensions.Toolkit;
using ValueGenerator.Helpers;

namespace ValueGenerator;

public partial class ValueGeneratorCommandsProvider : CommandProvider
{
    private readonly ICommandItem[] _commands;
    private readonly SettingsManager _settingsManager = new();
    private readonly FallbackValueGeneratorItem _fallback = new();

    public ValueGeneratorCommandsProvider()
    {
        Id = "ValueGenerator.Provider";
        DisplayName = "Value Generator";
        Icon = Icons.Generator;
        _commands = [
            new CommandItem(new ValueGeneratorPage())
            {
                Title = DisplayName,
                MoreCommands = [new CommandContextItem(_settingsManager.Settings.SettingsPage)],
            },
        ];

        Settings = _settingsManager.Settings;
    }

    public override ICommandItem[] TopLevelCommands()
    {
        return _commands;
    }

    public override IFallbackCommandItem[] FallbackCommands() => [_fallback];
}
