namespace DocumentProcessor.Tests;

public abstract class TestBase
{
    protected string _testDataPath;
    protected string _outputPath;
    protected string _baseDirectory;

    [TestInitialize]
    public virtual void Setup()
    {
        // Get the solution directory path
        var currentDir = AppDomain.CurrentDomain.BaseDirectory;
        _baseDirectory = Path.GetFullPath(Path.Combine(currentDir, "..", "..", "..", "..", ".."));

        // Set up paths
        _testDataPath = Path.Combine(_baseDirectory, "src", "DocumentProcessor.Console", "input");
        _outputPath = Path.Combine(_baseDirectory, "src", "DocumentProcessor.Console", "output");

        Console.WriteLine($"Base Directory: {_baseDirectory}");
        Console.WriteLine($"Test Data Path: {_testDataPath}");
        Console.WriteLine($"Output Path: {_outputPath}");

        // Ensure test directories exist
        EnsureDirectoryExists(_testDataPath);
        EnsureDirectoryExists(_outputPath);

        // Clean existing files with retry
        SafeCleanDirectory(_testDataPath);
        SafeCleanDirectory(_outputPath);

        // Copy test document with retry
        RetryOperation(() => CopyTestFiles(), "Copying test files");
    }

    [TestCleanup]
    public virtual void Cleanup()
    {
        RetryOperation(() => SafeCleanDirectory(_outputPath), "Cleaning output directory");
        RetryOperation(() => SafeCleanDirectory(_testDataPath), "Cleaning test data directory");
    }

    private void EnsureDirectoryExists(string path)
    {
        if (!Directory.Exists(path))
        {
            Directory.CreateDirectory(path);
        }
    }

    private void SafeCleanDirectory(string path)
    {
        if (!Directory.Exists(path)) return;

        foreach (var file in Directory.GetFiles(path))
        {
            RetryOperation(() =>
            {
                if (File.Exists(file))
                {
                    File.SetAttributes(file, FileAttributes.Normal);
                    File.Delete(file);
                }
            }, $"Deleting file {Path.GetFileName(file)}");
        }
    }

    private void CopyTestFiles()
    {
        var sourceDoc = Path.Combine(_baseDirectory, "tests", "DocumentProcessor.Tests", "TestFiles", "TestDocument.docx");
        var targetDoc = Path.Combine(_testDataPath, "TestDocument.docx");

        if (File.Exists(sourceDoc))
        {
            using (var sourceStream = new FileStream(sourceDoc, FileMode.Open, FileAccess.Read, FileShare.Read))
            using (var targetStream = new FileStream(targetDoc, FileMode.Create, FileAccess.Write, FileShare.None))
            {
                sourceStream.CopyTo(targetStream);
            }
            Console.WriteLine($"Copied test document to {targetDoc}");
        }
        else
        {
            throw new FileNotFoundException($"Test document not found at {sourceDoc}");
        }
    }

    private void RetryOperation(Action operation, string operationName, int maxRetries = 3)
    {
        for (int i = 0; i < maxRetries; i++)
        {
            try
            {
                operation();
                return;
            }
            catch (IOException ex)
            {
                Console.WriteLine($"Attempt {i + 1} failed for {operationName}: {ex.Message}");
                if (i == maxRetries - 1) throw;
                System.Threading.Thread.Sleep(500); // Wait before retry
            }
        }
    }
}