namespace Rawr
{
	partial class ComparisonGraph
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
			this.SuspendLayout();
			// 
			// ComparisonGraph
			// 
			this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 12F);
			this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
			this.BackColor = System.Drawing.Color.White;
			this.Font = new System.Drawing.Font("Verdana", 7F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
			this.Margin = new System.Windows.Forms.Padding(4);
			this.Name = "ComparisonGraph";
			this.Size = new System.Drawing.Size(150, 138);
			this.MouseLeave += new System.EventHandler(this.ComparisonGraph_MouseLeave);
			this.MouseMove += new System.Windows.Forms.MouseEventHandler(this.ComparisonGraph_MouseMove);
			this.MouseClick += new System.Windows.Forms.MouseEventHandler(this.ComparisonGraph_MouseClick);
			this.Resize += new System.EventHandler(this.ComparisonGraph_Resize);
			this.ResumeLayout(false);

		}

		#endregion
	}
}
