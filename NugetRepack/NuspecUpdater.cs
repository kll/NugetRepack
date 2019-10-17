// Copyright (c) Oak Aged LLC. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

namespace NugetRepack
{
    using System.IO;
    using System.Linq;
    using System.Text.RegularExpressions;
    using System.Threading.Tasks;

    using Serilog;

    public class NuspecUpdater : INuspecUpdater
    {
        public NuspecUpdater(IFileSystem fileSystem)
        {
            this.FileSystem = fileSystem;
        }

        private static ILogger Logger { get; } = Log.ForContext<NuspecUpdater>();

        private IFileSystem FileSystem { get; }

        public async Task UpdateNuspec(string path, string? packageId, string? version)
        {
            var source = this.FileSystem.GetDirectory(path);
            var files = source.EnumerateFiles("*.nuspec", SearchOption.AllDirectories).ToList();

            if (files.Count == 0)
            {
                throw new MissingNuspecException();
            }

            if (files.Count > 1)
            {
                throw new MultipleNuspecException();
            }

            var nuspec = files.First();

            var content = await this.FileSystem.ReadAllText(nuspec.FullName);

            if (!string.IsNullOrWhiteSpace(packageId))
            {
                content = this.UpdatePackageId(content, packageId);
            }

            if (!string.IsNullOrWhiteSpace(version))
            {
                content = this.UpdateVersion(content, version);
            }

            await this.FileSystem.WriteAllText(nuspec.FullName, content);
        }

        private string UpdatePackageId(string content, string packageId)
        {
            var regex = new Regex(@"<id>(?<id>.*)</id>");
            var match = regex.Match(content);

            if (!match.Success || !match.Groups.ContainsKey("id"))
            {
                throw new NuspecNotUpdatedException($"Failed to locate the package ID in the nuspec file.");
            }

            Logger.Information(
                "Updating package ID in nuspec file from {CurrentId} to {NewId}",
                match.Groups["id"].Value,
                packageId);

            return regex.Replace(content, $"<id>{packageId}</id>");
        }

        private string UpdateVersion(string content, string version)
        {
            var regex = new Regex(@"<version>(?<version>.*)</version>");
            var match = regex.Match(content);

            if (!match.Success || !match.Groups.ContainsKey("version"))
            {
                throw new NuspecNotUpdatedException($"Failed to locate the version number in the nuspec file.");
            }

            Logger.Information(
                "Updating version in nuspec file from {CurrentVersion} to {NewVersion}",
                match.Groups["version"].Value,
                version);

            return regex.Replace(content, $"<version>{version}</version>");
        }
    }
}
