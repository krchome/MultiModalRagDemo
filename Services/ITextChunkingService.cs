using MultiModalRagDemo.Models; 
namespace MultiModalRagDemo.Services
{
    public interface ITextChunkingService
    {
        List<TextChunk> ChunkText(string text, int chunkSize = 800, int overlapSize = 100);
    }
}
