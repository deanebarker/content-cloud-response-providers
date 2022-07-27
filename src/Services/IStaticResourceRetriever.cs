using System.Collections.Generic;

namespace DeaneBarker.Optimizely.StaticSites.Services
{
    public interface IStaticResourceRetriever
    {
        byte[] GetBytesOfResource(byte[] archiveBytes, string path);
        IEnumerable<string> GetResourceNames(byte[] archiveBytes);
    }
}