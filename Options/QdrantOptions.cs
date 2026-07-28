namespace MultiModalRagDemo.Options
{
    public class QdrantOptions
    {
        public const string SectionName = "Qdrant";

        public string BaseUrl { get; set; } = "http://localhost:6333";

        public string CollectionName { get; set; } = "rag_chunks";

        public int VectorSize { get; set; } = 384;

        public int DefaultTopK { get; set; } = 5;
    }
}
