using System.Collections.Concurrent;
using System.Collections.Generic;
using System;
using System.Linq;
using Microsoft.AspNetCore.Mvc;
using DeaneBarker.Optimizely.StaticSites.Services;

namespace DeaneBarker.Optimizely.StaticSites
{
    public class StaticSiteCache : IStaticSiteCache
    {
        private ConcurrentDictionary<Guid, ConcurrentDictionary<string, ActionResult>> cache = new();

        public void Put(Guid siteId, string path, ActionResult value)
        {
            EnsureSiteCache(siteId);
            cache[siteId][path] = value;
        }

        public ActionResult Get(Guid siteId, string path)
        {
            return cache.GetValueOrDefault(siteId)?.GetValueOrDefault(path);
        }

        private void EnsureSiteCache(Guid siteId)
        {
            if (!cache.ContainsKey(siteId))
            {
                cache[siteId] = new();
            }
        }

        public void Clear(Guid siteId)
        {
            cache[siteId] = new();
        }

        public IEnumerable<string> Show(Guid siteId)
        {
            return cache[siteId].Keys;
        }
    }
}
