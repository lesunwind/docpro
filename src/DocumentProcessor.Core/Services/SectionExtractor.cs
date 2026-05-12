// src/DocumentProcessor.Core/Services/SectionExtractor.cs
using System.Text.Json;
using DocumentFormat.OpenXml;
using DocumentFormat.OpenXml.Packaging;
using DocumentFormat.OpenXml.Wordprocessing;
using DocumentProcessor.Core.Models;
using DocumentProcessor.Core.Handlers;

namespace DocumentProcessor.Core.Services
{
    public class SectionExtractor
    {
        private readonly ImageHandler imageHandler;
        public void ExtractSectionToNewDocument_v0219(string jsonPath, string outputPath, string[] sectionPath)
        {
            try
            {
                Console.WriteLine($"\nStarting section extraction with detailed logging...");

                var targetSection = GetTargetSection(jsonPath, sectionPath);
                if (targetSection == null)
                {
                    Console.WriteLine("Target section not found!");
                    return;
                }

                Console.WriteLine($"\nTarget section details:");
                Console.WriteLine($"Title: {targetSection.Title}");
                Console.WriteLine($"Content length: {targetSection.Content?.Length ?? 0}");
                Console.WriteLine($"Number of tables: {targetSection.Tables?.Count ?? 0}");
                Console.WriteLine($"Number of images: {targetSection.Images?.Count ?? 0}");

                if (targetSection.Images != null)
                {
                    foreach (var img in targetSection.Images)
                    {
                        Console.WriteLine($"\nImage details:");
                        Console.WriteLine($"ID: {img.Id}");
                        Console.WriteLine($"Content Type: {img.ContentType}");
                        Console.WriteLine($"Dimensions: {img.Width}x{img.Height}");
                        Console.WriteLine($"Base64 data length: {img.Base64Data?.Length ?? 0}");
                    }
                }

                using (var document = WordprocessingDocument.Create(outputPath, WordprocessingDocumentType.Document))
                {
                    Console.WriteLine("\nCreating document...");
                    CreateDocumentWithStyles(document);

                    var mainPart = document.MainDocumentPart!;
                    var body = mainPart.Document.Body!;

                    Console.WriteLine("Adding section title...");
                    AppendSectionTitle(body, targetSection);

                    Console.WriteLine("Adding section content...");
                    AppendSectionContent(body, targetSection);

                    if (targetSection.Tables != null && targetSection.Tables.Any())
                    {
                        Console.WriteLine($"Processing {targetSection.Tables.Count} tables...");
                        AppendSectionTables(body, targetSection.Tables);
                    }

                    if (targetSection.Images != null && targetSection.Images.Any())
                    {
                        Console.WriteLine($"\nStarting image processing for {targetSection.Images.Count} images...");
                        AppendSectionImages(body, targetSection.Images, document);
                    }
                    else
                    {
                        Console.WriteLine("No images to process in this section.");
                    }

                    Console.WriteLine("\nSaving document...");
                    document.Save();
                }

                Console.WriteLine($"Document created successfully at: {outputPath}");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"\nERROR in ExtractSectionToNewDocument: {ex.Message}");
                Console.WriteLine($"Stack trace: {ex.StackTrace}");
                throw;
            }
        }

        public void ExtractSectionToNewDocument(string jsonPath, string outputPath, string[] sectionPath)
        {
            try
            {
                Console.WriteLine($"\nStarting section extraction with detailed logging...");

                var targetSection = GetTargetSection(jsonPath, sectionPath);
                if (targetSection == null)
                {
                    Console.WriteLine("Target section not found!");
                    return;
                }

                using (var document = WordprocessingDocument.Create(outputPath, WordprocessingDocumentType.Document))
                {
                    Console.WriteLine("\nCreating document...");
                    CreateDocumentWithStyles(document);

                    var mainPart = document.MainDocumentPart!;
                    var body = mainPart.Document.Body!;

                    // Process the main section and all its subsections recursively
                    ProcessSectionHierarchy(body, targetSection, document);

                    Console.WriteLine("\nSaving document...");
                    document.Save();
                }

                Console.WriteLine($"Document created successfully at: {outputPath}");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"\nERROR in ExtractSectionToNewDocument: {ex.Message}");
                Console.WriteLine($"Stack trace: {ex.StackTrace}");
                throw;
            }
        }
        private void ProcessSectionHierarchy(Body body, Section section, WordprocessingDocument doc)
        {
            Console.WriteLine($"\nProcessing section: {section.Title}");

            // Add section title
            AppendSectionTitle(body, section);

            // Add section content
            if (!string.IsNullOrEmpty(section.Content))
            {
                AppendSectionContent(body, section);
            }

            // Add section tables
            if (section.Tables != null && section.Tables.Any())
            {
                Console.WriteLine($"Processing {section.Tables.Count} tables...");
                AppendSectionTables(body, section.Tables);
            }

            // Add section images
            if (section.Images != null && section.Images.Any())
            {
                Console.WriteLine($"Processing {section.Images.Count} images...");
                AppendSectionImages(body, section.Images, doc);
            }

            // Process all subsections recursively
            if (section.Subsections != null && section.Subsections.Any())
            {
                foreach (var subsection in section.Subsections)
                {
                    ProcessSectionHierarchy(body, subsection, doc);
                }
            }
        }
        public void ExtractSectionToNewDocument_bk(string jsonPath, string outputPath, string[] sectionPath)
        {
            try
            {
                Console.WriteLine($"\nStarting section extraction with detailed logging...");

                var targetSection = GetTargetSection(jsonPath, sectionPath);
                if (targetSection == null)
                {
                    Console.WriteLine("Target section not found!");
                    return;
                }

                Console.WriteLine($"\nTarget section details:");
                Console.WriteLine($"Title: {targetSection.Title}");
                Console.WriteLine($"Content length: {targetSection.Content?.Length ?? 0}");
                Console.WriteLine($"Number of tables: {targetSection.Tables?.Count ?? 0}");
                Console.WriteLine($"Number of images: {targetSection.Images?.Count ?? 0}");

                if (targetSection.Images != null)
                {
                    foreach (var img in targetSection.Images)
                    {
                        Console.WriteLine($"\nImage details:");
                        Console.WriteLine($"ID: {img.Id}");
                        Console.WriteLine($"Content Type: {img.ContentType}");
                        Console.WriteLine($"Dimensions: {img.Width}x{img.Height}");
                        Console.WriteLine($"Base64 data length: {img.Base64Data?.Length ?? 0}");
                    }
                }

                using (var document = WordprocessingDocument.Create(outputPath, WordprocessingDocumentType.Document))
                {
                    Console.WriteLine("\nCreating document...");
                    CreateDocumentWithStyles(document);

                    var mainPart = document.MainDocumentPart!;
                    var body = mainPart.Document.Body!;

                    Console.WriteLine("Adding section title...");
                    AppendSectionTitle(body, targetSection);

                    Console.WriteLine("Adding section content...");
                    AppendSectionContent(body, targetSection);

                    if (targetSection.Tables != null && targetSection.Tables.Any())
                    {
                        Console.WriteLine($"Processing {targetSection.Tables.Count} tables...");
                        AppendSectionTables(body, targetSection.Tables);
                    }

                    if (targetSection.Images != null && targetSection.Images.Any())
                    {
                        Console.WriteLine($"\nStarting image processing for {targetSection.Images.Count} images...");
                        AppendSectionImages(body, targetSection.Images, document);
                    }
                    else
                    {
                        Console.WriteLine("No images to process in this section.");
                    }

                    Console.WriteLine("\nSaving document...");
                    document.Save();
                }

                Console.WriteLine($"Document created successfully at: {outputPath}");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"\nERROR in ExtractSectionToNewDocument: {ex.Message}");
                Console.WriteLine($"Stack trace: {ex.StackTrace}");
                throw;
            }
        }

        private Section? GetTargetSection(string jsonPath, string[] sectionPath)
        {
            try
            {
                Console.WriteLine($"\nReading JSON from: {jsonPath}");
                var jsonContent = File.ReadAllText(jsonPath);
                Console.WriteLine($"JSON content length: {jsonContent.Length:N0} characters");

                var options = new JsonSerializerOptions
                {
                    PropertyNameCaseInsensitive = true,
                    WriteIndented = true
                };

                var sections = JsonSerializer.Deserialize<List<Section>>(jsonContent, options);

                if (sections == null)
                {
                    Console.WriteLine("Failed to parse JSON content - sections is null");
                    throw new InvalidOperationException("Failed to parse JSON content");
                }

                Console.WriteLine($"Successfully deserialized {sections.Count} top-level sections");

                var targetSection = FindSection(sections, sectionPath);
                if (targetSection != null)
                {
                    Console.WriteLine($"Found target section: {targetSection.Title}");
                    Console.WriteLine($"Number of subsections: {targetSection.Subsections?.Count ?? 0}");
                }

                return targetSection;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error reading section: {ex.Message}");
                Console.WriteLine($"Stack trace: {ex.StackTrace}");
                return null;
            }
        }
        private Section? GetTargetSection_v0219(string jsonPath, string[] sectionPath)
        {
            try
            {
                System.Console.WriteLine($"\nReading JSON from: {jsonPath}");
                var jsonContent = File.ReadAllText(jsonPath);
                System.Console.WriteLine($"JSON content length: {jsonContent.Length:N0} characters");

                var options = new JsonSerializerOptions
                {
                    PropertyNameCaseInsensitive = true,
                    WriteIndented = true
                };

                var sections = JsonSerializer.Deserialize<List<Section>>(jsonContent, options);

                if (sections == null)
                {
                    System.Console.WriteLine("Failed to parse JSON content - sections is null");
                    throw new InvalidOperationException("Failed to parse JSON content");
                }

                System.Console.WriteLine($"Successfully deserialized {sections.Count} top-level sections");

                return FindSection(sections, sectionPath);
            }
            catch (Exception ex)
            {
                System.Console.WriteLine($"Error reading section: {ex.Message}");
                System.Console.WriteLine($"Stack trace: {ex.StackTrace}");
                return null;
            }
        }
        private Section? GetTargetSection_old(string jsonPath, string[] sectionPath)
        {
            try
            {
                var jsonContent = File.ReadAllText(jsonPath);
                var sections = JsonSerializer.Deserialize<List<Section>>(jsonContent);

                if (sections == null)
                    throw new InvalidOperationException("Failed to parse JSON content");

                return FindSection(sections, sectionPath);
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error reading section: {ex.Message}");
                return null;
            }
        }

        private void AppendSectionTitle(Body body, Section section)
        {
            var titleParagraph = new Paragraph(
                new ParagraphProperties(
                    new ParagraphStyleId() { Val = $"Heading{section.Level}" },
                    new Justification() { Val = JustificationValues.Left },
                    new SpacingBetweenLines() { After = "0", Before = "240" }
                ),
                new Run(
                    new RunProperties(new Bold()),
                    new Text(section.Title)
                )
            );
            body.AppendChild(titleParagraph);
        }
        private void AppendSectionContent(Body body, Section section)
        {
            if (string.IsNullOrEmpty(section.Content))
                return;

            // Split content into lines and process each line
            var lines = section.Content.Split(new[] { "\n", "\r\n" }, StringSplitOptions.RemoveEmptyEntries);
            var processedLines = new List<string>();
            var seenContent = new HashSet<string>();

            foreach (var line in lines)
            {
                var trimmedLine = line.Trim();

                // Skip empty lines
                if (string.IsNullOrWhiteSpace(trimmedLine))
                    continue;

                // Skip figure captions - they will be handled by AppendSectionImages
                if (trimmedLine.StartsWith("Figure", StringComparison.OrdinalIgnoreCase) &&
                    trimmedLine.Contains("Configuration"))
                {
                    continue;
                }

                // Skip duplicate content
                if (seenContent.Contains(trimmedLine))
                    continue;

                seenContent.Add(trimmedLine);
                processedLines.Add(trimmedLine);
            }

            // Process the cleaned-up lines
            foreach (var line in processedLines)
            {
                var isBulletPoint = ShouldBeBulletPoint(line);

                var paragraph = new Paragraph(
                    new ParagraphProperties(
                        new ParagraphStyleId() { Val = "Normal" },
                        new SpacingBetweenLines()
                        {
                            After = "200",
                            Line = "276",
                            LineRule = LineSpacingRuleValues.Auto
                        }
                    )
                );

                if (isBulletPoint)
                {
                    paragraph.ParagraphProperties.AppendChild(
                        new NumberingProperties(
                            new NumberingLevelReference() { Val = 0 },
                            new NumberingId() { Val = 1 }
                        )
                    );
                }

                paragraph.AppendChild(new Run(
                    new RunProperties(new FontSize() { Val = "24" }),
                    new Text(line) { Space = SpaceProcessingModeValues.Preserve }
                ));

                body.AppendChild(paragraph);
            }
        }

        private void AppendSectionImages(Body body, List<ImageInfo> images, WordprocessingDocument doc)
        {
            if (images == null || !images.Any())
                return;

            var imageHandler = new ImageHandler();

            foreach (var image in images)
            {
                try
                {
                    // Insert the image
                    var drawing = imageHandler.InsertImage(image.Base64Data, image.ContentType, doc);
                    body.AppendChild(new Paragraph(
                        new ParagraphProperties(
                            new Justification() { Val = JustificationValues.Center },
                            new SpacingBetweenLines() { Before = "240", After = "120" }
                        ),
                        new Run(drawing)
                    ));

                    // Always add the cleaned figure caption below the image
                    var figureCaption = !string.IsNullOrEmpty(image.Title) ?
                        CleanupFieldCodes(image.Title) :
                        "Figure Caption";

                    // If title doesn't start with "Figure", prepend it
                    if (!figureCaption.StartsWith("Figure", StringComparison.OrdinalIgnoreCase))
                    {
                        // Try to extract figure number from content or generate one
                        var figureNumber = ExtractFigureNumber(image.Title) ?? "1";
                        figureCaption = $"Figure {figureNumber} Configuration";
                    }

                    body.AppendChild(new Paragraph(
                        new ParagraphProperties(
                            new ParagraphStyleId() { Val = "ImageCaption" },
                            new Justification() { Val = JustificationValues.Center },
                            new SpacingBetweenLines() { Before = "120", After = "240" }
                        ),
                        new Run(
                            new RunProperties(new FontSize() { Val = "22" }),
                            new Text(figureCaption)
                        )
                    ));
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"Error processing image: {ex.Message}");
                    body.AppendChild(new Paragraph(
                        new Run(new Text($"[Image processing failed: {image.Title}]"))
                    ));
                }
            }
        }

        private string? ExtractFigureNumber(string text)
        {
            if (string.IsNullOrEmpty(text))
                return null;

            var match = System.Text.RegularExpressions.Regex.Match(
                text,
                @"Figure\s+(\d+)",
                System.Text.RegularExpressions.RegexOptions.IgnoreCase
            );

            return match.Success ? match.Groups[1].Value : null;
        }
        private void AppendSectionContent_v0220b(Body body, Section section)
        {
            if (string.IsNullOrEmpty(section.Content))
            {
                Console.WriteLine("Section content is empty, returning...");
                return;
            }

            Console.WriteLine($"\nProcessing content for section: {section.Title}");
            Console.WriteLine("Raw content length: " + section.Content.Length);

            // Split content into lines and process each line
            var lines = section.Content.Split(new[] { "\n", "\r\n" }, StringSplitOptions.RemoveEmptyEntries);
            Console.WriteLine($"Number of raw lines: {lines.Length}");

            var processedLines = new List<string>();
            var seenContent = new HashSet<string>();
            var firstParagraphOfSection = true;
            var sectionFirstParagraph = string.Empty;

            Console.WriteLine("\nProcessing lines:");
            for (int i = 0; i < lines.Length; i++)
            {
                var line = lines[i];
                var trimmedLine = line.Trim();

                Console.WriteLine($"\nLine {i + 1}:");
                Console.WriteLine($"Original: {trimmedLine}");

                // Skip empty lines
                if (string.IsNullOrWhiteSpace(trimmedLine))
                {
                    Console.WriteLine("Skipping empty line");
                    continue;
                }

                // Handle figure captions
                if (trimmedLine.Contains("Figure") && trimmedLine.Contains("Configuration"))
                {
                    if (trimmedLine.Contains("SEQ"))
                    {
                        var cleanedLine = CleanupFieldCodes(trimmedLine);
                        Console.WriteLine($"Cleaned figure caption: {cleanedLine}");
                        processedLines.Add(cleanedLine);
                    }
                    else
                    {
                        Console.WriteLine($"Adding figure caption: {trimmedLine}");
                        processedLines.Add(trimmedLine);
                    }
                    continue;
                }

                // Store the first paragraph of the section
                if (firstParagraphOfSection && trimmedLine.Contains("configuration") &&
                    trimmedLine.Contains("downlinks"))
                {
                    Console.WriteLine("Found first paragraph of section");
                    sectionFirstParagraph = trimmedLine;
                    firstParagraphOfSection = false;
                }

                // Check for duplicates
                if (seenContent.Contains(trimmedLine))
                {
                    Console.WriteLine($"Duplicate content found: {trimmedLine}");
                    // If this is the section's first paragraph appearing at the end, skip it
                    if (trimmedLine == sectionFirstParagraph)
                    {
                        Console.WriteLine("Skipping duplicate first paragraph");
                        continue;
                    }
                }

                seenContent.Add(trimmedLine);
                processedLines.Add(trimmedLine);
                Console.WriteLine("Added to processed lines");
            }

            Console.WriteLine($"\nNumber of processed lines: {processedLines.Count}");

            // Add the processed lines to the document
            Console.WriteLine("\nAdding lines to document:");
            foreach (var line in processedLines)
            {
                Console.WriteLine($"\nProcessing line for document: {line.Substring(0, Math.Min(50, line.Length))}...");

                var isBulletPoint = ShouldBeBulletPoint(line);
                if (isBulletPoint)
                {
                    Console.WriteLine("Formatting as bullet point");
                }

                var paragraph = new Paragraph(
                    new ParagraphProperties(
                        new ParagraphStyleId() { Val = "Normal" },
                        new SpacingBetweenLines()
                        {
                            After = "200",
                            Line = "276",
                            LineRule = LineSpacingRuleValues.Auto
                        }
                    )
                );

                if (isBulletPoint)
                {
                    paragraph.ParagraphProperties.AppendChild(
                        new NumberingProperties(
                            new NumberingLevelReference() { Val = 0 },
                            new NumberingId() { Val = 1 }
                        )
                    );
                }

                paragraph.AppendChild(new Run(
                    new RunProperties(new FontSize() { Val = "24" }),
                    new Text(line) { Space = SpaceProcessingModeValues.Preserve }
                ));

                body.AppendChild(paragraph);
                Console.WriteLine("Added paragraph to document");
            }

            Console.WriteLine("Finished processing section content");
        }
        private void AppendSectionContent_v0220a(Body body, Section section)
        {
            if (string.IsNullOrEmpty(section.Content))
                return;

            // Split content into lines and process each line
            var lines = section.Content.Split(new[] { "\n", "\r\n" }, StringSplitOptions.RemoveEmptyEntries);
            var processedLines = new List<string>();

            // Track if we've seen this content before (to avoid duplicates)
            var seenContent = new HashSet<string>();

            foreach (var line in lines)
            {
                var trimmedLine = line.Trim();

                // Skip empty lines
                if (string.IsNullOrWhiteSpace(trimmedLine))
                    continue;

                // Skip duplicate content
                if (seenContent.Contains(trimmedLine))
                    continue;

                seenContent.Add(trimmedLine);

                // Clean up field codes
                if (trimmedLine.Contains("SEQ"))
                {
                    trimmedLine = CleanupFieldCodes(trimmedLine);
                }

                processedLines.Add(trimmedLine);
            }

            // Process the cleaned-up lines
            foreach (var line in processedLines)
            {
                // Determine if this line should be a bullet point
                var isBulletPoint = ShouldBeBulletPoint(line);

                var paragraph = new Paragraph(
                    new ParagraphProperties(
                        new ParagraphStyleId() { Val = "Normal" },
                        new SpacingBetweenLines()
                        {
                            After = "200",
                            Line = "276",
                            LineRule = LineSpacingRuleValues.Auto
                        }
                    )
                );

                if (isBulletPoint)
                {
                    // Add bullet formatting
                    paragraph.ParagraphProperties.AppendChild(
                        new NumberingProperties(
                            new NumberingLevelReference() { Val = 0 },
                            new NumberingId() { Val = 1 }
                        )
                    );
                }

                paragraph.AppendChild(new Run(
                    new RunProperties(new FontSize() { Val = "24" }),
                    new Text(line) { Space = SpaceProcessingModeValues.Preserve }
                ));

                body.AppendChild(paragraph);
            }
        }
       private bool ShouldBeBulletPoint(string line)
        {
            // Check if this line matches the pattern of configuration items
            var configPatterns = new[]
            {
        "Direct Line of Sight (LOS) configuration",
        "Reach configuration",
        "Beyond Line of Sight (BLOS) configuration"
    };

            return configPatterns.Any(pattern =>
                line.Trim().Equals(pattern, StringComparison.OrdinalIgnoreCase));
        }

        private string CleanupFieldCodes(string text)
        {
            // Remove SEQ field codes
            text = System.Text.RegularExpressions.Regex.Replace(
                text,
                @"SEQ\s+Figure\s+\\\*\s+ARABIC\s+",
                "",
                System.Text.RegularExpressions.RegexOptions.IgnoreCase
            );

            // Clean up any remaining field codes or special characters
            text = System.Text.RegularExpressions.Regex.Replace(
                text,
                @"\{[^}]+\}",
                ""
            );

            // Ensure proper spacing in figure captions
            text = System.Text.RegularExpressions.Regex.Replace(
                text,
                @"Figure\s+(\d+)\s*",
                "Figure $1 "
            );

            return text.Trim();
        }

        private void AppendSectionContent_v0219a(Body body, Section section)
        {
            if (!string.IsNullOrEmpty(section.Content))
            {
                var contentLines = section.Content.Split('\n', StringSplitOptions.RemoveEmptyEntries);
                foreach (var line in contentLines)
                {
                    if (!string.IsNullOrWhiteSpace(line))
                    {
                        var contentParagraph = new Paragraph(
                            new ParagraphProperties(
                                new ParagraphStyleId() { Val = "Normal" },
                                new SpacingBetweenLines() { After = "200", Line = "276", LineRule = LineSpacingRuleValues.Auto }
                            ),
                            new Run(
                                new RunProperties(new FontSize() { Val = "24" }),
                                new Text(line.Trim()) { Space = SpaceProcessingModeValues.Preserve }
                            )
                        );
                        body.AppendChild(contentParagraph);
                    }
                }
            }
        }

        private void AppendSectionTables(Body body, List<TableInfo> tables)
        {
            int tableNumber = 1; // Reset table numbering for this section

            foreach (var table in tables)
            {
                // Add spacing before table
                body.AppendChild(new Paragraph(
                    new ParagraphProperties(
                        new SpacingBetweenLines() { Before = "240", After = "240" }
                    )
                ));

                // Add table title with clean numbering
                AppendTableTitle(body, table, tableNumber++);

                // Create and append table
                var docTable = CreateTable(table);
                body.AppendChild(docTable);
            }
        }

        private void AppendTableTitle(Body body, TableInfo table, int tableNumber)
        {
            if (string.IsNullOrEmpty(table.Title)) return;

            // Clean up the title by removing specific patterns
            var cleanTitle = table.Number + " " + table.Title;
            cleanTitle = cleanTitle
                .Replace("SEQ Table * ARABIC", "")  // Remove the whole pattern
                .Replace("Table  SEQ ", "Table ")    // Remove first part
                .Replace(" * ARABIC", "");           // Remove second part

            var tableTitleParagraph = new Paragraph(
                new ParagraphProperties(
                    new ParagraphStyleId() { Val = "TableTitle" },
                    new Justification() { Val = JustificationValues.Left },
                    new SpacingBetweenLines() { After = "120" }
                ),
                new Run(
                    new RunProperties(new Bold()),
                    new Text(cleanTitle.Trim())
                )
            );

            body.AppendChild(tableTitleParagraph);
        }
        private string CleanupTableTitle(string title)
        {
            if (string.IsNullOrEmpty(title)) return string.Empty;

            // Remove field codes
            title = System.Text.RegularExpressions.Regex.Replace(title,
                @"SEQ\s+Table\s+\*\s+ARABIC",
                "",
                System.Text.RegularExpressions.RegexOptions.IgnoreCase);

            // Remove any table prefixes that might have been duplicated
            title = System.Text.RegularExpressions.Regex.Replace(title,
                @"^Table\s+\d+:?\s*",
                "",
                System.Text.RegularExpressions.RegexOptions.IgnoreCase);

            // Remove GUIDs
            title = System.Text.RegularExpressions.Regex.Replace(title,
                @"[0-9a-f]{8}[-]?([0-9a-f]{4}[-]?){3}[0-9a-f]{12}",
                "",
                System.Text.RegularExpressions.RegexOptions.IgnoreCase);

            // Clean up whitespace and special characters
            title = System.Text.RegularExpressions.Regex.Replace(title, @"\s+", " ");
            title = System.Text.RegularExpressions.Regex.Replace(title, @"[^\w\s\-]", "");

            return title.Trim();
        }

        private string CleanupTableNumber(string number)
        {
            // Extract just the number part
            var match = System.Text.RegularExpressions.Regex.Match(number, @"Table\s*(\d+)");
            if (match.Success)
            {
                return $"Table {match.Groups[1].Value}";
            }
            return number;
        }
        private void AppendTableTitle_old(Body body, TableInfo table)
        {
            if (!string.IsNullOrEmpty(table.Title) || !string.IsNullOrEmpty(table.Number))
            {
                var tableTitleParagraph = new Paragraph(
                    new ParagraphProperties(
                        new ParagraphStyleId() { Val = "TableTitle" },
                        new Justification() { Val = JustificationValues.Left },
                        new SpacingBetweenLines() { After = "120" }
                    ),
                    new Run(
                        new RunProperties(new Bold()),
                        new Text($"{table.Number}{(string.IsNullOrEmpty(table.Title) ? "" : ": " + table.Title)}")
                    )
                );
                body.AppendChild(tableTitleParagraph);
            }
        }

        private Table CreateTable(TableInfo tableInfo)
        {
            var table = new Table(
                new TableProperties(
                    new TableBorders(
                        new TopBorder() { Val = BorderValues.Single, Size = 1 },
                        new BottomBorder() { Val = BorderValues.Single, Size = 1 },
                        new LeftBorder() { Val = BorderValues.Single, Size = 1 },
                        new RightBorder() { Val = BorderValues.Single, Size = 1 },
                        new InsideHorizontalBorder() { Val = BorderValues.Single, Size = 1 },
                        new InsideVerticalBorder() { Val = BorderValues.Single, Size = 1 }
                    ),
                    new TableWidth() { Width = "5000", Type = TableWidthUnitValues.Pct }
                )
            );

            // Add table data
            bool isFirstRow = true;
            foreach (var rowData in tableInfo.Data)
            {
                var tr = new TableRow();
                foreach (var cellData in rowData)
                {
                    var tc = new TableCell(
                        new TableCellProperties(
                            new TableCellWidth() { Type = TableWidthUnitValues.Auto }
                        ),
                        new Paragraph(
                            new ParagraphProperties(
                                new ParagraphStyleId() { Val = "TableContents" }
                            ),
                            new Run(
                                new RunProperties(
                                    new FontSize() { Val = "22" },
                                    isFirstRow ? new Bold() : null
                                ),
                                new Text(cellData) { Space = SpaceProcessingModeValues.Preserve }
                            )
                        )
                    );
                    tr.AppendChild(tc);
                }
                table.AppendChild(tr);
                isFirstRow = false;
            }

            return table;
        }

        private void AppendSectionImages_v0220b(Body body, List<ImageInfo> images, WordprocessingDocument doc)
        {
            if (images == null || !images.Any())
                return;

            var imageHandler = new ImageHandler();

            foreach (var image in images)
            {
                try
                {
                    // Add spacing before image
                    body.AppendChild(new Paragraph(
                        new ParagraphProperties(
                            new SpacingBetweenLines() { Before = "240", After = "120" }
                        )
                    ));

                    // Insert the image
                    var drawing = imageHandler.InsertImage(image.Base64Data, image.ContentType, doc);
                    body.AppendChild(new Paragraph(
                        new ParagraphProperties(
                            new Justification() { Val = JustificationValues.Center },
                            new SpacingBetweenLines() { After = "120" }
                        ),
                        new Run(drawing)
                    ));

                    // Add the figure caption below the image
                    if (!string.IsNullOrEmpty(image.Title) &&
                        !image.Title.Contains("configuration (Figure") &&
                        !image.Title.Contains("downlinks data"))
                    {
                        var caption = CleanupFieldCodes(image.Title);
                        body.AppendChild(new Paragraph(
                            new ParagraphProperties(
                                new ParagraphStyleId() { Val = "ImageCaption" },
                                new Justification() { Val = JustificationValues.Center },
                                new SpacingBetweenLines() { Before = "120", After = "240" }
                            ),
                            new Run(
                                new RunProperties(
                                    new FontSize() { Val = "22" },  // Slightly smaller font for captions
                                    new Italic()  // Optional: makes captions italic
                                ),
                                new Text(caption)
                            )
                        ));
                    }
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"Error processing image: {ex.Message}");
                    body.AppendChild(new Paragraph(
                        new Run(new Text($"[Image processing failed: {image.Title}]"))
                    ));
                }
            }
        }
        private void AppendSectionImages_v0220a(Body body, List<ImageInfo> images, WordprocessingDocument doc)
        {
            if (images == null || !images.Any())
                return;

            var imageHandler = new ImageHandler();

            foreach (var image in images)
            {
                try
                {
                    // Add spacing before image
                    body.AppendChild(new Paragraph(
                        new ParagraphProperties(
                            new SpacingBetweenLines() { Before = "240", After = "240" }
                        )
                    ));

                    // Insert the image
                    var drawing = imageHandler.InsertImage(image.Base64Data, image.ContentType, doc);
                    body.AppendChild(new Paragraph(
                        new ParagraphProperties(
                            new Justification() { Val = JustificationValues.Center }
                        ),
                        new Run(drawing)
                    ));

                    // Skip adding the image title if it contains paragraph content
                    // (Look for telltale signs of it being a full paragraph vs. a caption)
                    if (!string.IsNullOrEmpty(image.Title) &&
                        !image.Title.Contains("configuration (Figure") &&
                        !image.Title.Contains("downlinks data"))
                    {
                        var caption = CleanupFieldCodes(image.Title);
                        body.AppendChild(new Paragraph(
                            new ParagraphProperties(
                                new ParagraphStyleId() { Val = "ImageCaption" },
                                new Justification() { Val = JustificationValues.Center },
                                new SpacingBetweenLines() { Before = "120", After = "240" }
                            ),
                            new Run(new Text(caption))
                        ));
                    }
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"Error processing image: {ex.Message}");
                    body.AppendChild(new Paragraph(
                        new Run(new Text($"[Image processing failed: {image.Title}]"))
                    ));
                }
            }
        }
        private void AppendSectionImages_v0220(Body body, List<ImageInfo> images, WordprocessingDocument doc)
        {
            if (images == null || !images.Any())
                return;

            var imageHandler = new ImageHandler();

            foreach (var image in images)
            {
                try
                {
                    // Add spacing before image
                    body.AppendChild(new Paragraph(
                        new ParagraphProperties(
                            new SpacingBetweenLines() { Before = "240", After = "240" }
                        )
                    ));

                    // Insert the image
                    var drawing = imageHandler.InsertImage(image.Base64Data, image.ContentType, doc);
                    body.AppendChild(new Paragraph(
                        new ParagraphProperties(
                            new Justification() { Val = JustificationValues.Center }
                        ),
                        new Run(drawing)
                    ));

                    // Add image caption if it exists and clean up any field codes
                    if (!string.IsNullOrEmpty(image.Title))
                    {
                        var caption = CleanupFieldCodes(image.Title);
                        body.AppendChild(new Paragraph(
                            new ParagraphProperties(
                                new ParagraphStyleId() { Val = "ImageCaption" },
                                new Justification() { Val = JustificationValues.Center },
                                new SpacingBetweenLines() { Before = "120", After = "240" }
                            ),
                            new Run(new Text(caption))
                        ));
                    }
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"Error processing image: {ex.Message}");
                    body.AppendChild(new Paragraph(
                        new Run(new Text($"[Image processing failed: {image.Title}]"))
                    ));
                }
            }
        }
        private void AppendSectionImages_v0219a(Body body, List<ImageInfo> images, WordprocessingDocument doc)
        {
            Console.WriteLine($"\nEntering AppendSectionImages with {images.Count} images");
            var imageHandler = new ImageHandler();

            foreach (var image in images)
            {
                try
                {
                    Console.WriteLine($"\nProcessing image {image.Id}:");
                    Console.WriteLine($"Content Type: {image.ContentType}");
                    Console.WriteLine($"Dimensions: {image.Width}x{image.Height}");

                    // Verify base64 data
                    if (string.IsNullOrEmpty(image.Base64Data))
                    {
                        Console.WriteLine("WARNING: Base64 data is empty!");
                        continue;
                    }

                    // Add spacing before image
                    Console.WriteLine("Adding spacing before image...");
                    body.AppendChild(new Paragraph(
                        new ParagraphProperties(
                            new SpacingBetweenLines() { Before = "240", After = "240" }
                        )
                    ));

                    // Add image title
                    if (!string.IsNullOrEmpty(image.Title))
                    {
                        Console.WriteLine($"Adding image title: {image.Title}");
                        body.AppendChild(new Paragraph(
                            new ParagraphProperties(
                                new ParagraphStyleId() { Val = "ImageTitle" },
                                new Justification() { Val = JustificationValues.Center }
                            ),
                            new Run(new Text(image.Title))
                        ));
                    }

                    // Insert image
                    Console.WriteLine("Calling InsertImage...");
                    var drawing = imageHandler.InsertImage(image.Base64Data, image.ContentType, doc);

                    Console.WriteLine("Creating paragraph for image...");
                    var imageParagraph = new Paragraph(
                        new ParagraphProperties(
                            new Justification() { Val = JustificationValues.Center }
                        ),
                        new Run(drawing)
                    );

                    Console.WriteLine("Appending image paragraph to document...");
                    body.AppendChild(imageParagraph);

                    // Add description
                    if (!string.IsNullOrEmpty(image.Description))
                    {
                        Console.WriteLine($"Adding image description: {image.Description}");
                        body.AppendChild(new Paragraph(
                            new ParagraphProperties(
                                new ParagraphStyleId() { Val = "ImageCaption" },
                                new Justification() { Val = JustificationValues.Center }
                            ),
                            new Run(new Text(image.Description))
                        ));
                    }

                    Console.WriteLine("Image processing completed successfully");
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"ERROR processing image {image.Id}: {ex.Message}");
                    Console.WriteLine($"Stack trace: {ex.StackTrace}");

                    // Add placeholder text
                    body.AppendChild(new Paragraph(
                        new Run(new Text($"[Image insertion failed: {image.Title}]"))
                    ));
                }
            }
        }
        private void AppendSectionImages_old(Body body, List<ImageInfo> images, WordprocessingDocument doc)
        {
            if (images == null || !images.Any()) return;

            var imageHandler = new ImageHandler();

            foreach (var image in images)
            {
                // Add spacing before image
                body.AppendChild(new Paragraph(
                    new ParagraphProperties(
                        new SpacingBetweenLines() { Before = "240", After = "240" }
                    )
                ));

                // Add image title if exists
                if (!string.IsNullOrEmpty(image.Title))
                {
                    body.AppendChild(new Paragraph(
                        new ParagraphProperties(
                            new ParagraphStyleId() { Val = "ImageTitle" },
                            new Justification() { Val = JustificationValues.Center }
                        ),
                        new Run(new Text(image.Title))
                    ));
                }

                try
                {
                    // Insert the image
                    var drawing = imageHandler.InsertImage(image.Base64Data, image.ContentType, doc);
                    body.AppendChild(new Paragraph(drawing));

                    // Add image description/caption if exists
                    if (!string.IsNullOrEmpty(image.Description))
                    {
                        body.AppendChild(new Paragraph(
                            new ParagraphProperties(
                                new ParagraphStyleId() { Val = "ImageCaption" },
                                new Justification() { Val = JustificationValues.Center }
                            ),
                            new Run(new Text(image.Description))
                        ));
                    }
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"Error inserting image: {ex.Message}");
                    // Add placeholder text indicating image insertion failed
                    body.AppendChild(new Paragraph(
                        new Run(new Text($"[Image insertion failed: {image.Title}]"))
                    ));
                }
            }
        }
        private void CreateDocumentWithStyles_notGeneratingImage(WordprocessingDocument document)
        {
            var mainPart = document.AddMainDocumentPart();
            mainPart.Document = new Document(new Body());

            // Set page margins
            var sectionProps = new SectionProperties(
                new PageMargin()
                {
                    Top = 1440, // 1 inch
                    Right = 1440,
                    Bottom = 1440,
                    Left = 1440,
                    Header = 720,
                    Footer = 720
                }
            );
            mainPart.Document.Body.Append(sectionProps);

            var stylesPart = mainPart.AddNewPart<StyleDefinitionsPart>();
            var styles = new Styles();

            // Add styles
            styles.AppendChild(CreateParagraphStyle("Normal", "Normal", "Times New Roman", 24));

            for (int i = 1; i <= 4; i++)
            {
                styles.AppendChild(CreateParagraphStyle($"Heading{i}", $"Heading {i}", "Arial", 24 + (4 - i) * 4, true));
            }

            styles.AppendChild(CreateParagraphStyle("TableContents", "Table Contents", "Times New Roman", 22));
            styles.AppendChild(CreateParagraphStyle("TableTitle", "Table Title", "Times New Roman", 22, true));
            styles.AppendChild(CreateParagraphStyle("ImageTitle", "Image Title", "Times New Roman", 22, true));
            styles.AppendChild(CreateParagraphStyle("ImageCaption", "Image Caption", "Times New Roman", 20));

            stylesPart.Styles = styles;
        }
        private void CreateDocumentWithStyles(WordprocessingDocument document)
        {
            var mainPart = document.AddMainDocumentPart();
            mainPart.Document = new Document(new Body());

            var stylesPart = mainPart.AddNewPart<StyleDefinitionsPart>();
            var styles = new Styles();

            // Add styles
            styles.AppendChild(CreateParagraphStyle("Normal", "Normal", "Times New Roman", 24));

            for (int i = 1; i <= 4; i++)
            {
                styles.AppendChild(CreateParagraphStyle($"Heading{i}", $"Heading {i}", "Arial", 24 + (4 - i) * 4, true));
            }

            styles.AppendChild(CreateParagraphStyle("TableContents", "Table Contents", "Times New Roman", 22));
            styles.AppendChild(CreateParagraphStyle("TableTitle", "Table Title", "Times New Roman", 22, true));

            styles.AppendChild(CreateParagraphStyle("ImageTitle", "Image Title", "Times New Roman", 22, true));
            styles.AppendChild(CreateParagraphStyle("ImageCaption", "Image Caption", "Times New Roman", 20, false));

            stylesPart.Styles = styles;
        }

        private Style CreateParagraphStyle(string styleId, string styleName, string fontName, int fontSize, bool isBold = false)
        {
            return new Style(
                new StyleName() { Val = styleName },
                new BasedOn() { Val = "Normal" },
                new NextParagraphStyle() { Val = "Normal" },
                new StyleRunProperties(
                    new RunFonts() { Ascii = fontName },
                    new FontSize() { Val = fontSize.ToString() },
                    isBold ? new Bold() : null
                )
            )
            {
                Type = StyleValues.Paragraph,
                StyleId = styleId,
                Default = true
            };
        }

        private Section? FindSection(List<Section> sections, string[] path)
        {
            Console.WriteLine("\nStarting section search:");
            Console.WriteLine($"Path to find: {string.Join(" > ", path)}");

            var currentSections = sections;
            Section? currentSection = null;

            foreach (var sectionName in path)
            {
                Console.WriteLine($"\nSearching for: '{sectionName}'");
                Console.WriteLine("Available sections at this level:");
                foreach (var section in currentSections)
                {
                    Console.WriteLine($"- '{section.Title}'");
                    if (section.Subsections != null && section.Subsections.Any())
                    {
                        Console.WriteLine($"  Subsections:");
                        foreach (var sub in section.Subsections)
                        {
                            Console.WriteLine($"  - '{sub.Title}'");
                        }
                    }
                }

                currentSection = currentSections
                    .FirstOrDefault(s => s.Title.Trim().Equals(sectionName.Trim(), StringComparison.OrdinalIgnoreCase));

                if (currentSection == null)
                {
                    Console.WriteLine($"Could not find section: '{sectionName}'");
                    return null;
                }

                Console.WriteLine($"Found section: '{currentSection.Title}'");
                Console.WriteLine($"Level: {currentSection.Level}");
                Console.WriteLine($"Content length: {currentSection.Content?.Length ?? 0} characters");
                Console.WriteLine($"Tables: {currentSection.Tables?.Count ?? 0}");
                Console.WriteLine($"Images: {currentSection.Images?.Count ?? 0}");
                Console.WriteLine($"Subsections: {currentSection.Subsections?.Count ?? 0}");

                if (currentSection.Images?.Any() == true)
                {
                    Console.WriteLine("\nImage details:");
                    foreach (var img in currentSection.Images)
                    {
                        Console.WriteLine($"- Image ID: {img.Id}");
                        Console.WriteLine($"  Type: {img.ContentType}");
                        Console.WriteLine($"  Size: {img.Width}x{img.Height}");
                        Console.WriteLine($"  Base64 length: {img.Base64Data?.Length ?? 0}");
                        if (string.IsNullOrEmpty(img.Base64Data))
                        {
                            Console.WriteLine("  WARNING: No Base64 data!");
                        }
                    }
                }

                currentSections = currentSection.Subsections;
            }

            return currentSection;
        }
        public void ExtractSectionsToNewDocument(string jsonPath, string outputPath, string[][] sectionPaths)
        {
            // Read JSON
            var jsonContent = File.ReadAllText(jsonPath);
            var sections = JsonSerializer.Deserialize<List<Section>>(jsonContent);

            if (sections == null)
                throw new InvalidOperationException("Failed to parse JSON content");

            // Create new document
            using (var document = WordprocessingDocument.Create(outputPath, WordprocessingDocumentType.Document))
            {
                CreateDocumentWithStyles(document);
                var mainPart = document.MainDocumentPart!;
                var body = mainPart.Document.Body!;

                // Process each section path
                foreach (var sectionPath in sectionPaths)
                {
                    var targetSection = FindSection(sections, sectionPath);
                    if (targetSection == null)
                    {
                        Console.WriteLine($"Warning: Section not found: {string.Join(" > ", sectionPath)}");
                        continue;
                    }

                    Console.WriteLine($"Processing section: {targetSection.Title}");

                    // Add section title
                    AppendSectionTitle(body, targetSection);

                    // Add section content
                    AppendSectionContent(body, targetSection);

                    // Add section tables
                    if (targetSection.Tables != null && targetSection.Tables.Any())
                    {
                        Console.WriteLine($"Adding {targetSection.Tables.Count} tables from {targetSection.Title}");
                        AppendSectionTables(body, targetSection.Tables);
                    }

                    // Add page break between sections
                    if (sectionPath != sectionPaths.Last())
                    {
                        body.AppendChild(new Paragraph(
                            new Run(
                                new Break() { Type = BreakValues.Page }
                            )
                        ));
                    }
                }
            }
        }
    }
}