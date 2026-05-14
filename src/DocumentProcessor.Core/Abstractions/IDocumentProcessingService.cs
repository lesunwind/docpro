using DocumentProcessor.Core.Models;

namespace DocumentProcessor.Core.Abstractions;

public interface IDocumentProcessingService
{
    Task<DocumentParseResult> ParseAsync(Stream input, string fileName, string? contentType = null, DocumentProcessingOptions? options = null, CancellationToken cancellationToken = default);
}
