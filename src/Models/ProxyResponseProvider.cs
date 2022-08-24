using DeaneBarker.Optimizely.ResponseProviders.PathTranslators;
using DeaneBarker.Optimizely.ResponseProviders.SourceProviders;
using EPiServer.DataAnnotations;
using EPiServer.ServiceLocation;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Net;

namespace DeaneBarker.Optimizely.ResponseProviders.Models
{
    [ContentType(DisplayName = "Proxy Site Root", GUID = "68C1BDCC-34E8-52CE-98C5-05BE3A0EBAF0")]
    public class ProxyResponseProvider : BaseResponseProvider
    {
        [Display(Name = "Base Proxy URL", Description = "The URL to which the content path will be appended.")]
        public virtual string ProxyPath { get; set; }

        public override ISourceProvider GetResponseProvider()
        {
            return new ProxySourceProvider();
        }
        public override IResponseProviderPathTranslator GetPathTranslator()
        {
            return new SimplePathTranslator();
        }
    }

}
