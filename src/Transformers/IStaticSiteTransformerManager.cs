using System.Collections.Generic;

namespace DeaneBarker.Optimizely.StaticSites.Transformers
{
    public interface IStaticSiteTransformerManager
    {
        List<ITransformer> Transformers { get; set; }

        byte[] Transform(byte[] content, string path, string mimeType);
    }
}