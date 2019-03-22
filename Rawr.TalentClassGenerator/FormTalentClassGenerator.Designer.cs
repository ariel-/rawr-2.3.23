namespace Rawr.TalentClassGenerator
{
	partial class FormTalentClassGenerator
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
            this.label1 = new System.Windows.Forms.Label();
            this.textBoxUrl = new System.Windows.Forms.TextBox();
            this.textBoxCode = new System.Windows.Forms.TextBox();
            this.buttonGenerateCode = new System.Windows.Forms.Button();
            this.SuspendLayout();
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(12, 9);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(408, 13);
            this.label1.TabIndex = 0;
            this.label1.Text = "URL of the talents xml file to parse and generate strongly typed talent class bas" +
                "ed on:";
            // 
            // textBoxUrl
            // 
            this.textBoxUrl.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left)
                        | System.Windows.Forms.AnchorStyles.Right)));
            this.textBoxUrl.Location = new System.Drawing.Point(12, 25);
            this.textBoxUrl.Name = "textBoxUrl";
            this.textBoxUrl.Size = new System.Drawing.Size(498, 20);
            this.textBoxUrl.TabIndex = 0;
            this.textBoxUrl.Text = "http://www.wowarmory.com/talent-tree.xml?cid={0}";
            // 
            // textBoxCode
            // 
            this.textBoxCode.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom)
                        | System.Windows.Forms.AnchorStyles.Left)
                        | System.Windows.Forms.AnchorStyles.Right)));
            this.textBoxCode.Location = new System.Drawing.Point(12, 80);
            this.textBoxCode.Multiline = true;
            this.textBoxCode.Name = "textBoxCode";
            this.textBoxCode.ScrollBars = System.Windows.Forms.ScrollBars.Both;
            this.textBoxCode.Size = new System.Drawing.Size(498, 383);
            this.textBoxCode.TabIndex = 3;
            // 
            // buttonGenerateCode
            // 
            this.buttonGenerateCode.Location = new System.Drawing.Point(12, 51);
            this.buttonGenerateCode.Name = "buttonGenerateCode";
            this.buttonGenerateCode.Size = new System.Drawing.Size(87, 23);
            this.buttonGenerateCode.TabIndex = 1;
            this.buttonGenerateCode.Text = "Generate Code";
            this.buttonGenerateCode.UseVisualStyleBackColor = true;
            this.buttonGenerateCode.Click += new System.EventHandler(this.buttonGenerateCode_Click);
            // 
            // FormTalentClassGenerator
            // 
            this.AcceptButton = this.buttonGenerateCode;
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(522, 475);
            this.Controls.Add(this.buttonGenerateCode);
            this.Controls.Add(this.textBoxCode);
            this.Controls.Add(this.textBoxUrl);
            this.Controls.Add(this.label1);
            this.Name = "FormTalentClassGenerator";
            this.Text = "Talent Class Generator";
            this.ResumeLayout(false);
            this.PerformLayout();

		}

		#endregion

		private System.Windows.Forms.Label label1;
		private System.Windows.Forms.TextBox textBoxUrl;
		private System.Windows.Forms.TextBox textBoxCode;
		private System.Windows.Forms.Button buttonGenerateCode;


	}
}

