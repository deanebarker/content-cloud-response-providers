using System;
using System.Text;

namespace DeaneBarker.Optimizely.StaticSites.Transformers
{
    public class EnsureDocType : ITransformer
    {
        private string _docType;

        public EnsureDocType(string docType = "<!DOCTYPE html>")
        {
            _docType = docType;
        }

        public byte[] Transform(byte[] content, string path, string mimeType)
        {
            if (mimeType != "text/html") return content;

            var html = Encoding.UTF8.GetString(content);

            if (html.Trim().StartsWith("<!DOCTYPE")) return content; // Already has a doctype

            html = string.Concat(_docType, Environment.NewLine, html);

            return Encoding.UTF8.GetBytes(html);
        }

    }
}
