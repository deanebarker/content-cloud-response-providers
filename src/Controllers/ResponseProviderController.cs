using DeaneBarker.Optimizely.ResponseProviders.Models;
using DeaneBarker.Optimizely.ResponseProviders.Services;
using DeaneBarker.Optimizely.ResponseProviders.Transformers;
using EPiServer.Web.Mvc;
using Microsoft.AspNetCore.Mvc;
using System.Text;

namespace DeaneBarker.Optimizely.ResponseProviders.Controllers
{
    public class ResponseProviderController : PageController<BaseResponseProvider>
    {
        private IResponseProviderCache _ResponseProviderCache;
        private IResponseProviderCommandManager _ResponseProviderCommandManager;
        private IResponseProviderTransformerManager _ResponseProviderTransformerManager;
        private IMimeTypeManager _mimeTypeManager;

        public ResponseProviderController(IResponseProviderCache ResponseProviderCache, IResponseProviderCommandManager ResponseProviderCommandManager, IResponseProviderTransformerManager ResponseProviderTransformerManager, IMimeTypeManager mimeTypeManager)
        {
            _ResponseProviderCache = ResponseProviderCache;
            _ResponseProviderCommandManager = ResponseProviderCommandManager;
            _ResponseProviderTransformerManager = ResponseProviderTransformerManager;
            _mimeTypeManager = mimeTypeManager;
        }

        public ActionResult Index(BaseResponseProvider currentPage)
        {
            var siteId = currentPage.ContentGuid;
            var path = Request.Path.ToString();

            ActionResult response;

            // Try to get the result from cache
            response = _ResponseProviderCache.Get(siteId, path);
            if (response != null) return response;

            // Is it a command?
            var commandResult = _ResponseProviderCommandManager.ProcessCommands(currentPage, path);
            if (commandResult != null)
            {
                return commandResult; // This is the result of command
            }

            // Nothing in cache, not a command, so generate a new result
            var statusCode = 200; // Assumed until proven otherwise
            var effectivePath = currentPage.GetPathTranslator().GetTranslatedPath(currentPage, path);

            // Try to retrieve the actual bytes of what was requested
            var sourcePayload = currentPage.GetResponseProvider().GetSourcePayload(currentPage, effectivePath);
            if (sourcePayload == SourcePayload.Empty)
            {
                // Didn't find the requested resource; try to rerieve a 404 page
                sourcePayload = currentPage.GetResponseProvider().GetSourcePayload(currentPage, currentPage.GetPathTranslator().NotFoundDocument);
                if (sourcePayload == SourcePayload.Empty)
                {
                    return new NotFoundResult(); // Can't find the resource or a 404 page; give up
                }

                // If we got here, then we have a 404 page
                statusCode = 404;
                sourcePayload.Content = new byte[0];
                sourcePayload.ContentType = "text/html"; // I think this is a fair assumption for a 404 page?
            }

            sourcePayload.Content = _ResponseProviderTransformerManager.Transform(sourcePayload.Content, effectivePath, currentPage, sourcePayload.ContentType);

            // Form the response
            if (_mimeTypeManager.IsText(sourcePayload.ContentType))
            {
                response = new ContentResult() { Content = Encoding.UTF8.GetString(sourcePayload.Content), ContentType = sourcePayload.ContentType, StatusCode = statusCode };
            }
            else
            {
                response = new FileContentResult(sourcePayload.Content, sourcePayload.ContentType);
            }

            _ResponseProviderCache.Put(siteId, path, response);
            return response;
        }

    }
}
