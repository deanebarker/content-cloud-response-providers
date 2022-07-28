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
        private readonly IStaticResourceRetriever _staticResourceRetriever;
        private readonly IStaticSiteCommandManager _staticSiteCommandManager;
        private readonly IMimeTypeManager _mimeTypeMap;
        private readonly IStaticSiteLog _logger;
        private readonly IStaticSiteCache _staticSiteCache;


        public StaticSiteRootController(IStaticSiteCache staticSiteCache, IStaticSiteLog logger, IMimeTypeManager mimeTypeMap, IStaticSiteCommandManager staticSiteCommandManager, IStaticSitePathTranslator staticSitePathTranslator, IStaticResourceRetriever staticResourceRetriever)
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
            var effectivePath = _staticSitePathTranslator.GetTranslatedPath(currentPage, path);
            var bytes = _staticResourceRetriever.GetBytesOfResource(currentPage, effectivePath);
            if(bytes == null)
            {
                return new NotFoundResult();
            }      

            // Figure out what type of content it is (the effectivePath should always have a file extension)
            var contentType = _mimeTypeMap.GetMimeType(effectivePath);
            
            // Form the response
            if (_mimeTypeMap.IsText(contentType))
            {
                 response = new ContentResult() { Content = Encoding.UTF8.GetString(bytes), ContentType = contentType };
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
