namespace DocumentProcessor.Tests;

[TestClass]
public class WordDocumentParserTests : TestBase
{
    [TestMethod]
    public void ProcessDocument_ValidDocument_CreatesJsonOutput()
    {
        // Arrange
        var parser = new WordDocumentParser(_outputPath);
        var inputDoc = Path.Combine(_testDataPath, "TestDocument.docx");
        var expectedJsonPath = Path.Combine(_outputPath, "document_structure.json");

        // Act
        parser.ProcessDocument(inputDoc);

        // Wait briefly for file operations to complete
        System.Threading.Thread.Sleep(100);

        //// Assert
        //Assert.IsTrue(File.Exists(expectedJsonPath), "JSON output file should be created");
        //var jsonContent = File.ReadAllText(expectedJsonPath);
        //var sections = JsonSerializer.Deserialize<List<Section>>(jsonContent);
        //Assert.IsNotNull(sections, "Sections should be deserialized from JSON");
        //Assert.IsTrue(sections.Count > 0, "At least one section should be present");


        // Assert
        Assert.IsTrue(File.Exists(expectedJsonPath), "JSON output file should be created");
        using (var jsonStream = new FileStream(expectedJsonPath, FileMode.Open, FileAccess.Read, FileShare.Read))
        using (var reader = new StreamReader(jsonStream))
        {
            var jsonContent = reader.ReadToEnd();
            var sections = JsonSerializer.Deserialize<List<Section>>(jsonContent);
            Assert.IsNotNull(sections, "Sections should be deserialized from JSON");
            Assert.IsTrue(sections.Count > 0, "At least one section should be present");
        }
    }

    [TestMethod]
    public void ProcessDocument_ValidDocument_ExtractsImages()
    {
        // Arrange
        var parser = new WordDocumentParser(_outputPath);
        var inputDoc = Path.Combine(_testDataPath, "TestDocument.docx");

        // Act
        parser.ProcessDocument(inputDoc);
        var jsonContent = File.ReadAllText(Path.Combine(_outputPath, "document_structure.json"));
        var sections = JsonSerializer.Deserialize<List<Section>>(jsonContent);

        // Assert
        Assert.IsNotNull(sections);
        var imagesFound = sections
            .SelectMany(s => GetAllSectionsRecursive(s))
            .SelectMany(s => s.Images ?? new List<ImageInfo>())
            .Any();
        Assert.IsTrue(imagesFound, "Document should contain at least one image");
    }

    [TestMethod]
    [ExpectedException(typeof(FileNotFoundException))]
    public void ProcessDocument_NonexistentFile_ThrowsException()
    {
        // Arrange
        var parser = new WordDocumentParser(_outputPath);
        var nonexistentFile = Path.Combine(_testDataPath, "NonexistentFile.docx");

        // Act
        parser.ProcessDocument(nonexistentFile);
    }

    private IEnumerable<Section> GetAllSectionsRecursive(Section section)
    {
        yield return section;
        if (section.Subsections != null)
        {
            foreach (var subsection in section.Subsections)
            {
                foreach (var s in GetAllSectionsRecursive(subsection))
                {
                    yield return s;
                }
            }
        }
    }
}