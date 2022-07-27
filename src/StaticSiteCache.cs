using System.Collections.Concurrent;
using System.Collections.Generic;
using System;
using System.Linq;

namespace DeaneBarker.Optimizely.StaticSites
{
    public class StaticSiteCache
    {
        public static StaticSiteCache Instance { get; } = new();
        
        private ConcurrentDictionary<Guid, ConcurrentDictionary<string, byte[]>> cache = new();

        static StaticSiteCache()
        {
            Instance = new StaticSiteCache();
        }

        public void Put(Guid siteId, string path, byte[] value)
        {
            EnsureSiteCache(siteId);
            cache[siteId][path] = value;
        }

        public byte[] Get(Guid siteId, string path)
        {
            return cache.GetValueOrDefault(siteId)?.GetValueOrDefault(path);
        }

        public void Clear(Guid siteId)
        {
            cache[siteId] = new();
        }

        public Dictionary<string, int> Show(Guid siteId)
        {
            EnsureSiteCache(siteId);
            return cache[siteId].ToDictionary(x => x.Key, y => y.Value.Length);
        }

        private void EnsureSiteCache(Guid siteId)
        {
            if (!cache.ContainsKey(siteId))
            {
                cache[siteId] = new();
            }
        }
    }
}
