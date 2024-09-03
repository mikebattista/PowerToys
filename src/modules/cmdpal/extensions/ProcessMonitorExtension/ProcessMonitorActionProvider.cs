﻿// Copyright (c) Microsoft Corporation
// The Microsoft Corporation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using Microsoft.Windows.CommandPalette.Extensions;
using Microsoft.Windows.CommandPalette.Extensions.Helpers;

namespace ProcessMonitorExtension;

internal sealed class ProcessMonitorActionProvider : ICommandProvider
{
    public string DisplayName => "Process Monitor Commands";

    public IconDataType Icon => new(string.Empty); // Optionally provide an icon URL

    public void Dispose()
    {
    }

    private readonly IListItem[] _actions = [
        new ListItem(new ProcessListPage())
        {
            Title = "Process Manager",
            Subtitle = "Kill processes",
        },
    ];

    public IListItem[] TopLevelCommands()
    {
        return _actions;
    }
}
