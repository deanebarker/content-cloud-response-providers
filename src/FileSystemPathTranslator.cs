using DeaneBarker.Optimizely.ResponseProviders.Models;
using DeaneBarker.Optimizely.ResponseProviders.Services;
using EPiServer.Core;
using EPiServer.ServiceLocation;
using EPiServer.Web.Routing;

namespace DeaneBarker.Optimizely.ResponseProviders
{
    public class FileSystemPathTranslator : SimplePathTranslator, IResponseProviderPathTranslator
    {
        // Public so you can change them if you like
        public new string DefaultDocument { get; set; } = "index.html";
        public new string NotFoundDocument { get; set; } = "404.html";


        private IUrlResolver _urlResolver;

        public FileSystemPathTranslator()
        {
            _urlResolver = ServiceLocator.Current.GetInstance<UrlResolver>();
        }

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
