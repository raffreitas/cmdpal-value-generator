// Copyright (c) Microsoft Corporation
// The Microsoft Corporation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using Microsoft.CommandPalette.Extensions;
using Microsoft.CommandPalette.Extensions.Toolkit;
using System.Linq;
using ValueGenerator.Commands;
using ValueGenerator.Helpers;

namespace ValueGenerator;

internal sealed partial class ValueGeneratorPage : DynamicListPage
{
    private IListItem[] _items = [];

    public ValueGeneratorPage()
    {
        Id = "ValueGenerator.Page";
        Icon = Icons.Generator;
        Title = "Value Generator";
        Name = "Open";
        PlaceholderText = "guid7, sha256 text, base64 text, ulid";
        UpdateSearchText(string.Empty, string.Empty);
    }

    internal void SetQuery(string query)
    {
        SearchText = query;
        OnPropertyChanged(nameof(SearchText));
    }

    public override void UpdateSearchText(string oldSearch, string newSearch)
    {
        _items = string.IsNullOrWhiteSpace(newSearch)
            ? CommandCatalog.GeneratorDataList.Select(generator => (IListItem)new ListItem(
                new SetSearchTextCommand(this, generator.Keyword + " "))
            {
                Title = CommandCatalog.GetResultTitle(generator),
                Subtitle = CommandCatalog.GetResultSubtitle(generator),
                TextToSuggest = generator.Keyword + " ",
                Icon = Icons.Generator,
            }).ToArray()
            : [QueryEvaluator.Evaluate(newSearch)];

        RaiseItemsChanged(_items.Length);
    }

    public override IListItem[] GetItems() => _items;
}
