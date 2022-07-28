using EPiServer;
using EPiServer.Core;
using EPiServer.Framework.Blobs;
using EPiServer.ServiceLocation;
using EPiServer.Web.Routing;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using DeaneBarker.Optimizely.StaticSites.Services;
using DeaneBarker.Optimizely.StaticSites.Models;
using System;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.Json;

namespace DeaneBarker.Optimizely.StaticSites
{
    public static class StaticSiteCommands
    {
        private static readonly string[] allowedExtensions = new[] { ".js", ".css", ".json" };

        private static IMimeTypeMap _mimeTypeMap;
        private static IUrlResolver _urlResolver;
        private static IHttpContextAccessor _httpContext;
        private static IStaticResourceRetriever _staticResourceRetriever;
        private static IStaticSiteCache _staticSiteCache;

        static StaticSiteCommands()
        {
            _mimeTypeMap = ServiceLocator.Current.GetInstance<IMimeTypeMap>();
            _httpContext = ServiceLocator.Current.GetInstance<IHttpContextAccessor>();
            _urlResolver = ServiceLocator.Current.GetInstance<IUrlResolver>();
            _staticResourceRetriever = ServiceLocator.Current.GetInstance<IStaticResourceRetriever>();
            _staticSiteCache = ServiceLocator.Current.GetInstance<IStaticSiteCache>();
        }

        public static ActionResult ShowAsset(StaticSiteRoot currentPage, string path)
        {
            var extension = Path.GetExtension(path);
            if (!allowedExtensions.Contains(extension))
            {
                return new ForbidResult();
            }

            var contentAssetHelper = ServiceLocator.Current.GetInstance<ContentAssetHelper>();
            var assetFolder = contentAssetHelper.GetOrCreateAssetFolder(((IContent)currentPage).ContentLink);
            var repo = ServiceLocator.Current.GetInstance<IContentRepository>();
            var assets = repo.GetChildren<MediaData>(assetFolder.ContentLink);
            var asset = assets.FirstOrDefault(a => a.Name == Path.GetFileName(path));

            if (asset == null)
            {
                return new NotFoundResult();
            }

            var contentType = _mimeTypeMap.GetMimeType(path);
            return new ContentResult()
            {
                Content = Encoding.UTF8.GetString(asset.BinaryData.ReadAllBytes()),
                ContentType = contentType
            };
        }

        public static ActionResult ShowContext(StaticSiteRoot staticSiteRoot, string _)
        {

            var contextVars = new
            {
                rootId = staticSiteRoot.ContentLink.ID,
                baseUrl = _urlResolver.GetUrl(staticSiteRoot),
                userName = _httpContext.HttpContext.User?.Identity?.Name
            };

            return new JsonResult(contextVars);
        }

        public static ActionResult ShowContents(StaticSiteRoot currentPage, string _)
        {
            var resources = _staticResourceRetriever.GetResourceNames(currentPage);

            var sb = new StringBuilder();
            sb.AppendLine($"Total resources: {resources.Count()}");
            sb.AppendLine();
            foreach (var resource in resources)
            {
                sb.AppendLine(resource);
            }

            return new ContentResult()
            {
                Content = sb.ToString(),
                ContentType = "text/plain"
            };
        }

        public static ActionResult ShowCache(StaticSiteRoot currentPage, string _)
        {
            var lines = _staticSiteCache.Show(currentPage.ContentGuid);
            return new ContentResult()
            {
                Content = string.Join(Environment.NewLine, lines),
                ContentType = "text/plain"
            };
        }

        public static ActionResult ClearCache(StaticSiteRoot currentPage, string _)
        {

            _staticSiteCache.Clear(currentPage.ContentGuid);
            return new ContentResult()
            {
                Content = "Cache Cleared",
                ContentType = "text/plain"
            };
        }

        public static ActionResult ShowLog(StaticSiteRoot currentPage, string _)
        {
            var _log = ServiceLocator.Current.GetInstance<IStaticSiteLog>();
            return new ContentResult()
            {
                Content = string.Join(Environment.NewLine, _log.Entries),
                ContentType = "text/plain"
            };
        }
    }
}
