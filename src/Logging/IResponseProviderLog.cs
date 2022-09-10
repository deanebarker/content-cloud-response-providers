using System.Collections.Generic;

namespace DeaneBarker.Optimizely.ResponseProviders.Logging
{
    public interface IResponseProviderLog
    {

        List<string> Entries { get; }
        void Log(object text);
        List<string> GetEntriesForKey(string key);
        void Clear();
    }
}