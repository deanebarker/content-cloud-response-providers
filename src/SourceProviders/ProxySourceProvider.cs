using DeaneBarker.Optimizely.ResponseProviders;
using DeaneBarker.Optimizely.ResponseProviders.Models;
using EPiServer.DataAnnotations;
using EPiServer.ServiceLocation;
using System;
using System.Collections.Generic;
using System.Net;

namespace DeaneBarker.Optimizely.ResponseProviders.SourceProviders
{
    public class ProxySourceProvider : ISourceProvider
    {
        private IMimeTypeManager mimeTypeManager;

        public ProxySourceProvider()
        {
            mimeTypeManager = ServiceLocator.Current.GetInstance<IMimeTypeManager>();
        }

        public virtual SourcePayload GetSourcePayload(BaseResponseProvider siteRoot, string url)
        {
            var sourcePayload = new SourcePayload()
            {
                ContentType = mimeTypeManager.GetMimeType(url)
            };

            var wc = new WebClient();

            try
            {
                sourcePayload.Content = wc.DownloadData(url);
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
