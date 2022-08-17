namespace DeaneBarker.Optimizely.ResponseProviders.Services
{
    public interface IMimeTypeManager
    {
        string GetMimeType(string path);
        bool IsText(string mimeType);
    }
}