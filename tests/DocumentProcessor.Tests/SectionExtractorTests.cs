namespace DocumentProcessor.Tests;

[TestClass]
public class SectionExtractorTests : TestBase
{
    [TestMethod]
    public void ExtractSectionToNewDocument_ValidPath_CreatesDocument()
    {
        // Arrange
        var extractor = new SectionExtractor();
        var outputDoc = Path.Combine(_outputPath, "section.docx");
        string[] sectionPath = new[] {
            "GENERAL INFORMATION",
            "System Description",
            "Configurations",
            "Direct LOS Configuration"
        };

        // Act
        extractor.ExtractSectionToNewDocument(
            Path.Combine(_testDataPath, "document_structure.json"),
            outputDoc,
            sectionPath);

        // Assert
        Assert.IsTrue(File.Exists(outputDoc), "Output document should be created");
        var fileInfo = new FileInfo(outputDoc);
        Assert.IsTrue(fileInfo.Length > 0, "Output document should not be empty");
    }

    [TestMethod]
    public void ExtractSectionsToNewDocument_MultipleSections_CreatesDocument()
    {
        // Arrange
        var extractor = new SectionExtractor();
        var outputDoc = Path.Combine(_outputPath, "multiple_sections.docx");
        var sectionsToExtract = new[]
        {
            new[] { "GENERAL INFORMATION", "Overview", "Abbreviations and Acronyms" },
            new[] { "GENERAL INFORMATION", "Overview", "Referenced Documents" }
        };

        // Act
        extractor.ExtractSectionsToNewDocument(
            Path.Combine(_testDataPath, "document_structure.json"),
            outputDoc,
            sectionsToExtract);

        // Assert
        Assert.IsTrue(File.Exists(outputDoc), "Output document should be created");
        var fileInfo = new FileInfo(outputDoc);
        Assert.IsTrue(fileInfo.Length > 0, "Output document should not be empty");
    }

    [TestMethod]
    public void ExtractSectionToNewDocument_InvalidPath_HandlesError()
    {
        // Arrange
        var extractor = new SectionExtractor();
        var outputDoc = Path.Combine(_outputPath, "error_section.docx");
        string[] invalidPath = new[] { "NonexistentSection" };

        // Act
        extractor.ExtractSectionToNewDocument(
            Path.Combine(_testDataPath, "document_structure.json"),
            outputDoc,
            invalidPath);

        // Assert
        Assert.IsFalse(File.Exists(outputDoc), "No document should be created for invalid path");
    }

    [TestMethod]
    public void ExtractSectionToNewDocument_WithImages_PreservesImages()
    {
        // Arrange
        var extractor = new SectionExtractor();
        var outputDoc = Path.Combine(_outputPath, "section_with_images.docx");
        string[] sectionPath = new[] {
            "GENERAL INFORMATION",
            "System Description",
            "Configurations",
            "Direct LOS Configuration"
        };

        // Act
        extractor.ExtractSectionToNewDocument(
            Path.Combine(_testDataPath, "document_structure.json"),
            outputDoc,
            sectionPath);

        // Assert
        Assert.IsTrue(File.Exists(outputDoc), "Output document should be created");
        var fileInfo = new FileInfo(outputDoc);
        Assert.IsTrue(fileInfo.Length > 100000, "Output document should be large enough to contain images");
    }
}