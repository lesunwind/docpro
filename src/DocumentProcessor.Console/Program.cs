using DocumentProcessor.Core.Services;
using DocumentProcessor.Core.Models;
using System;

namespace DocumentProcessor.Console;

public class Program
{
    public static void Main(string[] args)
    {
        //TestParsing();
        TestExtraction_Configuration();
       TestExtraction_OperatingSites();
        //TestExtraction_DirectLOSConfiguration();
        //TestExtraction1();
        //// TestExtraction2();
        // TestExtraction3();
        //TestParsing();
    }

    public static void TestExtraction_Configuration()
    {
        System.Console.WriteLine("\nTesting section extraction...");

        var currentDirectory = Directory.GetCurrentDirectory();
        var inputPath = Path.Combine(currentDirectory, "input");
        var outputPath = Path.Combine(currentDirectory, "output");

        var jsonPath = Path.Combine(inputPath, "document_structure.json");
        var outputDocPath = Path.Combine(outputPath, "Configuration.docx");

        System.Console.WriteLine($"JSON Path: {jsonPath}");
        System.Console.WriteLine($"Output Path: {outputDocPath}");

        if (!File.Exists(jsonPath))
        {
            System.Console.WriteLine($"ERROR: JSON file not found at {jsonPath}");
            return;
        }

        try
        {

           
            var extractor = new SectionExtractor();

            // Updated section path to include parent sections
            string[] sectionPath = new[] { "GENERAL INFORMATION", "System Description", "Configurations" };

            System.Console.WriteLine($"\nAttempting to extract section with path: {string.Join(" > ", sectionPath)}");
            extractor.ExtractSectionToNewDocument(jsonPath, outputDocPath, sectionPath);

            if (File.Exists(outputDocPath))
            {
                System.Console.WriteLine("Processing completed successfully!");
                System.Console.WriteLine($"Document created at: {outputDocPath}");
                var fileInfo = new FileInfo(outputDocPath);
                System.Console.WriteLine($"File size: {fileInfo.Length:N0} bytes");
            }
            else
            {
                System.Console.WriteLine("ERROR: Output document was not created!");
            }
        }
        catch (Exception ex)
        {
            System.Console.WriteLine($"Error during extraction: {ex.Message}");
            System.Console.WriteLine($"Stack trace: {ex.StackTrace}");
        }
    }
    public static void TestExtraction_DirectLOSConfiguration()
    {
        System.Console.WriteLine("\nTesting section extraction...");

        var currentDirectory = Directory.GetCurrentDirectory();
        var inputPath = Path.Combine(currentDirectory, "input");
        var outputPath = Path.Combine(currentDirectory, "output");

        var jsonPath = Path.Combine(inputPath, "document_structure.json");
        var outputDocPath = Path.Combine(outputPath, "DirectLOSConfiguration.docx");

        System.Console.WriteLine($"JSON Path: {jsonPath}");
        System.Console.WriteLine($"Output Path: {outputDocPath}");

        if (!File.Exists(jsonPath))
        {
            System.Console.WriteLine($"ERROR: JSON file not found at {jsonPath}");
            return;
        }

        try
        {


            var extractor = new SectionExtractor();

            // Updated section path to include parent sections
            string[] sectionPath = new[] { "GENERAL INFORMATION", "System Description", "Configurations", "Direct LOS Configuration" };

            System.Console.WriteLine($"\nAttempting to extract section with path: {string.Join(" > ", sectionPath)}");
            extractor.ExtractSectionToNewDocument(jsonPath, outputDocPath, sectionPath);

            if (File.Exists(outputDocPath))
            {
                System.Console.WriteLine("Processing completed successfully!");
                System.Console.WriteLine($"Document created at: {outputDocPath}");
                var fileInfo = new FileInfo(outputDocPath);
                System.Console.WriteLine($"File size: {fileInfo.Length:N0} bytes");
            }
            else
            {
                System.Console.WriteLine("ERROR: Output document was not created!");
            }
        }
        catch (Exception ex)
        {
            System.Console.WriteLine($"Error during extraction: {ex.Message}");
            System.Console.WriteLine($"Stack trace: {ex.StackTrace}");
        }
    }

    public static void TestExtraction_OperatingSites()
    {
        System.Console.WriteLine("\nTesting section extraction...");

        var currentDirectory = Directory.GetCurrentDirectory();
        var inputPath = Path.Combine(currentDirectory, "input");
        var outputPath = Path.Combine(currentDirectory, "output");

        var jsonPath = Path.Combine(inputPath, "document_structure.json");
        var outputDocPath = Path.Combine(outputPath, "OperatingSites.docx");

        System.Console.WriteLine($"JSON Path: {jsonPath}");
        System.Console.WriteLine($"Output Path: {outputDocPath}");

        if (!File.Exists(jsonPath))
        {
            System.Console.WriteLine($"ERROR: JSON file not found at {jsonPath}");
            return;
        }

        try
        {


            var extractor = new SectionExtractor();

            // Updated section path to include parent sections
            string[] sectionPath = new[] { "GENERAL INFORMATION", "System Description", "Operating Sites" };

            System.Console.WriteLine($"\nAttempting to extract section with path: {string.Join(" > ", sectionPath)}");
            extractor.ExtractSectionToNewDocument(jsonPath, outputDocPath, sectionPath);

            if (File.Exists(outputDocPath))
            {
                System.Console.WriteLine("Processing completed successfully!");
                System.Console.WriteLine($"Document created at: {outputDocPath}");
                var fileInfo = new FileInfo(outputDocPath);
                System.Console.WriteLine($"File size: {fileInfo.Length:N0} bytes");
            }
            else
            {
                System.Console.WriteLine("ERROR: Output document was not created!");
            }
        }
        catch (Exception ex)
        {
            System.Console.WriteLine($"Error during extraction: {ex.Message}");
            System.Console.WriteLine($"Stack trace: {ex.StackTrace}");
        }
    }

    private static void TestExtraction2()
    {
        var extractor = new SectionExtractor();
        string baseDirectory = AppDomain.CurrentDomain.BaseDirectory;
        string projectDirectory = Path.GetFullPath(Path.Combine(baseDirectory, "..", "..", ".."));

        string jsonPath = Path.Combine(projectDirectory, "input", "document_structure.json");
        string outputPath = Path.Combine(projectDirectory, "output", "extracted_sections.docx");

        var sectionsToExtract = new[]
        {
            new[] { "GENERAL INFORMATION", "Overview", "Abbreviations and Acronyms" },
            new[] { "GENERAL INFORMATION", "Overview", "Referenced Documents" }
        };

        try
        {
            System.Console.WriteLine($"Reading JSON from: {jsonPath}");
            System.Console.WriteLine($"Writing output to: {outputPath}");
            System.Console.WriteLine("Extracting sections:");
            foreach (var sectionPath in sectionsToExtract)
            {
                System.Console.WriteLine($"- {string.Join(" > ", sectionPath)}");
            }

            Directory.CreateDirectory(Path.GetDirectoryName(outputPath)!);

            extractor.ExtractSectionsToNewDocument(jsonPath, outputPath, sectionsToExtract);
            System.Console.WriteLine($"Sections extracted successfully to {outputPath}");
        }
        catch (Exception ex)
        {
            System.Console.WriteLine($"Error extracting sections: {ex.Message}");
            System.Console.WriteLine(ex.StackTrace);
        }
    }

    private static void TestExtraction3()
    {
        var extractor = new SectionExtractor();
        string baseDirectory = AppDomain.CurrentDomain.BaseDirectory;
        string projectDirectory = Path.GetFullPath(Path.Combine(baseDirectory, "..", "..", ".."));

        string jsonPath = Path.Combine(projectDirectory, "input", "document_structure.json");
        string outputPath = Path.Combine(projectDirectory, "output", "scope_section.docx");
        string[] sectionPath = new[] { "GENERAL INFORMATION", "Overview", "Scope" };

        try
        {
            System.Console.WriteLine($"Reading JSON from: {jsonPath}");
            System.Console.WriteLine($"Writing output to: {outputPath}");

            Directory.CreateDirectory(Path.GetDirectoryName(outputPath)!);

            extractor.ExtractSectionToNewDocument(jsonPath, outputPath, sectionPath);
            System.Console.WriteLine($"Section extracted successfully to {outputPath}");
        }
        catch (Exception ex)
        {
            System.Console.WriteLine($"Error extracting section: {ex.Message}");
            System.Console.WriteLine(ex.StackTrace);
        }
    }

    public static void TestParsing()
    {
        System.Console.WriteLine("\nTesting document parsing...");

        // Get the current directory
        var currentDirectory = Directory.GetCurrentDirectory();
        System.Console.WriteLine($"Current Directory: {currentDirectory}");

        // Define paths
        var inputPath = Path.Combine(currentDirectory, "input");
        var outputPath = Path.Combine(currentDirectory, "output");
        var docPath = Path.Combine(inputPath, "TestDocument.docx");

        // Verify directories and file
        System.Console.WriteLine($"\nVerifying paths:");
        System.Console.WriteLine($"Input directory: {inputPath}");
        System.Console.WriteLine($"Output directory: {outputPath}");
        System.Console.WriteLine($"Document path: {docPath}");

        // Create directories if they don't exist
        Directory.CreateDirectory(inputPath);
        Directory.CreateDirectory(outputPath);

        // Check if document exists
        if (!File.Exists(docPath))
        {
            System.Console.WriteLine($"\nERROR: Document not found at {docPath}");
            System.Console.WriteLine("Please ensure TestDocument.docx is placed in the input directory.");
            return;
        }

        try
        {
            System.Console.WriteLine("\nStarting document processing...");
            using (var parser = new WordDocumentParser(outputPath))
            {
                parser.ProcessDocument(docPath);
                System.Console.WriteLine("Document parsing completed.");
            }

            // Handle JSON file
            var jsonOutputPath = Path.Combine(outputPath, "document_structure.json");
            var jsonInputPath = Path.Combine(inputPath, "document_structure.json");

            if (File.Exists(jsonOutputPath))
            {
                File.Copy(jsonOutputPath, jsonInputPath, true);
                System.Console.WriteLine($"JSON file copied to: {jsonInputPath}");
            }
            else
            {
                System.Console.WriteLine("Warning: JSON file was not generated in output folder");
            }

            System.Console.WriteLine("\nProcessing completed successfully.");
        }
        catch (Exception ex)
        {
            System.Console.WriteLine($"\nError during parsing: {ex.Message}");
            System.Console.WriteLine($"Stack trace: {ex.StackTrace}");
        }
    }
    private static void TestParsing_old()
    {
        string projectDirectory = Path.GetFullPath(Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "..", "..", ".."));
        string inputDirectory = Path.GetFullPath(Path.Combine(projectDirectory, "input"));
        string outputDirectory = Path.GetFullPath(Path.Combine(projectDirectory, "output"));

        Directory.CreateDirectory(inputDirectory);
        Directory.CreateDirectory(outputDirectory);

        string inputFile = Path.Combine(inputDirectory, "TestDocument.docx");
        if (!File.Exists(inputFile))
        {
            System.Console.WriteLine($"Error: Input file not found: {inputFile}");
            System.Console.WriteLine($"Please place TestDocument.docx in the input directory: {inputDirectory}");
            return;
        }

        try
        {
            System.Console.WriteLine($"Project directory: {projectDirectory}");
            System.Console.WriteLine($"Input directory: {inputDirectory}");
            System.Console.WriteLine($"Output directory: {outputDirectory}");
            System.Console.WriteLine($"Processing document: {inputFile}");

            using var parser = new WordDocumentParser(outputDirectory);
            parser.ProcessDocument(inputFile);

            System.Console.WriteLine("Document processing completed successfully!");
            System.Console.WriteLine($"Results saved to: {Path.Combine(outputDirectory, "document_structure.json")}");
        }
        catch (Exception ex)
        {
            System.Console.WriteLine($"Error processing document: {ex.Message}");
            System.Console.WriteLine(ex.StackTrace);
        }
    }
}