namespace Rawr
{
    partial class FormItemComparison
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
            this.addSetButton = new System.Windows.Forms.Button();
            this.graphScrollBar = new System.Windows.Forms.VScrollBar();
            this.comparisonPanel = new System.Windows.Forms.Panel();
            this.comparisonGraph = new Rawr.ComparisonGraph();
            this.tabControl = new System.Windows.Forms.TabControl();
            this.comparisonPanel.SuspendLayout();
            this.SuspendLayout();
            // 
            // addSetButton
            // 
            this.addSetButton.AutoSize = true;
            this.addSetButton.Location = new System.Drawing.Point(143, 372);
            this.addSetButton.Name = "addSetButton";
            this.addSetButton.Size = new System.Drawing.Size(80, 23);
            this.addSetButton.TabIndex = 1;
            this.addSetButton.Text = "Add New Set";
            this.addSetButton.UseVisualStyleBackColor = true;
            this.addSetButton.Click += new System.EventHandler(this.addSetButton_Click);
            // 
            // graphScrollBar
            // 
            this.graphScrollBar.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom)
                        | System.Windows.Forms.AnchorStyles.Right)));
            this.graphScrollBar.LargeChange = 41;
            this.graphScrollBar.Location = new System.Drawing.Point(433, 0);
            this.graphScrollBar.Maximum = 40;
            this.graphScrollBar.Name = "graphScrollBar";
            this.graphScrollBar.Size = new System.Drawing.Size(17, 483);
            this.graphScrollBar.SmallChange = 32;
            this.graphScrollBar.TabIndex = 1;
            this.graphScrollBar.Visible = false;
            // 
            // comparisonPanel
            // 
            this.comparisonPanel.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom)
                        | System.Windows.Forms.AnchorStyles.Left)
                        | System.Windows.Forms.AnchorStyles.Right)));
            this.comparisonPanel.BackColor = System.Drawing.Color.White;
            this.comparisonPanel.BorderStyle = System.Windows.Forms.BorderStyle.Fixed3D;
            this.comparisonPanel.Controls.Add(this.comparisonGraph);
            this.comparisonPanel.Controls.Add(this.graphScrollBar);
            this.comparisonPanel.Location = new System.Drawing.Point(229, 0);
            this.comparisonPanel.Name = "comparisonPanel";
            this.comparisonPanel.Size = new System.Drawing.Size(454, 487);
            this.comparisonPanel.TabIndex = 2;
            // 
            // comparisonGraph
            // 
            this.comparisonGraph.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom)
                        | System.Windows.Forms.AnchorStyles.Left)
                        | System.Windows.Forms.AnchorStyles.Right)));
            this.comparisonGraph.BackColor = System.Drawing.Color.White;
            this.comparisonGraph.Character = null;
            this.comparisonGraph.CustomRendered = false;
            this.comparisonGraph.CustomRenderedChartName = null;
            this.comparisonGraph.DisplayMode = Rawr.ComparisonGraph.GraphDisplayMode.Subpoints;
            this.comparisonGraph.Font = new System.Drawing.Font("Verdana", 7F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.comparisonGraph.ItemCalculations = new Rawr.ComparisonCalculationBase[0];
            this.comparisonGraph.Location = new System.Drawing.Point(0, 0);
            this.comparisonGraph.Margin = new System.Windows.Forms.Padding(4);
            this.comparisonGraph.Name = "comparisonGraph";
            this.comparisonGraph.RoundValues = true;
            this.comparisonGraph.ScrollBar = this.graphScrollBar;
            this.comparisonGraph.Size = new System.Drawing.Size(432, 480);
            this.comparisonGraph.SlotMap = null;
            this.comparisonGraph.Sort = Rawr.ComparisonGraph.ComparisonSort.Overall;
            this.comparisonGraph.TabIndex = 0;
            // 
            // tabControl
            // 
            this.tabControl.Location = new System.Drawing.Point(3, 3);
            this.tabControl.Name = "tabControl";
            this.tabControl.SelectedIndex = 0;
            this.tabControl.Size = new System.Drawing.Size(220, 363);
            this.tabControl.TabIndex = 0;
            // 
            // FormItemComparison
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(683, 487);
            this.Controls.Add(this.addSetButton);
            this.Controls.Add(this.tabControl);
            this.Controls.Add(this.comparisonPanel);
            this.MinimumSize = new System.Drawing.Size(626, 441);
            this.Name = "FormItemComparison";
            this.ShowIcon = false;
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterParent;
            this.Text = "Item Set Comparison";
            this.comparisonPanel.ResumeLayout(false);
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Button addSetButton;
        private ComparisonGraph comparisonGraph;
        private System.Windows.Forms.VScrollBar graphScrollBar;
        private System.Windows.Forms.Panel comparisonPanel;
        private System.Windows.Forms.TabControl tabControl;
    }
}