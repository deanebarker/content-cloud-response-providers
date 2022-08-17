using System;

namespace DeaneBarker.Optimizely.ResponseProviders
{
    public class SourcePayload
    {
        public byte[] Content{ get; set; }
        public string ContentType { get; set; }

        public SourcePayload() { }

        public SourcePayload(byte[] content, string contentType)
        {
            Content = content;
            ContentType = contentType;
        }

        public static SourcePayload Empty => new();

        public override bool Equals(object obj)
        {
            if (obj == null)
            {
                return false;
            }

            if (!(obj is SourcePayload))
            {
                return false;
            }

            if(Content == null && ((SourcePayload)obj).Content == null)
            {
                return true;
            }

            return Content == ((SourcePayload)obj).Content;
        }

        public override int GetHashCode()
        {
            return Content.GetHashCode();
        }
    }

    
}
