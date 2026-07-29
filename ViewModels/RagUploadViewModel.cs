using MultiModalRagDemo.Models;
using System.ComponentModel.DataAnnotations;

namespace MultiModalRagDemo.ViewModels;

public class RagUploadViewModel
{
    [Display(Name = "Document File")]
    public IFormFile? DocumentFile { get; set; }

    [Display(Name = "Image File")]
    public IFormFile? ImageFile { get; set; }

    [Display(Name = "Your Question")]
    public string? Question { get; set; } = string.Empty;

    public string? UploadedDocumentName { get; set; }

    public string ExtractedText { get; set; } = string.Empty;

    public List<TextChunk> Chunks { get; set; } = new();

    public bool EmbeddingSucceeded { get; set; }

    public string EmbeddingStatusMessage { get; set; } = string.Empty;

    public int EmbeddingCount { get; set; }

    public int VectorDimension { get; set; }
    public bool VectorsStored { get; set; }

    public int StoredVectorCount { get; set; }

    public string? VectorStoreMessage { get; set; }

    [Display(Name = "Number of chunks to retrieve")]
    [Range(1, 10)]
    public int TopK { get; set; } = 5;

    public List<RetrievedChunk> RetrievedChunks { get; set; } = [];

    public string SearchMessage { get; set; } = string.Empty;

    public string GeneratedAnswer { get; set; } = string.Empty;

    public bool AnswerGenerationSucceeded { get; set; }

    public string AnswerGenerationMessage { get; set; } = string.Empty;


}