using EPiServer;
using EPiServer.Core;
using EPiServer.Framework.Blobs;
using EPiServer.ServiceLocation;
using DeaneBarker.Optimizely.StaticSites.Services;
using DeaneBarker.Optimizely.StaticSites.Models;
using System.Linq;

namespace DeaneBarker.Optimizely.StaticSites
{
    public class StaticSourceLocator : IStaticSourceLocator
    {
        private IContentLoader _loader;
        private const string defaultArchiveName = "_source.zip";

        public StaticSourceLocator(IContentLoader loader)
        {
            _loader = loader;
        }

        public byte[] GetBytesOfSource(StaticSiteRoot siteRoot)
        {
            MediaData archive = null;
            if (siteRoot.ArchiveFile != null)
            {
                archive = _loader.Get<MediaData>(siteRoot.ArchiveFile);
            }
            else
            {
                var contentAssetHelper = ServiceLocator.Current.GetInstance<ContentAssetHelper>();
                var assetFolder = contentAssetHelper.GetOrCreateAssetFolder(((IContent)siteRoot).ContentLink);
                var assets = _loader.GetChildren<MediaData>(assetFolder.ContentLink);

                archive = assets.FirstOrDefault(a => a.Name == defaultArchiveName);

                if (archive == null)
                {
                    archive = assets.FirstOrDefault(a => a.Name.EndsWith(".zip"));
                }
            }

            if(archive == null)
            {
                throw new ContentNotFoundException("ZIP archive not found");
            }

            return archive.BinaryData.ReadAllBytes();
        }

    }
}
