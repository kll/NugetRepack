namespace NugetRepack.UnitTests
{
    using System.IO;

    public class WillNotDisposeMemoryStream : MemoryStream
    {
        protected override void Dispose(bool disposing)
        {
            // Do nothing!
        }
    }
}
