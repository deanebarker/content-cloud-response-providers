namespace DeaneBarker.Optimizely.StaticSites.Services
{
    public interface IMimeTypeManager
    {
        string GetMimeType(string path);
        bool IsText(string mimeType);
    }
}