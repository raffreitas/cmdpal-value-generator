// Copyright (c) Microsoft Corporation
// The Microsoft Corporation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using Microsoft.CommandPalette.Extensions.Toolkit;
using ValueGenerator.Helpers;

namespace ValueGenerator;

internal sealed partial class FallbackValueGeneratorItem : FallbackCommandItem
{
    internal FallbackValueGeneratorItem()
        : base(new NoOpCommand(), "Value Generator", "ValueGenerator.Fallback")
    {
        Icon = Icons.Generator;
        Title = string.Empty;
    }

    public override void UpdateQuery(string query)
    {
        Command = new NoOpCommand();
        Title = string.Empty;
        Subtitle = string.Empty;
        MoreCommands = [];

        if (!CommandCatalog.IsKnownQuery(query))
        {
            return;
        }

        var result = QueryEvaluator.Evaluate(query);
        Command = result.Command;
        Title = result.Title;
        // Include the query so the host's matching retains a computed result.
        Subtitle = $"{query} — {result.Subtitle}";
        MoreCommands = result.MoreCommands;
    }
}
