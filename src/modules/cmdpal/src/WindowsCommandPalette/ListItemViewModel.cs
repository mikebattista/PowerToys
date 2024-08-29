// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

using Microsoft.UI.Xaml.Controls;
using Microsoft.Windows.CommandPalette.Extensions;
using System.ComponentModel;
using Microsoft.UI.Dispatching;
using CmdPal.Models;
using System.Runtime.InteropServices;

namespace DeveloperCommandPalette;

public sealed class ListItemViewModel : INotifyPropertyChanged, IDisposable
{
    private readonly DispatcherQueue DispatcherQueue;

    internal ExtensionObject<IListItem> ListItem { get; init; }

    internal string Title { get; private set; }

    internal string Subtitle { get; private set; }

    internal string Icon { get; private set; }

    internal Lazy<DetailsViewModel?> _Details;

    internal DetailsViewModel? Details => _Details.Value;

    public event PropertyChangedEventHandler? PropertyChanged;

    internal ICommand? DefaultAction
    {
        get
        {
            try
            {
                return ListItem.Unsafe.Command;
            }
            catch (COMException)
            {
                return null;
            }
        }
    }

    internal bool CanInvoke => DefaultAction != null && DefaultAction is IInvokableCommand or IPage;

    internal IconElement IcoElement => Microsoft.Terminal.UI.IconPathConverter.IconMUX(Icon);

    private IEnumerable<ICommandContextItem> contextActions
    {
        get
        {
            try
            {
                var item = ListItem.Unsafe;
                return item.MoreCommands == null ?
                    [] :
                    item.MoreCommands.Where(i => i is ICommandContextItem).Select(i => (ICommandContextItem)i);
            }
            catch (COMException)
            {
                /* log something */
                return [];
            }
        }
    }

    internal bool HasMoreCommands => contextActions.Any();

    internal TagViewModel[] Tags = [];

    internal bool HasTags => Tags.Length > 0;

    internal IList<ContextItemViewModel> ContextActions
    {
        get
        {
            try
            {
                var l = contextActions.Select(a => new ContextItemViewModel(a)).ToList();
                var def = DefaultAction;
                if (def!=null) l.Insert(0, new(def));
                return l;
            }
            catch (COMException)
            {
                /* log something */
                return [];
            }
        }
    }

    public ListItemViewModel(IListItem model)
    {
        model.PropChanged += ListItem_PropertyChanged;
        this.ListItem = new(model);
        this.Title = model.Title;
        this.Subtitle = model.Subtitle;
        this.Icon = model.Command.Icon.Icon;
        if (model.Tags != null)
        {
            this.Tags = model.Tags.Select(t => new TagViewModel(t)).ToArray();
        }

        this._Details = new(() => {
            try
            {
                var item = this.ListItem.Unsafe;
                return item.Details != null ? new(item.Details) : null;
            }
            catch (COMException)
            {
                /* log something */
                return null;
            }
        });

        this.DispatcherQueue = DispatcherQueue.GetForCurrentThread();
    }

    private void ListItem_PropertyChanged(object sender, Microsoft.Windows.CommandPalette.Extensions.PropChangedEventArgs args)
    {
        try{
            var item = ListItem.Unsafe;
            switch (args.PropertyName)
            {
                case "Name":
                case nameof(Title):
                    {
                        this.Title = item.Title;
                    }
                    break;
                case nameof(Subtitle):
                    {
                        this.Subtitle = item.Subtitle;
                    }
                    break;
                case "MoreCommands":
                    {
                        BubbleXamlPropertyChanged(nameof(HasMoreCommands));
                        BubbleXamlPropertyChanged(nameof(ContextActions));
                    }
                    break;
                case nameof(Icon):
                    {
                        this.Icon = item.Command.Icon.Icon;
                        BubbleXamlPropertyChanged(nameof(IcoElement));
                    }
                    break;
            }

            BubbleXamlPropertyChanged(args.PropertyName);

        } catch (COMException) {
            /* log something */
        }
    }

    private void BubbleXamlPropertyChanged(string propertyName)
    {
        if (this.DispatcherQueue == null)
        {
            // this is highly unusual
            return;
        }
        this.DispatcherQueue.TryEnqueue(DispatcherQueuePriority.Normal, () =>
        {
            this.PropertyChanged?.Invoke(this, new(propertyName));
        });
    }

    public void Dispose()
    {
        try{
            this.ListItem.Unsafe.PropChanged -= ListItem_PropertyChanged;
        } catch (COMException) {
            /* log something */
        }
    }
}
