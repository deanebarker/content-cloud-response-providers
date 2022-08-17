using DeaneBarker.Optimizely.ResponseProviders.Models;
using System.Collections.Generic;

namespace DeaneBarker.Optimizely.ResponseProviders.Services
{
    public interface ISourceProvider
    {
        IEnumerable<string> GetResourceNames(BaseResponseProvider siteRoot);

        SourcePayload GetSourcePayload(BaseResponseProvider siteRoot, string path);
    }
}