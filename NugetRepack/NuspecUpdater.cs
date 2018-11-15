namespace NugetRepack
{
    using System.IO;
    using System.Linq;
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

        public async Task UpdateNuspec(string path, string currentVersion, string newVersion)
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

            Logger.Verbose("Updating version in nuspec file: {File}", nuspec.FullName);

            var replaced = await this.FileSystem.ReplaceInFile(nuspec.FullName, currentVersion, newVersion);

            if (!replaced)
            {
                throw new NuspecNotUpdatedException(
                    $"Failed to update the version number in nuspec file: {nuspec.FullName}");
            }
        }
    }
}
