using DeaneBarker.Optimizely.ResponseProviders.Models;

namespace DeaneBarker.Optimizely.ResponseProviders.Transformers
{
    public interface ITransformer
    {
        byte[] Transform(byte[] content, string path, BaseResponseProvider siteRoot, string mimeType);
    }
}