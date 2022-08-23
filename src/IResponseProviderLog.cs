using System.Collections.Generic;

namespace DeaneBarker.Optimizely.ResponseProviders
{
    public interface IResponseProviderLog
    {

        List<string> Entries { get; }
        void Log(object text);
    }
}