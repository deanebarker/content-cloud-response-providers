using DeaneBarker.Optimizely.ResponseProviders.Models;
using DeaneBarker.Optimizely.ResponseProviders.Services;

namespace DeaneBarker.Optimizely.ResponseProviders
{
    public class FileSystemPathTranslator : SimplePathTranslator, IResponseProviderPathTranslator
    {
        // Public so you can change it if you like
        public new string DefaultDocument { get; set; } = "index.html";

        public new string GetTranslatedPath(BaseResponseProvider siteRoot, string requestedPath)
        {
            var relativePath = base.GetTranslatedPath(siteRoot, requestedPath);

            if (relativePath.EndsWith("/") || string.IsNullOrWhiteSpace(relativePath))
            {
                relativePath = string.Concat(relativePath, DefaultDocument);
            }

            return relativePath.Trim('/');
        }
    }
}
