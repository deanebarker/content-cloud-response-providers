using DeaneBarker.Optimizely.ResponseProviders.Models;
using System.Collections.Generic;

namespace DeaneBarker.Optimizely.ResponseProviders.Transformers
{
    public interface IResponseProviderTransformerManager
    {
        List<ITransformer> Transformers { get; set; }

        byte[] Transform(byte[] content, string path, BaseResponseProvider siteRoot, string mimeType);
    }
}