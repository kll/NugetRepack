namespace NugetRepack
{
    using System;
    using System.IO;
    using System.IO.Compression;
    using System.Threading;
    using System.Threading.Tasks;

    using JetBrains.Annotations;

    using Serilog;

    using Thinktecture;
    using Thinktecture.IO;
    using Thinktecture.IO.Adapters;

    public class Zipper : IZipper
    {
        public Zipper([NotNull] IFileSystem fileSystem)
        {
            this.FileSystem = fileSystem;
        }

        private static ILogger Logger { get; } = Log.ForContext<Zipper>();

        private IFileSystem FileSystem { get; }

        public async Task Unzip(
            string zipFile,
            string targetDirectory,
            CancellationToken cancellationToken = default(CancellationToken))
        {
            targetDirectory = this.FileSystem.GetFullPath(targetDirectory);
            var directoryInfo = this.FileSystem.GetDirectory(targetDirectory);

            if (!directoryInfo.Exists)
            {
                Logger.Verbose("Creating target directory: {Directory}", targetDirectory);
                directoryInfo.Create();
            }

            Logger.Verbose("Extracting archive to directory: {Directory}", targetDirectory);

            var source = ((IStream)this.FileSystem.GetFile(zipFile).OpenRead()).ToImplementation();

            if (source == null)
            {
                throw new ArgumentException("ZIP file not found", nameof(zipFile));
            }

            using (var archive = new ZipArchive(source, ZipArchiveMode.Read))
            {
                foreach (var entry in archive.Entries)
                {
                    await this.Extract(entry, targetDirectory, cancellationToken);
                }
            }
        }

        public async Task Zip(
            string rootDirectory,
            string zipFile,
            CancellationToken cancellationToken = default)
        {
            var target = ((IStream)this.FileSystem.GetFile(zipFile).OpenWrite()).ToImplementation();

            if (target == null)
            {
                throw new ArgumentException("ZIP file could not be created", nameof(zipFile));
            }

            rootDirectory = this.FileSystem.GetFullPath(rootDirectory);
            var source = this.FileSystem.GetDirectory(rootDirectory);
            var files = source.EnumerateFiles("*", SearchOption.AllDirectories);

            using (var archive = new ZipArchive(target, ZipArchiveMode.Create))
            {
                foreach (var file in files)
                {
                    var relativePath = Path.GetRelativePath(rootDirectory, file.FullName);
                    var entry = archive.CreateEntry(relativePath);
                    await this.Compress(entry, file, cancellationToken);
                }
            }
        }

        private static string RemoveRelativeSegments(string targetDirectory, string path)
        {
            var combined = Path.Combine(targetDirectory, path);
            return Path.GetFullPath(combined);
        }

        private static bool VerifyPathIsSafe(string targetDirectory, string destination)
        {
            // Use Ordinal match because case-sensitive volumes can be mounted
            // within volumes that are case-insensitive.
            return destination.StartsWith(targetDirectory, StringComparison.Ordinal);
        }

        private async Task Compress(ZipArchiveEntry entry, IFileInfo sourceFile, CancellationToken cancellationToken)
        {
            Logger.Verbose("Compressing file: {File}", sourceFile.FullName);

            using (var source = sourceFile.OpenRead())
            {
                using (var destination = entry.Open())
                {
                    await source.CopyToAsync(destination, cancellationToken);
                }
            }
        }

        private async Task Extract(ZipArchiveEntry entry, string targetDirectory, CancellationToken cancellationToken)
        {
            Logger.Verbose("Extracting file: {File}", entry.FullName);

            var destinationPath = RemoveRelativeSegments(targetDirectory, entry.FullName);

            if (!VerifyPathIsSafe(targetDirectory, destinationPath))
            {
                Logger.Warning("Skipping potentially malicious file: {File}", entry.FullName);

                return;
            }

            using (var source = new StreamAdapter(entry.Open()))
            {
                var file = this.FileSystem.GetFile(destinationPath);
                var directory = file.Directory;

                if (directory?.Exists == false)
                {
                    directory.Create();
                }

                using (var destination = file.OpenWrite())
                {
                    await source.CopyToAsync(destination, cancellationToken);
                }
            }
        }
    }
}
