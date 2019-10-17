// Copyright (c) Oak Aged LLC. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

namespace NugetRepack
{
    using System;
    using System.IO;
    using System.Text.RegularExpressions;

    public class PackageFilenameParser : IPackageFilenameParser
    {
        private static Regex PackageRegex { get; } = new Regex(
            @"^(?<name>[0-9A-Za-z-\.]+)\.(?<version>\d+\.\d+\.\d+)(-(?<prerelease>[0-9A-Za-z-\.]+))?(\+(?<metadata>[0-9A-Za-z-\.]+))?$",
            RegexOptions.ExplicitCapture);

        public PackageInfo Parse(string package)
        {
            var baseName = Path.GetFileNameWithoutExtension(package);
            var match = PackageRegex.Match(baseName);
            if (!match.Success)
            {
                throw new ArgumentOutOfRangeException(
                    $"Failed to parse the version number from package: {package}",
                    nameof(package));
            }

            var packageName = match.Groups["name"].Value;
            var version = match.Groups["version"].Value;
            var prerelease = match.Groups["prerelease"].Value;
            var metadata = match.Groups["metadata"].Value;

            var packageInfo = new PackageInfo(packageName, version, prerelease, metadata);

            return packageInfo;
        }
    }
}
