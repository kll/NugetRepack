// Copyright (c) Oak Aged LLC. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

namespace NugetRepack
{
    using System.IO;
    using System.Threading.Tasks;

    using Serilog;

    using Thinktecture.IO.Adapters;

    public class NugetRepacker : INugetRepacker
    {
        public NugetRepacker(
            IFileSystem fileSystem,
            IPackageFilenameParser parser,
            IZipper zipper,
            INuspecUpdater nuspecUpdater)
        {
            this.FileSystem = fileSystem;
            this.Parser = parser;
            this.Zipper = zipper;
            this.NuspecUpdater = nuspecUpdater;
        }

        private static ILogger Logger { get; } = Log.ForContext<NugetRepacker>();

        private IFileSystem FileSystem { get; }

        private INuspecUpdater NuspecUpdater { get; }

        private IPackageFilenameParser Parser { get; }

        private IZipper Zipper { get; }

        public async Task RepackPackage(string package, string? newPackageId, bool stripPrerelease)
        {
            Logger.Information("Repacking package: {Package}", package);

            var packageInfo = this.Parser.Parse(package);
            var version = stripPrerelease ? packageInfo.FullVersionWithoutPrerelease : packageInfo.FullVersion;

            var tempFolder = GenerateTemporaryPath();
            await this.Zipper.Unzip(package, tempFolder);

            await this.NuspecUpdater.UpdateNuspec(
                tempFolder,
                newPackageId,
                stripPrerelease ? packageInfo.FullVersionWithoutPrerelease : null);

            if (!string.IsNullOrWhiteSpace(newPackageId))
            {
                // New package ID means we also nead to update the nuspec filename.
                this.RenameNuspecFile(packageInfo, tempFolder, newPackageId);
            }

            var tempFile = GenerateTemporaryPath();
            await this.Zipper.Zip(tempFolder, tempFile);

            this.Cleanup(package, tempFolder);

            var newPackage = GetNewPackageName(package, newPackageId ?? packageInfo.Name, version);
            this.FileSystem.MoveFile(tempFile, newPackage);
            Logger.Information("New package is at: {Package}", newPackage);
        }

        private static string GenerateTemporaryPath()
        {
            return Path.Combine(Path.GetTempPath(), Path.GetRandomFileName());
        }

        private static string GetNewPackageName(string originalFile, string packageName, string newVersion)
        {
            var directoryInfo = new FileInfoAdapter(originalFile).Directory;

            if (directoryInfo == null)
            {
                throw new DirectoryNotFoundException($"Failed to find the directory for '{originalFile}'.");
            }

            var directory = directoryInfo.FullName;

            return Path.Combine(directory, $"{packageName}.{newVersion}.nupkg");
        }

        private void Cleanup(string oldPackage, string tempFolder)
        {
            Logger.Information("Cleaning up temporary files.");

            Logger.Debug("Deleting old package: {File}", oldPackage);
            this.FileSystem.DeleteFile(oldPackage);

            Logger.Debug("Deleting temp folder: {Folder}", tempFolder);
            this.FileSystem.DeleteDirectory(tempFolder);
        }

        private void RenameNuspecFile(PackageInfo packageInfo, string tempFolder, string newPackageId)
        {
            var originalNuspec = Path.Combine(tempFolder, $"{packageInfo.Name}.nuspec");
            var newNuspec = Path.Combine(tempFolder, $"{newPackageId}.nuspec");
            Logger.Debug("Renaming {OriginalNuspec} file to {NewNuspec}", originalNuspec, newNuspec);
            this.FileSystem.MoveFile(originalNuspec, newNuspec);
        }
    }
}
