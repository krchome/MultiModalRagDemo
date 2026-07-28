
using System.Text.Json.Serialization;

namespace MultiModalRagDemo.Services.Embedding
{
    
    
    public class EmbeddingRequestDto
    {
        [JsonPropertyName("texts")]
        public List<string> Texts { get; set; } = new();
    }

    public class EmbeddingResponseDto
    {
        [JsonPropertyName("embeddings")]
        public List<List<float>> Embeddings { get; set; } = new();
    }

    public class EmbeddingClientResult
    {
        public bool IsSuccess { get; set; }

        public string Message { get; set; } = string.Empty;

        public int EmbeddingCount { get; set; }

        public int VectorDimension { get; set; }

        public List<List<float>> Embeddings { get; set; } = new();

        public static EmbeddingClientResult Success(List<List<float>> embeddings)
        {
            var vectorDimension = embeddings.Count > 0
                ? embeddings[0].Count
                : 0;

            return new EmbeddingClientResult
            {
                IsSuccess = true,
                Message = "Embeddings generated successfully.",
                EmbeddingCount = embeddings.Count,
                VectorDimension = vectorDimension,
                Embeddings = embeddings
            };
        }

        public static EmbeddingClientResult Failure(string message)
        {
            return new EmbeddingClientResult
            {
                IsSuccess = false,
                Message = message
            };
        }
    }
}
