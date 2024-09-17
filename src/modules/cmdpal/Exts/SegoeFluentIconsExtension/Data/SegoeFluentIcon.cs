// Copyright (c) Microsoft Corporation
// The Microsoft Corporation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SegoeFluentIconsExtension.Data;

internal sealed class SegoeFluentIcon
{
    internal string IconString { get; set; }

    internal string Name { get; set; }

    internal SegoeFluentIcon(string iconString, string name)
    {
        IconString = iconString;
        Name = name;
    }
}
