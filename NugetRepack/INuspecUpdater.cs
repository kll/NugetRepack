// Copyright (c) Oak Aged LLC. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

namespace NugetRepack
{
    using System.Threading.Tasks;

    public interface INuspecUpdater
    {
        Task UpdateNuspec(string path, string currentVersion, string newVersion);
    }
}
