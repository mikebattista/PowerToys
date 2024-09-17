// Copyright (c) Microsoft Corporation
// The Microsoft Corporation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.CmdPal.Extensions.Helpers;
using VirtualDesktopExtension.Data;

namespace VirtualDesktopExtension.Commands;

internal sealed partial class SwitchToVirtualDesktopCommand : InvokableCommand
{
    private readonly VirtualDesktop _virtualDesktop;

    internal SwitchToVirtualDesktopCommand(VirtualDesktop virtualDesktop)
    {
        _virtualDesktop = virtualDesktop;
        Name = "Switch";
        Icon = new("\uE8A7");
    }

    public override CommandResult Invoke()
    {
        // TODO: Issue keybinds to switch to appropriate virtual desktop or wait for public APIs.
        Debug.WriteLine($"Switch to Desktop {_virtualDesktop.Number}.");

        return CommandResult.Dismiss();
    }
}
