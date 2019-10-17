// Copyright (c) Oak Aged LLC. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

namespace NugetRepack.UnitTests
{
    using FluentAssertions;

    using Xunit;

    public class PackageFilenameParserSolitaryTests
    {
        [Theory]
        [InlineData("AwesomePackage.1.0.0.nupkg", "AwesomePackage", "1.0.0")]
        [InlineData("AwesomePackage.1.0.0-beta.nupkg", "AwesomePackage", "1.0.0-beta")]
        [InlineData("AwesomePackage.1.0.0-beta.2.nupkg", "AwesomePackage", "1.0.0-beta.2")]
        [InlineData("AwesomePackage.1.0.0-beta.2+build.4.nupkg", "AwesomePackage", "1.0.0-beta.2+build.4")]
        public void CanParsePackageName(string filename, string packageName, string fullVersion)
        {
            var target = new PackageFilenameParser();

            var parsed = target.Parse(filename);

            parsed.Name.Should().Be(packageName);
            parsed.FullVersion.Should().Be(fullVersion);
        }
    }
}
