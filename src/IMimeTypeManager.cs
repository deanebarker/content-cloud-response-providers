namespace DeaneBarker.Optimizely.ResponseProviders
{
    public interface IMimeTypeManager
    {
        string GetMimeType(string path);
        bool IsText(string mimeType);
    }
}