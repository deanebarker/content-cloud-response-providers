using Microsoft.AspNetCore.Mvc;
using DeaneBarker.Optimizely.StaticSites.Models;

namespace DeaneBarker.Optimizely.StaticSites.Services
{
    public interface IStaticSiteCommandManager
    {
        ActionResult ProcessCommands(StaticSiteRoot siteRoot, string path);
    }
}