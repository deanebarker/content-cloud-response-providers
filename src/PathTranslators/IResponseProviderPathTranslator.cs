﻿using DeaneBarker.Optimizely.ResponseProviders.Models;

namespace DeaneBarker.Optimizely.ResponseProviders.PathTranslators
{
    public interface IResponseProviderPathTranslator
    {
        string GetTranslatedPath(BaseResponseProvider siteRoot, string requestedPath);
        string NotFoundDocument { get; }
        string DefaultDocument { get; }
    }
}