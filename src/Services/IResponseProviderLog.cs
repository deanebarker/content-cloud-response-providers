using System.Collections.Generic;

namespace DeaneBarker.Optimizely.ResponseProviders.Services
{
    public interface IResponseProviderLog
    {

        List<string> Entries { get; }
        void Log(object text);
    }
}