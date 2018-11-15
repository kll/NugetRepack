namespace NugetRepack.UnitTests
{
    using FluentAssertions;

    using Xunit;

    public class PackageFilenameParserSolitaryTests
    {
        [Theory]
        [InlineData("AwesomePackage.1.0.0.nupkg", "AwesomePackage", "1.0.0", "1.0.0")]
        [InlineData("AwesomePackage.1.0.0-beta.nupkg", "AwesomePackage", "1.0.0-beta", "1.0.0")]
        [InlineData("AwesomePackage.1.0.0-beta.2.nupkg", "AwesomePackage", "1.0.0-beta.2", "1.0.0")]
        [InlineData("AwesomePackage.1.0.0-beta.2+build.4.nupkg", "AwesomePackage", "1.0.0-beta.2+build.4", "1.0.0")]
        public void CanParsePackageName(string filename, string packageName, string currentVersion, string newVersion)
        {
            var target = new PackageFilenameParser();

            var parsed = target.Parse(filename);

            parsed.name.Should().Be(packageName);
            parsed.currentVersion.Should().Be(currentVersion);
            parsed.newVersion.Should().Be(newVersion);
        }
    }
}
