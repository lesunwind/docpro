// src/DocumentProcessor.Core/Services/WordDocumentParser.cs
using DocumentFormat.OpenXml;
using DocumentFormat.OpenXml.Packaging;
using DocumentFormat.OpenXml.Wordprocessing;
using System.Text.Json;
using DocumentProcessor.Core.Models;
using DocumentProcessor.Core.Handlers;
using A = DocumentFormat.OpenXml.Drawing;
using DW = DocumentFormat.OpenXml.Drawing.Wordprocessing;
using PIC = DocumentFormat.OpenXml.Drawing.Pictures;
using V = DocumentFormat.OpenXml.Vml;

public class WordDocumentParser : IDisposable
{
    private readonly string outputPath;
    private readonly ImageHandler imageHandler;
    private readonly TableHandler tableHandler;
    private bool disposed = false;
    private readonly string outputFilename;

    // Modify the constructor to accept an output filename
    public WordDocumentParser(string outputDirectory, string outputFilename = "document_structure.json")
    {
        outputPath = outputDirectory;
        this.outputFilename = outputFilename;
        imageHandler = new ImageHandler();
        tableHandler = new TableHandler();

        if (!Directory.Exists(outputPath))
        {
            Directory.CreateDirectory(outputPath);
        }
    }

    private List<Section> ParseDocument(WordprocessingDocument doc)
    {
        var mainPart = doc.MainDocumentPart;
        if (mainPart?.Document.Body == null) return new List<Section>();

        Console.WriteLine("\n=== Document Structure Analysis ===");

        // Check for image parts
        var imageParts = mainPart.Parts.Where(p => p.OpenXmlPart is ImagePart);
        Console.WriteLine($"Total Image Parts found in document: {imageParts.Count()}");
        foreach (var part in imageParts)
        {
            var imagePart = part.OpenXmlPart as ImagePart;
            Console.WriteLine($"Found Image Part: {imagePart.ContentType}");
        }

        // Check for drawings/pictures
        var body = mainPart.Document.Body;
        var drawings = body.Descendants<Drawing>().ToList();
        Console.WriteLine($"\nTotal Drawing elements found: {drawings.Count}");

        var pictures = body.Descendants<PIC.Picture>().ToList();
        Console.WriteLine($"Total Picture elements found: {pictures.Count}");

        var shapes = body.Descendants<V.Shape>().ToList();
        Console.WriteLine($"Total Shape elements found: {shapes.Count}");

        var inlineShapes = drawings.Where(d => d.Descendants<DW.Inline>().Any()).ToList();
        Console.WriteLine($"Total InlineShape elements found: {inlineShapes.Count}\n");

        Console.WriteLine("=== Starting Section Processing ===");

        var sections = new List<Section>();
        var elements = body.Elements().ToList();

        Section currentSection = null;
        var sectionStack = new Stack<Section>();
        var currentContent = new List<string>();
        var currentSectionElements = new List<OpenXmlElement>();
        int currentLevel = 0;

        for (int i = 0; i < elements.Count; i++)
        {
            var element = elements[i];

            if (element is Paragraph paragraph)
            {
                var styleId = paragraph.ParagraphProperties?.ParagraphStyleId?.Val?.Value;

                if (IsHeadingStyle(styleId))
                {
                    // If we have a current section, finalize it
                    if (currentSection != null)
                    {
                        currentSection.Content = string.Join("\n", currentContent);
                        currentSection.Tables = GetTablesForSection(currentSectionElements, currentSection.Title);
                        currentSection.Images = ExtractImages(currentSectionElements, mainPart);
                        // Clear for next section
                        currentContent = new List<string>();
                        currentSectionElements = new List<OpenXmlElement>();
                    }

                    var headingLevel = GetHeadingLevel(styleId);

                    // Pop sections from stack if we're going to a higher level
                    while (sectionStack.Count > 0 && headingLevel <= currentLevel)
                    {
                        sectionStack.Pop();
                        currentLevel--;
                    }

                    // Create new section
                    currentSection = new Section
                    {
                        Title = paragraph.InnerText.Trim(),
                        Level = headingLevel,
                        Subsections = new List<Section>(),
                        Tables = new List<TableInfo>(),
                        Images = new List<ImageInfo>(),
                        Metadata = new Dictionary<string, string>()
                    };

                    // Add to parent section or main list
                    if (sectionStack.Count > 0)
                    {
                        sectionStack.Peek().Subsections.Add(currentSection);
                    }
                    else
                    {
                        sections.Add(currentSection);
                    }

                    sectionStack.Push(currentSection);
                    currentLevel = headingLevel;
                }
                else if (currentSection != null)
                {
                    // Add content to current section
                    var text = paragraph.InnerText.Trim();
                    if (!string.IsNullOrEmpty(text))
                    {
                        currentContent.Add(text);
                    }
                    currentSectionElements.Add(element);
                }
            }
            else if (currentSection != null)
            {
                currentSectionElements.Add(element);
            }
        }

        // Handle the last section
        if (currentSection != null)
        {
            currentSection.Content = string.Join("\n", currentContent);
            currentSection.Tables = GetTablesForSection(currentSectionElements, currentSection.Title);
            currentSection.Images = ExtractImages(currentSectionElements, mainPart);
        }

        Console.WriteLine($"\nTotal sections processed: {sections.Count}");
        return sections;
    }
    private List<TableInfo> GetTablesForSection(List<OpenXmlElement> elements, string sectionTitle)
    {
        var tables = new List<TableInfo>();
        var currentTable = elements.OfType<Table>().ToList();

        foreach (var table in currentTable)
        {
            // Get preceding text for table title
            var tableIndex = elements.IndexOf(table);
            var precedingElements = elements.Take(tableIndex).ToList();
            var precedingText = string.Join(" ", precedingElements.OfType<Paragraph>()
                .Select(p => p.InnerText));

            var tableInfo = tableHandler.ExtractTableInfo(table, precedingText, sectionTitle);
            if (tableInfo != null)
            {
                tables.Add(tableInfo);
            }
        }

        return tables;
    }

    private bool IsHeadingStyle(string styleId)
    {
        return styleId?.StartsWith("Heading", StringComparison.OrdinalIgnoreCase) == true;
    }

    private int GetHeadingLevel(string styleId)
    {
        if (styleId == null) return 0;
        return int.TryParse(styleId.Replace("Heading", "", StringComparison.OrdinalIgnoreCase),
            out int level) ? level : 0;
    }

private void SaveToJson(List<Section> sections)
{
    try
    {
            Console.WriteLine("\nPreparing to save to JSON...");
            var options = new JsonSerializerOptions
            {
                WriteIndented = true,
                DefaultIgnoreCondition = System.Text.Json.Serialization.JsonIgnoreCondition.WhenWritingNull
            };

            string json = JsonSerializer.Serialize(sections, options);
            Console.WriteLine($"JSON serialization completed. Length: {json.Length:N0} characters");

            var outputPath1 = Path.Combine(outputPath, outputFilename);
            File.WriteAllText(outputPath1, json);
            Console.WriteLine($"JSON file saved to: {outputPath1}");

            // Verify the saved data
            var verificationSections = JsonSerializer.Deserialize<List<Section>>(json);
        if (verificationSections != null)
        {
            Console.WriteLine("\nVerifying saved data:");
            foreach (var section in verificationSections)
            {
                DebugSection(section, 0);
            }
        }
    }
    catch (Exception ex)
    {
        Console.WriteLine($"Error saving JSON: {ex.Message}");
        Console.WriteLine(ex.StackTrace);
        throw;
    }
}
    public void ProcessDocument(string filePath)
    {
        try
        {
            using var doc = WordprocessingDocument.Open(filePath, false);
            var sections = ParseDocument(doc);

            // Debug sections before saving
            foreach (var section in sections)
            {
                DebugSection(section, 0);
            }

            SaveToJson(sections);
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Error processing document: {ex.Message}");
            throw;
        }
    }
    private void DebugSection(Section section, int level)
    {
        var indent = new string(' ', level * 2);
        Console.WriteLine($"{indent}Section: {section.Title}");
        Console.WriteLine($"{indent}Level: {section.Level}");
        Console.WriteLine($"{indent}Content Length: {section.Content?.Length ?? 0}");
        Console.WriteLine($"{indent}Tables: {section.Tables?.Count ?? 0}");
        Console.WriteLine($"{indent}Images: {section.Images?.Count ?? 0}");

        if (section.Images != null && section.Images.Any())
        {
            foreach (var image in section.Images)
            {
                Console.WriteLine($"{indent}  Image: {image.Id}");
                Console.WriteLine($"{indent}    Type: {image.ContentType}");
                Console.WriteLine($"{indent}    Size: {image.Width}x{image.Height}");
                Console.WriteLine($"{indent}    Base64 Length: {image.Base64Data?.Length ?? 0}");
            }
        }

        if (section.Subsections != null)
        {
            foreach (var subsection in section.Subsections)
            {
                DebugSection(subsection, level + 1);
            }
        }
    }

    public void Dispose()
    {
        Dispose(true);
        GC.SuppressFinalize(this);
    }

    protected virtual void Dispose(bool disposing)
    {
        if (!disposed)
        {
            disposed = true;
        }
    }
      
        private string GetPrecedingText(IList<OpenXmlElement> elements, int currentIndex)
        {
            // Look back up to 3 paragraphs to find table title/caption
            var precedingText = new List<string>();
            for (int i = Math.Max(0, currentIndex - 3); i < currentIndex; i++)
            {
                if (elements[i] is Paragraph p)
                {
                    precedingText.Add(p.InnerText);
                }
            }
            return string.Join("\n", precedingText);
        }

       

        private TableInfo? ExtractTableInfo_old(Table table, string precedingText)
        {
            try
            {
                var tableData = new List<List<string>>();
                foreach (var row in table.Elements<TableRow>())
                {
                    var rowData = new List<string>();
                    foreach (var cell in row.Elements<TableCell>())
                    {
                        rowData.Add(string.Join(" ", cell.Elements<Paragraph>().Select(p => p.InnerText.Trim())));
                    }
                    tableData.Add(rowData);
                }

                // Try to find table number and title from preceding text
                var match = System.Text.RegularExpressions.Regex.Match(precedingText, @"Table\s+(\d+)[\s:\-]+(.+)");

                return new TableInfo
                {
                    Id = Guid.NewGuid().ToString(),
                    Number = match.Success ? $"Table {match.Groups[1].Value}" : $"Table {Guid.NewGuid():N}",
                    Title = match.Success ? match.Groups[2].Value.Trim() : "",
                    Data = tableData
                };
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error extracting table: {ex.Message}");
                return null;
            }
        }
        // src/DocumentProcessor.Core/Services/WordDocumentParser.cs

        private TableInfo? ExtractTableInfo(Table table, string precedingText)
        {
            try
            {
                // Extract table data
                var tableData = ExtractTableData(table);

                // Clean and parse the table title
                var cleanTitle = CleanupTableTitle(precedingText);
                Console.WriteLine($"Processing table title from: {cleanTitle}"); // Debug

                // Look for table caption patterns
                var titlePatterns = new[]
                {
            @"Table\s+(?:SEQ\s+Table\s+\*\s+ARABIC\s+)?(\d+)[:\s-]+(.+?)(?=\r|\n|$)",
            @"Table\s+(\d+)[:\s-]+(.+)",
            @"Table\s+(\d+)\s*(.+)"
        };

                string tableNumber = "";
                string tableTitle = "";

                foreach (var pattern in titlePatterns)
                {
                    var match = System.Text.RegularExpressions.Regex.Match(cleanTitle, pattern,
                        System.Text.RegularExpressions.RegexOptions.IgnoreCase |
                        System.Text.RegularExpressions.RegexOptions.Singleline);

                    if (match.Success)
                    {
                        tableNumber = match.Groups[1].Value.Trim();
                        tableTitle = match.Groups[2].Value.Trim();
                        Console.WriteLine($"Matched pattern: {pattern}"); // Debug
                        Console.WriteLine($"Found number: {tableNumber}, title: {tableTitle}"); // Debug
                        break;
                    }
                }

                // If no number found, generate one
                if (string.IsNullOrEmpty(tableNumber))
                {
                    tableNumber = "1";
                }

                var result = new TableInfo
                {
                    Id = Guid.NewGuid().ToString(),
                    Number = $"Table {tableNumber}",
                    Title = tableTitle,
                    Data = tableData
                };

                Console.WriteLine($"Created TableInfo: Number={result.Number}, Title={result.Title}"); // Debug
                return result;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error extracting table: {ex.Message}");
                return null;
            }
        }

        private string CleanupTableTitle(string text)
        {
            if (string.IsNullOrEmpty(text)) return string.Empty;

            // Remove field codes and extra formatting
            var cleanText = text
                .Replace("SEQ Table * ARABIC", "")
                .Replace("SEQ Table *ARABIC", "")
                .Replace("SEQ Table", "")
                .Replace("*ARABIC", "");

            // Remove any GUID-like strings
            cleanText = System.Text.RegularExpressions.Regex.Replace(cleanText,
                @"[0-9a-f]{8}[-]?([0-9a-f]{4}[-]?){3}[0-9a-f]{12}",
                "",
                System.Text.RegularExpressions.RegexOptions.IgnoreCase);

            // Clean up whitespace
            cleanText = System.Text.RegularExpressions.Regex.Replace(cleanText, @"\s+", " ");

            // Remove any remaining special characters
            cleanText = System.Text.RegularExpressions.Regex.Replace(cleanText, @"[^\w\s:\-]", "");

            return cleanText.Trim();
        }

        private List<List<string>> ExtractTableData(Table table)
        {
            var data = new List<List<string>>();
            foreach (var row in table.Elements<TableRow>())
            {
                var rowData = new List<string>();
                foreach (var cell in row.Elements<TableCell>())
                {
                    rowData.Add(string.Join(" ", cell.Elements<Paragraph>()
                        .Select(p => p.InnerText.Trim())));
                }
                data.Add(rowData);
            }
            return data;
        }


    private List<ImageInfo> ExtractImages(IEnumerable<OpenXmlElement> elements, MainDocumentPart mainPart)
    {
        var images = new List<ImageInfo>();
        var processedBlipIds = new HashSet<string>();
        var currentFigureCaption = string.Empty;

        Console.WriteLine("\n=== Image Extraction Debug ===");

        foreach (var element in elements)
        {
            // Check for figure captions or references
            if (element is Paragraph p)
            {
                var text = p.InnerText.Trim();

                // Identify figure captions (should start with "Figure")
                if (text.StartsWith("Figure", StringComparison.OrdinalIgnoreCase))
                {
                    currentFigureCaption = text;
                    Console.WriteLine($"Found figure caption: {currentFigureCaption}");
                    continue;
                }

                // Look for drawings in the paragraph
                var drawings = p.Descendants<Drawing>().ToList();
                if (drawings.Any())
                {
                    Console.WriteLine($"Found {drawings.Count} drawing(s) in paragraph");

                    foreach (var drawing in drawings)
                    {
                        var blips = drawing.Descendants<A.Blip>().ToList();
                        foreach (var blip in blips)
                        {
                            if (blip?.Embed?.Value != null && !processedBlipIds.Contains(blip.Embed.Value))
                            {
                                processedBlipIds.Add(blip.Embed.Value);
                                try
                                {
                                    var imagePart = (ImagePart)mainPart.GetPartById(blip.Embed.Value);
                                    Console.WriteLine($"Processing image part: {imagePart.ContentType}");

                                    using (var stream = imagePart.GetStream())
                                    using (var ms = new MemoryStream())
                                    {
                                        stream.CopyTo(ms);
                                        var imageBytes = ms.ToArray();
                                        ms.Position = 0;

                                        using (var img = System.Drawing.Image.FromStream(ms))
                                        {
                                            var imageInfo = new ImageInfo
                                            {
                                                Id = Guid.NewGuid().ToString(),
                                                // Use the actual figure caption instead of the paragraph text
                                                Title = currentFigureCaption,
                                                ContentType = imagePart.ContentType,
                                                Base64Data = Convert.ToBase64String(imageBytes),
                                                Width = img.Width,
                                                Height = img.Height,
                                                Description = "" // Could be used for additional metadata if needed
                                            };
                                            Console.WriteLine($"Successfully extracted image: {imageInfo.Width}x{imageInfo.Height}");
                                            Console.WriteLine($"Using caption: {imageInfo.Title}");
                                            images.Add(imageInfo);

                                            // Reset the caption after using it
                                            currentFigureCaption = string.Empty;
                                        }
                                    }
                                }
                                catch (Exception ex)
                                {
                                    Console.WriteLine($"Error processing image: {ex.Message}");
                                }
                            }
                        }
                    }
                }
            }
        }

        Console.WriteLine($"\nTotal images found in section: {images.Count}");
        return images;
    }
    private List<ImageInfo> ExtractImages_bk(IEnumerable<OpenXmlElement> elements, MainDocumentPart mainPart)
    {
        var images = new List<ImageInfo>();
        Console.WriteLine("\n=== Image Extraction Debug ===");

        // First, let's look at what elements we have
        foreach (var element in elements)
        {
            Console.WriteLine($"Element type: {element.GetType().Name}");
            if (element is Paragraph p)
            {
                // Check if this paragraph contains a figure reference
                var text = p.InnerText;
                if (text.Contains("Figure") || text.Contains("fig."))
                {
                    Console.WriteLine($"Found figure reference: {text}");

                    // Check for any drawings in this paragraph
                    var drawings = p.Descendants<Drawing>().ToList();
                    if (drawings.Any())
                    {
                        Console.WriteLine($"Found {drawings.Count} drawing(s) in figure paragraph");
                    }

                    // Look for any related image parts
                    var blips = p.Descendants<A.Blip>().ToList();
                    if (blips.Any())
                    {
                        Console.WriteLine($"Found {blips.Count} blip(s) in figure paragraph");
                        foreach (var blip in blips)
                        {
                            Console.WriteLine($"Blip Embed ID: {blip.Embed?.Value}");
                        }
                    }
                }
            }
        }

        // Now look for specific image-related elements
        var imageContainers = elements.SelectMany(e => e.Descendants()).Where(e =>
            e is Drawing ||
            e is PIC.Picture ||
            e is V.Shape ||
            (e is Run r && r.Descendants<Drawing>().Any()));

        foreach (var container in imageContainers)
        {
            Console.WriteLine($"\nProcessing image container: {container.GetType().Name}");

            try
            {
                // Look for image relationships
                var blips = container.Descendants<A.Blip>().ToList();
                foreach (var blip in blips)
                {
                    if (blip.Embed != null)
                    {
                        try
                        {
                            var imagePart = (ImagePart)mainPart.GetPartById(blip.Embed.Value);
                            Console.WriteLine($"Found image part: {imagePart.ContentType}");

                            // Try to get preceding text for title
                            var parentPara = container.Ancestors<Paragraph>().FirstOrDefault();
                            var figureTitle = parentPara?.InnerText ?? "";
                            Console.WriteLine($"Associated text: {figureTitle}");

                            using (var stream = imagePart.GetStream())
                            using (var ms = new MemoryStream())
                            {
                                stream.CopyTo(ms);
                                var imageBytes = ms.ToArray();
                                ms.Position = 0;

                                using (var img = System.Drawing.Image.FromStream(ms))
                                {
                                    var imageInfo = new ImageInfo
                                    {
                                        Id = Guid.NewGuid().ToString(),
                                        Title = figureTitle,
                                        ContentType = imagePart.ContentType,
                                        Base64Data = Convert.ToBase64String(imageBytes),
                                        Width = img.Width,
                                        Height = img.Height
                                    };
                                    Console.WriteLine($"Successfully extracted image: {imageInfo.Width}x{imageInfo.Height}");
                                    images.Add(imageInfo);
                                }
                            }
                        }
                        catch (Exception ex)
                        {
                            Console.WriteLine($"Error processing blip: {ex.Message}");
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error processing container: {ex.Message}");
            }
        }

        Console.WriteLine($"\nTotal images found in section: {images.Count}");
        return images;
    }

    private List<ImageInfo> ExtractImages_v0220(IEnumerable<OpenXmlElement> elements, MainDocumentPart mainPart)
    {
        var images = new List<ImageInfo>();
        var processedBlipIds = new HashSet<string>();

        Console.WriteLine("\n=== Image Extraction Debug ===");

        // Track the text that might be associated with an image
        string currentFigureText = "";

        foreach (var element in elements)
        {
            // Gather text that might be relevant to the next image
            if (element is Paragraph p)
            {
                var text = p.InnerText.Trim();
                if (text.Contains("Figure") || text.Contains("fig."))
                {
                    currentFigureText = text;
                    Console.WriteLine($"Found figure reference: {text}");
                }

                // Look for drawings in the paragraph
                var drawings = p.Descendants<Drawing>().ToList();
                if (drawings.Any())
                {
                    Console.WriteLine($"Found {drawings.Count} drawing(s) in paragraph");

                    foreach (var drawing in drawings)
                    {
                        var blips = drawing.Descendants<A.Blip>().ToList();
                        foreach (var blip in blips)
                        {
                            if (blip?.Embed?.Value != null && !processedBlipIds.Contains(blip.Embed.Value))
                            {
                                processedBlipIds.Add(blip.Embed.Value);
                                try
                                {
                                    var imagePart = (ImagePart)mainPart.GetPartById(blip.Embed.Value);
                                    Console.WriteLine($"Processing image part: {imagePart.ContentType}");

                                    using (var stream = imagePart.GetStream())
                                    using (var ms = new MemoryStream())
                                    {
                                        stream.CopyTo(ms);
                                        var imageBytes = ms.ToArray();
                                        ms.Position = 0;

                                        using (var img = System.Drawing.Image.FromStream(ms))
                                        {
                                            var imageInfo = new ImageInfo
                                            {
                                                Id = Guid.NewGuid().ToString(),
                                                Title = currentFigureText,
                                                ContentType = imagePart.ContentType,
                                                Base64Data = Convert.ToBase64String(imageBytes),
                                                Width = img.Width,
                                                Height = img.Height,
                                                Description = ""
                                            };
                                            Console.WriteLine($"Successfully extracted image: {imageInfo.Width}x{imageInfo.Height}");
                                            images.Add(imageInfo);
                                        }
                                    }
                                }
                                catch (Exception ex)
                                {
                                    Console.WriteLine($"Error processing image: {ex.Message}");
                                }
                            }
                        }
                    }
                }
            }
        }

        Console.WriteLine($"\nTotal images found in section: {images.Count}");
        return images;
    }
    private Section ParseSection(OpenXmlElement element, MainDocumentPart mainPart)
        {
            var section = new Section();
            // ... existing section parsing code ...

            // Add image extraction
            section.Images = ExtractImages(element.Elements(), mainPart);

            return section;
        }
        
       
    
}