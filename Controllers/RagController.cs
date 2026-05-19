using Microsoft.AspNetCore.Mvc;
using MultiModalRagDemo.ViewModels;

namespace MultiModalRagDemo.Controllers;

public class RagController : Controller
{
    private readonly IWebHostEnvironment _environment;

    public RagController(IWebHostEnvironment environment)
    {
        _environment = environment;
    }

    [HttpGet]
    public IActionResult Index()
    {
        return View(new RagUploadViewModel());
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Index(RagUploadViewModel model)
    {
        if (!ModelState.IsValid)
        {
            return View(model);
        }

        var uploadPath = Path.Combine(_environment.WebRootPath, "uploads");

        if (!Directory.Exists(uploadPath))
        {
            Directory.CreateDirectory(uploadPath);
        }

        string? documentFileName = null;
        string? imageFileName = null;

        if (model.DocumentFile is not null && model.DocumentFile.Length > 0)
        {
            documentFileName = await SaveFileAsync(model.DocumentFile, uploadPath);
        }

        if (model.ImageFile is not null && model.ImageFile.Length > 0)
        {
            imageFileName = await SaveFileAsync(model.ImageFile, uploadPath);
        }

        ViewBag.Message = "Files uploaded successfully.";
        ViewBag.DocumentFile = documentFileName;
        ViewBag.ImageFile = imageFileName;
        ViewBag.Question = model.Question;

        return View(model);
    }

    private static async Task<string> SaveFileAsync(IFormFile file, string uploadPath)
    {
        var safeFileName = $"{Guid.NewGuid()}_{Path.GetFileName(file.FileName)}";
        var filePath = Path.Combine(uploadPath, safeFileName);

        await using var stream = new FileStream(filePath, FileMode.Create);
        await file.CopyToAsync(stream);

        return safeFileName;
    }
}