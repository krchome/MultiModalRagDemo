using MultiModalRagDemo.Models;

namespace MultimodalRagDemo.Services.Interfaces
{
    public interface ITextChunkingService
    {
        List<TextChunk> ChunkText(string text, int chunkSize = 800, int overlapSize = 100);
    }
}
