namespace DocumentProcessor.Tests;

[TestClass]
public class IntegrationTests : TestBase
{
    [TestMethod]
    public void FullWorkflow_ParseAndExtract_Success()
    {
        // Arrange
        var inputDoc = Path.Combine(_testDataPath, "TestDocument.docx");
        var jsonPath = Path.Combine(_outputPath, "document_structure.json");
        var finalDoc = Path.Combine(_outputPath, "final_section.docx");

        string[] sectionPath = new[] {
            "GENERAL INFORMATION",
            "System Description",
            "Configurations",
            "Direct LOS Configuration"
        };

        // Act - Parse document
        using (var parser = new WordDocumentParser(_outputPath))
        {
            parser.ProcessDocument(inputDoc);
        }

        // Verify JSON was created
        Assert.IsTrue(File.Exists(jsonPath), "JSON file should be created");

        // Act - Extract section
        var extractor = new SectionExtractor();
        extractor.ExtractSectionToNewDocument(jsonPath, finalDoc, sectionPath);

        // Assert
        Assert.IsTrue(File.Exists(finalDoc), "Final document should be created");
        var fileInfo = new FileInfo(finalDoc);
        Assert.IsTrue(fileInfo.Length > 0, "Final document should not be empty");
    }

    [TestMethod]
    public void FullWorkflow_MultipleExtraction_Success()
    {
        // Arrange
        var inputDoc = Path.Combine(_testDataPath, "TestDocument.docx");
        var jsonPath = Path.Combine(_outputPath, "document_structure.json");
        var finalDoc = Path.Combine(_outputPath, "multiple_sections.docx");

        var sectionsToExtract = new[]
        {
            new[] { "GENERAL INFORMATION", "Overview", "Abbreviations and Acronyms" },
            new[] { "GENERAL INFORMATION", "Overview", "Referenced Documents" }
        };

        // Act - Parse document
        using (var parser = new WordDocumentParser(_outputPath))
        {
            parser.ProcessDocument(inputDoc);
        }

        // Act - Extract sections
        var extractor = new SectionExtractor();
        extractor.ExtractSectionsToNewDocument(jsonPath, finalDoc, sectionsToExtract);

        // Assert
        Assert.IsTrue(File.Exists(finalDoc), "Final document should be created");
        var fileInfo = new FileInfo(finalDoc);
        Assert.IsTrue(fileInfo.Length > 0, "Final document should not be empty");
    }
}