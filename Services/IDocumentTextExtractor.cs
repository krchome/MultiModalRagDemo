namespace MultiModalRagDemo.Services
{
    public interface IDocumentTextExtractor
    {
        Task<string> ExtractTextAsync(string filePath);
    }
}
