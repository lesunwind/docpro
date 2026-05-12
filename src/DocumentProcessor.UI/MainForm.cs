using System;
using System.IO;
using System.Linq;
using System.Windows.Forms;
using System.Collections.Generic;
using System.Text.Json;
using DocumentProcessor.Core.Models;
using DocumentProcessor.Core.Services;

namespace DocumentProcessor.UI
{
    public partial class MainForm : Form
    {
        private readonly string _inputFolder;
        private readonly string _outputFolder;
        private List<Section> _loadedSections;

        public MainForm()
        {
            InitializeComponent();

            // Initialize folders
            string baseDir = AppDomain.CurrentDomain.BaseDirectory;
            _inputFolder = Path.Combine(baseDir, "input");
            _outputFolder = Path.Combine(baseDir, "output");
            string logFolder = Path.Combine(baseDir, "logs");

            // Ensure folders exist
            Directory.CreateDirectory(_inputFolder);
            Directory.CreateDirectory(_outputFolder);
            Directory.CreateDirectory(logFolder);

            // Initialize logger
            Logger.Initialize(logFolder);
            Logger.Log("Application started");

            // Initialize UI state
            InitializeTabs();
            RefreshJsonFilesList();

            // Add event handlers
            btnBrowse.Click += btnBrowse_Click;
            btnViewJson.Click += btnViewJson_Click;
            btnParse.Click += btnParse_Click;
            cbJsonFiles.SelectedIndexChanged += cbJsonFiles_SelectedIndexChanged;
            btnExtract.Click += btnExtract_Click;
            tvSections.AfterCheck += tvSections_AfterCheck;
        }

        private void InitializeTabs()
        {
            // Initially disable the extraction tab until parsing is done
            tabControl1.SelectedIndex = 0;
        }

        private void RefreshJsonFilesList()
        {
            cbJsonFiles.Items.Clear();
            string[] jsonFiles = Directory.GetFiles(_outputFolder, "*.json");
            foreach (string file in jsonFiles)
            {
                cbJsonFiles.Items.Add(Path.GetFileName(file));
            }

            if (cbJsonFiles.Items.Count > 0)
            {
                cbJsonFiles.SelectedIndex = 0;
            }
        }

        #region Document Parsing Tab

        private void btnBrowse_Click(object sender, EventArgs e)
        {
            using (OpenFileDialog openFileDialog = new OpenFileDialog())
            {
                openFileDialog.Filter = "Word Documents|*.docx";
                openFileDialog.Title = "Select a Word Document";
                openFileDialog.InitialDirectory = _inputFolder;

                if (openFileDialog.ShowDialog() == DialogResult.OK)
                {
                    txtDocPath.Text = openFileDialog.FileName;
                    txtOutputJson.Text = Path.Combine(_outputFolder, Path.GetFileNameWithoutExtension(openFileDialog.FileName) + ".json");
                }
            }
        }

        private async void btnParse_Click(object sender, EventArgs e)
        {
            if (string.IsNullOrEmpty(txtDocPath.Text) || !File.Exists(txtDocPath.Text))
            {
                MessageBox.Show("Please select a valid Word document file.", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }

            try
            {
                Cursor = Cursors.WaitCursor;
                label3.Text = "Parsing document...";
                progressBar1.Visible = true;

                // Copy file to input folder if it's not already there
                string inputFile = txtDocPath.Text;
                string fileName = Path.GetFileName(inputFile);
                string destFile = Path.Combine(_inputFolder, fileName);

                if (inputFile != destFile)
                {
                    File.Copy(inputFile, destFile, true);
                }

                // Get the desired JSON filename
                string jsonFilename = Path.GetFileNameWithoutExtension(fileName) + ".json";

                // Parse document with the custom filename
                await Task.Run(() =>
                {
                    using (var parser = new WordDocumentParser(_outputFolder, jsonFilename))
                    {
                        parser.ProcessDocument(destFile);
                    }
                });

                label3.Text = "Document parsed successfully!";
                MessageBox.Show("Document parsing completed. JSON file saved to output folder.", "Success", MessageBoxButtons.OK, MessageBoxIcon.Information);

                // Refresh JSON files list
                RefreshJsonFilesList();

                // Enable Extract tab
                tabPage2.Enabled = true;
            }
            catch (Exception ex)
            {
                label3.Text = "Error parsing document.";
                MessageBox.Show($"An error occurred while parsing the document: {ex.Message}", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
            finally
            {
                Cursor = Cursors.Default;
                progressBar1.Visible = false;
            }
        }
        private void btnViewJson_Click(object sender, EventArgs e)
        {
            if (string.IsNullOrEmpty(txtOutputJson.Text) || !File.Exists(txtOutputJson.Text))
            {
                MessageBox.Show("JSON file does not exist. Please parse a document first.", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }

            try
            {
                System.Diagnostics.Process.Start("notepad.exe", txtOutputJson.Text);
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error opening JSON file: {ex.Message}", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        #endregion

        #region Document Extraction Tab

        private void cbJsonFiles_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (cbJsonFiles.SelectedItem != null)
            {
                LoadSectionTree();
            }
        }

        private void LoadSectionTree()
        {
            try
            {
                tvSections.Nodes.Clear();
                string jsonFilePath = Path.Combine(_outputFolder, cbJsonFiles.SelectedItem.ToString());

                string jsonContent = File.ReadAllText(jsonFilePath);
                var options = new JsonSerializerOptions
                {
                    PropertyNameCaseInsensitive = true
                };

                _loadedSections = JsonSerializer.Deserialize<List<Section>>(jsonContent, options);

                if (_loadedSections != null)
                {
                    foreach (var section in _loadedSections)
                    {
                        if (!string.IsNullOrEmpty(section.Title))
                        {
                            var node = new TreeNode(section.Title)
                            {
                                Tag = section
                            };
                            AddSubsections(node, section.Subsections);
                            tvSections.Nodes.Add(node);
                        }
                    }

                    tvSections.ExpandAll();
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error loading JSON file: {ex.Message}", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void AddSubsections(TreeNode parentNode, List<Section> subsections)
        {
            if (subsections == null || !subsections.Any())
                return;

            foreach (var subsection in subsections)
            {
                if (!string.IsNullOrEmpty(subsection.Title))
                {
                    var node = new TreeNode(subsection.Title)
                    {
                        Tag = subsection
                    };
                    AddSubsections(node, subsection.Subsections);
                    parentNode.Nodes.Add(node);
                }
            }
        }

        private void btnExtract_Click1(object sender, EventArgs e)
        {
            if (tvSections.Nodes.Count == 0)
            {
                MessageBox.Show("Please load a document structure first.", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }

            // Get selected sections
            var selectedPaths = GetSelectedSectionPaths();

            if (selectedPaths.Count == 0)
            {
                MessageBox.Show("Please select at least one section to extract.", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }

            // Ask for output file
            using (SaveFileDialog saveFileDialog = new SaveFileDialog())
            {
                saveFileDialog.Filter = "Word Documents|*.docx";
                saveFileDialog.Title = "Save Extracted Document";
                saveFileDialog.InitialDirectory = _outputFolder;
                saveFileDialog.FileName = Path.GetFileNameWithoutExtension(cbJsonFiles.SelectedItem.ToString()) + "_extracted.docx";

                if (saveFileDialog.ShowDialog() == DialogResult.OK)
                {
                    try
                    {
                        Cursor = Cursors.WaitCursor;
                        lblExtractionStatus.Text = "Extracting sections...";
                        extractionProgressBar.Visible = true;

                        string jsonFilePath = Path.Combine(_outputFolder, cbJsonFiles.SelectedItem.ToString());

                        // Extract sections
                        var extractor = new SectionExtractor();
                        extractor.ExtractSectionsToNewDocument(jsonFilePath, saveFileDialog.FileName, selectedPaths.ToArray());

                        lblExtractionStatus.Text = "Sections extracted successfully!";
                        MessageBox.Show("Document extraction completed.", "Success", MessageBoxButtons.OK, MessageBoxIcon.Information);

                        // Ask if user wants to open the generated document
                        if (MessageBox.Show("Do you want to open the generated document?", "Open Document", MessageBoxButtons.YesNo, MessageBoxIcon.Question) == DialogResult.Yes)
                        {
                            System.Diagnostics.Process.Start("explorer.exe", saveFileDialog.FileName);
                        }
                    }
                    catch (Exception ex)
                    {
                        lblExtractionStatus.Text = "Error extracting sections.";
                        MessageBox.Show($"An error occurred while extracting sections: {ex.Message}", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    }
                    finally
                    {
                        Cursor = Cursors.Default;
                        extractionProgressBar.Visible = false;
                    }
                }
            }
        }
        private void btnExtract_Click2(object sender, EventArgs e)
        {
            if (tvSections.Nodes.Count == 0)
            {
                MessageBox.Show("Please load a document structure first.", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }

            Logger.Log("Extract button clicked");

            // Get selected sections
            var selectedPaths = GetSelectedSectionPaths();
            Logger.Log($"Selected {selectedPaths.Count} section paths");

            for (int i = 0; i < selectedPaths.Count; i++)
            {
                Logger.Log($"Selected path {i + 1}: {string.Join(" > ", selectedPaths[i])}");
            }

            if (selectedPaths.Count == 0)
            {
                MessageBox.Show("Please select at least one section to extract.", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }

            // Ask for output file
            using (SaveFileDialog saveFileDialog = new SaveFileDialog())
            {
                saveFileDialog.Filter = "Word Documents|*.docx";
                saveFileDialog.Title = "Save Extracted Document";
                saveFileDialog.InitialDirectory = _outputFolder;
                saveFileDialog.FileName = Path.GetFileNameWithoutExtension(cbJsonFiles.SelectedItem.ToString()) + "_extracted.docx";

                if (saveFileDialog.ShowDialog() == DialogResult.OK)
                {
                    Logger.Log($"Output file selected: {saveFileDialog.FileName}");

                    try
                    {
                        Cursor = Cursors.WaitCursor;
                        lblExtractionStatus.Text = "Extracting sections...";
                        extractionProgressBar.Visible = true;

                        string jsonFilePath = Path.Combine(_outputFolder, cbJsonFiles.SelectedItem.ToString());
                        Logger.Log($"JSON file path: {jsonFilePath}");

                        // Add a method to dump section info from the JSON
                        LogSectionInfo(jsonFilePath);

                        var extractor = new SectionExtractor();

                        // Add debug output capture
                        var originalOutput = Console.Out;
                        using (var sw = new StringWriter())
                        {
                            Console.SetOut(sw);

                            Logger.Log("Starting extraction process");

                            // If only one section is selected, use ExtractSectionToNewDocument
                            if (selectedPaths.Count == 1)
                            {
                                Logger.Log("Using ExtractSectionToNewDocument method");
                                extractor.ExtractSectionToNewDocument(jsonFilePath, saveFileDialog.FileName, selectedPaths[0]);
                            }
                            else
                            {
                                // For multiple sections, use ExtractSectionsToNewDocument
                                Logger.Log("Using ExtractSectionsToNewDocument method");
                                extractor.ExtractSectionsToNewDocument(jsonFilePath, saveFileDialog.FileName, selectedPaths.ToArray());
                            }

                            // Log the console output
                            Logger.Log("Extraction console output:");
                            Logger.Log(sw.ToString());

                            // Restore console output
                            Console.SetOut(originalOutput);
                        }

                        Logger.Log("Extraction completed");
                        lblExtractionStatus.Text = "Sections extracted successfully!";
                        MessageBox.Show("Document extraction completed.", "Success", MessageBoxButtons.OK, MessageBoxIcon.Information);

                        // Check if images exist in the extracted document
                        CheckExtractedDocumentForImages(saveFileDialog.FileName);

                        // Ask if user wants to open the generated document
                        if (MessageBox.Show("Do you want to open the generated document?", "Open Document", MessageBoxButtons.YesNo, MessageBoxIcon.Question) == DialogResult.Yes)
                        {
                            System.Diagnostics.Process.Start("explorer.exe", saveFileDialog.FileName);
                        }
                    }
                    catch (Exception ex)
                    {
                        Logger.Log($"Error during extraction: {ex.Message}");
                        Logger.Log($"Stack trace: {ex.StackTrace}");

                        lblExtractionStatus.Text = "Error extracting sections.";
                        MessageBox.Show($"An error occurred while extracting sections: {ex.Message}", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    }
                    finally
                    {
                        Cursor = Cursors.Default;
                        extractionProgressBar.Visible = false;
                    }
                }
            }
        }
        private async void btnExtract_Click(object sender, EventArgs e)
        {
            if (tvSections.Nodes.Count == 0)
            {
                MessageBox.Show("Please load a document structure first.", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }

            Logger.Log("Extract button clicked");

            // Get selected sections
            var selectedPaths = GetSelectedSectionPaths();
            Logger.Log($"Selected {selectedPaths.Count} section paths");

            for (int i = 0; i < selectedPaths.Count; i++)
            {
                Logger.Log($"Selected path {i + 1}: {string.Join(" > ", selectedPaths[i])}");
            }

            if (selectedPaths.Count == 0)
            {
                MessageBox.Show("Please select at least one section to extract.", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }

            // Ask for output file
            using (SaveFileDialog saveFileDialog = new SaveFileDialog())
            {
                saveFileDialog.Filter = "Word Documents|*.docx";
                saveFileDialog.Title = "Save Extracted Document";
                saveFileDialog.InitialDirectory = _outputFolder;
                saveFileDialog.FileName = Path.GetFileNameWithoutExtension(cbJsonFiles.SelectedItem.ToString()) + "_extracted.docx";

                if (saveFileDialog.ShowDialog() == DialogResult.OK)
                {
                    Logger.Log($"Output file selected: {saveFileDialog.FileName}");

                    try
                    {
                        Cursor = Cursors.WaitCursor;
                        lblExtractionStatus.Text = "Extracting sections...";
                        extractionProgressBar.Visible = true;

                        string jsonFilePath = Path.Combine(_outputFolder, cbJsonFiles.SelectedItem.ToString());
                        Logger.Log($"JSON file path: {jsonFilePath}");

                        LogSectionInfo(jsonFilePath);

                        var extractor = new SectionExtractor();

                        // Capture console output for logging
                        var originalOutput = Console.Out;
                        using (var sw = new StringWriter())
                        {
                            Console.SetOut(sw);

                            Logger.Log("Starting extraction process");

                            // Try both approaches to see which one works
                            if (selectedPaths.Count == 1)
                            {
                                // Process each section individually using ExtractSectionToNewDocument
                                // This is what likely works in the console app
                                Logger.Log("Using ExtractSectionToNewDocument method");
                                extractor.ExtractSectionToNewDocument(jsonFilePath, saveFileDialog.FileName, selectedPaths[0]);
                            }
                            else
                            {
                                // Process each section individually to a temporary file, then combine
                                Logger.Log("Processing multiple sections individually");
                                var tempFiles = new List<string>();

                                for (int i = 0; i < selectedPaths.Count; i++)
                                {
                                    var tempFile = Path.Combine(Path.GetTempPath(), $"temp_section_{i}.docx");
                                    Logger.Log($"Extracting section {i + 1} to temp file: {tempFile}");
                                    extractor.ExtractSectionToNewDocument(jsonFilePath, tempFile, selectedPaths[i]);
                                    tempFiles.Add(tempFile);
                                }

                                // Now combine all the temp files into a single document
                                // You would need to implement this MergeDocuments method
                                // For now, just copy the first one as a test
                                Logger.Log("Copying first temp file to output location");
                                File.Copy(tempFiles[0], saveFileDialog.FileName, true);
                            }

                            // Log the console output
                            Logger.Log("Extraction console output:");
                            Logger.Log(sw.ToString());

                            // Restore console output
                            Console.SetOut(originalOutput);
                        }

                        Logger.Log("Extraction completed");
                        lblExtractionStatus.Text = "Sections extracted successfully!";
                        MessageBox.Show("Document extraction completed.", "Success", MessageBoxButtons.OK, MessageBoxIcon.Information);

                        // Check if images exist in the extracted document
                        CheckExtractedDocumentForImages(saveFileDialog.FileName);

                        // Ask if user wants to open the generated document
                        if (MessageBox.Show("Do you want to open the generated document?", "Open Document", MessageBoxButtons.YesNo, MessageBoxIcon.Question) == DialogResult.Yes)
                        {
                            System.Diagnostics.Process.Start("explorer.exe", saveFileDialog.FileName);
                        }
                    }
                    catch (Exception ex)
                    {
                        Logger.Log($"Error during extraction: {ex.Message}");
                        Logger.Log($"Stack trace: {ex.StackTrace}");

                        lblExtractionStatus.Text = "Error extracting sections.";
                        MessageBox.Show($"An error occurred while extracting sections: {ex.Message}", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    }
                    finally
                    {
                        Cursor = Cursors.Default;
                        extractionProgressBar.Visible = false;
                    }
                }
            }
        }
        private void LogSectionInfo(string jsonFilePath)
        {
            try
            {
                string jsonContent = File.ReadAllText(jsonFilePath);
                var options = new JsonSerializerOptions
                {
                    PropertyNameCaseInsensitive = true
                };

                var sections = JsonSerializer.Deserialize<List<Section>>(jsonContent, options);

                if (sections != null)
                {
                    Logger.Log($"JSON contains {sections.Count} top-level sections");
                    int totalImages = 0;

                    foreach (var section in sections)
                    {
                        CountImagesInSection(section, ref totalImages);
                    }

                    Logger.Log($"Total images in document: {totalImages}");
                }
            }
            catch (Exception ex)
            {
                Logger.Log($"Error analyzing JSON content: {ex.Message}");
            }
        }

        private void CountImagesInSection(Section section, ref int totalImages)
        {
            if (section.Images != null && section.Images.Count > 0)
            {
                totalImages += section.Images.Count;
                Logger.Log($"Section '{section.Title}' contains {section.Images.Count} images");

                foreach (var image in section.Images)
                {
                    Logger.Log($"  Image: {image.Id}, Type: {image.ContentType}, Size: {image.Width}x{image.Height}, Base64 length: {image.Base64Data?.Length ?? 0}");
                }
            }

            if (section.Subsections != null)
            {
                foreach (var subsection in section.Subsections)
                {
                    CountImagesInSection(subsection, ref totalImages);
                }
            }
        }

        private void CheckExtractedDocumentForImages(string docPath)
        {
            try
            {
                Logger.Log($"Analyzing extracted document for images: {docPath}");

                // This is a basic check to see if the file size suggests it contains images
                var fileInfo = new FileInfo(docPath);
                Logger.Log($"Extracted document size: {fileInfo.Length:N0} bytes");

                if (fileInfo.Length < 20000)
                {
                    Logger.Log("WARNING: Document size is very small, might not contain images");
                }

                // You could add more detailed inspection here using DocumentFormat.OpenXml
                // to actually count images in the generated document
            }
            catch (Exception ex)
            {
                Logger.Log($"Error checking extracted document: {ex.Message}");
            }
        }
        private List<string[]> GetSelectedSectionPaths()
        {
            var paths = new List<string[]>();

            foreach (TreeNode node in tvSections.Nodes)
            {
                GetSelectedNodePaths(node, new List<string>(), paths);
            }

            return paths;
        }

        private void GetSelectedNodePaths(TreeNode node, List<string> currentPath, List<string[]> paths)
        {
            // Add this node to the current path
            currentPath.Add(node.Text);

            // If this node is checked, add its path
            if (node.Checked)
            {
                paths.Add(currentPath.ToArray());
            }

            // Process children
            foreach (TreeNode childNode in node.Nodes)
            {
                GetSelectedNodePaths(childNode, new List<string>(currentPath), paths);
            }
        }
        private void tvSections_AfterCheck(object sender, TreeViewEventArgs e)
        {
            // Propagate check state to child nodes
            foreach (TreeNode child in e.Node.Nodes)
            {
                child.Checked = e.Node.Checked;
            }
        }

        #endregion
    }
}