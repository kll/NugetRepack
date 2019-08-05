// Copyright (c) Oak Aged LLC. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

namespace NugetRepack.UnitTests
{
    using System.IO;
    using System.Threading.Tasks;

    using FluentAssertions;

    using Xunit;

    /// <summary>
    /// Tests for FileSystem that interact with other classes.
    /// </summary>
    /// <remark>
    /// I called these tests social because they interact with the real file
    /// system and classes from Thinktecture.IO. No other classes from this
    /// code base are used though so solitary could fit as well. It is a gray
    /// area.
    /// </remark>
    public class FileSystemSocialTests
    {
        public FileSystemSocialTests()
        {
            this.Target = new FileSystem();
        }

        private FileSystem Target { get; }

        [Fact]
        public void CanDeleteDirectory()
        {
            Directory.CreateDirectory("temp");

            this.Target.DeleteDirectory("temp");

            Directory.Exists("temp").Should().BeFalse("because delete directory was called");
        }

        [Fact]
        public void CanDeleteFile()
        {
            File.WriteAllText("temp.txt", "delete me if found");

            this.Target.DeleteFile("temp.txt");

            File.Exists("temp.txt").Should().BeFalse("because delete file was called");
        }

        [Fact]
        public void CanGetDirectory()
        {
            var directory = this.Target.GetDirectory("temp");

            directory.Should().NotBeNull();
        }

        [Fact]
        public void CanGetFile()
        {
            var file = this.Target.GetFile("temp.txt");

            file.Should().NotBeNull();
        }

        [Fact]
        public async Task CanReadAllText()
        {
            File.WriteAllText("temp.txt", "delete me if found");
            string result;

            try
            {
                result = await this.Target.ReadAllText("temp.txt");
            }
            finally
            {
                File.Delete("temp.txt");
            }

            result.Should().Be("delete me if found", "because that is what was written to the file");
        }

        [Fact]
        public async Task CanReplaceInFile()
        {
            File.WriteAllText("temp.txt", "delete this if found");
            bool result;
            string replaced;

            try
            {
                result = await this.Target.ReplaceInFile("temp.txt", "this", "me");
                replaced = File.ReadAllText("temp.txt");
            }
            finally
            {
                File.Delete("temp.txt");
            }

            result.Should().BeTrue("because that is returned when the text is found");
            replaced.Should().Be("delete me if found", "because that reflects the replaced content");
        }

        [Fact]
        public async Task CanWriteAllText()
        {
            string result;

            try
            {
                await this.Target.WriteAllText("temp.txt", "delete me if found");
                result = File.ReadAllText("temp.txt");
            }
            finally
            {
                File.Delete("temp.txt");
            }

            result.Should().Be("delete me if found", "because that is what was written to the file");
        }
    }
}
