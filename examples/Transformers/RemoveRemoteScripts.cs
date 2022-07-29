using AngleSharp.Html;
using AngleSharp.Html.Parser;
using System.IO;
using System.Text;

namespace DeaneBarker.Optimizely.StaticSites.Transformers
{
    public class RemoveRemoteScripts : ITransformer
    {
        public byte[] Transform(byte[] content, string path, string mimeType)
        {
            if (mimeType != "text/html") return content;

            var html = Encoding.UTF8.GetString(content);

            var parser = new HtmlParser();
            var doc = parser.ParseDocument(html);

            foreach(var scriptTag in doc.QuerySelectorAll("script[src]"))
            {
                if(scriptTag.GetAttribute("src").Contains("//")) // Is this valid? A remote script would *have* to have double slashes in the path, right?
                {
                    scriptTag.ParentElement.RemoveChild(scriptTag);
                }
            }

            var sb = new StringBuilder();
            var textWriter = new StringWriter(sb);
            doc.ToHtml(textWriter, new PrettyMarkupFormatter());
            html = sb.ToString();

            return Encoding.UTF8.GetBytes(html);
        }
    }
}
