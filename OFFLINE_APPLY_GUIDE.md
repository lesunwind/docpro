# Offline Apply Guide: Desktop + API Architecture Changes

This guide explains how an offline team (no GitHub access) can apply the recent architecture update manually to an existing checkout of this repository.

## 1) What changed

The change set introduces:

- A new API project: `src/DocumentProcessor.Api`
- New Core abstractions for parser-based architecture
- New Core application services for document type detection and routing
- New shared parse result models
- New parsing adapters for Word and PDF (PDF currently returns a structured "not implemented" warning)
- Solution update to include the API project
- Core project multi-target update to support API host runtime

## 1.1) Project structure before vs after

### Previous structure (before this architecture update)

```text
DocumentProcessor.sln
├─ src/
│  ├─ DocumentProcessor.Core/
│  │  ├─ Handlers/
│  │  ├─ Models/
│  │  ├─ Services/
│  │  └─ DocumentProcessor.Core.csproj
│  ├─ DocumentProcessor.Console/
│  │  └─ DocumentProcessor.Console.csproj
│  └─ DocumentProcessor.UI/
│     └─ DocumentProcessor.UI.csproj
└─ tests/
   └─ DocumentProcessor.Tests/
```

### New structure (after this architecture update)

```text
DocumentProcessor.sln
├─ src/
│  ├─ DocumentProcessor.Api/                    <-- NEW
│  │  ├─ DocumentProcessor.Api.csproj
│  │  └─ Program.cs
│  ├─ DocumentProcessor.Core/
│  │  ├─ Abstractions/                          <-- NEW
│  │  │  ├─ IDocumentParser.cs
│  │  │  ├─ IDocumentProcessingService.cs
│  │  │  └─ IDocumentTypeDetector.cs
│  │  ├─ Application/                           <-- NEW
│  │  │  ├─ DefaultDocumentTypeDetector.cs
│  │  │  └─ DocumentProcessingService.cs
│  │  ├─ Parsing/                               <-- NEW
│  │  │  ├─ PdfDocumentParser.cs
│  │  │  └─ WordDocumentParserAdapter.cs
│  │  ├─ Handlers/
│  │  ├─ Models/
│  │  │  └─ DocumentModels.cs                   <-- NEW
│  │  ├─ Services/
│  │  └─ DocumentProcessor.Core.csproj          <-- UPDATED (multitarget)
│  ├─ DocumentProcessor.Console/
│  │  └─ DocumentProcessor.Console.csproj
│  └─ DocumentProcessor.UI/
│     └─ DocumentProcessor.UI.csproj
├─ tests/
│  └─ DocumentProcessor.Tests/
├─ SOURCE_CODE_OVERVIEW.md
└─ OFFLINE_APPLY_GUIDE.md
```

## 2) Files to add

Create the following files exactly as shown in section **5) File contents**:

- `src/DocumentProcessor.Api/DocumentProcessor.Api.csproj`
- `src/DocumentProcessor.Api/Program.cs`
- `src/DocumentProcessor.Core/Abstractions/IDocumentParser.cs`
- `src/DocumentProcessor.Core/Abstractions/IDocumentProcessingService.cs`
- `src/DocumentProcessor.Core/Abstractions/IDocumentTypeDetector.cs`
- `src/DocumentProcessor.Core/Application/DefaultDocumentTypeDetector.cs`
- `src/DocumentProcessor.Core/Application/DocumentProcessingService.cs`
- `src/DocumentProcessor.Core/Models/DocumentModels.cs`
- `src/DocumentProcessor.Core/Parsing/WordDocumentParserAdapter.cs`
- `src/DocumentProcessor.Core/Parsing/PdfDocumentParser.cs`
- `OFFLINE_APPLY_GUIDE.md` (this guide, optional to keep)

## 3) Files to modify

### 3.1 `src/DocumentProcessor.Core/DocumentProcessor.Core.csproj`

Change:

```xml
<TargetFramework>net6.0-windows</TargetFramework>
```

To:

```xml
<TargetFrameworks>net6.0-windows;net8.0</TargetFrameworks>
```

No other edit is required in this file.

### 3.2 `DocumentProcessor.sln`

Add project entry:

```text
Project("{FAE04EC0-301F-11D3-BF4B-00C04F79EFBC}") = "DocumentProcessor.Api", "src\DocumentProcessor.Api\DocumentProcessor.Api.csproj", "{A1B2C3D4-E5F6-47A8-9123-ABCDEF123456}"
EndProject
```

Add project configuration mappings:

```text
{A1B2C3D4-E5F6-47A8-9123-ABCDEF123456}.Debug|Any CPU.ActiveCfg = Debug|Any CPU
{A1B2C3D4-E5F6-47A8-9123-ABCDEF123456}.Debug|Any CPU.Build.0 = Debug|Any CPU
{A1B2C3D4-E5F6-47A8-9123-ABCDEF123456}.Release|Any CPU.ActiveCfg = Release|Any CPU
{A1B2C3D4-E5F6-47A8-9123-ABCDEF123456}.Release|Any CPU.Build.0 = Release|Any CPU
```

Add nested project mapping under `src` folder:

```text
{A1B2C3D4-E5F6-47A8-9123-ABCDEF123456} = {DF6ABB5B-1748-4B3B-92EB-2A563CC76C31}
```

> Note: The GUID is fixed in the delivered patch above. If your team prefers to generate a new GUID, update it consistently in all three places.

## 4) Directory structure to create

Create missing directories:

- `src/DocumentProcessor.Api/`
- `src/DocumentProcessor.Core/Abstractions/`
- `src/DocumentProcessor.Core/Application/`
- `src/DocumentProcessor.Core/Parsing/`

## 5) File contents

## `src/DocumentProcessor.Api/DocumentProcessor.Api.csproj`

```xml
<Project Sdk="Microsoft.NET.Sdk.Web">
  <PropertyGroup>
    <TargetFramework>net8.0</TargetFramework>
    <Nullable>enable</Nullable>
    <ImplicitUsings>enable</ImplicitUsings>
  </PropertyGroup>

  <ItemGroup>
    <ProjectReference Include="..\DocumentProcessor.Core\DocumentProcessor.Core.csproj" />
  </ItemGroup>
</Project>
```

## `src/DocumentProcessor.Api/Program.cs`

```csharp
using DocumentProcessor.Core.Abstractions;
using DocumentProcessor.Core.Application;
using DocumentProcessor.Core.Parsing;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddSingleton<IDocumentTypeDetector, DefaultDocumentTypeDetector>();
builder.Services.AddSingleton<IDocumentParser, WordDocumentParserAdapter>();
builder.Services.AddSingleton<IDocumentParser, PdfDocumentParser>();
builder.Services.AddSingleton<IDocumentProcessingService, DocumentProcessingService>();

var app = builder.Build();

app.MapPost("/api/v1/parse", async (IFormFile file, IDocumentProcessingService service, CancellationToken ct) =>
{
    if (file == null || file.Length == 0)
    {
        return Results.BadRequest(new { message = "No file uploaded." });
    }

    await using var stream = file.OpenReadStream();
    var result = await service.ParseAsync(stream, file.FileName, file.ContentType, cancellationToken: ct);
    return Results.Ok(result);
});

app.Run();
```

## `src/DocumentProcessor.Core/Abstractions/IDocumentParser.cs`

```csharp
using DocumentProcessor.Core.Models;

namespace DocumentProcessor.Core.Abstractions;

public interface IDocumentParser
{
    bool CanHandle(DocumentType documentType);
    Task<DocumentParseResult> ParseAsync(Stream input, string fileName, DocumentProcessingOptions? options = null, CancellationToken cancellationToken = default);
}
```

## `src/DocumentProcessor.Core/Abstractions/IDocumentProcessingService.cs`

```csharp
using DocumentProcessor.Core.Models;

namespace DocumentProcessor.Core.Abstractions;

public interface IDocumentProcessingService
{
    Task<DocumentParseResult> ParseAsync(Stream input, string fileName, string? contentType = null, DocumentProcessingOptions? options = null, CancellationToken cancellationToken = default);
}
```

## `src/DocumentProcessor.Core/Abstractions/IDocumentTypeDetector.cs`

```csharp
using DocumentProcessor.Core.Models;

namespace DocumentProcessor.Core.Abstractions;

public interface IDocumentTypeDetector
{
    DocumentType Detect(string fileName, string? contentType = null);
}
```

## `src/DocumentProcessor.Core/Application/DefaultDocumentTypeDetector.cs`

```csharp
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
```

## `src/DocumentProcessor.Core/Application/DocumentProcessingService.cs`

```csharp
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
```

## `src/DocumentProcessor.Core/Models/DocumentModels.cs`

```csharp
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
```

## `src/DocumentProcessor.Core/Parsing/WordDocumentParserAdapter.cs`

```csharp
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
```

## `src/DocumentProcessor.Core/Parsing/PdfDocumentParser.cs`

```csharp
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
```

## 6) Offline verification checklist

If your offline environment has .NET SDK installed:

1. `dotnet --info`
2. `dotnet restore DocumentProcessor.sln`
3. `dotnet build DocumentProcessor.sln`
4. `dotnet test tests/DocumentProcessor.Tests/DocumentProcessor.Tests.csproj`
5. `dotnet run --project src/DocumentProcessor.Api/DocumentProcessor.Api.csproj`
6. POST a file to `http://localhost:5000/api/v1/parse` (or launch URL shown by runtime)

If your offline environment does not have `dotnet`, complete only static file checks and defer runtime checks.

## 7) Packaging for offline transfer

Recommended package:

1. Copy repository folder after applying changes.
2. Create archive: `tar -czf docprocessor-offline-update.tgz <repo-folder>`
3. Share archive via internal secure channel (USB/internal file share).
4. Receiver extracts and runs checklist above.

## 8) Known limitations in this change set

- PDF path is currently a scaffold and returns warning metadata instead of extracted sections.
- Word adapter currently uses temporary file I/O to reuse existing parser implementation.

