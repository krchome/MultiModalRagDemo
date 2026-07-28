using System.Net;
using System.Net.Http.Json;
using MultiModalRagDemo.Models;

namespace MultiModalRagDemo.Services;

public class VectorStoreService : IVectorStoreService
{
    private readonly HttpClient _httpClient;
    private readonly IConfiguration _configuration;
    private readonly ILogger<VectorStoreService> _logger;

    private const int MaxRetries = 3;
    private static readonly TimeSpan BaseDelay = TimeSpan.FromSeconds(1);

    public VectorStoreService(
        HttpClient httpClient,
        IConfiguration configuration,
        ILogger<VectorStoreService> logger)
    {
        _httpClient = httpClient ?? throw new ArgumentNullException(nameof(httpClient));
        _configuration = configuration ?? throw new ArgumentNullException(nameof(configuration));
        _logger = logger ?? throw new ArgumentNullException(nameof(logger));
    }

    public async Task<int> StoreVectorsAsync(List<VectorRecord> records)
    {
        if (records is null) throw new ArgumentNullException(nameof(records));
        if (records.Count == 0) return 0;

        await EnsureCollectionExistsAsync();

        var request = new QdrantUpsertRequest
        {
            Points = records.Select(record => new QdrantPoint
            {
                Id = record.Id.ToString(),
                Vector = record.Embedding,
                Payload = new QdrantPayload
                {
                    ChunkText = record.ChunkText,
                    ChunkIndex = record.ChunkIndex,
                    CharacterCount = record.CharacterCount
                }
            }).ToList()
        };

        var collectionName = GetCollectionName();
        var uri = $"/collections/{collectionName}/points?wait=true";

        for (var attempt = 1; ; attempt++)
        {
            var response = await _httpClient.PutAsJsonAsync(uri, request);
            if (response.IsSuccessStatusCode)
            {
                _logger.LogInformation("Stored {Count} vector(s) into Qdrant collection '{Collection}'.", records.Count, collectionName);
                return records.Count;
            }

            // If not transient or we've exhausted retries, fail.
            if (attempt >= MaxRetries || !IsTransient(response.StatusCode))
            {
                var error = await SafeReadContentAsync(response);
                _logger.LogError("Failed to store vectors in Qdrant. Status: {StatusCode}. Error: {Error}", response.StatusCode, error);
                throw new InvalidOperationException($"Failed to store vectors in Qdrant. Status: {response.StatusCode}");
            }

            var delay = TimeSpan.FromMilliseconds(BaseDelay.TotalMilliseconds * Math.Pow(2, attempt - 1));
            _logger.LogWarning("Transient failure storing vectors (attempt {Attempt}/{MaxRetries}). Status: {StatusCode}. Retrying in {Delay}ms.",
                attempt, MaxRetries, response.StatusCode, delay.TotalMilliseconds);
            await Task.Delay(delay);
        }
    }

    private static bool IsTransient(HttpStatusCode statusCode)
    {
        return statusCode == HttpStatusCode.RequestTimeout
               || statusCode == (HttpStatusCode)429 // Too Many Requests
               || (int)statusCode >= 500;
    }

    private async Task EnsureCollectionExistsAsync()
    {
        var collectionName = GetCollectionName();

        var getResponse = await _httpClient.GetAsync($"/collections/{collectionName}");
        if (getResponse.IsSuccessStatusCode) return;

        var vectorSize = _configuration.GetValue<int?>("Qdrant:VectorSize")
                         ?? throw new InvalidOperationException("Qdrant vector size is missing.");

        var createRequest = new QdrantCreateCollectionRequest
        {
            Vectors = new QdrantVectorConfig
            {
                Size = vectorSize,
                Distance = "Cosine"
            }
        };

        var createResponse = await _httpClient.PutAsJsonAsync($"/collections/{collectionName}", createRequest);
        if (createResponse.IsSuccessStatusCode) return;

        // If another instance created the collection concurrently, Qdrant may return 409 Conflict.
        if (createResponse.StatusCode == HttpStatusCode.Conflict)
        {
            _logger.LogInformation("Qdrant collection '{Collection}' already exists (conflict).", collectionName);
            return;
        }

        var error = await SafeReadContentAsync(createResponse);
        _logger.LogError("Failed to create Qdrant collection. Status: {StatusCode}. Error: {Error}", createResponse.StatusCode, error);
        throw new InvalidOperationException($"Failed to create Qdrant collection. Status: {createResponse.StatusCode}");
    }

    private string GetCollectionName()
    {
        return _configuration["Qdrant:CollectionName"]
               ?? throw new InvalidOperationException("Qdrant collection name is missing.");
    }

    private static async Task<string> SafeReadContentAsync(HttpResponseMessage response)
    {
        try
        {
            return await response.Content.ReadAsStringAsync();
        }
        catch
        {
            return "<could not read response body>";
        }
    }
}