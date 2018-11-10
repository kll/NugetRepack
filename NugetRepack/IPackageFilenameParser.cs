namespace NugetRepack
{
    public interface IPackageFilenameParser
    {
        (string name, string currentVersion, string newVersion) Parse(string package);
    }
}
