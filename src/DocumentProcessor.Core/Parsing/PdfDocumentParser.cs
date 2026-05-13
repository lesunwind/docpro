using DocumentProcessor.Core.Abstractions;
using DocumentProcessor.Core.Models;

namespace DocumentProcessor.Core.Parsing;

public class PdfDocumentParser : IDocumentParser
{
    public bool CanHandle(DocumentType documentType) => documentType == DocumentType.Pdf;

    public Task<DocumentParseResult> ParseAsync(Stream input, string fileName, DocumentProcessingOptions? options = null, CancellationToken cancellationToken = default)
    {
        var result = new DocumentParseResult
        {
            DocumentType = DocumentType.Pdf,
            SourceFileName = fileName,
            Sections = new List<Section>(),
            Issues = new List<ProcessingIssue>
            {
                new()
                {
                    Code = "PDF_PARSER_NOT_IMPLEMENTED",
                    Severity = "Warning",
                    Message = "PDF parser contract is wired, but native PDF extraction is not yet implemented."
                }
            }
        };

        return Task.FromResult(result);
    }
}
