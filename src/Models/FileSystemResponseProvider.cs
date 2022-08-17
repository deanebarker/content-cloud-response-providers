using DeaneBarker.Optimizely.ResponseProviders.Services;
using EPiServer.DataAnnotations;
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
            public byte[] GetBytesOfResource(BaseResponseProvider siteRoot, string path)
            {
                return File.ReadAllBytes(Path.Combine(((FileSystemResponseProvider)siteRoot).FileSystemPath, path));
            }

            public IEnumerable<string> GetResourceNames(BaseResponseProvider siteRoot)
            {
                throw new System.NotImplementedException();
            }
        }
    }
}
