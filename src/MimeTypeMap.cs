using DeaneBarker.Optimizely.StaticSites.Services;
using Microsoft.AspNetCore.StaticFiles;
using System.Linq;

namespace DeaneBarker.Optimizely.StaticSites
{
    public class MimeTypeMap : IMimeTypeMap
    {
        private readonly string[] textMime = new[] { "text/plain", "text/css", "application/javascript", "application/json" };

        public string GetMimeType(string path)
        {
            new FileExtensionContentTypeProvider().TryGetContentType(path, out var contentType);
            return contentType ?? "text/plain";
        }

        public bool IsText(string mimeType)
        {
            if(mimeType.ToLower().StartsWith("text/"))
            {
                return true;
            }

            return textMime.Contains(mimeType);
        }
    }
}
