// Copyright (c) Oak Aged LLC. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

namespace NugetRepack.UnitTests
{
    using System.Collections.Generic;
    using System.IO;
    using System.Linq;
    using System.Text;
    using System.Threading;
    using System.Threading.Tasks;

    using Moq;

    using Thinktecture.IO;
    using Thinktecture.IO.Adapters;

    public sealed class FakeFileSystem : FileSystem
    {
        public FakeFileSystem()
            : this(Path.GetPathRoot(Path.GetFullPath(Directory.GetCurrentDirectory())))
        {
        }

        public FakeFileSystem(string root)
        {
            this.Root = root;
            this.Content = new Dictionary<string, WillNotDisposeMemoryStream>();
        }

        private string Root { get; }

        private Dictionary<string, WillNotDisposeMemoryStream> Content { get; }

        public override IDirectoryInfo GetDirectory(string directoryPath)
        {
            directoryPath = Path.GetFullPath(directoryPath, this.Root);

            var mock = new Mock<IDirectoryInfo>();
            mock.Setup(directoryInfo => directoryInfo.EnumerateFiles(It.IsAny<string>(), It.IsAny<SearchOption>()))
                .Returns(
                    this.Content.Where(keyPair => keyPair.Key.StartsWith(directoryPath))
                        .Select(keyPair => this.GetFile(keyPair.Key)));

            return mock.Object;
        }

        public override IFileInfo GetFile(string filePath)
        {
            filePath = Path.GetFullPath(filePath, this.Root);

            var mock = new Mock<IFileInfo>();
            mock.SetupGet(fileInfo => fileInfo.FullName)
                .Returns(filePath);
            mock.Setup(fileInfo => fileInfo.OpenRead())
                .Returns(
                    () =>
                    {
                        this.ThrowIfFileNotFound(filePath);

                        var stream = this.Content[filePath];
                        stream.Position = 0;

                        return new FakeFileStream(filePath, stream);
                    });
            mock.Setup(fileInfo => fileInfo.OpenWrite())
                .Returns(
                    () =>
                    {
                        if (!this.Content.ContainsKey(filePath))
                        {
                            this.Content[filePath] = new WillNotDisposeMemoryStream();
                        }

                        var stream = this.Content[filePath];
                        stream.Position = 0;

                        return new FakeFileStream(filePath, stream);
                    });

            return mock.Object;
        }

        public override string GetFullPath(string filePath)
        {
            return Path.GetFullPath(filePath, this.Root);
        }

        public override Task<string> ReadAllText(string filePath, CancellationToken cancellationToken = default)
        {
            var file = this.GetFile(filePath);

            using (var reader = new StreamReaderAdapter(file.OpenRead()))
            {
                return reader.ReadToEndAsync();
            }
        }

        public override async Task WriteAllText(string filePath, string contents, CancellationToken cancellationToken = default)
        {
            var memoryStream = new WillNotDisposeMemoryStream();

            using (var writer = new StreamWriter(memoryStream))
            {
                await writer.WriteAsync(contents);
            }

            memoryStream.Position = 0;

            this.Content[this.GetFullPath(filePath)] = memoryStream;
        }

        public void AddFile(string filePath, string contents)
        {
            var file = this.GetFile(filePath);
            var stream = file.OpenWrite();

            using (var writer = new StreamWriterAdapter(stream, Encoding.UTF8))
            {
                writer.Write(contents);
            }
        }

        public void AddFile(string filePath, byte[] contents)
        {
            var file = this.GetFile(filePath);
            var fileStream = file.OpenWrite();

            using (var memoryStream = new MemoryStreamAdapter(contents))
            {
                memoryStream.CopyTo(fileStream);
            }
        }

        public byte[] ReadAllBytes(string filePath)
        {
            var file = this.GetFile(filePath);
            var fileStream = file.OpenRead();

            using (var memoryStream = new MemoryStreamAdapter())
            {
                fileStream.CopyTo(memoryStream);

                return memoryStream.ToArray();
            }
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
