using EPiServer.Core.Routing;
using EPiServer.Framework;
using EPiServer.Framework.Initialization;
using EPiServer.ServiceLocation;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.DependencyInjection;
using DeaneBarker.Optimizely.StaticSites;
using DeaneBarker.Optimizely.StaticSites.Services;

namespace opti.deanebarker.net.PathServicing
{
    [InitializableModule]
    public class StaticSiteInit : IConfigurableModule
    {
        public void ConfigureContainer(ServiceConfigurationContext context)
        {
            context.Services.AddSingleton<IPartialRouter, StaticSitePartialRouter>();
            context.Services.AddSingleton<IHttpContextAccessor, HttpContextAccessor>();
            context.Services.AddSingleton<IStaticSitePathTranslator, StaticSitePathTranslator>();
            context.Services.AddSingleton<IStaticResourceProvider, StaticResourceProvider>();
            context.Services.AddSingleton<IStaticSiteCommandManager, StaticSiteCommandManager>();
            context.Services.AddSingleton<IMimeTypeManager, MimeTypeManager>();
            context.Services.AddSingleton<IStaticSiteLog, StaticSiteLog>();
            context.Services.AddSingleton<IStaticSiteCache, StaticSiteCache>();
        }

        public void Initialize(InitializationEngine context)
        {
            
        }

        public void Uninitialize(InitializationEngine context)
        {
            
        }
    }
}
