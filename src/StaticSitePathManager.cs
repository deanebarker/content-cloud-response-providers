using EPiServer.Web.Routing;
using DeaneBarker.Optimizely.StaticSites.Services;
using DeaneBarker.Optimizely.StaticSites.Models;

namespace DeaneBarker.Optimizely.StaticSites
{
    public class StaticSitePathManager : IStaticSitePathManager
    {
        private IUrlResolver _urlResolver;

        public StaticSitePathManager(IUrlResolver urlResolver)
        {
            _urlResolver = urlResolver;
        }

        public string GetRelativePath(StaticSiteRoot siteRoot, string requestedPath)
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
                relativePath = string.Concat(relativePath, "index.html");
            }

            return relativePath.Trim('/');
        }
    }
}
