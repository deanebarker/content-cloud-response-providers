using DeaneBarker.Optimizely.ResponseProviders.Models;
using System.Text;

namespace DeaneBarker.Optimizely.ResponseProviders.Transformers
{
    public class AddScriptTag : ITransformer
    {
        private string _path;

        public AddScriptTag(string path)
        {
            _path = path;
        }

        public byte[] Transform(byte[] content, string path, BaseResponseProvider siteRoot, string mimeType)
        {
            if (mimeType != "text/html") return content;

            var html = Encoding.UTF8.GetString(content);

            // This is crude but effective

            var closingBodyTag = "</body>";
            var scriptTag = $"<script src=\"{_path}\"></script>";

            html = html.Replace(closingBodyTag, string.Concat(scriptTag, closingBodyTag));

            return Encoding.UTF8.GetBytes(html);
        }

    }
}
