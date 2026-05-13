using DocumentProcessor.Core.Models;

namespace DocumentProcessor.Core.Abstractions;

public interface IDocumentParser
{
    bool CanHandle(DocumentType documentType);
    Task<DocumentParseResult> ParseAsync(Stream input, string fileName, DocumentProcessingOptions? options = null, CancellationToken cancellationToken = default);
}
