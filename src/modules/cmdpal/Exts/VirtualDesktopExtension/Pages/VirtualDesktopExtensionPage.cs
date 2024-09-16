// Copyright (c) Microsoft Corporation
// The Microsoft Corporation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.CmdPal.Extensions;
using Microsoft.CmdPal.Extensions.Helpers;
using Microsoft.Win32;
using VirtualDesktopExtension.Data;

namespace VirtualDesktopExtension;

internal sealed partial class VirtualDesktopExtensionPage : ListPage
{
    public VirtualDesktopExtensionPage()
    {
        Icon = new(string.Empty);
        Name = "Virtual Desktops";
    }

    public override ISection[] GetItems()
    {
        var virtualDesktops = GetVirtualDesktops();

        return [
            new ListSection()
            {
                Items = virtualDesktops.Select((virtualDesktop) => new ListItem(new NoOpCommand())
                {
                    Title = $"Switch to Desktop {virtualDesktop.Number}{((!string.IsNullOrEmpty(virtualDesktop.Name)) ? $" ({virtualDesktop.Name})" : $"{string.Empty}")}",
                }).ToArray(),
            }
        ];
    }

    private static List<VirtualDesktop> GetVirtualDesktops()
    {
        var virtualDesktops = new List<VirtualDesktop>();

        var virtualDesktopIds = BitConverter.ToString((byte[])Registry.GetValue("HKEY_CURRENT_USER\\Software\\Microsoft\\Windows\\CurrentVersion\\Explorer\\VirtualDesktops", "VirtualDesktopIDs", null)).Replace("-", string.Empty);
        for (var i = 0; i < virtualDesktopIds.Length / 32; i++)
        {
            var virtualDesktopGuid = Guid.Parse(ConvertVirtualDesktopId(virtualDesktopIds[(i * 32)..((i * 32) + 32)]));

            var virtualDesktop = new VirtualDesktop
            {
                ID = virtualDesktopGuid,
                Number = i + 1,
                Name = Registry.GetValue($"HKEY_CURRENT_USER\\Software\\Microsoft\\Windows\\CurrentVersion\\Explorer\\VirtualDesktops\\Desktops\\{virtualDesktopGuid:B}", "Name", string.Empty).ToString(),
            };

            virtualDesktops.Add(virtualDesktop);
        }

        return virtualDesktops;
    }

    private static string ConvertVirtualDesktopId(string virtualDesktopId)
    {
        var convertedVirtualDesktopId = new StringBuilder();

        convertedVirtualDesktopId.Append('{');
        convertedVirtualDesktopId.Append(virtualDesktopId[6..8]);
        convertedVirtualDesktopId.Append(virtualDesktopId[4..6]);
        convertedVirtualDesktopId.Append(virtualDesktopId[2..4]);
        convertedVirtualDesktopId.Append(virtualDesktopId[0..2]);
        convertedVirtualDesktopId.Append('-');
        convertedVirtualDesktopId.Append(virtualDesktopId[10..12]);
        convertedVirtualDesktopId.Append(virtualDesktopId[8..10]);
        convertedVirtualDesktopId.Append('-');
        convertedVirtualDesktopId.Append(virtualDesktopId[14..16]);
        convertedVirtualDesktopId.Append(virtualDesktopId[12..14]);
        convertedVirtualDesktopId.Append('-');
        convertedVirtualDesktopId.Append(virtualDesktopId[16..18]);
        convertedVirtualDesktopId.Append(virtualDesktopId[18..20]);
        convertedVirtualDesktopId.Append('-');
        convertedVirtualDesktopId.Append(virtualDesktopId[20..]);
        convertedVirtualDesktopId.Append('}');

        return convertedVirtualDesktopId.ToString();
    }
}
