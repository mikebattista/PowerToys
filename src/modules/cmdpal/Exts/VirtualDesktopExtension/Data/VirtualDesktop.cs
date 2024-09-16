// Copyright (c) Microsoft Corporation
// The Microsoft Corporation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System;

namespace VirtualDesktopExtension.Data;

internal sealed class VirtualDesktop
{
    public Guid ID { get; set; }

    public int Number { get; set; }

    public string Name { get; set; }
}
