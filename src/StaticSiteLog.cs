using DeaneBarker.Optimizely.StaticSites.Services;
using System.Collections.Generic;

namespace DeaneBarker.Optimizely.StaticSites
{
    public class StaticSiteLog : IStaticSiteLog
    {
        private List<string> entries = new();

        public List<string> Entries => entries;

        public void Log(object text)
        {
            entries.Add(text.ToString());
        }

    }
}
