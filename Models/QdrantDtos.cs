using System.Text.Json;
using System.Text.Json.Serialization;

namespace MultiModalRagDemo.Models;

public class QdrantCreateCollectionRequest
{
    [JsonPropertyName("vectors")]
    public QdrantVectorConfig Vectors { get; set; } = new();
}

public class QdrantVectorConfig
{
    [JsonPropertyName("size")]
    public int Size { get; set; }

    [JsonPropertyName("distance")]
    public string Distance { get; set; } = "Cosine";
}

public class QdrantUpsertRequest
{
    [JsonPropertyName("points")]
    public List<QdrantPoint> Points { get; set; } = [];
}

public class QdrantPoint
{
    [JsonPropertyName("id")]
    public string Id { get; set; } = string.Empty;

    [JsonPropertyName("vector")]
    public float[] Vector { get; set; } = [];

    [JsonPropertyName("payload")]
    public QdrantPayload Payload { get; set; } = new();
}

public class QdrantPayload
{
    [JsonPropertyName("chunkText")]
    public string ChunkText { get; set; } = string.Empty;

    [JsonPropertyName("chunkIndex")]
    public int ChunkIndex { get; set; }

    [JsonPropertyName("characterCount")]
    public int CharacterCount { get; set; }
}

public class QdrantQueryRequest
{
    [JsonPropertyName("query")]
    public float[] Query { get; set; } = [];

    [JsonPropertyName("limit")]
    public int Limit { get; set; } = 5;

    [JsonPropertyName("with_payload")]
    public bool WithPayload { get; set; } = true;

    [JsonPropertyName("with_vector")]
    public bool WithVector { get; set; } = false;
}

public class QdrantQueryResponse
{
    [JsonPropertyName("status")]
    public string Status { get; set; } = string.Empty;

    [JsonPropertyName("time")]
    public double Time { get; set; }

    [JsonPropertyName("result")]
    public QdrantQueryResult Result { get; set; } = new();
}

public class QdrantQueryResult
{
    [JsonPropertyName("points")]
    public List<QdrantScoredPoint> Points { get; set; } = [];
}

public class QdrantScoredPoint
{
    [JsonPropertyName("id")]
    public JsonElement Id { get; set; }

    [JsonPropertyName("score")]
    public double Score { get; set; }

    [JsonPropertyName("payload")]
    public QdrantPayload Payload { get; set; } = new();

    public string GetPointId()
    {
        return Id.ValueKind switch
        {
            JsonValueKind.String => Id.GetString() ?? string.Empty,
            JsonValueKind.Number => Id.ToString(),
            _ => Id.ToString()
        };
    }
}
