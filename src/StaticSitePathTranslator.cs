using EPiServer.Web.Routing;
using DeaneBarker.Optimizely.StaticSites.Services;
using DeaneBarker.Optimizely.StaticSites.Models;

namespace DeaneBarker.Optimizely.StaticSites
{
    public class StaticSitePathTranslator : IStaticSitePathTranslator
    {
        // Public so you can change them if you like
        public string DefaultDocument { get; set; } = "index.html";
        public string NotFoundDocument { get; set; } = "404.html";


        private IUrlResolver _urlResolver;

        public StaticSitePathTranslator(IUrlResolver urlResolver)
        {
            _urlResolver = urlResolver;
        }

        public string GetTranslatedPath(StaticSiteRoot siteRoot, string requestedPath)
        {
            var pathToRoot = _urlResolver.GetUrl(siteRoot).Trim('/');
            requestedPath = requestedPath.Trim('/');

            string relativePath = "/";
            if (requestedPath != pathToRoot)
            {
                relativePath = requestedPath == string.Empty | requestedPath == "/" ? "/" : requestedPath.Substring(pathToRoot.Length, requestedPath.Length - pathToRoot.Length);
            }

            if(relativePath.EndsWith("/") || string.IsNullOrWhiteSpace(relativePath))
            {
                relativePath = string.Concat(relativePath, DefaultDocument);
            }

            return relativePath.Trim('/');
        }
    }
}
