using DeaneBarker.Optimizely.ResponseProviders.Models;
using DeaneBarker.Optimizely.ResponseProviders.Services;
using EPiServer.Core;
using EPiServer.Web.Routing;

namespace DeaneBarker.Optimizely.ResponseProviders
{
    public class FileSystemPathTranslator : IResponseProviderPathTranslator
    {
        // Public so you can change them if you like
        public string DefaultDocument { get; set; } = "index.html";
        public string NotFoundDocument { get; set; } = "404.html";


        private IUrlResolver _urlResolver;

        public FileSystemPathTranslator(IUrlResolver urlResolver)
        {
            _urlResolver = urlResolver;
        }

        public string GetTranslatedPath(BaseResponseProvider siteRoot, string requestedPath)
        {
            var pathToRoot = _urlResolver.GetUrl((PageData)siteRoot).TrimStart('/');
            requestedPath = requestedPath.TrimStart('/');

            string relativePath = "/";
            if (requestedPath != pathToRoot)
            {
                relativePath = requestedPath == string.Empty | requestedPath == "/" ? "/" : requestedPath.Substring(pathToRoot.Length, requestedPath.Length - pathToRoot.Length);
            }

            if (relativePath.EndsWith("/") || string.IsNullOrWhiteSpace(relativePath))
            {
                relativePath = string.Concat(relativePath, DefaultDocument);
            }

            return relativePath.Trim('/');
        }
    }
}
