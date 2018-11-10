namespace NugetRepack
{
    using System.Threading.Tasks;

    public interface INuspecUpdater
    {
        Task UpdateNuspec(string path, string currentVersion, string newVersion);
    }
}
