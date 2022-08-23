using DeaneBarker.Optimizely.ResponseProviders.PathTranslators;
using DeaneBarker.Optimizely.ResponseProviders.SourceProviders;
using EPiServer.DataAnnotations;
using EPiServer.ServiceLocation;
using System;
using System.Collections.Generic;
using System.Net;

namespace DeaneBarker.Optimizely.ResponseProviders.Models
{
    [ContentType(DisplayName = "Proxy Site Root", GUID = "68C1BDCC-34E8-52CE-98C5-05BE3A0EBAF0")]
    public class ProxyResponseProvider : BaseResponseProvider
    {
        public virtual string ProxyPath { get; set; }

        public override ISourceProvider GetResponseProvider()
        {
            return new ProxySiteSourceProvider();
        }
        public override IResponseProviderPathTranslator GetPathTranslator()
        {
            return new SimplePathTranslator();
        }
    }

    public class ProxySiteSourceProvider : ISourceProvider
    {
        private IMimeTypeManager mimeTypeManager;

        public ProxySiteSourceProvider()
        {
            mimeTypeManager = ServiceLocator.Current.GetInstance<IMimeTypeManager>();
        }

        public SourcePayload GetSourcePayload(BaseResponseProvider siteRoot, string path)
        {
            var sourcePayload = new SourcePayload()
            {
                ContentType = mimeTypeManager.GetMimeType(path)
            };

            var proxySiteRoot = (ProxyResponseProvider)siteRoot;

            var wc = new WebClient();

            try
            {
                sourcePayload.Content = wc.DownloadData(string.Concat(proxySiteRoot.ProxyPath, path));
            }
            catch (Exception e)
            {
                // Just swallow it; the Content property will remain null which will trigger a 404
            }

            return sourcePayload;
        }

        public IEnumerable<string> GetResourceNames(BaseResponseProvider siteRoot)
        {
            throw new System.NotImplementedException();
        }
    }
}
