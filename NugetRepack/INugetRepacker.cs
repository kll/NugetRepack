// Copyright (c) Oak Aged LLC. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

namespace NugetRepack
{
    using System.Threading.Tasks;

    public interface INugetRepacker
    {
        Task RepackPackage(
            string package,
            string? newPackageId,
            bool stripPrerelease,
            string? additionalContentPath);
    }
}
