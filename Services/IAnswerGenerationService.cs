using MultiModalRagDemo.Models;

namespace MultiModalRagDemo.Services
{
    public interface IAnswerGenerationService
    {
        Task<RagAnswerResult> GenerateAnswerAsync(
        string question,
        IReadOnlyList<RetrievedChunk> retrievedChunks,
        CancellationToken cancellationToken = default);

    }
}
