using DeaneBarker.Optimizely.ResponseProviders.Services;
using EPiServer.Core;

namespace DeaneBarker.Optimizely.ResponseProviders.Models
{
    public abstract class BaseResponseProvider : PageData
    {
        public abstract ISourceProvider GetResponseProvider();
    }
}
