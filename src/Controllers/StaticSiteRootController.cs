using DeaneBarker.Optimizely.StaticSites.Models;
using DeaneBarker.Optimizely.StaticSites.Services;
using EPiServer;
using EPiServer.Core;
using EPiServer.Framework.Blobs;
using EPiServer.ServiceLocation;
using EPiServer.Web.Mvc;
using EPiServer.Web.Routing;
using Microsoft.AspNetCore.Mvc;
using System;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.Json;

namespace DeaneBarker.Optimizely.StaticSites.Controllers
{
    public class StaticSiteRootController : PageController<StaticSiteRoot>
    {
        private readonly string[] allowedExtensions = new[] { ".js", ".css", ".json" };
        private readonly IStaticSitePathTranslator _staticSitePathTranslator;
        private readonly IStaticResourceProvider _staticResourceRetriever;
        private readonly IStaticSiteCommandManager _staticSiteCommandManager;
        private readonly IMimeTypeManager _mimeTypeMap;
        private readonly IStaticSiteLog _logger;
        private readonly IStaticSiteCache _staticSiteCache;


        public StaticSiteRootController(IStaticSiteCache staticSiteCache, IStaticSiteLog logger, IMimeTypeManager mimeTypeMap, IStaticSiteCommandManager staticSiteCommandManager, IStaticSitePathTranslator staticSitePathTranslator, IStaticResourceProvider staticResourceRetriever)
        {
            _staticSitePathTranslator = staticSitePathTranslator;
            _staticResourceRetriever = staticResourceRetriever;
            _staticSiteCommandManager = staticSiteCommandManager;
            _mimeTypeMap = mimeTypeMap;
            _logger = logger;
            _staticSiteCache = staticSiteCache;
        }

        public ActionResult Index(StaticSiteRoot currentPage)
        {
            var siteId = currentPage.ContentGuid;
            var path = Request.Path.ToString();

            ActionResult response;

            // Try to get the result from cache
            response = _staticSiteCache.Get(siteId, path);
            if (response != null) return response;

            // Is it a command?
            var commandResult = _staticSiteCommandManager.ProcessCommands(currentPage, path);
            if (commandResult != null)
            {
                return commandResult; // This is the result of command
            }

            // Nothing in cache, not a command, so generate a new result
            var statusCode = 200; // Assumed until proven otherwise
            var effectivePath = _staticSitePathTranslator.GetTranslatedPath(currentPage, path);
            var contentType = _mimeTypeMap.GetMimeType(effectivePath); // By this point, the effectivePath should have file extension
            
            // Try to retrieve the actual bytes of what was requested
            var bytes = _staticResourceRetriever.GetBytesOfResource(currentPage, effectivePath);
            if (bytes == null)
            {
                // Didn't find the requested resource; try to rerieve a 404 page
                bytes = _staticResourceRetriever.GetBytesOfResource(currentPage, _staticSitePathTranslator.NotFoundDocument);
                if (bytes == null)
                {
                    return new NotFoundResult(); // Can't find the resource or a 404 page; give up
                }

                // If we got here, then we have a 404 page
                statusCode = 404;
                contentType = "text/html"; // I think this is a fair assumption for a 404 page?
            }
            
            // Form the response
            if (_mimeTypeMap.IsText(contentType))
            {
                 response = new ContentResult() { Content = Encoding.UTF8.GetString(bytes), ContentType = contentType, StatusCode = statusCode  };
            }
            else
            {
                response = new FileContentResult(bytes, contentType);
            }

            _staticSiteCache.Put(siteId, path, response);
            return response;
        }

    }
}
