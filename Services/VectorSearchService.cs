using Microsoft.Extensions.Options;
using MultiModalRagDemo.Models;
using MultiModalRagDemo.Options;
using MultiModalRagDemo.Services.Embedding;


namespace MultiModalRagDemo.Services;

public class VectorSearchService : IVectorSearchService
{
    private readonly HttpClient _httpClient;
    private readonly IEmbeddingClient _embeddingClient;
    private readonly QdrantOptions _qdrantOptions;
    private readonly ILogger<VectorSearchService> _logger;

    public VectorSearchService(
        HttpClient httpClient,
        IEmbeddingClient embeddingClient,
        IOptions<QdrantOptions> qdrantOptions,
        ILogger<VectorSearchService> logger)
    {
        _httpClient = httpClient;
        _embeddingClient = embeddingClient;
        _qdrantOptions = qdrantOptions.Value;
        _logger = logger;
    }

    public async Task<IReadOnlyList<RetrievedChunk>> SearchAsync(
        string question,
        int topK,
        CancellationToken cancellationToken = default)
    {
        if (string.IsNullOrWhiteSpace(question))
        {
            return [];
        }

        topK = Math.Clamp(topK, 1, 10);

        var embeddingResult = await _embeddingClient.CreateEmbeddingsAsync(
    new List<string> { question },
    cancellationToken);

        var questionEmbedding = embeddingResult.Embeddings.FirstOrDefault();


        if (questionEmbedding is null || questionEmbedding.Count == 0)
        {
            throw new InvalidOperationException(
                "The embedding service returned an empty question embedding.");
        }

        if (questionEmbedding.Count != _qdrantOptions.VectorSize)
        {
            throw new InvalidOperationException(
                $"Question embedding dimension mismatch. Expected {_qdrantOptions.VectorSize}, but received {questionEmbedding.Count}.");
        }

        var queryRequest = new QdrantQueryRequest
        {
            Query = questionEmbedding.ToArray(),
            Limit = topK,
            WithPayload = true,
            WithVector = false
        };

        var endpoint =
            $"/collections/{_qdrantOptions.CollectionName}/points/query";

        var response = await _httpClient.PostAsJsonAsync(
            endpoint,
            queryRequest,
            cancellationToken);

        if (!response.IsSuccessStatusCode)
        {
            var errorBody = await response.Content.ReadAsStringAsync(cancellationToken);

            _logger.LogError(
                "Qdrant vector search failed. StatusCode: {StatusCode}. Body: {Body}",
                response.StatusCode,
                errorBody);

            throw new InvalidOperationException(
                $"Qdrant vector search failed with status code {response.StatusCode}.");
        }

        var queryResponse =
            await response.Content.ReadFromJsonAsync<QdrantQueryResponse>(
                cancellationToken: cancellationToken);

        var points = queryResponse?.Result?.Points ?? [];
        foreach (var point in points)
        {
            _logger.LogInformation(
                "Qdrant point {PointId}: raw score = {Score:R}, chunk index = {ChunkIndex}",
                point.GetPointId(),
                point.Score,
                point.Payload.ChunkIndex);
        }

        return points
            .Select(point => new RetrievedChunk
            {
                PointId = point.GetPointId(),
                Score = point.Score,
                ChunkIndex = point.Payload.ChunkIndex,
                CharacterCount = point.Payload.CharacterCount,
                ChunkText = point.Payload.ChunkText
            })
            .Where(chunk => !string.IsNullOrWhiteSpace(chunk.ChunkText))
            .ToList();
    }
}