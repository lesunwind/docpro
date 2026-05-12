namespace DocumentProcessor.UI
{
    partial class MainForm
    {
        /// <summary>
        ///  Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        ///  Clean up any resources being used.
        /// </summary>
        /// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Windows Form Designer generated code

        /// <summary>
        ///  Required method for Designer support - do not modify
        ///  the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            tabControl1 = new TabControl();
            tabPage1 = new TabPage();
            btnParse = new Button();
            progressBar1 = new ProgressBar();
            label3 = new Label();
            btnViewJson = new Button();
            txtOutputJson = new TextBox();
            label2 = new Label();
            btnBrowse = new Button();
            txtDocPath = new TextBox();
            label1 = new Label();
            tabPage2 = new TabPage();
            bntCombine = new Button();
            extractionProgressBar = new ProgressBar();
            lblExtractionStatus = new Label();
            btnExtract = new Button();
            label5 = new Label();
            tvSections = new TreeView();
            cbJsonFiles = new ComboBox();
            label4 = new Label();
            tabPage3 = new TabPage();
            splitContainer1 = new SplitContainer();
            panel1 = new Panel();
            label6 = new Label();
            cbAvailableDocuments = new ComboBox();
            tvAvailableSections = new TreeView();
            btnAddSelection = new Button();
            panel2 = new Panel();
            label7 = new Label();
            tvCombinedStructure = new TreeView();
            btnAddSection = new Button();
            btnRemoveSection = new Button();
            panel3 = new Panel();
            combineStatusLabel = new Label();
            combineProgressBar = new ProgressBar();
            btnCombineDocuments = new Button();
            tabControl1.SuspendLayout();
            tabPage1.SuspendLayout();
            tabPage2.SuspendLayout();
            tabPage3.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)splitContainer1).BeginInit();
            splitContainer1.Panel1.SuspendLayout();
            splitContainer1.Panel2.SuspendLayout();
            splitContainer1.SuspendLayout();
            panel1.SuspendLayout();
            panel2.SuspendLayout();
            panel3.SuspendLayout();
            SuspendLayout();
            // 
            // tabControl1
            // 
            tabControl1.Controls.Add(tabPage1);
            tabControl1.Controls.Add(tabPage2);
            tabControl1.Controls.Add(tabPage3);
            tabControl1.Location = new Point(4, 4);
            tabControl1.Name = "tabControl1";
            tabControl1.SelectedIndex = 0;
            tabControl1.Size = new Size(1128, 928);
            tabControl1.TabIndex = 0;
            // 
            // tabPage1
            // 
            tabPage1.Controls.Add(btnParse);
            tabPage1.Controls.Add(progressBar1);
            tabPage1.Controls.Add(label3);
            tabPage1.Controls.Add(btnViewJson);
            tabPage1.Controls.Add(txtOutputJson);
            tabPage1.Controls.Add(label2);
            tabPage1.Controls.Add(btnBrowse);
            tabPage1.Controls.Add(txtDocPath);
            tabPage1.Controls.Add(label1);
            tabPage1.Location = new Point(4, 29);
            tabPage1.Name = "tabPage1";
            tabPage1.Padding = new Padding(3);
            tabPage1.Size = new Size(1120, 895);
            tabPage1.TabIndex = 0;
            tabPage1.Text = "Document Parsing";
            tabPage1.UseVisualStyleBackColor = true;
            // 
            // btnParse
            // 
            btnParse.Location = new Point(8, 139);
            btnParse.Name = "btnParse";
            btnParse.Size = new Size(169, 29);
            btnParse.TabIndex = 8;
            btnParse.Text = "Parse Document";
            btnParse.UseVisualStyleBackColor = true;
            // 
            // progressBar1
            // 
            progressBar1.Location = new Point(11, 205);
            progressBar1.Name = "progressBar1";
            progressBar1.Size = new Size(1083, 29);
            progressBar1.Style = ProgressBarStyle.Marquee;
            progressBar1.TabIndex = 7;
            progressBar1.Visible = false;
            // 
            // label3
            // 
            label3.AutoSize = true;
            label3.Location = new Point(11, 182);
            label3.Name = "label3";
            label3.Size = new Size(50, 20);
            label3.TabIndex = 6;
            label3.Text = "Ready";
            // 
            // btnViewJson
            // 
            btnViewJson.Location = new Point(1000, 96);
            btnViewJson.Name = "btnViewJson";
            btnViewJson.Size = new Size(94, 29);
            btnViewJson.TabIndex = 5;
            btnViewJson.Text = "View";
            btnViewJson.UseVisualStyleBackColor = true;
            // 
            // txtOutputJson
            // 
            txtOutputJson.Location = new Point(11, 95);
            txtOutputJson.Name = "txtOutputJson";
            txtOutputJson.ReadOnly = true;
            txtOutputJson.Size = new Size(983, 27);
            txtOutputJson.TabIndex = 4;
            // 
            // label2
            // 
            label2.AutoSize = true;
            label2.Location = new Point(10, 72);
            label2.Name = "label2";
            label2.Size = new Size(129, 20);
            label2.TabIndex = 3;
            label2.Text = "JSON Output Path:";
            // 
            // btnBrowse
            // 
            btnBrowse.Location = new Point(1000, 28);
            btnBrowse.Name = "btnBrowse";
            btnBrowse.Size = new Size(94, 29);
            btnBrowse.TabIndex = 2;
            btnBrowse.Text = "Browse...";
            btnBrowse.UseVisualStyleBackColor = true;
            // 
            // txtDocPath
            // 
            txtDocPath.Location = new Point(11, 27);
            txtDocPath.Name = "txtDocPath";
            txtDocPath.ReadOnly = true;
            txtDocPath.Size = new Size(983, 27);
            txtDocPath.TabIndex = 1;
            // 
            // label1
            // 
            label1.AutoSize = true;
            label1.Location = new Point(11, 4);
            label1.Name = "label1";
            label1.Size = new Size(108, 20);
            label1.TabIndex = 0;
            label1.Text = "Document File:";
            // 
            // tabPage2
            // 
            tabPage2.Controls.Add(bntCombine);
            tabPage2.Controls.Add(extractionProgressBar);
            tabPage2.Controls.Add(lblExtractionStatus);
            tabPage2.Controls.Add(btnExtract);
            tabPage2.Controls.Add(label5);
            tabPage2.Controls.Add(tvSections);
            tabPage2.Controls.Add(cbJsonFiles);
            tabPage2.Controls.Add(label4);
            tabPage2.Location = new Point(4, 29);
            tabPage2.Name = "tabPage2";
            tabPage2.Padding = new Padding(3);
            tabPage2.Size = new Size(1120, 895);
            tabPage2.TabIndex = 1;
            tabPage2.Text = "Document Extraction";
            tabPage2.UseVisualStyleBackColor = true;
            // 
            // bntCombine
            // 
            bntCombine.Location = new Point(298, 767);
            bntCombine.Name = "bntCombine";
            bntCombine.Size = new Size(245, 29);
            bntCombine.TabIndex = 7;
            bntCombine.Text = "Combine Extracted Sections";
            bntCombine.UseVisualStyleBackColor = true;
            // 
            // extractionProgressBar
            // 
            extractionProgressBar.Location = new Point(29, 840);
            extractionProgressBar.Name = "extractionProgressBar";
            extractionProgressBar.Size = new Size(1070, 29);
            extractionProgressBar.Style = ProgressBarStyle.Marquee;
            extractionProgressBar.TabIndex = 6;
            extractionProgressBar.Visible = false;
            // 
            // lblExtractionStatus
            // 
            lblExtractionStatus.AutoSize = true;
            lblExtractionStatus.Location = new Point(27, 813);
            lblExtractionStatus.Name = "lblExtractionStatus";
            lblExtractionStatus.Size = new Size(50, 20);
            lblExtractionStatus.TabIndex = 5;
            lblExtractionStatus.Text = "Ready";
            // 
            // btnExtract
            // 
            btnExtract.Location = new Point(16, 767);
            btnExtract.Name = "btnExtract";
            btnExtract.Size = new Size(245, 29);
            btnExtract.TabIndex = 4;
            btnExtract.Text = "Extract Selected Sections";
            btnExtract.UseVisualStyleBackColor = true;
            // 
            // label5
            // 
            label5.AutoSize = true;
            label5.Location = new Point(21, 85);
            label5.Name = "label5";
            label5.Size = new Size(178, 20);
            label5.TabIndex = 3;
            label5.Text = "Select Sections to Extract:";
            // 
            // tvSections
            // 
            tvSections.CheckBoxes = true;
            tvSections.Location = new Point(21, 117);
            tvSections.Name = "tvSections";
            tvSections.Size = new Size(1078, 644);
            tvSections.TabIndex = 2;
            // 
            // cbJsonFiles
            // 
            cbJsonFiles.FormattingEnabled = true;
            cbJsonFiles.Location = new Point(20, 37);
            cbJsonFiles.Name = "cbJsonFiles";
            cbJsonFiles.Size = new Size(1079, 28);
            cbJsonFiles.TabIndex = 1;
            // 
            // label4
            // 
            label4.AutoSize = true;
            label4.Location = new Point(16, 12);
            label4.Name = "label4";
            label4.Size = new Size(188, 20);
            label4.TabIndex = 0;
            label4.Text = "Select Document Structure:";
            // 
            // tabPage3
            // 
            tabPage3.Controls.Add(splitContainer1);
            tabPage3.Controls.Add(panel3);
            tabPage3.Location = new Point(4, 29);
            tabPage3.Name = "tabPage3";
            tabPage3.Size = new Size(1120, 895);
            tabPage3.TabIndex = 2;
            tabPage3.Text = "Document Combination";
            // 
            // splitContainer1
            // 
            splitContainer1.Dock = DockStyle.Fill;
            splitContainer1.Location = new Point(0, 0);
            splitContainer1.Name = "splitContainer1";
            // 
            // splitContainer1.Panel1
            // 
            splitContainer1.Panel1.Controls.Add(panel1);
            // 
            // splitContainer1.Panel2
            // 
            splitContainer1.Panel2.Controls.Add(panel2);
            splitContainer1.Size = new Size(1120, 815);
            splitContainer1.SplitterDistance = 903;
            splitContainer1.TabIndex = 0;
            // 
            // panel1
            // 
            panel1.Controls.Add(label6);
            panel1.Controls.Add(cbAvailableDocuments);
            panel1.Controls.Add(tvAvailableSections);
            panel1.Controls.Add(btnAddSelection);
            panel1.Dock = DockStyle.Fill;
            panel1.Location = new Point(0, 0);
            panel1.Name = "panel1";
            panel1.Size = new Size(903, 815);
            panel1.TabIndex = 0;
            // 
            // label6
            // 
            label6.AutoSize = true;
            label6.Location = new Point(10, 10);
            label6.Name = "label6";
            label6.Size = new Size(153, 20);
            label6.TabIndex = 0;
            label6.Text = "Available Documents:";
            // 
            // cbAvailableDocuments
            // 
            cbAvailableDocuments.DropDownStyle = ComboBoxStyle.DropDownList;
            cbAvailableDocuments.Location = new Point(10, 35);
            cbAvailableDocuments.Name = "cbAvailableDocuments";
            cbAvailableDocuments.Size = new Size(300, 28);
            cbAvailableDocuments.TabIndex = 1;
            // 
            // tvAvailableSections
            // 
            tvAvailableSections.Anchor = AnchorStyles.Top | AnchorStyles.Bottom | AnchorStyles.Left | AnchorStyles.Right;
            tvAvailableSections.CheckBoxes = true;
            tvAvailableSections.Location = new Point(10, 70);
            tvAvailableSections.Name = "tvAvailableSections";
            tvAvailableSections.Size = new Size(1003, 1115);
            tvAvailableSections.TabIndex = 2;
            // 
            // btnAddSelection
            // 
            btnAddSelection.Anchor = AnchorStyles.Bottom | AnchorStyles.Left | AnchorStyles.Right;
            btnAddSelection.Location = new Point(10, 1195);
            btnAddSelection.Name = "btnAddSelection";
            btnAddSelection.Size = new Size(1003, 30);
            btnAddSelection.TabIndex = 3;
            btnAddSelection.Text = "Add Selected Sections >>";
            // 
            // panel2
            // 
            panel2.Controls.Add(label7);
            panel2.Controls.Add(tvCombinedStructure);
            panel2.Controls.Add(btnAddSection);
            panel2.Controls.Add(btnRemoveSection);
            panel2.Dock = DockStyle.Fill;
            panel2.Location = new Point(0, 0);
            panel2.Name = "panel2";
            panel2.Size = new Size(213, 815);
            panel2.TabIndex = 0;
            // 
            // label7
            // 
            label7.AutoSize = true;
            label7.Location = new Point(10, 10);
            label7.Name = "label7";
            label7.Size = new Size(194, 20);
            label7.TabIndex = 0;
            label7.Text = "Output Document Structure:";
            // 
            // tvCombinedStructure
            // 
            tvCombinedStructure.Anchor = AnchorStyles.Top | AnchorStyles.Bottom | AnchorStyles.Left | AnchorStyles.Right;
            tvCombinedStructure.Location = new Point(10, 35);
            tvCombinedStructure.Name = "tvCombinedStructure";
            tvCombinedStructure.Size = new Size(313, 1115);
            tvCombinedStructure.TabIndex = 1;
            // 
            // btnAddSection
            // 
            btnAddSection.Anchor = AnchorStyles.Bottom | AnchorStyles.Left;
            btnAddSection.Location = new Point(10, 1195);
            btnAddSection.Name = "btnAddSection";
            btnAddSection.Size = new Size(145, 30);
            btnAddSection.TabIndex = 2;
            btnAddSection.Text = "Add Section";
            // 
            // btnRemoveSection
            // 
            btnRemoveSection.Anchor = AnchorStyles.Bottom | AnchorStyles.Right;
            btnRemoveSection.Location = new Point(178, 1195);
            btnRemoveSection.Name = "btnRemoveSection";
            btnRemoveSection.Size = new Size(145, 30);
            btnRemoveSection.TabIndex = 3;
            btnRemoveSection.Text = "Remove Selection";
            // 
            // panel3
            // 
            panel3.Controls.Add(combineStatusLabel);
            panel3.Controls.Add(combineProgressBar);
            panel3.Controls.Add(btnCombineDocuments);
            panel3.Dock = DockStyle.Bottom;
            panel3.Location = new Point(0, 815);
            panel3.Name = "panel3";
            panel3.Size = new Size(1120, 80);
            panel3.TabIndex = 1;
            // 
            // combineStatusLabel
            // 
            combineStatusLabel.AutoSize = true;
            combineStatusLabel.Location = new Point(10, 15);
            combineStatusLabel.Name = "combineStatusLabel";
            combineStatusLabel.Size = new Size(50, 20);
            combineStatusLabel.TabIndex = 0;
            combineStatusLabel.Text = "Ready";
            // 
            // combineProgressBar
            // 
            combineProgressBar.Location = new Point(10, 40);
            combineProgressBar.Name = "combineProgressBar";
            combineProgressBar.Size = new Size(640, 23);
            combineProgressBar.Style = ProgressBarStyle.Marquee;
            combineProgressBar.TabIndex = 1;
            combineProgressBar.Visible = false;
            // 
            // btnCombineDocuments
            // 
            btnCombineDocuments.Location = new Point(500, 10);
            btnCombineDocuments.Name = "btnCombineDocuments";
            btnCombineDocuments.Size = new Size(150, 30);
            btnCombineDocuments.TabIndex = 2;
            btnCombineDocuments.Text = "Combine Documents";
            // 
            // MainForm
            // 
            AutoScaleDimensions = new SizeF(8F, 20F);
            AutoScaleMode = AutoScaleMode.Font;
            ClientSize = new Size(1133, 944);
            Controls.Add(tabControl1);
            Name = "MainForm";
            StartPosition = FormStartPosition.CenterScreen;
            Text = "Document Processor";
            tabControl1.ResumeLayout(false);
            tabPage1.ResumeLayout(false);
            tabPage1.PerformLayout();
            tabPage2.ResumeLayout(false);
            tabPage2.PerformLayout();
            tabPage3.ResumeLayout(false);
            splitContainer1.Panel1.ResumeLayout(false);
            splitContainer1.Panel2.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)splitContainer1).EndInit();
            splitContainer1.ResumeLayout(false);
            panel1.ResumeLayout(false);
            panel1.PerformLayout();
            panel2.ResumeLayout(false);
            panel2.PerformLayout();
            panel3.ResumeLayout(false);
            panel3.PerformLayout();
            ResumeLayout(false);
        }

        #endregion

        private TabControl tabControl1;
        private TabPage tabPage1;
        private TabPage tabPage2;
        private TextBox txtDocPath;
        private Label label1;
        private Button btnViewJson;
        private TextBox txtOutputJson;
        private Label label2;
        private Button btnBrowse;
        private ProgressBar progressBar1;
        private Label label3;
        private ComboBox cbJsonFiles;
        private Label label4;
        private Button btnExtract;
        private Label label5;
        private TreeView tvSections;
        private ProgressBar extractionProgressBar;
        private Label lblExtractionStatus;
        private Button btnParse;
        private Button bntCombine;



        // Declare these variables at the class level
        private TabPage tabPage3;
        private SplitContainer splitContainer1;
        private Panel panel1, panel2, panel3;
        private Label label6, label7, combineStatusLabel;
        private ComboBox cbAvailableDocuments;
        private TreeView tvAvailableSections;
        private TreeView tvCombinedStructure;
        private Button btnAddSelection, btnAddSection, btnRemoveSection, btnCombineDocuments;
        private ProgressBar combineProgressBar;
    }
}
