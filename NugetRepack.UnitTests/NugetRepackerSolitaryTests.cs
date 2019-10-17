// Copyright (c) Oak Aged LLC. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

namespace NugetRepack.UnitTests
{
    using System.Threading.Tasks;

    using Moq;

    using Xunit;

    public class NugetRepackerSolitaryTests
    {
        public NugetRepackerSolitaryTests()
        {
            this.FileSystem = new FakeFileSystem();
            this.NuspecUpdaterMock = new Mock<INuspecUpdater>();
            this.ParserMock = new Mock<IPackageFilenameParser>();
            this.ZipperMock = new Mock<IZipper>();

            this.Target = new NugetRepacker(
                this.FileSystem,
                this.ParserMock.Object,
                this.ZipperMock.Object,
                this.NuspecUpdaterMock.Object);
        }

        private FakeFileSystem FileSystem { get; }

        private Mock<INuspecUpdater> NuspecUpdaterMock { get; }

        private Mock<IPackageFilenameParser> ParserMock { get; }

        private NugetRepacker Target { get; set; }

        private Mock<IZipper> ZipperMock { get; }

        [Fact]
        public async Task StripsPrereleaseWhenAsked()
        {
            this.ParserMock
                .Setup(parser => parser.Parse(It.IsAny<string>()))
                .Returns(new PackageInfo("AwesomePackage", "1.0.0", "beta.2", "build.4"));

            this.NuspecUpdaterMock
                .Setup(updater => updater.UpdateNuspec(It.IsAny<string>(), It.IsAny<string?>(), "1.0.0+build.4"))
                .Verifiable();

            await this.Target.RepackPackage("/tmp/AwesomePackage.1.0.0-beta.2+build.4.nupkg", null, true, null);

            this.NuspecUpdaterMock.Verify();
        }

        [Fact]
        public async Task DoesNotUpdateVersionWhenStripPrereleaseIsFalse()
        {
            this.ParserMock
                .Setup(parser => parser.Parse(It.IsAny<string>()))
                .Returns(new PackageInfo("AwesomePackage", "1.0.0", "beta.2", "build.4"));

            this.NuspecUpdaterMock
                .Setup(updater => updater.UpdateNuspec(It.IsAny<string>(), It.IsAny<string?>(), null))
                .Verifiable();

            await this.Target.RepackPackage("/tmp/AwesomePackage.1.0.0-beta.2+build.4.nupkg", null, false, null);

            this.NuspecUpdaterMock.Verify();
        }

        [Fact]
        public async Task UpdatesPackageIdWhenAsked()
        {
            this.ParserMock
                .Setup(parser => parser.Parse(It.IsAny<string>()))
                .Returns(new PackageInfo("AwesomePackage", "1.0.0", "beta.2", "build.4"));

            this.NuspecUpdaterMock
                .Setup(updater => updater.UpdateNuspec(It.IsAny<string>(), "AnotherAwesomePackage", It.IsAny<string?>()))
                .Verifiable();

            await this.Target.RepackPackage(
                "/tmp/AwesomePackage.1.0.0-beta.2+build.4.nupkg",
                "AnotherAwesomePackage",
                false,
                null);

            this.NuspecUpdaterMock.Verify();
        }

        [Fact]
        public async Task DoesNotUpdatePackageIdWhenNotAsked()
        {
            this.ParserMock
                .Setup(parser => parser.Parse(It.IsAny<string>()))
                .Returns(new PackageInfo("AwesomePackage", "1.0.0", "beta.2", "build.4"));

            this.NuspecUpdaterMock
                .Setup(updater => updater.UpdateNuspec(It.IsAny<string>(), null, It.IsAny<string?>()))
                .Verifiable();

            await this.Target.RepackPackage("/tmp/AwesomePackage.1.0.0-beta.2+build.4.nupkg", null, false, null);

            this.NuspecUpdaterMock.Verify();
        }
    }
}
