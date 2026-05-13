namespace DocumentProcessor.Core.Models;

public enum DocumentType
{
    Unknown = 0,
    Word = 1,
    Pdf = 2
}

public class DocumentProcessingOptions
{
    public bool IncludeImages { get; set; } = true;
    public bool IncludeTables { get; set; } = true;
}

public class ProcessingIssue
{
    public string Code { get; set; } = string.Empty;
    public string Message { get; set; } = string.Empty;
    public string Severity { get; set; } = "Info";
}

public class DocumentParseResult
{
    public string SchemaVersion { get; set; } = "1.0";
    public DocumentType DocumentType { get; set; }
    public string SourceFileName { get; set; } = string.Empty;
    public List<Section> Sections { get; set; } = new();
    public List<ProcessingIssue> Issues { get; set; } = new();
}
