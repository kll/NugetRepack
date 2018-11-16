namespace NugetRepack.UnitTests
{
    using System.Collections.Generic;
    using System.IO;
    using System.IO.Compression;
    using System.Threading.Tasks;

    using FluentAssertions;

    using Xunit;

    public class ZipperSolitaryTests
    {
        public ZipperSolitaryTests()
        {
            this.FileSystem = new FakeFileSystem();
            this.Target = new Zipper(this.FileSystem);
        }

        private FakeFileSystem FileSystem { get; }

        private Zipper Target { get; }

        [Fact]
        public async Task CanUnzipFile()
        {
            var zipFile = TestData.GetZipFile(
                new Dictionary<string, string>
                {
                    { "file.txt", "This is a text file." },
                    { Path.Combine("directory", "file.txt"), "This is another text file." },
                });
            this.FileSystem.AddFile("file.zip", zipFile);

            await this.Target.Unzip(@"file.zip", "directory");

            var rootFileContent = await this.FileSystem.ReadAllText(Path.Combine("directory", "file.txt"));
            rootFileContent.Should().Be("This is a text file.");
            var directoryFileContent =
                await this.FileSystem.ReadAllText(Path.Combine("directory", "directory", "file.txt"));
            directoryFileContent.Should().Be("This is another text file.");
        }

        [Fact]
        public async Task CanZipFile()
        {
            this.FileSystem.AddFile(Path.Combine("directory", "file.txt"), "This is a text file.");
            this.FileSystem.AddFile(
                Path.Combine("directory", "subdirectory", "file.txt"),
                "This is another text file.");

            await this.Target.Zip("directory", "file.zip");

            using (var stream = new MemoryStream(this.FileSystem.ReadAllBytes("file.zip")))
            {
                using (var archive = new ZipArchive(stream, ZipArchiveMode.Read))
                {
                    var rootFileEntry = archive.GetEntry("file.txt");
                    rootFileEntry.Should().NotBeNull();

                    using (var rootFileReader = new StreamReader(rootFileEntry.Open()))
                    {
                        var rootFileContent = rootFileReader.ReadToEnd();
                        rootFileContent.Should().Be("This is a text file.");
                    }

                    var directoryEntry = archive.GetEntry(Path.Combine("subdirectory", "file.txt"));
                    directoryEntry.Should().NotBeNull();

                    using (var directoryEntryReader = new StreamReader(directoryEntry.Open()))
                    {
                        var directoryFileContent = directoryEntryReader.ReadToEnd();
                        directoryFileContent.Should().Be("This is another text file.");
                    }
                }
            }
        }
    }
}
