﻿// Copyright (c) Microsoft Corporation
// The Microsoft Corporation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System;
using System.Data;
using Microsoft.CmdPal.Extensions;
using Microsoft.CmdPal.Extensions.Helpers;

// To learn more about WinUI, the WinUI project structure,
// and more about our project templates, see: http://aka.ms/winui-project-info.
namespace Microsoft.CmdPal.Ext.Calc;

public class CalculatorTopLevelListItem : ListItem, IFallbackHandler
{
    public CalculatorTopLevelListItem()
        : base(new CalculatorAction())
    {
        // In the case of the calculator, the ListItem itself is the fallback
        // handler, so that it can update it's Title and Subtitle accodingly.
        FallbackHandler = this;
        Subtitle = "Type an equation";
    }

    public void UpdateQuery(string query)
    {
        if (string.IsNullOrEmpty(query) || query == "=")
        {
            Title = "=";
        }
        else if (query.StartsWith('='))
        {
            Title = ParseQuery(query);
            Subtitle = query;
        }
        else
        {
            Title = string.Empty;
        }
    }

    private string ParseQuery(string query)
    {
        var equation = query.Substring(1);
        try
        {
            var result = new DataTable().Compute(equation, null);
            var resultString = result.ToString() ?? string.Empty;
            ((CalculatorAction)Command).SetResult(resultString, true);

            return resultString;
        }
        catch (Exception e)
        {
            ((CalculatorAction)Command).SetResult(string.Empty, false);

            return $"Error: {e.Message}";
        }
    }
}
