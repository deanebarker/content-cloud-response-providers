using DeaneBarker.Optimizely.StaticSites.Models;

namespace DeaneBarker.Optimizely.StaticSites.Services
{
    public interface IStaticSitePathManager
    {
        string GetRelativePath(StaticSiteRoot siteRoot, string requestedPath);
    }
}