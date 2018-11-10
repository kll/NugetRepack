namespace NugetRepack.UnitTests
{
    using System.Collections.Generic;
    using System.IO;
    using System.IO.Compression;

    internal static class TestData
    {
        internal static byte[] GetZipFile(IReadOnlyDictionary<string, string> data)
        {
            using (var stream = new MemoryStream())
            {
                using (var archive = new ZipArchive(stream, ZipArchiveMode.Create))
                {
                    foreach (var keyPair in data)
                    {
                        var entry = archive.CreateEntry(keyPair.Key);

                        using (var writer = new StreamWriter(entry.Open()))
                        {
                            writer.Write(keyPair.Value);
                        }
                    }
                }

                return stream.ToArray();
            }
        }
    }
}
