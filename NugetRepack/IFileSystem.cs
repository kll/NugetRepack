namespace NugetRepack
{
    using System.Threading.Tasks;

    using Thinktecture.IO;

    public interface IFileSystem
    {
        void DeleteDirectory(string path);

        void DeleteFile(string fileName);

        IDirectoryInfo GetDirectory(string directoryPath);

        IFileInfo GetFile(string filePath);

        string GetFullPath(string filePath);

        Task<string> ReadAllText(string path);

        Task<bool> ReplaceInFile(string path, string findText, string replaceText);

        Task WriteAllText(string path, string content);
    }
}
