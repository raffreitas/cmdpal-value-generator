// Copyright (c) Microsoft Corporation
// The Microsoft Corporation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System;
using System.Runtime.InteropServices;
using System.Security.Cryptography;
using Microsoft.CommandPalette.Extensions.Toolkit;
using ValueGenerator.GUID;

namespace ValueGenerator.Helpers;

internal static class QueryEvaluator
{
    internal static ListItem Evaluate(string query)
    {
        try
        {
            var request = new InputParser(ClipboardHelper.GetText).ParseInput(query);
            if (!request.Compute())
            {
                return Error(request.ErrorMessage);
            }

            var text = request.ResultToString();
            var item = new ListItem(new CopyTextCommand(text))
            {
                Title = text,
                Subtitle = request.Description,
                Icon = Icons.Generator,
            };

            if (request is GUIDRequest)
            {
                var guid = new Guid(request.Result);
                item.MoreCommands =
                [
                    CopyFormat(guid.ToString("N"), "Copy without hyphens"),
                    CopyFormat(guid.ToString("D").ToUpperInvariant(), "Copy uppercase"),
                    CopyFormat(guid.ToString("B"), "Copy with braces"),
                ];
            }

            return item;
        }
        catch (Exception exception) when (exception is ArgumentException or FormatException
            or InvalidOperationException or CryptographicException or ExternalException)
        {
            return Error(exception.Message);
        }
    }

    private static ListItem Error(string message) => new(new NoOpCommand())
    {
        Title = "Value Generator Error",
        Subtitle = message,
        Icon = Icons.Generator,
    };

    private static CommandContextItem CopyFormat(string value, string title) =>
        new CommandContextItem(new CopyTextCommand(value) { Name = title }) { Title = title };
}
