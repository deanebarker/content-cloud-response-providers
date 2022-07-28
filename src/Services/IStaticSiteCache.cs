using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;

namespace DeaneBarker.Optimizely.StaticSites.Services
{
    public interface IStaticSiteCache
    {
        void Clear(Guid siteId);
        ActionResult Get(Guid siteId, string path);
        void Put(Guid siteId, string path, ActionResult value);
        IEnumerable<string> Show(Guid siteId);
    }
}