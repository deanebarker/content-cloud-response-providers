using DeaneBarker.Optimizely.ResponseProviders.Models;
using DeaneBarker.Optimizely.ResponseProviders.Services;
using EPiServer;
using EPiServer.Core;
using EPiServer.Framework.Blobs;
using EPiServer.ServiceLocation;
using System;
using System.Collections.Generic;
using System.IO;
using System.IO.Compression;
using System.Linq;

namespace DeaneBarker.Optimizely.ResponseProviders
{
    public class ZipArchiveSourceProvider : ISourceProvider
    {
        private const string defaultArchiveName = "_source.zip";
        private readonly IContentLoader _loader;
        private readonly IMimeTypeManager _mimeTypeManager;

        public ZipArchiveSourceProvider()
        {
            _loader = ServiceLocator.Current.GetInstance<IContentLoader>();
            _mimeTypeManager = ServiceLocator.Current.GetInstance<IMimeTypeManager>();
        }

        public SourcePayload GetSourcePayload(BaseResponseProvider siteRoot, string path)
        {
            if (string.IsNullOrWhiteSpace(path))
            {
                throw new ArgumentNullException(nameof(path));
            }

            var zip = LocateZipArchive(siteRoot);

            if (string.IsNullOrWhiteSpace(path))
            {
                throw new ArgumentNullException(nameof(path));
            }

            var entry = zip.GetEntry(path.Trim("/".ToCharArray()));
            if (entry == null)
            {
                return null;
            }

            var stream = entry.Open();

            var sourcePayload = new SourcePayload()
            {
                ContentType = _mimeTypeManager.GetMimeType(path)
            };

            byte[] buffer = new byte[16 * 1024];
            using (var memoryStream = new MemoryStream())
            {
                int read;
                while ((read = stream.Read(buffer, 0, buffer.Length)) > 0)
                {
                    memoryStream.Write(buffer, 0, read);
                }
                stream.Close();
                sourcePayload.Content = memoryStream.ToArray();
            }

            return sourcePayload;
        }

        public IEnumerable<string> GetResourceNames(BaseResponseProvider siteRoot)
        {
            return LocateZipArchive(siteRoot).Entries.Select(e => e.FullName);
        }

        private ZipArchive LocateZipArchive(BaseResponseProvider siteRoot)
        {
            MediaData archive = null;
            if (((ZipAssetResponseProvider)siteRoot).ArchiveFile != null)
            {
                archive = _loader.Get<MediaData>(((ZipAssetResponseProvider)siteRoot).ArchiveFile);
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

            if (archive == null)
            {
                return null;
            }

            var archiveBytes = archive.BinaryData.ReadAllBytes();

            return new ZipArchive(new MemoryStream(archiveBytes));
        }
    }


}
