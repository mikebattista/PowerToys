﻿// Copyright (c) Microsoft Corporation
// The Microsoft Corporation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System.ComponentModel;
using DeveloperCommandPalette;
using Microsoft.UI.Input;
using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using Microsoft.UI.Xaml.Controls.Primitives;
using Microsoft.UI.Xaml.Input;
using Microsoft.UI.Xaml.Navigation;
using Microsoft.Windows.CommandPalette.Extensions;

namespace WindowsCommandPalette.Views;

public sealed class MarkdownPageViewModel : PageViewModel
{
    internal IMarkdownPage Page => (IMarkdownPage)this.pageAction;

    internal string[] MarkdownContent = [string.Empty];

    internal string Title => Page.Title;

    private IEnumerable<ICommandContextItem> contextActions => Page.Commands.Where(i => i is ICommandContextItem).Select(i => (ICommandContextItem)i);

    internal bool HasMoreCommands => contextActions.Any();

    internal IList<ContextItemViewModel> ContextActions => contextActions.Select(a => new ContextItemViewModel(a)).ToList();

    public MarkdownPageViewModel(IMarkdownPage page)
        : base(page)
    {
    }

    internal async Task InitialRender(MarkdownPage markdownPage)
    {
        var t = new Task<string[]>(() => {
            return this.Page.Bodies();
        });
        t.Start();
        this.MarkdownContent = await t;
    }
}
