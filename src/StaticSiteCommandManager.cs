using Microsoft.AspNetCore.Mvc;
using DeaneBarker.Optimizely.StaticSites.Services;
using DeaneBarker.Optimizely.StaticSites.Models;
using System;
using System.Collections.Generic;
using System.Linq;

namespace DeaneBarker.Optimizely.StaticSites
{
    public class StaticSiteCommandManager : IStaticSiteCommandManager
    {
        private const string commandPrefix = "__";
        private Dictionary<string, Func<StaticSiteRoot, string, ActionResult>> commandMap = new();
        public StaticSiteCommandManager()
        {
            commandMap.Add("asset", StaticSiteCommands.ShowAsset);
            commandMap.Add("context", StaticSiteCommands.ShowContext);
            commandMap.Add("contents", StaticSiteCommands.ShowContents);
            commandMap.Add("cache", StaticSiteCommands.ShowCache);
            commandMap.Add("clear", StaticSiteCommands.ClearCache);
            commandMap.Add("log", StaticSiteCommands.ShowLog);
        }

        public ActionResult ProcessCommands(StaticSiteRoot siteRoot, string path)
        {
            var commandSegment = path.Split("/").FirstOrDefault(s => s.StartsWith(commandPrefix));
            if(commandSegment == null)
            {
                return null; // No command segemtn present
            }

            commandSegment = commandSegment.Replace(commandPrefix, string.Empty);
            if(!commandMap.ContainsKey(commandSegment))
            {
                return new NotFoundResult(); // No command found for the segment
            }

            return commandMap[commandSegment](siteRoot, path);
        }
    }
}
