using DocumentProcessor.Core.Abstractions;
using DocumentProcessor.Core.Models;

namespace DocumentProcessor.Core.Application;

public class DefaultDocumentTypeDetector : IDocumentTypeDetector
{
    public DocumentType Detect(string fileName, string? contentType = null)
    {
        var extension = Path.GetExtension(fileName)?.ToLowerInvariant();

        return extension switch
        {
            ".docx" => DocumentType.Word,
            ".pdf" => DocumentType.Pdf,
            _ => DetectByContentType(contentType)
        };
    }

    private static DocumentType DetectByContentType(string? contentType)
    {
        if (string.IsNullOrWhiteSpace(contentType))
        {
            return DocumentType.Unknown;
        }

        if (contentType.Contains("wordprocessingml", StringComparison.OrdinalIgnoreCase))
        {
            return DocumentType.Word;
        }

        if (contentType.Contains("pdf", StringComparison.OrdinalIgnoreCase))
        {
            return DocumentType.Pdf;
        }

        return DocumentType.Unknown;
    }
}
