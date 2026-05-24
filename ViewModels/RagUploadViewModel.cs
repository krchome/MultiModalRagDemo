using Microsoft.AspNetCore.Http;
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


}