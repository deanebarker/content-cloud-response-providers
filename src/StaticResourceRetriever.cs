using DeaneBarker.Optimizely.StaticSites.Services;
using System;
using System.Collections.Generic;
using System.IO;
using System.IO.Compression;
using System.Linq;

namespace DeaneBarker.Optimizely.StaticSites
{
    public class StaticResourceRetriever : IStaticResourceRetriever
    {
        public byte[] GetBytesOfResource(byte[] archiveBytes, string path)
        {
            var zip = new ZipArchive(new MemoryStream(archiveBytes));

            if (string.IsNullOrWhiteSpace(path))
            {
                throw new ArgumentNullException(nameof(path));
            }

            var entry = zip.GetEntry(path.Trim("/".ToCharArray()));
            if (entry == null)
            {
                return null;
            }

            var stream = entry.Open();

            byte[] buffer = new byte[16 * 1024];
            using (var memoryStream = new MemoryStream())
            {
                int read;
                while ((read = stream.Read(buffer, 0, buffer.Length)) > 0)
                {
                    memoryStream.Write(buffer, 0, read);
                }
                stream.Close();
                return memoryStream.ToArray();
            }
        }

        public IEnumerable<string> GetResourceNames(byte[] archiveBytes)
        {
            var zip = new ZipArchive(new MemoryStream(archiveBytes));
            return zip.Entries.Select(e => e.FullName);
        }
    }


}
