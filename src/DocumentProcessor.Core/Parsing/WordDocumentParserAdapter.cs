using System.Text.Json;
using DocumentProcessor.Core.Abstractions;
using DocumentProcessor.Core.Models;

namespace DocumentProcessor.Core.Parsing;

public class WordDocumentParserAdapter : IDocumentParser
{
    public bool CanHandle(DocumentType documentType) => documentType == DocumentType.Word;

    public Task<DocumentParseResult> ParseAsync(Stream input, string fileName, DocumentProcessingOptions? options = null, CancellationToken cancellationToken = default)
    {
        var tempRoot = Path.Combine(Path.GetTempPath(), "DocumentProcessor", Guid.NewGuid().ToString("N"));
        Directory.CreateDirectory(tempRoot);

        var inputPath = Path.Combine(tempRoot, fileName);
        var outputJsonName = $"{Path.GetFileNameWithoutExtension(fileName)}.json";
        var outputJsonPath = Path.Combine(tempRoot, outputJsonName);

        using (var fs = File.Create(inputPath))
        {
            input.CopyTo(fs);
        }

        using (var parser = new WordDocumentParser(tempRoot, outputJsonName))
        {
            parser.ProcessDocument(inputPath);
        }

        var json = File.ReadAllText(outputJsonPath);
        var sections = JsonSerializer.Deserialize<List<Section>>(json) ?? new List<Section>();

        var result = new DocumentParseResult
        {
            DocumentType = DocumentType.Word,
            SourceFileName = fileName,
            Sections = sections
        };

        try
        {
            Directory.Delete(tempRoot, true);
        }
        catch
        {
            // Cleanup failure should not fail parsing.
        }

        return Task.FromResult(result);
    }
}
