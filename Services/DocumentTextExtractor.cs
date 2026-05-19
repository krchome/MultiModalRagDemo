using UglyToad.PdfPig;

namespace MultiModalRagDemo.Services;

public class DocumentTextExtractor : IDocumentTextExtractor
{
    public async Task<string> ExtractTextAsync(string filePath)
    {
        if (string.IsNullOrWhiteSpace(filePath))
        {
            return string.Empty;
        }

        if (!File.Exists(filePath))
        {
            return string.Empty;
        }

        var extension = Path.GetExtension(filePath).ToLowerInvariant();

        return extension switch
        {
            ".txt" => await ExtractFromTextFileAsync(filePath),
            ".pdf" => ExtractFromPdf(filePath),
            _ => "Unsupported file type. Please upload a .txt or .pdf file."
        };
    }

    private static async Task<string> ExtractFromTextFileAsync(string filePath)
    {
        return await File.ReadAllTextAsync(filePath);
    }

    private static string ExtractFromPdf(string filePath)
    {
        using var document = PdfDocument.Open(filePath);

        var textParts = new List<string>();

        foreach (var page in document.GetPages())
        {
            textParts.Add(page.Text);
        }

        return string.Join(Environment.NewLine, textParts);
    }
}

