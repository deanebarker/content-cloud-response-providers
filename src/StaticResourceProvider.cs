﻿using DeaneBarker.Optimizely.StaticSites.Models;
using DeaneBarker.Optimizely.StaticSites.Services;
using EPiServer;
using EPiServer.Core;
using EPiServer.Framework.Blobs;
using EPiServer.ServiceLocation;
using System;
using System.Collections.Generic;
using System.IO;
using System.IO.Compression;
using System.Linq;

namespace DeaneBarker.Optimizely.StaticSites
{
    public class StaticResourceProvider : IStaticResourceProvider
    {
        private const string defaultArchiveName = "_source.zip";
        private readonly IContentLoader _loader;

        public StaticResourceProvider(IContentLoader loader)
        {
            _loader = loader;
        }

        public byte[] GetBytesOfResource(StaticSiteRoot siteRoot, string path)
        {
            if(string.IsNullOrWhiteSpace(path))
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

            byte[] buffer = new byte[16 * 1024];
            using (var memoryStream = new MemoryStream())
            {
                int read;
                while ((read = stream.Read(buffer, 0, buffer.Length)) > 0)
                {
                    memoryStream.Write(buffer, 0, read);
                }
                stream.Close();
                return memoryStream.ToArray();
            }
        }

        public IEnumerable<string> GetResourceNames(StaticSiteRoot siteRoot)
        {
            return LocateZipArchive(siteRoot).Entries.Select(e => e.FullName);
        }

        private ZipArchive LocateZipArchive(StaticSiteRoot siteRoot)
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

            if (archive == null)
            {
                return null;
            }

            var archiveBytes = archive.BinaryData.ReadAllBytes();

            return new ZipArchive(new MemoryStream(archiveBytes));
        }
    }


}
