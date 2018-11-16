namespace NugetRepack.UnitTests
{
    using System.Collections.Generic;
    using System.IO;
    using System.Threading.Tasks;

    using FluentAssertions;

    using Xunit;

    public class NuspecUpdaterSolitaryTests
    {
        public NuspecUpdaterSolitaryTests()
        {
            this.FileSystem = new FakeFileSystem();
            this.Target = new NuspecUpdater(this.FileSystem);
        }

        private static string OriginalNuspec => @"<?xml version='1.0' encoding='utf-8'?>
<package xmlns='http://schemas.microsoft.com/packaging/2012/06/nuspec.xsd'>
  <metadata>
    <id>NugetRepack.Tool</id>
    <version>0.1.5-rc.11</version>
  </metadata>
</package>";

        private static string UpdatedNuspec => @"<?xml version='1.0' encoding='utf-8'?>
<package xmlns='http://schemas.microsoft.com/packaging/2012/06/nuspec.xsd'>
  <metadata>
    <id>NugetRepack.Tool</id>
    <version>0.1.5</version>
  </metadata>
</package>";

        private FakeFileSystem FileSystem { get; }

        private NuspecUpdater Target { get; }

        [Fact]
        public async Task CanUpdateNuspec()
        {
            this.FileSystem.AddFile("file.nuspec", OriginalNuspec);

            await this.Target.UpdateNuspec("file.nuspec", "0.1.5-rc.11", "0.1.5");

            var result = await this.FileSystem.ReadAllText("file.nuspec");
            result.Should().Be(UpdatedNuspec);
        }
    }
}
