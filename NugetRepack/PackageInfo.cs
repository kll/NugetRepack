// Copyright (c) Oak Aged LLC. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

namespace NugetRepack
{
    public sealed class PackageInfo
    {
        public PackageInfo(string name, string version, string? prerelease = null, string? metadata = null)
        {
            this.Name = name;
            this.Version = version;
            this.Prerelease = prerelease;
            this.Metadata = metadata;
        }

        public string FullVersion
        {
            get
            {
                var version = this.Version;

                if (!string.IsNullOrWhiteSpace(this.Prerelease))
                {
                    version = $"{version}-{this.Prerelease}";
                }

                if (!string.IsNullOrWhiteSpace(this.Metadata))
                {
                    version = $"{version}+{this.Metadata}";
                }

                return version;
            }
        }

        public string FullVersionWithoutPrerelease
        {
            get
            {
                var version = this.Version;

                if (!string.IsNullOrWhiteSpace(this.Metadata))
                {
                    version = $"{version}+{this.Metadata}";
                }

                return version;
            }
        }

        public string? Metadata { get; }

        public string Name { get; }

        public string? Prerelease { get; }

        public string Version { get; }
    }
}
