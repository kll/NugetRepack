namespace NugetRepack.UnitTests
{
    using System.Collections.Generic;
    using System.IO;
    using System.IO.Compression;
    using System.Text;
    using System.Threading.Tasks;

    using FluentAssertions;

    using Xunit;

    public class ZipperSolitaryTests
    {
        [Fact]
        public async Task CanUnzipFile()
        {
            var zipFile = TestData.GetZipFile(
                new Dictionary<string, string>
                {
                    { "file.txt", "This is a text file." },
                    { @"directory\file.txt", "This is another text file." }
                });
            var fileSystem = new FakeFileSystem(
                new Dictionary<string, byte[]>
                {
                    { @"C:\file.zip", zipFile }
                });
            var target = new Zipper(fileSystem);

            await target.Unzip(@"C:\file.zip", @"C:\");

            var rootFileContent = await fileSystem.ReadAllText(@"C:\file.txt");
            rootFileContent.Should().Be("This is a text file.");
            var directoryFileContent = await fileSystem.ReadAllText(@"C:\directory\file.txt");
            directoryFileContent.Should().Be("This is another text file.");
        }

        [Fact]
        public async Task CanZipFile()
        {
            var fileSystem = new FakeFileSystem(
                new Dictionary<string, byte[]>
                {
                    { @"C:\directory\file.txt", Encoding.UTF8.GetBytes("This is a text file.") },
                    { @"C:\directory\subdirectory\file.txt", Encoding.UTF8.GetBytes("This is another text file.") }
                });
            var target = new Zipper(fileSystem);

            await target.Zip(@"C:\directory", @"C:\file.zip");

            using (var stream = new MemoryStream(fileSystem.ReadAllBytes(@"C:\file.zip")))
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

                    var directoryEntry = archive.GetEntry(@"subdirectory\file.txt");
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
