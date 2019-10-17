// Copyright (c) Oak Aged LLC. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

namespace NugetRepack
{
    using System.Threading;
    using System.Threading.Tasks;

    using Thinktecture.IO;

    public interface IFileSystem
    {
        void DeleteDirectory(string path);

        void DeleteFile(string fileName);

        void MoveFile(string sourceFile, string targetFile);

        IDirectoryInfo GetDirectory(string directoryPath);

        IFileInfo GetFile(string filePath);

        string GetFullPath(string filePath);

        Task<string> ReadAllText(string path, CancellationToken cancellationToken = default);

        Task WriteAllText(string path, string contents, CancellationToken cancellationToken = default);
    }
}
