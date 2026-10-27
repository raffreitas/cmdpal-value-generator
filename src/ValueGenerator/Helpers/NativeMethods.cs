// Copyright (c) Microsoft Corporation
// The Microsoft Corporation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System;
using System.Runtime.InteropServices;

namespace ValueGenerator.Helpers;

internal static partial class NativeMethods
{
    internal const int RpcSuccess = 0;
    internal const int RpcUuidLocalOnly = 1824;

    [LibraryImport("rpcrt4.dll")]
    internal static partial int UuidCreateSequential(out Guid guid);
}
