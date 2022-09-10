using DeaneBarker.Optimizely.ResponseProviders.Caches;
using DeaneBarker.Optimizely.ResponseProviders.Commands;
using DeaneBarker.Optimizely.ResponseProviders.Logging;
using DeaneBarker.Optimizely.ResponseProviders.PathTranslators;
using DeaneBarker.Optimizely.ResponseProviders.SourceProviders;
using DeaneBarker.Optimizely.ResponseProviders.Transformers;
using DeaneBarker.Optimizely.ResponseProviders.UserManagers;
using EPiServer.Core.Routing;
using EPiServer.Framework;
using EPiServer.Framework.Initialization;
using EPiServer.ServiceLocation;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.DependencyInjection;

namespace DeaneBarker.Optimizely.ResponseProviders
{
    [InitializableModule]
    public class ResponseProviderInit : IConfigurableModule
    {
        public void ConfigureContainer(ServiceConfigurationContext context)
        {
            context.Services.AddSingleton<IPartialRouter, ResponseProviderPartialRouter>();
            context.Services.AddSingleton<IHttpContextAccessor, HttpContextAccessor>();
            context.Services.AddSingleton<IResponseProviderPathTranslator, FileSystemPathTranslator>();
            context.Services.AddSingleton<ISourceProvider, ZipArchiveSourceProvider>();
            context.Services.AddSingleton<IResponseProviderCommandManager, ResponseProviderCommandManager>();
            context.Services.AddSingleton<IMimeTypeManager, MimeTypeManager>();
            context.Services.AddSingleton<IResponseProviderLog, InMemoryLog>();
            context.Services.AddSingleton<IResponseProviderCache, InMemoryCache>();
            context.Services.AddSingleton<IResponseProviderTransformerManager, ResponseProviderTransformerManager>();
            context.Services.AddSingleton<IResponseProviderUserManager, SimpleUserManager>();
        }

        public void Initialize(InitializationEngine context)
        {

        }

        public void Uninitialize(InitializationEngine context)
        {

        }
    }
}
