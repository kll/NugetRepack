namespace NugetRepack
{
    using System.Threading.Tasks;

    public interface INugetRepacker
    {
        Task RepackPackage(string package);
    }
}