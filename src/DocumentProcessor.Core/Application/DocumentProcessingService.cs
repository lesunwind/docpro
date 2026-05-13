using DocumentProcessor.Core.Abstractions;
using DocumentProcessor.Core.Models;

namespace DocumentProcessor.Core.Application;

public class DocumentProcessingService : IDocumentProcessingService
{
    private readonly IEnumerable<IDocumentParser> _parsers;
    private readonly IDocumentTypeDetector _typeDetector;

    public DocumentProcessingService(IEnumerable<IDocumentParser> parsers, IDocumentTypeDetector typeDetector)
    {
        _parsers = parsers;
        _typeDetector = typeDetector;
    }

    public async Task<DocumentParseResult> ParseAsync(Stream input, string fileName, string? contentType = null, DocumentProcessingOptions? options = null, CancellationToken cancellationToken = default)
    {
        var docType = _typeDetector.Detect(fileName, contentType);
        var parser = _parsers.FirstOrDefault(p => p.CanHandle(docType));

        if (parser == null)
        {
            throw new NotSupportedException($"No parser is registered for document type: {docType}");
        }

        var result = await parser.ParseAsync(input, fileName, options, cancellationToken);
        result.DocumentType = docType;
        result.SourceFileName = fileName;

        return result;
    }
}
