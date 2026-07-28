namespace MultiModalRagDemo.Models
{
    public class VectorRecord
    {
        public Guid Id { get; set; } = Guid.NewGuid();

        public string ChunkText { get; set; } = string.Empty;

        public int ChunkIndex { get; set; }

        public int CharacterCount { get; set; }

        public float[] Embedding { get; set; } = [];

    }
}
