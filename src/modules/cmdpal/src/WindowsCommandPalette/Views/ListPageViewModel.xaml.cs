﻿// Copyright (c) Microsoft Corporation
// The Microsoft Corporation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System.Collections.ObjectModel;
using System.Runtime.InteropServices;
using DeveloperCommandPalette;
using Microsoft.UI.Dispatching;
using Microsoft.CmdPal.Extensions;
using Microsoft.CmdPal.Extensions.Helpers;

namespace WindowsCommandPalette.Views;

public sealed class ListPageViewModel : PageViewModel
{
    internal readonly ObservableCollection<SectionInfoList> Items = [];
    internal readonly ObservableCollection<SectionInfoList> FilteredItems = [];

    internal IListPage Page => (IListPage)this.pageAction;

    private bool isDynamic => Page is IDynamicListPage;

    private IDynamicListPage? dynamicPage => Page as IDynamicListPage;

    private readonly DispatcherQueue _dispatcherQueue = DispatcherQueue.GetForCurrentThread();
    internal string Query = string.Empty;

    public ListPageViewModel(IListPage page)
        : base(page)
    {
    }

    internal Task InitialRender()
    {
        return UpdateListItems();
    }

    internal async Task UpdateListItems()
    {
        // on main thread
        var t = new Task<ISection[]>(() =>
        {
            try
            {
                return dynamicPage != null ?
                    dynamicPage.GetItems(Query) :
                    this.Page.GetItems();
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine(ex);
                return [new ListSection()
                {
                    Title = "Error",
                    Items = [new ErrorListItem(ex)],
                }
                ];
            }
        });
        t.Start();
        var sections = await t;

        // still on main thread

        // TODO! For dynamic lists, we're clearing out the whole list of items
        // we already have, then rebuilding it. We shouldn't do that. We should
        // still use the results from GetItems and put them into the code in
        // UpdateFilter to intelligently add/remove as needed.
        // Items.Clear();
        // FilteredItems.Clear();
        Collection<SectionInfoList> newItems = new();

        var size = sections.Length;
        for (var sectionIndex = 0; sectionIndex < size; sectionIndex++)
        {
            var section = sections[sectionIndex];
            var sectionItems = new SectionInfoList(
                section,
                section.Items
                    .Where(i => i != null && !string.IsNullOrEmpty(i.Title))
                    .Select(i => new ListItemViewModel(i)));

            // var items = section.Items;
            // for (var i = 0; i < items.Length; i++) {
            //     ListItemViewModel vm = new(items[i]);
            //     Items.Add(vm);
            //     FilteredItems.Add(vm);
            // }
            newItems.Add(sectionItems);

            // Items.Add(sectionItems);
            // FilteredItems.Add(sectionItems);
        }

        ListHelpers.InPlaceUpdateList(Items, newItems);
        ListHelpers.InPlaceUpdateList(FilteredItems, newItems);
    }

    internal async Task<Collection<SectionInfoList>> GetFilteredItems(string query)
    {
        if (query == Query)
        {
            return FilteredItems;
        }

        Query = query;
        if (isDynamic)
        {
            await UpdateListItems();
            return FilteredItems;
        }
        else
        {
            // Static lists don't need to re-fetch the items
            if (string.IsNullOrEmpty(query))
            {
                return Items;
            }

            //// TODO! Probably bad that this turns list view models into listitems back to NEW view models
            // return ListHelpers.FilterList(Items.Select(vm => vm.ListItem), Query).Select(li => new ListItemViewModel(li)).ToList();
            try
            {
                var allFilteredItems = ListHelpers.FilterList(
                    Items
                        .SelectMany(section => section)
                        .Select(vm => vm.ListItem.Unsafe),
                    Query).Select(li => new ListItemViewModel(li));

                var newSection = new SectionInfoList(null, allFilteredItems);
                return [newSection];
            }
            catch (COMException ex)
            {
                return [new SectionInfoList(null, [new ListItemViewModel(new ErrorListItem(ex))])];
            }
        }
    }
}
