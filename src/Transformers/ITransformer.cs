namespace DeaneBarker.Optimizely.StaticSites.Transformers
{
    public interface ITransformer
    {
        byte[] Transform(byte[] content, string path, string mimeType);
    }
}