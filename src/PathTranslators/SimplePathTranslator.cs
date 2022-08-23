using DeaneBarker.Optimizely.ResponseProviders.Models;
using EPiServer.ServiceLocation;
using EPiServer.Web.Routing;

namespace DeaneBarker.Optimizely.ResponseProviders.PathTranslators
{
    public class SimplePathTranslator : IResponseProviderPathTranslator
    {
        public string NotFoundDocument => "404";

        public string DefaultDocument => string.Empty;

        private IUrlResolver _urlResolver;

        public SimplePathTranslator()
        {
            _urlResolver = ServiceLocator.Current.GetInstance<UrlResolver>();
        }

        public string GetTranslatedPath(BaseResponseProvider siteRoot, string requestedPath)
        {
            var pathToRoot = _urlResolver.GetUrl(siteRoot).TrimStart('/');
            requestedPath = requestedPath.TrimStart('/');

            string relativePath = "/";
            if (requestedPath != pathToRoot)
            {
                relativePath = requestedPath == string.Empty | requestedPath == "/" ? "/" : requestedPath.Substring(pathToRoot.Length, requestedPath.Length - pathToRoot.Length);
            }

            return relativePath;
        }
    }
}