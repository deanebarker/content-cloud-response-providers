using DeaneBarker.Optimizely.StaticSites.Models;

namespace DeaneBarker.Optimizely.StaticSites.Services
{
    public interface IStaticSitePathTranslator
    {
        string GetTranslatedPath(StaticSiteRoot siteRoot, string requestedPath);
    }
}