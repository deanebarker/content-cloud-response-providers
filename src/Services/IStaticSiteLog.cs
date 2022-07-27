using System.Collections.Generic;

namespace DeaneBarker.Optimizely.StaticSites.Services
{
    public interface IStaticSiteLog
    {

        List<string> Entries { get; }
        void Log(object text);
    }
}