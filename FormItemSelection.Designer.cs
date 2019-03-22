namespace Rawr
{
	partial class FormItemSelection
	{
		/// <summary>
		/// Required designer variable.
		/// </summary>
		private System.ComponentModel.IContainer components = null;

		#region Windows Form Designer generated code

		/// <summary>
		/// Required method for Designer support - do not modify
		/// the contents of this method with the code editor.
		/// </summary>
		private void InitializeComponent()
		{
			this.components = new System.ComponentModel.Container();
			this.panelMain = new System.Windows.Forms.Panel();
			this.panelLine = new System.Windows.Forms.Panel();
			this.panelItems = new System.Windows.Forms.Panel();
			this.toolStripItemComparison = new System.Windows.Forms.ToolStrip();
			this.toolStripLabel1 = new System.Windows.Forms.ToolStripLabel();
			this.toolStripTextBoxFilter = new System.Windows.Forms.ToolStripTextBox();
			this.toolStripLabel2 = new System.Windows.Forms.ToolStripLabel();
			this.toolStripDropDownButtonSort = new System.Windows.Forms.ToolStripDropDownButton();
			this.overallToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
			this.alphabeticalToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
			this.timerForceActivate = new System.Windows.Forms.Timer(this.components);
			this.panelMain.SuspendLayout();
			this.toolStripItemComparison.SuspendLayout();
			this.SuspendLayout();
			// 
			// panelMain
			// 
			this.panelMain.AutoScroll = true;
			this.panelMain.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
			this.panelMain.Controls.Add(this.panelLine);
			this.panelMain.Controls.Add(this.panelItems);
			this.panelMain.Controls.Add(this.toolStripItemComparison);
			this.panelMain.Dock = System.Windows.Forms.DockStyle.Fill;
			this.panelMain.Location = new System.Drawing.Point(0, 0);
			this.panelMain.Name = "panelMain";
			this.panelMain.Size = new System.Drawing.Size(290, 537);
			this.panelMain.TabIndex = 0;
			// 
			// panelLine
			// 
			this.panelLine.AutoScroll = true;
			this.panelLine.BackColor = System.Drawing.SystemColors.ControlText;
			this.panelLine.Dock = System.Windows.Forms.DockStyle.Top;
			this.panelLine.Location = new System.Drawing.Point(0, 25);
			this.panelLine.Name = "panelLine";
			this.panelLine.Size = new System.Drawing.Size(288, 1);
			this.panelLine.TabIndex = 6;
			// 
			// panelItems
			// 
			this.panelItems.AutoScroll = true;
			this.panelItems.Dock = System.Windows.Forms.DockStyle.Fill;
			this.panelItems.Location = new System.Drawing.Point(0, 25);
			this.panelItems.Name = "panelItems";
			this.panelItems.Size = new System.Drawing.Size(288, 510);
			this.panelItems.TabIndex = 0;
			// 
			// toolStripItemComparison
			// 
			this.toolStripItemComparison.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.toolStripLabel1,
            this.toolStripTextBoxFilter,
            this.toolStripLabel2,
            this.toolStripDropDownButtonSort});
			this.toolStripItemComparison.Location = new System.Drawing.Point(0, 0);
			this.toolStripItemComparison.Name = "toolStripItemComparison";
			this.toolStripItemComparison.Size = new System.Drawing.Size(288, 25);
			this.toolStripItemComparison.TabIndex = 5;
			this.toolStripItemComparison.Text = "toolStripItemComparison";
			// 
			// toolStripLabel1
			// 
			this.toolStripLabel1.Name = "toolStripLabel1";
			this.toolStripLabel1.Size = new System.Drawing.Size(36, 22);
			this.toolStripLabel1.Text = "Filter:";
			// 
			// toolStripTextBoxFilter
			// 
			this.toolStripTextBoxFilter.Name = "toolStripTextBoxFilter";
			this.toolStripTextBoxFilter.Size = new System.Drawing.Size(100, 25);
			this.toolStripTextBoxFilter.TextChanged += new System.EventHandler(this.toolStripTextBoxFilter_TextChanged);
			// 
			// toolStripLabel2
			// 
			this.toolStripLabel2.Name = "toolStripLabel2";
			this.toolStripLabel2.Size = new System.Drawing.Size(52, 22);
			this.toolStripLabel2.Text = "       Sort:";
			// 
			// toolStripDropDownButtonSort
			// 
			this.toolStripDropDownButtonSort.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Text;
			this.toolStripDropDownButtonSort.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.overallToolStripMenuItem,
            this.alphabeticalToolStripMenuItem});
			this.toolStripDropDownButtonSort.ImageTransparentColor = System.Drawing.Color.Magenta;
			this.toolStripDropDownButtonSort.Name = "toolStripDropDownButtonSort";
			this.toolStripDropDownButtonSort.Size = new System.Drawing.Size(57, 22);
			this.toolStripDropDownButtonSort.Text = "Overall";
			// 
			// overallToolStripMenuItem
			// 
			this.overallToolStripMenuItem.Checked = true;
			this.overallToolStripMenuItem.CheckState = System.Windows.Forms.CheckState.Checked;
			this.overallToolStripMenuItem.Name = "overallToolStripMenuItem";
			this.overallToolStripMenuItem.Size = new System.Drawing.Size(152, 22);
			this.overallToolStripMenuItem.Text = "Overall";
			this.overallToolStripMenuItem.Click += new System.EventHandler(this.sortToolStripMenuItem_Click);
			// 
			// alphabeticalToolStripMenuItem
			// 
			this.alphabeticalToolStripMenuItem.Name = "alphabeticalToolStripMenuItem";
			this.alphabeticalToolStripMenuItem.Size = new System.Drawing.Size(152, 22);
			this.alphabeticalToolStripMenuItem.Text = "Alphabetical";
			this.alphabeticalToolStripMenuItem.Click += new System.EventHandler(this.sortToolStripMenuItem_Click);
			// 
			// timerForceActivate
			// 
			this.timerForceActivate.Tick += new System.EventHandler(this.timerForceActivate_Tick);
			// 
			// FormItemSelection
			// 
			this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
			this.ClientSize = new System.Drawing.Size(290, 537);
			this.ControlBox = false;
			this.Controls.Add(this.panelMain);
			this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.None;
			this.MaximizeBox = false;
			this.MinimizeBox = false;
			this.Name = "FormItemSelection";
			this.ShowIcon = false;
			this.ShowInTaskbar = false;
			this.StartPosition = System.Windows.Forms.FormStartPosition.Manual;
			this.Text = "FormItemSelection";
			this.Deactivate += new System.EventHandler(this.CheckToHide);
			this.Leave += new System.EventHandler(this.CheckToHide);
			this.panelMain.ResumeLayout(false);
			this.panelMain.PerformLayout();
			this.toolStripItemComparison.ResumeLayout(false);
			this.toolStripItemComparison.PerformLayout();
			this.ResumeLayout(false);

		}

		#endregion

		private System.Windows.Forms.Panel panelMain;
		private System.Windows.Forms.Timer timerForceActivate;
		private System.Windows.Forms.Panel panelItems;
		private System.Windows.Forms.ToolStrip toolStripItemComparison;
		private System.Windows.Forms.ToolStripLabel toolStripLabel1;
		private System.Windows.Forms.ToolStripTextBox toolStripTextBoxFilter;
		private System.Windows.Forms.ToolStripLabel toolStripLabel2;
		private System.Windows.Forms.ToolStripDropDownButton toolStripDropDownButtonSort;
		private System.Windows.Forms.ToolStripMenuItem overallToolStripMenuItem;
		private System.Windows.Forms.ToolStripMenuItem alphabeticalToolStripMenuItem;
		private System.Windows.Forms.Panel panelLine;
	}
}