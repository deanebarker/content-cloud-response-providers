using DeaneBarker.Optimizely.StaticSites.Models;

namespace DeaneBarker.Optimizely.StaticSites.Services
{
    public interface IStaticSourceLocator
    {
        byte[] GetBytesOfSource(StaticSiteRoot siteRoot);
    }
}