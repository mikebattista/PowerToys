﻿// Copyright (c) Microsoft Corporation
// The Microsoft Corporation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using Microsoft.Windows.CommandPalette.Extensions;
using Microsoft.Windows.CommandPalette.Extensions.Helpers;
using Windows.Foundation;

namespace WindowsCommandPalette.BuiltinCommands;

public class QuitAction : InvokableCommand, IFallbackHandler
{
    public event TypedEventHandler<object?, object?>? QuitRequested;

    public QuitAction()
    {
        Icon = new("\uE711");
    }

    public override ICommandResult Invoke()
    {
        QuitRequested?.Invoke(this, new());
        return ActionResult.KeepOpen();
    }

    public void UpdateQuery(string query)
    {
        if (query.StartsWith('q'))
        {
            Name = "Quit";
        }
        else
        {
            Name = string.Empty;
        }
    }
}
