namespace Sudoku
{
    partial class SudokuForm
    {
        /// <summary>
        /// Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        /// Clean up any resources being used.
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
        /// Required method for Designer support - do not modify
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(SudokuForm));
            this.pictureBox1 = new System.Windows.Forms.PictureBox();
            this.toolStrip1 = new System.Windows.Forms.ToolStrip();
            this.loadButton = new System.Windows.Forms.ToolStripButton();
            this.reportButton = new System.Windows.Forms.ToolStripButton();
            this.solveButton = new System.Windows.Forms.ToolStripButton();
            this.stopButton = new System.Windows.Forms.ToolStripButton();
            this.analyseButton = new System.Windows.Forms.ToolStripButton();
            this.hideButton = new System.Windows.Forms.ToolStripButton();
            this.undoButton = new System.Windows.Forms.ToolStripButton();
            this.resetButton = new System.Windows.Forms.ToolStripButton();
            this.inputButton = new System.Windows.Forms.ToolStripButton();
            this.javaButton = new System.Windows.Forms.ToolStripButton();
            this.loadJavaAnalyserButton = new System.Windows.Forms.ToolStripButton();
            this.openFileDialog1 = new System.Windows.Forms.OpenFileDialog();
            this.backgroundWorker1 = new System.ComponentModel.BackgroundWorker();
            ((System.ComponentModel.ISupportInitialize)(this.pictureBox1)).BeginInit();
            this.toolStrip1.SuspendLayout();
            this.SuspendLayout();
            // 
            // pictureBox1
            // 
            this.pictureBox1.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.pictureBox1.Location = new System.Drawing.Point(12, 28);
            this.pictureBox1.Name = "pictureBox1";
            this.pictureBox1.Size = new System.Drawing.Size(610, 547);
            this.pictureBox1.TabIndex = 0;
            this.pictureBox1.TabStop = false;
            this.pictureBox1.MouseDown += new System.Windows.Forms.MouseEventHandler(this.pictureBox_MouseDown);
            // 
            // toolStrip1
            // 
            this.toolStrip1.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.loadButton,
            this.reportButton,
            this.solveButton,
            this.stopButton,
            this.analyseButton,
            this.hideButton,
            this.undoButton,
            this.resetButton,
            this.inputButton,
            this.javaButton,
            this.loadJavaAnalyserButton});
            this.toolStrip1.Location = new System.Drawing.Point(0, 0);
            this.toolStrip1.Name = "toolStrip1";
            this.toolStrip1.Size = new System.Drawing.Size(634, 25);
            this.toolStrip1.TabIndex = 1;
            this.toolStrip1.Text = "toolStrip1";
            // 
            // loadButton
            // 
            this.loadButton.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
            this.loadButton.Image = ((System.Drawing.Image)(resources.GetObject("loadButton.Image")));
            this.loadButton.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.loadButton.Name = "loadButton";
            this.loadButton.Size = new System.Drawing.Size(23, 22);
            this.loadButton.Text = "loadButton";
            this.loadButton.ToolTipText = "Load Sudoku";
            this.loadButton.Click += new System.EventHandler(this.loadButton_Click);
            // 
            // reportButton
            // 
            this.reportButton.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
            this.reportButton.Image = ((System.Drawing.Image)(resources.GetObject("reportButton.Image")));
            this.reportButton.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.reportButton.Name = "reportButton";
            this.reportButton.Size = new System.Drawing.Size(23, 22);
            this.reportButton.Text = "reportButton";
            this.reportButton.ToolTipText = "Report Soduko";
            this.reportButton.Click += new System.EventHandler(this.reportButton_Click);
            // 
            // solveButton
            // 
            this.solveButton.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
            this.solveButton.Image = ((System.Drawing.Image)(resources.GetObject("solveButton.Image")));
            this.solveButton.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.solveButton.Name = "solveButton";
            this.solveButton.Size = new System.Drawing.Size(23, 22);
            this.solveButton.Text = "solveButton";
            this.solveButton.ToolTipText = "Solve Sudoku";
            this.solveButton.Click += new System.EventHandler(this.solveButton_Click);
            // 
            // stopButton
            // 
            this.stopButton.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
            this.stopButton.Image = ((System.Drawing.Image)(resources.GetObject("stopButton.Image")));
            this.stopButton.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.stopButton.Name = "stopButton";
            this.stopButton.Size = new System.Drawing.Size(23, 22);
            this.stopButton.Text = "stopButton";
            this.stopButton.ToolTipText = "Stop Solving";
            this.stopButton.Click += new System.EventHandler(this.stopButton_Click);
            // 
            // analyseButton
            // 
            this.analyseButton.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
            this.analyseButton.Image = ((System.Drawing.Image)(resources.GetObject("analyseButton.Image")));
            this.analyseButton.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.analyseButton.Name = "analyseButton";
            this.analyseButton.Size = new System.Drawing.Size(23, 22);
            this.analyseButton.Text = "analyseButton";
            this.analyseButton.ToolTipText = "Analyseren";
            this.analyseButton.Click += new System.EventHandler(this.analyseButton_Click);
            // 
            // hideButton
            // 
            this.hideButton.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
            this.hideButton.Image = ((System.Drawing.Image)(resources.GetObject("hideButton.Image")));
            this.hideButton.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.hideButton.Name = "hideButton";
            this.hideButton.Size = new System.Drawing.Size(23, 22);
            this.hideButton.Text = "hideButton";
            this.hideButton.ToolTipText = "Show/Hide Candidates";
            this.hideButton.Click += new System.EventHandler(this.hideButton_Click);
            // 
            // undoButton
            // 
            this.undoButton.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
            this.undoButton.Image = ((System.Drawing.Image)(resources.GetObject("undoButton.Image")));
            this.undoButton.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.undoButton.Name = "undoButton";
            this.undoButton.Size = new System.Drawing.Size(23, 22);
            this.undoButton.Text = "undoButton";
            this.undoButton.ToolTipText = "Undo Move";
            this.undoButton.Click += new System.EventHandler(this.undoButton_Click);
            // 
            // resetButton
            // 
            this.resetButton.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
            this.resetButton.Image = ((System.Drawing.Image)(resources.GetObject("resetButton.Image")));
            this.resetButton.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.resetButton.Name = "resetButton";
            this.resetButton.Size = new System.Drawing.Size(23, 22);
            this.resetButton.Text = "resetButton";
            this.resetButton.ToolTipText = "Reset Board";
            this.resetButton.Click += new System.EventHandler(this.resetButton_Click);
            // 
            // inputButton
            // 
            this.inputButton.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
            this.inputButton.Image = ((System.Drawing.Image)(resources.GetObject("inputButton.Image")));
            this.inputButton.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.inputButton.Name = "inputButton";
            this.inputButton.Size = new System.Drawing.Size(23, 22);
            this.inputButton.Text = "inputButton";
            this.inputButton.Click += new System.EventHandler(this.inputButton_Click);
            // 
            // javaButton
            // 
            this.javaButton.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
            this.javaButton.Image = ((System.Drawing.Image)(resources.GetObject("javaButton.Image")));
            this.javaButton.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.javaButton.Name = "javaButton";
            this.javaButton.Size = new System.Drawing.Size(23, 22);
            this.javaButton.Text = "javaButton";
            this.javaButton.Click += new System.EventHandler(this.javaButton_Click);
            // 
            // loadJavaAnalyserButton
            // 
            this.loadJavaAnalyserButton.Name = "loadJavaAnalyserButton";
            this.loadJavaAnalyserButton.Size = new System.Drawing.Size(23, 22);
            // 
            // openFileDialog1
            // 
            this.openFileDialog1.FileName = "openFileDialog1";
            // 
            // backgroundWorker1
            // 
            this.backgroundWorker1.DoWork += new System.ComponentModel.DoWorkEventHandler(this.bw_DoWork);
            this.backgroundWorker1.RunWorkerCompleted += new System.ComponentModel.RunWorkerCompletedEventHandler(this.bw_RunWorkerCompleted);
            // 
            // SudokuForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(634, 587);
            this.Controls.Add(this.toolStrip1);
            this.Controls.Add(this.pictureBox1);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedDialog;
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.Name = "SudokuForm";
            this.Text = "Sudoku";
            this.Load += new System.EventHandler(this.Board_Load);
            ((System.ComponentModel.ISupportInitialize)(this.pictureBox1)).EndInit();
            this.toolStrip1.ResumeLayout(false);
            this.toolStrip1.PerformLayout();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.PictureBox pictureBox1;
        private System.Windows.Forms.ToolStrip toolStrip1;
        private System.Windows.Forms.ToolStripButton loadButton;
        private System.Windows.Forms.OpenFileDialog openFileDialog1;
        private System.Windows.Forms.ToolStripButton hideButton;
        private System.Windows.Forms.ToolStripButton solveButton;
        private System.Windows.Forms.ToolStripButton reportButton;
        private System.Windows.Forms.ToolStripButton undoButton;
        private System.Windows.Forms.ToolStripButton resetButton;
        private System.Windows.Forms.ToolStripButton stopButton;
        private System.ComponentModel.BackgroundWorker backgroundWorker1;
        private System.Windows.Forms.ToolStripButton analyseButton;
        private System.Windows.Forms.ToolStripButton javaButton;
        private System.Windows.Forms.ToolStripButton inputButton;
        private System.Windows.Forms.ToolStripButton loadJavaAnalyserButton;
    }
}

