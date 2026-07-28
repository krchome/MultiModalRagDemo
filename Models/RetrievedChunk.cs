namespace MultiModalRagDemo.Models;

public class RetrievedChunk
{
    public string PointId { get; set; } = string.Empty;

    public int ChunkIndex { get; set; }

    public int CharacterCount { get; set; }

    public double Score { get; set; }

    public string ChunkText { get; set; } = string.Empty;
}