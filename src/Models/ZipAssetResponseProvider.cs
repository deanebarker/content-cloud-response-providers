using DeaneBarker.Optimizely.ResponseProviders.PathTranslators;
using DeaneBarker.Optimizely.ResponseProviders.SourceProviders;
using EPiServer.Core;
using EPiServer.DataAnnotations;
using EPiServer.Web;
using System.ComponentModel.DataAnnotations;

namespace DeaneBarker.Optimizely.ResponseProviders.Models
{
    [ContentType(DisplayName = "Zip Archive Site Root", GUID = "68C1BDCC-34E9-52CE-98C5-05BE3A0EBAF0")]
    public class ZipAssetResponseProvider : BaseResponseProvider
    {
        [Display(Name = "Zip Archive File", Description = "The zip archive from which to retrieve the source. If not specified, the system will look for a local asset called _source.zip. If not found, it will use the first zip file it finds.")]
        [UIHint(UIHint.MediaFile)]
        public virtual ContentReference ArchiveFile { get; set; }

        [Display(Name = "Default Document Name", Description = "The filename for the default document when a directory is requested. If not provided, it is assumed to be \"index.html\".")]
        public virtual string DefaultDocument { get; set; }

        public override ISourceProvider GetResponseProvider()
        {
            return new ZipArchiveSourceProvider();
        }

        public override IResponseProviderPathTranslator GetPathTranslator()
        {
            if (!string.IsNullOrWhiteSpace(DefaultDocument))
            {
                return new FileSystemPathTranslator(DefaultDocument);
            }
            else
            {
                return new FileSystemPathTranslator();
            }
        }
    }
}
