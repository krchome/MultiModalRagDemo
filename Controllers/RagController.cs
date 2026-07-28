using Microsoft.AspNetCore.Mvc;
using MultiModalRagDemo.Models;

//using MultimodalRagDemo.Services.;
using MultiModalRagDemo.Services;
using MultiModalRagDemo.Services.Embedding;
using MultiModalRagDemo.ViewModels;
using OpenAI.Embeddings;

namespace MultimodalRagDemo.Controllers
{
    public class RagController(
        IDocumentTextExtractor documentTextExtractor,
        ITextChunkingService textChunkingService,
        IEmbeddingClient embeddingClient,
        IVectorStoreService _vectorStoreService,
        IVectorSearchService _vectorSearchService,
        ILogger<RagController> _logger) : Controller
    {
        [HttpGet]
        public IActionResult Index()
        {
            return View(new RagUploadViewModel());
        }

        [HttpPost]
        public async Task<IActionResult> Index(RagUploadViewModel model)
        {
            if (model.DocumentFile == null || model.DocumentFile.Length == 0)
            {
                ModelState.AddModelError(
                    nameof(model.DocumentFile),
                    "Please upload a PDF or TXT document.");

                return View(model);
            }

            string uploadsFolder = Path.Combine(
                Directory.GetCurrentDirectory(),
                "wwwroot",
                "uploads");

            Directory.CreateDirectory(uploadsFolder);

            string documentFilePath = Path.Combine(
                uploadsFolder,
                model.DocumentFile.FileName);

            await using (var stream = new FileStream(documentFilePath, FileMode.Create))
            {
                await model.DocumentFile.CopyToAsync(stream);
            }

            string extractedText =
                await documentTextExtractor.ExtractTextAsync(documentFilePath);

            model.ExtractedText = extractedText;

            model.Chunks = textChunkingService.ChunkText(
                extractedText,
                chunkSize: 800,
                overlapSize: 100);
            model.UploadedDocumentName = model.DocumentFile.FileName;
            if (model.Chunks.Count!=0)
            {
                try
                {
                    var chunkTexts = model.Chunks
                        .Select(chunk => chunk.Content)
                        .ToList();

                    var embeddingResult =
                        await embeddingClient.CreateEmbeddingsAsync(chunkTexts);

                    model.EmbeddingSucceeded = embeddingResult.IsSuccess;
                    model.EmbeddingStatusMessage = embeddingResult.Message;
                    model.EmbeddingCount = embeddingResult.EmbeddingCount;
                    model.VectorDimension = embeddingResult.VectorDimension;

                    var vectorRecords = new List<VectorRecord>();

                    for (var i = 0; i < model.Chunks.Count; i++)
                    {
                        if (i >= embeddingResult.EmbeddingCount)
                        {
                            break;
                        }

                        var chunk = model.Chunks[i];
                        var embedding = embeddingResult.Embeddings[i];

                        vectorRecords.Add(new VectorRecord
                        {
                            ChunkText = chunk.Content,
                            ChunkIndex = chunk.ChunkIndex,
                            CharacterCount = chunk.Content.Length,
                            Embedding = embedding.ToArray()
                        });
                    }
                    var storedCount =
               await _vectorStoreService.StoreVectorsAsync(vectorRecords);

                    model.VectorsStored = true;
                    model.StoredVectorCount = storedCount;
                    model.VectorStoreMessage =
                        $"{storedCount} chunk embeddings stored successfully in Qdrant.";

                }
                catch (Exception ex)
                {
                    _logger.LogError(ex, "Embedding generation or vector storage failed.");
                    //model.
                    model.VectorStoreMessage = "An error occurred while storing vectors.";
                }
            }

            return View(model);
        }
        [HttpPost]
        public async Task<IActionResult> Ask(
        RagUploadViewModel model,
        CancellationToken cancellationToken)
        {
            if (string.IsNullOrWhiteSpace(model.Question))
            {
                ModelState.AddModelError(
                    nameof(model.Question),
                    "Please enter a question.");

                return View("Index", model);
            }

            try
            {
                var retrievedChunks = await _vectorSearchService.SearchAsync(
                    model.Question,
                    model.TopK,
                    cancellationToken);

                model.RetrievedChunks = retrievedChunks.ToList();

                if (model.RetrievedChunks.Count == 0)
                {
                    model.SearchMessage =
                        "No matching chunks were found. Try a different question or upload a document first.";
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Semantic retrieval failed.");

                model.SearchMessage =
                    "Semantic retrieval failed. Please check that Qdrant and the FastAPI embedding service are running.";
            }

            return View("Index", model);

        }
    }
}
