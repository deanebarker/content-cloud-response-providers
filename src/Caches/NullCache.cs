using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;

namespace DeaneBarker.Optimizely.ResponseProviders.Caches
{
    // No cache
    public class NullCache : IResponseProviderCache
    {
        public void Clear(Guid siteId)
        {
            return;
        }

        public ActionResult Get(Guid siteId, string path)
        {
            return null;
        }

        public void Put(Guid siteId, string path, ActionResult value)
        {
            return;
        }

        public IEnumerable<string> Show(Guid siteId)
        {
            return new List<string>() { "This provider does not cache." };
        }
    }
}
