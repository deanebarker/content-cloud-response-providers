using DeaneBarker.Optimizely.ResponseProviders.Models;
using Microsoft.AspNetCore.Mvc;

namespace DeaneBarker.Optimizely.ResponseProviders.Commands
{
    public interface IResponseProviderCommandManager
    {
        ActionResult ProcessCommands(BaseResponseProvider siteRoot, string path);
    }
}