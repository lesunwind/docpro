using DocumentProcessor.Core.Models;

namespace DocumentProcessor.Core.Abstractions;

public interface IDocumentTypeDetector
{
    DocumentType Detect(string fileName, string? contentType = null);
}
