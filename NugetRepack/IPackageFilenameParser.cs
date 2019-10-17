// Copyright (c) Oak Aged LLC. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

namespace NugetRepack
{
    public interface IPackageFilenameParser
    {
        PackageInfo Parse(string package);
    }
}
