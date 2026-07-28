using MultiModalRagDemo.Models;

namespace MultiModalRagDemo.Services;

public interface IVectorSearchService
{
    Task<IReadOnlyList<RetrievedChunk>> SearchAsync(
        string question,
        int topK,
        CancellationToken cancellationToken = default);
}