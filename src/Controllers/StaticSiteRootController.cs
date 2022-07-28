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
        private readonly IStaticSitePathManager _staticSitePathManager;
        private readonly IStaticResourceRetriever _staticResourceRetriever;
        private readonly IStaticSiteCommandManager _staticSiteCommandManager;
        private readonly IMimeTypeMap _mimeTypeMap;
        private readonly IStaticSiteLog _logger;


        public StaticSiteRootController(IStaticSiteLog logger, IMimeTypeMap mimeTypeMap, IStaticSiteCommandManager staticSiteCommandManager, IStaticSitePathManager staticSitePathManager, IStaticResourceRetriever staticResourceRetriever)
        {
            _staticSitePathManager = staticSitePathManager;
            _staticResourceRetriever = staticResourceRetriever;
            _staticSiteCommandManager = staticSiteCommandManager;
            _mimeTypeMap = mimeTypeMap;
            _logger = logger;
        }

        public ActionResult Index(StaticSiteRoot currentPage)
        {
            var siteId = currentPage.ContentGuid;
            var path = Request.Path.ToString();

            ActionResult response;

            // Try to get the result from cache
            response = StaticSiteCache.Instance.Get(siteId, path);
            if (response != null) return response;

            // Is it a command?
            var commandResult = _staticSiteCommandManager.ProcessCommands(currentPage, path);
            if (commandResult != null)
            {
                return commandResult; // This is the result of command
            }

            // Nothing in cache, not a command, so generate a new result
            var effectivePath = _staticSitePathManager.GetRelativePath(currentPage, path);
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

            StaticSiteCache.Instance.Put(siteId, path, response);
            return response;
        }


        // Commands --

        private string ShowAsset(StaticSiteRoot currentPage, string path)
        {
            var extension = Path.GetExtension(path);
            if(!allowedExtensions.Contains(extension))
            {
                throw new ContentNotFoundException();
            }

            var contentAssetHelper = ServiceLocator.Current.GetInstance<ContentAssetHelper>();
            var assetFolder = contentAssetHelper.GetOrCreateAssetFolder(((IContent)currentPage).ContentLink);
            var repo = ServiceLocator.Current.GetInstance<IContentRepository>();
            var assets = repo.GetChildren<MediaData>(assetFolder.ContentLink);
            var asset = assets.FirstOrDefault(a => a.Name == Path.GetFileName(path));

            if(asset == null)
            {
                throw new ContentNotFoundException();
            }

            return Encoding.UTF8.GetString(asset.BinaryData.ReadAllBytes());
        }

        private string ShowContext(StaticSiteRoot staticSiteRoot)
        {
            var urlResolver = ServiceLocator.Current.GetInstance<IUrlResolver>();

            var contextVars = new
            {
                rootId = staticSiteRoot.ContentLink.ID,
                baseUrl = urlResolver.GetUrl(staticSiteRoot),
                userName = Request.HttpContext.User?.Identity?.Name
            };

            return JsonSerializer.Serialize(contextVars);
        }

        private string ShowContents(StaticSiteRoot currentPage)
        {
            var resources = _staticResourceRetriever.GetResourceNames(currentPage);

            var sb = new StringBuilder();
            sb.AppendLine($"Total resources: {resources.Count()}");
            sb.AppendLine();
            foreach (var resource in resources)
            {
                sb.AppendLine(resource);
            }

            return sb.ToString();
        }

        private string ShowCache(StaticSiteRoot currentPage)
        {
            var lines = StaticSiteCache.Instance.Show(currentPage.ContentGuid);
            return string.Join(Environment.NewLine, lines);
        }

        private string ClearCache(StaticSiteRoot currentPage)
        {
            StaticSiteCache.Instance.Clear(currentPage.ContentGuid);
            return "Cache cleared";
        }
    }
}
