using DeaneBarker.Optimizely.StaticSites.Models;

namespace DeaneBarker.Optimizely.StaticSites.Transformers
{
    public interface ITransformer
    {
        byte[] Transform(byte[] content, string path, StaticSiteRoot siteRoot, string mimeType);
    }
}