using DeaneBarker.Optimizely.ResponseProviders.Services;
using EPiServer.Core;
using EPiServer.DataAnnotations;
using EPiServer.ServiceLocation;
using EPiServer.Web;
using System.ComponentModel.DataAnnotations;

namespace DeaneBarker.Optimizely.ResponseProviders.Models
{
    [ContentType(DisplayName = "Zip Archive Site Root", GUID = "68C1BDCC-34E9-52CE-98C5-05BE3A0EBAF0")]
    public class ZipAssetResponseProvider : BaseResponseProvider
    {
        [UIHint(UIHint.MediaFile)]
        public virtual ContentReference ArchiveFile { get; set; }

        public override ISourceProvider GetResponseProvider()
        {
            return new ZipArchiveSourceProvider();
        }

        public override IResponseProviderPathTranslator GetPathTranslator()
        {
            return new FileSystemPathTranslator();
        }
    }
}
