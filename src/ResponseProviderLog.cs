using System.Collections.Generic;

namespace DeaneBarker.Optimizely.ResponseProviders
{
    // This is a dirty debug log
    // This should not take the place of real logging...
    public class ResponseProviderLog : IResponseProviderLog
    {
        private List<string> entries = new();

        public List<string> Entries => entries;

        public void Log(object text)
        {
            entries.Add(text.ToString());
        }

    }
}
