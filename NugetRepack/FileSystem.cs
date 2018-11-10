namespace NugetRepack
{
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

        public virtual IDirectoryInfo GetDirectory(string path)
        {
            return new DirectoryInfoAdapter(path);
        }

        public virtual IFileInfo GetFile(string fileName)
        {
            return new FileInfoAdapter(fileName);
        }

        public virtual async Task<string> ReadAllText(string path)
        {
            var file = this.GetFile(path);

            using (var stream = file.OpenRead())
            {
                using (var reader = new StreamReaderAdapter(stream))
                {
                    return await reader.ReadToEndAsync();
                }
            }
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

        public virtual async Task WriteAllText(string path, string content)
        {
            var file = this.GetFile(path);

            using (var stream = file.OpenWrite())
            {
                using (var writer = new StreamWriterAdapter(stream))
                {
                    await writer.WriteAsync(content);
                }
            }
        }
    }
}
