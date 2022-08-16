using DeaneBarker.Optimizely.StaticSites.Models;
using System.Collections.Generic;

namespace DeaneBarker.Optimizely.StaticSites.Transformers
{
    public class StaticSiteTransformerManager : IStaticSiteTransformerManager
    {
        public List<ITransformer> Transformers { get; set; } = new();

        public byte[] Transform(byte[] content, string path, StaticSiteRoot siteRoot, string mimeType)
        {
            foreach (var transformer in Transformers)
            {
                content = transformer.Transform(content, path, siteRoot, mimeType);
            }

            return content;
        }
    }
}
