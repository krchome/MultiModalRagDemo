using Microsoft.AspNetCore.Mvc;
using MultimodalRagDemo.Services.Interfaces;
using MultiModalRagDemo.Services;
using MultiModalRagDemo.ViewModels;

namespace MultiModalRagDemo.Controllers;

public class RagController : Controller
{
    private readonly IDocumentTextExtractor _documentTextExtractor;
    private readonly ITextChunkingService _textChunkingService;

    public RagController(
        IDocumentTextExtractor documentTextExtractor,
        ITextChunkingService textChunkingService)
    {
        _documentTextExtractor = documentTextExtractor;
        _textChunkingService = textChunkingService;
    }

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

        using (var stream = new FileStream(documentFilePath, FileMode.Create))
        {
            await model.DocumentFile.CopyToAsync(stream);
        }

        string extractedText =
            await _documentTextExtractor.ExtractTextAsync(documentFilePath);

        model.ExtractedText = extractedText;

        model.Chunks = _textChunkingService.ChunkText(
            extractedText,
            chunkSize: 800,
            overlapSize: 100);
        model.UploadedDocumentName = model.DocumentFile.FileName;


        return View(model);
    }
}

    

   

