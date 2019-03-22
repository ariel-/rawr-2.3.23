namespace Rawr
{
	partial class ItemComparison
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

		#region Component Designer generated code

		/// <summary> 
		/// Required method for Designer support - do not modify 
		/// the contents of this method with the code editor.
		/// </summary>
		private void InitializeComponent()
		{
			this.vScrollBarGraph = new System.Windows.Forms.VScrollBar();
			this.comparisonGraph1 = new Rawr.ComparisonGraph();
			this.SuspendLayout();
			// 
			// vScrollBarGraph
			// 
			this.vScrollBarGraph.Dock = System.Windows.Forms.DockStyle.Right;
			this.vScrollBarGraph.LargeChange = 21;
			this.vScrollBarGraph.Location = new System.Drawing.Point(409, 3);
			this.vScrollBarGraph.Maximum = 20;
			this.vScrollBarGraph.Name = "vScrollBarGraph";
			this.vScrollBarGraph.Size = new System.Drawing.Size(17, 437);
			this.vScrollBarGraph.SmallChange = 21;
			this.vScrollBarGraph.TabIndex = 1;
			// 
			// comparisonGraph1
			// 
			this.comparisonGraph1.Dock = System.Windows.Forms.DockStyle.Fill;
			this.comparisonGraph1.BackColor = System.Drawing.Color.White;
			this.comparisonGraph1.ItemCalculations = new Rawr.ComparisonCalculationBase[0];
			this.comparisonGraph1.Location = new System.Drawing.Point(3, 3);
			this.comparisonGraph1.Name = "comparisonGraph1";
			this.comparisonGraph1.ScrollBar = this.vScrollBarGraph;
			this.comparisonGraph1.Size = new System.Drawing.Size(407, 437);
			this.comparisonGraph1.TabIndex = 0;
			// 
			// ItemComparison
			// 
			this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
			this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
			this.BackColor = System.Drawing.Color.Violet;
			this.BorderStyle = System.Windows.Forms.BorderStyle.Fixed3D;
			this.Controls.Add(this.vScrollBarGraph);
			this.Controls.Add(this.comparisonGraph1);
			this.Name = "ItemComparison";
			this.Padding = new System.Windows.Forms.Padding(2, 2, 2, 2);
			this.Size = new System.Drawing.Size(430, 447);
			this.ResumeLayout(false);

		}

		#endregion

		private ComparisonGraph comparisonGraph1;
		private System.Windows.Forms.VScrollBar vScrollBarGraph;

	}
}
