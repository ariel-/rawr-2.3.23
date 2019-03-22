namespace Rawr.Mage
{
    partial class CycleAnalyzer
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
            this.statusStrip1 = new System.Windows.Forms.StatusStrip();
            this.statusProgressBar = new System.Windows.Forms.ToolStripProgressBar();
            this.statusLabel = new System.Windows.Forms.ToolStripStatusLabel();
            this.backgroundWorker = new System.ComponentModel.BackgroundWorker();
            this.textBoxDescription = new System.Windows.Forms.TextBox();
            this.label1 = new System.Windows.Forms.Label();
            this.textBoxControlString = new System.Windows.Forms.TextBox();
            this.textBoxResult = new System.Windows.Forms.TextBox();
            this.buttonCalculate = new System.Windows.Forms.Button();
            this.buttonOptimal = new System.Windows.Forms.Button();
            this.comboBoxCycleGenerator = new System.Windows.Forms.ComboBox();
            this.statusStrip1.SuspendLayout();
            this.SuspendLayout();
            // 
            // statusStrip1
            // 
            this.statusStrip1.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.statusProgressBar,
            this.statusLabel});
            this.statusStrip1.Location = new System.Drawing.Point(0, 544);
            this.statusStrip1.Name = "statusStrip1";
            this.statusStrip1.Size = new System.Drawing.Size(520, 22);
            this.statusStrip1.TabIndex = 0;
            this.statusStrip1.Text = "statusStrip1";
            // 
            // statusProgressBar
            // 
            this.statusProgressBar.Name = "statusProgressBar";
            this.statusProgressBar.Size = new System.Drawing.Size(200, 16);
            // 
            // statusLabel
            // 
            this.statusLabel.Name = "statusLabel";
            this.statusLabel.Size = new System.Drawing.Size(0, 17);
            // 
            // backgroundWorker
            // 
            this.backgroundWorker.WorkerReportsProgress = true;
            this.backgroundWorker.WorkerSupportsCancellation = true;
            this.backgroundWorker.DoWork += new System.ComponentModel.DoWorkEventHandler(this.backgroundWorker_DoWork);
            this.backgroundWorker.ProgressChanged += new System.ComponentModel.ProgressChangedEventHandler(this.backgroundWorker_ProgressChanged);
            this.backgroundWorker.RunWorkerCompleted += new System.ComponentModel.RunWorkerCompletedEventHandler(this.backgroundWorker_RunWorkerCompleted);
            // 
            // textBoxDescription
            // 
            this.textBoxDescription.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom)
                        | System.Windows.Forms.AnchorStyles.Left)
                        | System.Windows.Forms.AnchorStyles.Right)));
            this.textBoxDescription.BorderStyle = System.Windows.Forms.BorderStyle.None;
            this.textBoxDescription.Location = new System.Drawing.Point(12, 34);
            this.textBoxDescription.Multiline = true;
            this.textBoxDescription.Name = "textBoxDescription";
            this.textBoxDescription.ReadOnly = true;
            this.textBoxDescription.ScrollBars = System.Windows.Forms.ScrollBars.Vertical;
            this.textBoxDescription.Size = new System.Drawing.Size(492, 221);
            this.textBoxDescription.TabIndex = 1;
            this.textBoxDescription.TabStop = false;
            // 
            // label1
            // 
            this.label1.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(12, 262);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(73, 13);
            this.label1.TabIndex = 2;
            this.label1.Text = "Control String:";
            // 
            // textBoxControlString
            // 
            this.textBoxControlString.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)
                        | System.Windows.Forms.AnchorStyles.Right)));
            this.textBoxControlString.Location = new System.Drawing.Point(92, 262);
            this.textBoxControlString.Name = "textBoxControlString";
            this.textBoxControlString.Size = new System.Drawing.Size(250, 20);
            this.textBoxControlString.TabIndex = 0;
            // 
            // textBoxResult
            // 
            this.textBoxResult.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)
                        | System.Windows.Forms.AnchorStyles.Right)));
            this.textBoxResult.BorderStyle = System.Windows.Forms.BorderStyle.None;
            this.textBoxResult.Location = new System.Drawing.Point(12, 288);
            this.textBoxResult.Multiline = true;
            this.textBoxResult.Name = "textBoxResult";
            this.textBoxResult.ReadOnly = true;
            this.textBoxResult.ScrollBars = System.Windows.Forms.ScrollBars.Vertical;
            this.textBoxResult.Size = new System.Drawing.Size(492, 241);
            this.textBoxResult.TabIndex = 4;
            this.textBoxResult.TabStop = false;
            // 
            // buttonCalculate
            // 
            this.buttonCalculate.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.buttonCalculate.Location = new System.Drawing.Point(348, 262);
            this.buttonCalculate.Name = "buttonCalculate";
            this.buttonCalculate.Size = new System.Drawing.Size(75, 23);
            this.buttonCalculate.TabIndex = 1;
            this.buttonCalculate.Text = "Calculate";
            this.buttonCalculate.UseVisualStyleBackColor = true;
            this.buttonCalculate.Click += new System.EventHandler(this.buttonCalculate_Click);
            // 
            // buttonOptimal
            // 
            this.buttonOptimal.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.buttonOptimal.Location = new System.Drawing.Point(429, 262);
            this.buttonOptimal.Name = "buttonOptimal";
            this.buttonOptimal.Size = new System.Drawing.Size(75, 23);
            this.buttonOptimal.TabIndex = 2;
            this.buttonOptimal.Text = "Optimal";
            this.buttonOptimal.UseVisualStyleBackColor = true;
            this.buttonOptimal.Click += new System.EventHandler(this.buttonOptimal_Click);
            // 
            // comboBoxCycleGenerator
            // 
            this.comboBoxCycleGenerator.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left)
                        | System.Windows.Forms.AnchorStyles.Right)));
            this.comboBoxCycleGenerator.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.comboBoxCycleGenerator.FormattingEnabled = true;
            this.comboBoxCycleGenerator.Items.AddRange(new object[] {
            "Arcane (MB/2T10 duration collapsed)",
            "Arcane (Arcane Power, MB/2T10 duration collapsed)",
            "Arcane (ABar on cooldown only, MB/2T10 duration/ABar cooldown collapsed)",
            "Arcane (no ABar, MB duration collapsed)",
            "Arcane Beta",
            "Arcane Movement (average 1 per 10 sec for 1 sec)",
            "Frost",
            "Frost (no latency combos)",
            "Frost+Deep Freeze",
            "Frost+Deep Freeze (no latency combos)",
            "Frost+Deep Freeze (2T10 duration collapsed)",
            "Frost+Deep Freeze (2T10 duration collapsed, no latency combos)",
            "Frost+Deep Freeze (FOF instants on last charge only, 2T10 duration collapsed)",
            "Frost+Deep Freeze (FOF instants on last charge only, 2T10 duration collapsed, no " +
                "latency combos)",
            "Frost+Deep Freeze (FOF instants on last charge only, 2T10 duration collapsed, 10 " +
                "sec Deep Freeze cooldown)",
            "Frost+Deep Freeze (FOF instants on last charge only, 2T10 duration collapsed, 10 " +
                "sec Deep Freeze cooldown, no latency combos)",
            "Frost+Deep Freeze",
            "FrostFFB+Deep Freeze (no latency combos)",
            "FrostFFB+Deep Freeze (2T10 duration collapsed)",
            "FrostFFB+Deep Freeze (2T10 duration collapsed, no latency combos)",
            "FrostFFB+Deep Freeze (FOF instants on last charge only, 2T10 duration collapsed)",
            "FrostFFB+Deep Freeze (FOF instants on last charge only, 2T10 duration collapsed, " +
                "no latency combos)",
            "FrostFFB+Deep Freeze (FOF instants on last charge only, 2T10 duration collapsed, " +
                "10 sec Deep Freeze cooldown)",
            "FrostFFB+Deep Freeze (FOF instants on last charge only, 2T10 duration collapsed, " +
                "10 sec Deep Freeze cooldown, no latency combos)",
            "Fire"});
            this.comboBoxCycleGenerator.Location = new System.Drawing.Point(12, 7);
            this.comboBoxCycleGenerator.Name = "comboBoxCycleGenerator";
            this.comboBoxCycleGenerator.Size = new System.Drawing.Size(492, 21);
            this.comboBoxCycleGenerator.TabIndex = 5;
            this.comboBoxCycleGenerator.SelectedIndexChanged += new System.EventHandler(this.comboBoxCycleGenerator_SelectedIndexChanged);
            // 
            // CycleAnalyzer
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(520, 566);
            this.Controls.Add(this.comboBoxCycleGenerator);
            this.Controls.Add(this.buttonOptimal);
            this.Controls.Add(this.buttonCalculate);
            this.Controls.Add(this.textBoxResult);
            this.Controls.Add(this.textBoxControlString);
            this.Controls.Add(this.label1);
            this.Controls.Add(this.textBoxDescription);
            this.Controls.Add(this.statusStrip1);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.SizableToolWindow;
            this.Name = "CycleAnalyzer";
            this.Text = "Cycle Analyzer";
            this.FormClosing += new System.Windows.Forms.FormClosingEventHandler(this.CycleAnalyzer_FormClosing);
            this.statusStrip1.ResumeLayout(false);
            this.statusStrip1.PerformLayout();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.StatusStrip statusStrip1;
        private System.Windows.Forms.ToolStripProgressBar statusProgressBar;
        private System.Windows.Forms.ToolStripStatusLabel statusLabel;
        private System.ComponentModel.BackgroundWorker backgroundWorker;
        private System.Windows.Forms.TextBox textBoxDescription;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.TextBox textBoxControlString;
        private System.Windows.Forms.TextBox textBoxResult;
        private System.Windows.Forms.Button buttonCalculate;
        private System.Windows.Forms.Button buttonOptimal;
        private System.Windows.Forms.ComboBox comboBoxCycleGenerator;
    }
}