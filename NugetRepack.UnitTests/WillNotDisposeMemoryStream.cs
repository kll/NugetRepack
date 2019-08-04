// Copyright (c) Oak Aged LLC. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

namespace NugetRepack.UnitTests
{
    using System.IO;

    public class WillNotDisposeMemoryStream : MemoryStream
    {
        protected override void Dispose(bool disposing)
        {
            // Do nothing!
        }
    }
}
