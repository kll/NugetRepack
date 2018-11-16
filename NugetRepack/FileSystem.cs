namespace NugetRepack
{
    using System;
    using System.IO;
    using System.Threading;
    using System.Threading.Tasks;

    using Thinktecture.IO;
    using Thinktecture.IO.Adapters;

    public class FileSystem : IFileSystem
    {
        public virtual void DeleteDirectory(string path)
        {
            var directory = this.GetDirectory(path);
            directory.Delete(true);
        }

        public virtual void DeleteFile(string fileName)
        {
            var file = this.GetFile(fileName);
            file.Delete();
        }

        public virtual IDirectoryInfo GetDirectory(string directoryPath)
        {
            return new DirectoryInfoAdapter(directoryPath);
        }

        public virtual IFileInfo GetFile(string filePath)
        {
            return new FileInfoAdapter(filePath);
        }

        public virtual string GetFullPath(string filePath)
        {
            return Path.GetFullPath(filePath);
        }

        public virtual async Task<string> ReadAllText(string path, CancellationToken cancellationToken = default)
        {
            var adapter = new FileAdapter();

            return await adapter.ReadAllTextAsync(path, cancellationToken);
        }

        public virtual async Task<bool> ReplaceInFile(string path, string findText, string replaceText)
        {
            var contents = await this.ReadAllText(path);

            if (!contents.Contains(findText))
            {
                return false;
            }

            contents = contents.Replace(findText, replaceText);

            await this.WriteAllText(path, contents);

            return true;
        }

        public virtual async Task WriteAllText(
            string path,
            string contents,
            CancellationToken cancellationToken = default)
        {
            var adapter = new FileAdapter();
            await adapter.WriteAllTextAsync(path, contents, cancellationToken);
        }
    }
}
