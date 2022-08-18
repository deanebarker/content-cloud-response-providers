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
        private IResponseProviderCache _responseProviderCache;
        private IResponseProviderCommandManager _responseProviderCommandManager;
        private IResponseProviderTransformerManager _responseProviderTransformerManager;
        private IMimeTypeManager _mimeTypeManager;

        public ResponseProviderController(IResponseProviderCache responseProviderCache, IResponseProviderCommandManager responseProviderCommandManager, IResponseProviderTransformerManager responseProviderTransformerManager, IMimeTypeManager mimeTypeManager)
        {
            _responseProviderCache = responseProviderCache;
            _responseProviderCommandManager = responseProviderCommandManager;
            _responseProviderTransformerManager = responseProviderTransformerManager;
            _mimeTypeManager = mimeTypeManager;
        }

        public ActionResult Index(BaseResponseProvider currentPage)
        {
            var siteId = currentPage.ContentGuid;
            var path = Request.Path.ToString();

            ActionResult response;

            // Try to get the result from cache
            response = _responseProviderCache.Get(siteId, path);
            if (response != null) return response;

            // Is it a command?
            var commandResult = _responseProviderCommandManager.ProcessCommands(currentPage, path);
            if (commandResult != null)
            {
                return commandResult; // This is the result of command
            }

            // Nothing in cache, not a command, so generate a new result

            // Translate the path
            var effectivePath = currentPage.GetPathTranslator().GetTranslatedPath(currentPage, path);

            // Try to retrieve the actual bytes of what was requested
            var sourcePayload = currentPage.GetResponseProvider().GetSourcePayload(currentPage, effectivePath);
            if (sourcePayload.IsEmpty)
            {
                return new NotFoundResult(); // Hard 404; not found, and no 404 page
            }

            sourcePayload.Content = _responseProviderTransformerManager.Transform(sourcePayload.Content, effectivePath, currentPage, sourcePayload.ContentType);

            // Form the response
            if (_mimeTypeManager.IsText(sourcePayload.ContentType))
            {
                response = new ContentResult() { Content = Encoding.UTF8.GetString(sourcePayload.Content), ContentType = sourcePayload.ContentType, StatusCode = sourcePayload.StatusCode };
            }
            else
            {
                response = new FileContentResult(sourcePayload.Content, sourcePayload.ContentType);
                // No need to send a status code here? If we're sending back a file, then it has to be assumed we have a response?
                // ... is that true?
            }

            _responseProviderCache.Put(siteId, path, response);
            return response;
        }

    }
}
