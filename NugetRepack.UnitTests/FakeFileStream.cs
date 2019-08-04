// Copyright (c) Oak Aged LLC. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

namespace NugetRepack.UnitTests
{
    using System;
    using System.IO;

    using Thinktecture;
    using Thinktecture.IO;
    using Thinktecture.IO.Adapters;
    using Thinktecture.Win32.SafeHandles;

    public class FakeFileStream : MemoryStreamAdapter, IFileStream
    {
        public FakeFileStream(string name, MemoryStream stream)
            : base(stream)
        {
            this.Name = name;
            this.IsAsync = false;
        }

        public MemoryStream Contents => this.Implementation;

        Stream IAbstraction<Stream>.UnsafeConvert()
        {
            return this.Implementation;
        }

        public new FileStream UnsafeConvert()
        {
            throw new NotImplementedException();
        }

        public void Flush(bool flushToDisk)
        {
        }

        public void Lock(long position, long length)
        {
        }

        public void Unlock(long position, long length)
        {
        }

        public bool IsAsync { get; }

        public string Name { get; }

        public ISafeFileHandle SafeFileHandle => throw new NotImplementedException();
    }
}
