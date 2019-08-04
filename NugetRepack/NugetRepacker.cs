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

        public async Task RepackPackage(string package)
        {
            Logger.Verbose("Repacking package: {Package}", package);

            var (packageName, currentVersion, newVersion) = this.Parser.Parse(package);
            Logger.Information(
                "Changing version of package '{PackageName}' from '{CurrentVersion}' to '{NewVersion}'",
                packageName,
                currentVersion,
                newVersion);
            var tempFolder = GenerateTemporaryDirectoryName();
            await this.Zipper.Unzip(package, tempFolder);
            await this.NuspecUpdater.UpdateNuspec(tempFolder, currentVersion, newVersion);
            var newPackage = GetNewPackageName(package, packageName, newVersion);
            await this.Zipper.Zip(tempFolder, newPackage);
            this.Cleanup(package, tempFolder);
        }

        private static string GenerateTemporaryDirectoryName()
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
            Logger.Verbose("Deleting old package: {File}", oldPackage);
            this.FileSystem.DeleteFile(oldPackage);

            Logger.Verbose("Deleting temp folder: {Folder}", tempFolder);
            this.FileSystem.DeleteDirectory(tempFolder);
        }
    }
}
