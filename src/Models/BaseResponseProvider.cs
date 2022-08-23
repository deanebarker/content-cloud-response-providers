using DeaneBarker.Optimizely.ResponseProviders.PathTranslators;
using DeaneBarker.Optimizely.ResponseProviders.SourceProviders;
using EPiServer.Core;

namespace DeaneBarker.Optimizely.ResponseProviders.Models
{
    public abstract class BaseResponseProvider : PageData
    {
        public abstract ISourceProvider GetResponseProvider();
        public abstract IResponseProviderPathTranslator GetPathTranslator();
    }
}
