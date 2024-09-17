// Copyright (c) Microsoft Corporation
// The Microsoft Corporation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Linq;
using Microsoft.CmdPal.Extensions.Helpers;
using SegoeFluentIconsExtension.Data;
using Windows.ApplicationModel.DataTransfer;

namespace SegoeFluentIconsExtension.Commands;

internal sealed partial class CopyIconToClipboardCommand : InvokableCommand
{
    private readonly SegoeFluentIcon _icon;

    internal CopyIconToClipboardCommand(SegoeFluentIcon icon)
    {
        _icon = icon;
        Name = "Copy";
        Icon = new(icon.IconString);
    }

    public override CommandResult Invoke()
    {
        /*
        var dataPackage = new DataPackage();

        dataPackage.SetText(_icon.IconString);
        Clipboard.SetContent(dataPackage);
        */

        return CommandResult.Dismiss();
    }
}
