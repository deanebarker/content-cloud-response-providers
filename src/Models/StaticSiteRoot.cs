using EPiServer.Core;
using EPiServer.DataAnnotations;
using EPiServer.Web;
using System.ComponentModel.DataAnnotations;

namespace DeaneBarker.Optimizely.StaticSites.Models
{
    [ContentType(GUID = "68C1BDCC-34E9-52CE-98C5-05BE3A0EBAF0")]
    public class StaticSiteRoot : PageData
    {
        [UIHint(UIHint.MediaFile)]
        public virtual ContentReference ArchiveFile { get; set; }
    }
}
