using DeaneBarker.Optimizely.ResponseProviders.Services;
using EPiServer.DataAnnotations;
using EPiServer.ServiceLocation;
using System.Collections.Generic;
using System.IO;

namespace DeaneBarker.Optimizely.ResponseProviders.Models
{
    [ContentType(DisplayName = "File System Site Root", GUID = "78C1BDCC-34E9-52CE-98C5-05BE3A0EBAF0")]
    public class FileSystemResponseProvider : BaseResponseProvider
    {
        public virtual string FileSystemPath { get; set; }

        public override IResponseProviderPathTranslator GetPathTranslator()
        {
            return new FileSystemPathTranslator();
        }

        public override ISourceProvider GetResponseProvider()
        {
            return new FileSystemSourceProvider();
        }

        public class FileSystemSourceProvider : ISourceProvider
        {
            private IMimeTypeManager mimeTypeManager;

            public FileSystemSourceProvider()
            {
                mimeTypeManager = ServiceLocator.Current.GetInstance<IMimeTypeManager>();
            }

            public SourcePayload GetSourcePayload(BaseResponseProvider siteRoot, string path)
            {
                var content = File.ReadAllBytes(Path.Combine(((FileSystemResponseProvider)siteRoot).FileSystemPath, path));
                var contentType = mimeTypeManager.GetMimeType(path);
                return new SourcePayload(content, contentType);
            }

            public IEnumerable<string> GetResourceNames(BaseResponseProvider siteRoot)
            {
                throw new System.NotImplementedException();
            }
        }
    }
}
