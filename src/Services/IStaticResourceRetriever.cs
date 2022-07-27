using DeaneBarker.Optimizely.StaticSites.Models;
using System.Collections.Generic;

namespace DeaneBarker.Optimizely.StaticSites.Services
{
    public interface IStaticResourceRetriever
    {
        IEnumerable<string> GetResourceNames(StaticSiteRoot siteRoot);

        byte[] GetBytesOfResource(StaticSiteRoot siteRoot, string path);
    }
}