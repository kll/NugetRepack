namespace NugetRepack.UnitTests
{
    using System.Collections.Generic;
    using System.IO;
    using System.Linq;
    using System.Threading.Tasks;

    using Moq;

    using Thinktecture.IO;

    public sealed class FakeFileSystem : FileSystem
    {
        public FakeFileSystem(Dictionary<string, byte[]> content)
        {
            this.Content = content;
        }

        private Dictionary<string, byte[]> Content { get; }

        public override IDirectoryInfo GetDirectory(string path)
        {
            var mock = new Mock<IDirectoryInfo>();
            mock.Setup(directoryInfo => directoryInfo.EnumerateFiles(It.IsAny<string>(), It.IsAny<SearchOption>()))
                .Returns(
                    this.Content.Where(keyPair => keyPair.Key.StartsWith(path))
                        .Select(keyPair => this.GetFile(keyPair.Key)));

            return mock.Object;
        }

        public override IFileInfo GetFile(string fileName)
        {
            var mock = new Mock<IFileInfo>();
            mock.SetupGet(fileInfo => fileInfo.FullName)
                .Returns(fileName);
            mock.Setup(fileInfo => fileInfo.OpenRead())
                .Returns(
                    () =>
                    {
                        this.ThrowIfFileNotFound(fileName);

                        return new FakeFileStream(fileName, this.Content[fileName]);
                    });
            mock.Setup(fileInfo => fileInfo.OpenWrite())
                .Returns(
                    () =>
                    {
                        if (!this.Content.ContainsKey(fileName))
                        {
                            this.Content[fileName] = new byte[10 * 1024];
                        }

                        return new FakeFileStream(fileName, this.Content[fileName]);
                    });

            return mock.Object;
        }

        public override async Task<string> ReadAllText(string path)
        {
            // Trim the trailing nulls that are a result of the naive fake filesystem implementation.
            return (await base.ReadAllText(path)).TrimEnd('\0');
        }

        public byte[] ReadAllBytes(string fileName)
        {
            this.ThrowIfFileNotFound(fileName);

            return this.Content[fileName];
        }

        private void ThrowIfFileNotFound(string fileName)
        {
            if (!this.Content.ContainsKey(fileName))
            {
                throw new FileNotFoundException("File not found dummy!", fileName);
            }
        }
    }
}
