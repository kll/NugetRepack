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
        public FakeFileStream(string name)
            : this(name, new MemoryStream())
        {
        }

        public FakeFileStream(string name, byte[] buffer)
            : this(name, new MemoryStream(buffer))
        {
        }

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
