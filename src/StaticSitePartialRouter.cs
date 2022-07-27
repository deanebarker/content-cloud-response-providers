using DeaneBarker.Optimizely.StaticSites.Models;
using EPiServer.Core.Routing;
using EPiServer.Core.Routing.Pipeline;
using Microsoft.AspNetCore.Http;

namespace DeaneBarker.Optimizely.StaticSites
{
    public class StaticSitePartialRouter : IPartialRouter<StaticSiteRoot, StaticSiteRoot>
    {

        private IHttpContextAccessor _httpContext;

        public StaticSitePartialRouter(IHttpContextAccessor httpContext)
        {
            _httpContext = httpContext;
        }


        public object RoutePartial(StaticSiteRoot content, UrlResolverContext urlResolverContext)
        {
            urlResolverContext.RemainingSegments = null;
            return content;
        }

        public PartialRouteData GetPartialVirtualPath(StaticSiteRoot content, UrlGeneratorContext urlGeneratorContext)
        {
            return new PartialRouteData
            {
                BasePathRoot = content.ContentLink,
                PartialVirtualPath = string.Empty
            };
        }
    }
}
