namespace MultiModalRagDemo.Services.Embedding
{
    public class FastApiEmbeddingClient : IEmbeddingClient
    {
        private readonly HttpClient _httpClient;
        private readonly ILogger<FastApiEmbeddingClient> _logger;

        public FastApiEmbeddingClient(
            HttpClient httpClient,
            ILogger<FastApiEmbeddingClient> logger)
        {
            _httpClient = httpClient;
            _logger = logger;
        }

        public async Task<EmbeddingClientResult> CreateEmbeddingsAsync(
            List<string> texts,
            CancellationToken cancellationToken = default)
        {
            var validTexts = texts
                .Where(text => !string.IsNullOrWhiteSpace(text))
                .ToList();

            if (validTexts.Count==0)
            {
                return EmbeddingClientResult.Failure(
                    "No valid text chunks were available for embedding.");
            }

            var request = new EmbeddingRequestDto
            {
                Texts = validTexts
            };

            try
            {
                using var response = await _httpClient.PostAsJsonAsync(
                    "/embed",
                    request,
                    cancellationToken);

                if (!response.IsSuccessStatusCode)
                {
                    var errorBody = await response.Content.ReadAsStringAsync(
                        cancellationToken);

                    _logger.LogWarning(
                        "Embedding service returned status code {StatusCode}. Body: {Body}",
                        response.StatusCode,
                        errorBody);

                    return EmbeddingClientResult.Failure(
                        $"Embedding service returned an error: {(int)response.StatusCode} {response.ReasonPhrase}");
                }

                var embeddingResponse =
                    await response.Content.ReadFromJsonAsync<EmbeddingResponseDto>(
                        cancellationToken);

                if (embeddingResponse == null ||
                    embeddingResponse.Embeddings == null ||
                    embeddingResponse.Embeddings.Count==0)
                {
                    return EmbeddingClientResult.Failure(
                        "Embedding service returned an empty response.");
                }

                return EmbeddingClientResult.Success(embeddingResponse.Embeddings);
            }
            catch (HttpRequestException ex)
            {
                _logger.LogError(ex, "Could not reach the FastAPI embedding service.");

                return EmbeddingClientResult.Failure(
                    "Could not reach the FastAPI embedding service. Please make sure the Python service is running on http://localhost:8000.");
            }
            catch (TaskCanceledException ex) when (!cancellationToken.IsCancellationRequested)
            {
                _logger.LogError(ex, "The embedding service request timed out.");

                return EmbeddingClientResult.Failure(
                    "The embedding service request timed out. Try again or increase the timeout.");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Unexpected error while generating embeddings.");

                return EmbeddingClientResult.Failure(
                    "Unexpected error while generating embeddings.");
            }
        }
    }
}
