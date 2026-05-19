using Microsoft.AspNetCore.Http;
using System.ComponentModel.DataAnnotations;

namespace MultiModalRagDemo.ViewModels;

public class RagUploadViewModel
{
    [Display(Name = "Document File")]
    public IFormFile? DocumentFile { get; set; }

    [Display(Name = "Image File")]
    public IFormFile? ImageFile { get; set; }

    [Required]
    [Display(Name = "Your Question")]
    public string Question { get; set; } = string.Empty;
}