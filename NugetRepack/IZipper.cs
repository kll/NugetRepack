// Copyright (c) Oak Aged LLC. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

namespace NugetRepack
{
    using System.Collections.Generic;
    using System.IO;
    using System.Threading;
    using System.Threading.Tasks;

    using JetBrains.Annotations;

    public interface IZipper
    {
        Task Unzip(
            [NotNull] string zipFile,
            [NotNull] string targetDirectory,
            CancellationToken cancellationToken = default(CancellationToken));

        Task Zip(
            [NotNull] string rootDirectory,
            [NotNull] string zipFile,
            CancellationToken cancellationToken = default(CancellationToken));
    }
}
