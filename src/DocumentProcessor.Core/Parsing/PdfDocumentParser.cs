using System.Text.RegularExpressions;
using DocumentProcessor.Core.Abstractions;
using DocumentProcessor.Core.Models;
using UglyToad.PdfPig;

namespace DocumentProcessor.Core.Parsing;

public class PdfDocumentParser : IDocumentParser
{
    private static readonly Regex NumberedHeadingPattern = new(@"^(\d+(?:\.\d+)*)\s+(.+)$", RegexOptions.Compiled);

    public bool CanHandle(DocumentType documentType) => documentType == DocumentType.Pdf;

    public Task<DocumentParseResult> ParseAsync(Stream input, string fileName, DocumentProcessingOptions? options = null, CancellationToken cancellationToken = default)
    {
        var result = new DocumentParseResult
        {
            DocumentType = DocumentType.Pdf,
            SourceFileName = fileName
        };

        try
        {
            using var document = PdfDocument.Open(input);
            var sections = ParseSections(document, result.Issues);
            result.Sections = sections;

            if (sections.Count == 0)
            {
                result.Issues.Add(new ProcessingIssue
                {
                    Code = "PDF_NO_SECTIONS",
                    Severity = "Warning",
                    Message = "No heading-like structures were inferred from PDF text."
                });
            }
        }
        catch (Exception ex)
        {
            result.Issues.Add(new ProcessingIssue
            {
                Code = "PDF_PARSE_ERROR",
                Severity = "Error",
                Message = $"Failed to parse PDF: {ex.Message}"
            });
        }

        return Task.FromResult(result);
    }

    private static List<Section> ParseSections(PdfDocument document, List<ProcessingIssue> issues)
    {
        var sections = new List<Section>();
        var sectionStack = new Stack<Section>();
        Section? currentSection = null;

        foreach (var page in document.GetPages())
        {
            var lines = (page.Text ?? string.Empty)
                .Split(new[] { "\r\n", "\n" }, StringSplitOptions.RemoveEmptyEntries)
                .Select(l => l.Trim())
                .Where(l => !string.IsNullOrWhiteSpace(l))
                .ToList();

            foreach (var line in lines)
            {
                var heading = TryParseHeading(line);
                if (heading is not null)
                {
                    var (level, title) = heading.Value;

                    while (sectionStack.Count > 0 && sectionStack.Peek().Level >= level)
                    {
                        sectionStack.Pop();
                    }

                    var section = new Section
                    {
                        Title = title,
                        Level = level,
                        Metadata = new Dictionary<string, string>
                        {
                            ["source_page_start"] = page.Number.ToString(),
                            ["heading_confidence"] = "medium"
                        }
                    };

                    if (sectionStack.Count == 0)
                    {
                        sections.Add(section);
                    }
                    else
                    {
                        sectionStack.Peek().Subsections.Add(section);
                    }

                    sectionStack.Push(section);
                    currentSection = section;
                    continue;
                }

                if (currentSection == null)
                {
                    currentSection = new Section
                    {
                        Title = "Document Content",
                        Level = 1,
                        Metadata = new Dictionary<string, string>
                        {
                            ["source_page_start"] = page.Number.ToString(),
                            ["heading_confidence"] = "low"
                        }
                    };
                    sections.Add(currentSection);
                    sectionStack.Push(currentSection);
                    issues.Add(new ProcessingIssue
                    {
                        Code = "PDF_HEADING_FALLBACK",
                        Severity = "Info",
                        Message = "Created fallback top-level section because no heading was found at document start."
                    });
                }

                currentSection.Content = string.IsNullOrWhiteSpace(currentSection.Content)
                    ? line
                    : $"{currentSection.Content}\n{line}";
            }
        }

        return sections;
    }

    private static (int Level, string Title)? TryParseHeading(string line)
    {
        var numberedMatch = NumberedHeadingPattern.Match(line);
        if (numberedMatch.Success)
        {
            var numbering = numberedMatch.Groups[1].Value;
            var title = numberedMatch.Groups[2].Value.Trim();
            var level = numbering.Count(c => c == '.') + 1;
            return (Math.Clamp(level, 1, 6), title);
        }

        if (line.Length <= 80 && line.Equals(line.ToUpperInvariant(), StringComparison.Ordinal) && line.Any(char.IsLetter))
        {
            return (1, line.Trim());
        }

        return null;
    }
}
