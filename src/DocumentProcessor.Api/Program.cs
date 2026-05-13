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
