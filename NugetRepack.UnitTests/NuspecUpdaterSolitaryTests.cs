// Copyright (c) Oak Aged LLC. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

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

        private static string UpdatedId => @"<?xml version='1.0' encoding='utf-8'?>
<package xmlns='http://schemas.microsoft.com/packaging/2012/06/nuspec.xsd'>
  <metadata>
    <id>SomeOtherId</id>
    <version>0.1.5-rc.11</version>
  </metadata>
</package>";

        private static string UpdatedVersion => @"<?xml version='1.0' encoding='utf-8'?>
<package xmlns='http://schemas.microsoft.com/packaging/2012/06/nuspec.xsd'>
  <metadata>
    <id>NugetRepack.Tool</id>
    <version>0.1.5</version>
  </metadata>
</package>";

        private FakeFileSystem FileSystem { get; }

        private NuspecUpdater Target { get; }

        [Fact]
        public async Task CanUpdatePackageId()
        {
            this.FileSystem.AddFile("file.nuspec", OriginalNuspec);

            await this.Target.UpdateNuspec("file.nuspec", "SomeOtherId", null);

            var result = await this.FileSystem.ReadAllText("file.nuspec");
            result.Should().Be(UpdatedId);
        }

        [Fact]
        public async Task CanUpdateVersion()
        {
            this.FileSystem.AddFile("file.nuspec", OriginalNuspec);

            await this.Target.UpdateNuspec("file.nuspec", null, "0.1.5");

            var result = await this.FileSystem.ReadAllText("file.nuspec");
            result.Should().Be(UpdatedVersion);
        }
    }
}
