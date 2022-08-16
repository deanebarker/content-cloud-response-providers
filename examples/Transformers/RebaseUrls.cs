using DeaneBarker.Optimizely.StaticSites.Models;
using EPiServer.ServiceLocation;
using EPiServer.Web.Routing;
using System;
using System.Text;

namespace DeaneBarker.Optimizely.StaticSites.Transformers
{
    // Yes, yes, this is awful...
    // And yes, I've read the famous Stack Overflow question...
    // But in most cases, this works
    // If it doesn't, inject something else which uses AngleSharp, but I didn't want to create that dependency
    public class RebaseUrls : ITransformer
    {
        private UrlResolver _resolver = ServiceLocator.Current.GetInstance<UrlResolver>();

        public byte[] Transform(byte[] content, string path, StaticSiteRoot siteRoot, string mimeType)
        {
            var rootPath = _resolver.GetUrl(siteRoot);

            if (mimeType != "text/html") return content;

            var html = Encoding.UTF8.GetString(content);

            html = ReplaceAbsoluteAttributePath(html, "href", rootPath);
            html = ReplaceAbsoluteAttributePath(html, "src", rootPath);

            return Encoding.UTF8.GetBytes(html);
        }

        private string ReplaceAbsoluteAttributePath(string html, string attribute, string newRoot)
        {
            newRoot = newRoot.TrimEnd('/') + "/";

            return html.Replace($" {attribute}=\"/", $" {attribute}=\"{newRoot}", StringComparison.InvariantCultureIgnoreCase);
        }

    }
}
