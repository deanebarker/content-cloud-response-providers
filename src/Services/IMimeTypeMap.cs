namespace DeaneBarker.Optimizely.StaticSites.Services
{
    public interface IMimeTypeMap
    {
        string GetMimeType(string path);
        bool IsText(string mimeType);
    }
}