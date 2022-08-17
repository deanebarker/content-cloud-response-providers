using DeaneBarker.Optimizely.ResponseProviders.Models;
using EPiServer.Core.Routing;
using EPiServer.Core.Routing.Pipeline;
using Microsoft.AspNetCore.Http;

namespace DeaneBarker.Optimizely.ResponseProviders
{
    public class ResponseProviderPartialRouter : IPartialRouter<BaseResponseProvider, BaseResponseProvider>
    {

        private IHttpContextAccessor _httpContext;

        public ResponseProviderPartialRouter(IHttpContextAccessor httpContext)
        {
            _httpContext = httpContext;
        }


        public object RoutePartial(BaseResponseProvider content, UrlResolverContext urlResolverContext)
        {
            urlResolverContext.RemainingSegments = null;
            return content;
        }

        public PartialRouteData GetPartialVirtualPath(BaseResponseProvider content, UrlGeneratorContext urlGeneratorContext)
        {
            return new PartialRouteData
            {
                BasePathRoot = content.ContentLink,
                PartialVirtualPath = string.Empty
            };
        }
    }
}
