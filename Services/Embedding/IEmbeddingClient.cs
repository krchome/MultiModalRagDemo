namespace MultiModalRagDemo.Services.Embedding
{
    public interface IEmbeddingClient
    {
        Task<EmbeddingClientResult> CreateEmbeddingsAsync(
            List<string> texts,
            CancellationToken cancellationToken = default);
    }
}
