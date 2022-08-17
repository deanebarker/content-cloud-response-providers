using DeaneBarker.Optimizely.ResponseProviders.Services;
using EPiServer.DataAnnotations;
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
    }

    public class ProxySiteSourceProvider : ISourceProvider
    {
        public byte[] GetBytesOfResource(BaseResponseProvider siteRoot, string path)
        {
            var proxySiteRoot = (ProxyResponseProvider)siteRoot;

            var wc = new WebClient();
            return wc.DownloadData(string.Concat(proxySiteRoot.ProxyPath, path));
        }

        public IEnumerable<string> GetResourceNames(BaseResponseProvider siteRoot)
        {
            throw new System.NotImplementedException();
        }
    }
}
