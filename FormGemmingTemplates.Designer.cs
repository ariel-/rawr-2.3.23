namespace Rawr
{
    partial class FormGemmingTemplates
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
            this.buttonOK = new System.Windows.Forms.Button();
            this.panelGemmingTemplates = new System.Windows.Forms.Panel();
            this.label2 = new System.Windows.Forms.Label();
            this.label3 = new System.Windows.Forms.Label();
            this.label4 = new System.Windows.Forms.Label();
            this.label5 = new System.Windows.Forms.Label();
            this.label6 = new System.Windows.Forms.Label();
            this.label7 = new System.Windows.Forms.Label();
            this.numericUpDownCountGemmingsShown = new System.Windows.Forms.NumericUpDown();
            this.label8 = new System.Windows.Forms.Label();
            this.labelCountGemmingsShownDescription = new System.Windows.Forms.Label();
            ((System.ComponentModel.ISupportInitialize)(this.numericUpDownCountGemmingsShown)).BeginInit();
            this.SuspendLayout();
            // 
            // label1
            // 
            this.label1.Location = new System.Drawing.Point(12, 15);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(321, 39);
            this.label1.TabIndex = 0;
            this.label1.Text = "Rawr will build gemmings of each known item, based on enabled gemming templates. " +
                "The following gemming templates are defined for the current model:";
            // 
            // buttonOK
            // 
            this.buttonOK.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.buttonOK.DialogResult = System.Windows.Forms.DialogResult.OK;
            this.buttonOK.Location = new System.Drawing.Point(268, 483);
            this.buttonOK.Name = "buttonOK";
            this.buttonOK.Size = new System.Drawing.Size(75, 23);
            this.buttonOK.TabIndex = 11;
            this.buttonOK.Text = "OK";
            this.buttonOK.UseVisualStyleBackColor = true;
            this.buttonOK.Click += new System.EventHandler(this.buttonOK_Click);
            // 
            // panelGemmingTemplates
            // 
            this.panelGemmingTemplates.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom)
                        | System.Windows.Forms.AnchorStyles.Left)
                        | System.Windows.Forms.AnchorStyles.Right)));
            this.panelGemmingTemplates.AutoScroll = true;
            this.panelGemmingTemplates.BackColor = System.Drawing.SystemColors.Window;
            this.panelGemmingTemplates.BorderStyle = System.Windows.Forms.BorderStyle.Fixed3D;
            this.panelGemmingTemplates.Location = new System.Drawing.Point(12, 74);
            this.panelGemmingTemplates.Name = "panelGemmingTemplates";
            this.panelGemmingTemplates.Size = new System.Drawing.Size(331, 338);
            this.panelGemmingTemplates.TabIndex = 7;
            // 
            // label2
            // 
            this.label2.Font = new System.Drawing.Font("Microsoft Sans Serif", 6.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label2.Location = new System.Drawing.Point(60, 59);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(42, 12);
            this.label2.TabIndex = 1;
            this.label2.Text = "Enabled";
            this.label2.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // label3
            // 
            this.label3.Font = new System.Drawing.Font("Microsoft Sans Serif", 6.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label3.Location = new System.Drawing.Point(100, 59);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(42, 12);
            this.label3.TabIndex = 2;
            this.label3.Text = "Red";
            this.label3.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // label4
            // 
            this.label4.Font = new System.Drawing.Font("Microsoft Sans Serif", 6.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label4.Location = new System.Drawing.Point(142, 59);
            this.label4.Name = "label4";
            this.label4.Size = new System.Drawing.Size(42, 12);
            this.label4.TabIndex = 3;
            this.label4.Text = "Yellow";
            this.label4.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // label5
            // 
            this.label5.Font = new System.Drawing.Font("Microsoft Sans Serif", 6.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label5.Location = new System.Drawing.Point(184, 59);
            this.label5.Name = "label5";
            this.label5.Size = new System.Drawing.Size(42, 12);
            this.label5.TabIndex = 4;
            this.label5.Text = "Blue";
            this.label5.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // label6
            // 
            this.label6.Font = new System.Drawing.Font("Microsoft Sans Serif", 6.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label6.Location = new System.Drawing.Point(225, 59);
            this.label6.Name = "label6";
            this.label6.Size = new System.Drawing.Size(44, 12);
            this.label6.TabIndex = 5;
            this.label6.Text = "Prismatic";
            this.label6.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // label7
            // 
            this.label7.Font = new System.Drawing.Font("Microsoft Sans Serif", 6.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label7.Location = new System.Drawing.Point(268, 59);
            this.label7.Name = "label7";
            this.label7.Size = new System.Drawing.Size(42, 12);
            this.label7.TabIndex = 6;
            this.label7.Text = "Meta";
            this.label7.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // numericUpDownCountGemmingsShown
            // 
            this.numericUpDownCountGemmingsShown.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this.numericUpDownCountGemmingsShown.Location = new System.Drawing.Point(144, 418);
            this.numericUpDownCountGemmingsShown.Minimum = new decimal(new int[] {
            1,
            0,
            0,
            0});
            this.numericUpDownCountGemmingsShown.Name = "numericUpDownCountGemmingsShown";
            this.numericUpDownCountGemmingsShown.Size = new System.Drawing.Size(51, 20);
            this.numericUpDownCountGemmingsShown.TabIndex = 9;
            this.numericUpDownCountGemmingsShown.Value = new decimal(new int[] {
            3,
            0,
            0,
            0});
            this.numericUpDownCountGemmingsShown.ValueChanged += new System.EventHandler(this.numericUpDownCountGemmingsShown_ValueChanged);
            // 
            // label8
            // 
            this.label8.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this.label8.AutoSize = true;
            this.label8.Location = new System.Drawing.Point(12, 420);
            this.label8.Name = "label8";
            this.label8.Size = new System.Drawing.Size(126, 13);
            this.label8.TabIndex = 8;
            this.label8.Text = "Top Gemmings to Show: ";
            // 
            // labelCountGemmingsShownDescription
            // 
            this.labelCountGemmingsShownDescription.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this.labelCountGemmingsShownDescription.Location = new System.Drawing.Point(12, 441);
            this.labelCountGemmingsShownDescription.Name = "labelCountGemmingsShownDescription";
            this.labelCountGemmingsShownDescription.Size = new System.Drawing.Size(321, 39);
            this.labelCountGemmingsShownDescription.TabIndex = 10;
            this.labelCountGemmingsShownDescription.Tag = "Rawr will show the top {0} gemmings for an item, plus any equipped or custom gemm" +
                "ings, if not already included in the top {0}.";
            this.labelCountGemmingsShownDescription.Text = "Rawr will show the top {0} gemmings for an item, plus any equipped or custom gemm" +
                "ings, if not already included in the top {0}.";
            // 
            // FormGemmingTemplates
            // 
            this.AcceptButton = this.buttonOK;
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(355, 518);
            this.Controls.Add(this.numericUpDownCountGemmingsShown);
            this.Controls.Add(this.panelGemmingTemplates);
            this.Controls.Add(this.buttonOK);
            this.Controls.Add(this.label7);
            this.Controls.Add(this.label6);
            this.Controls.Add(this.label5);
            this.Controls.Add(this.label4);
            this.Controls.Add(this.label3);
            this.Controls.Add(this.label2);
            this.Controls.Add(this.labelCountGemmingsShownDescription);
            this.Controls.Add(this.label8);
            this.Controls.Add(this.label1);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedToolWindow;
            this.MaximizeBox = false;
            this.MaximumSize = new System.Drawing.Size(361, 2048);
            this.MinimizeBox = false;
            this.MinimumSize = new System.Drawing.Size(361, 300);
            this.Name = "FormGemmingTemplates";
            this.ShowIcon = false;
            this.ShowInTaskbar = false;
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterParent;
            this.Text = "Rawr Gemming Templates";
            ((System.ComponentModel.ISupportInitialize)(this.numericUpDownCountGemmingsShown)).EndInit();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Label label1;
		private System.Windows.Forms.Button buttonOK;
		private System.Windows.Forms.Panel panelGemmingTemplates;
		private System.Windows.Forms.Label label2;
		private System.Windows.Forms.Label label3;
		private System.Windows.Forms.Label label4;
		private System.Windows.Forms.Label label5;
		private System.Windows.Forms.Label label6;
		private System.Windows.Forms.Label label7;
		private System.Windows.Forms.NumericUpDown numericUpDownCountGemmingsShown;
		private System.Windows.Forms.Label label8;
		private System.Windows.Forms.Label labelCountGemmingsShownDescription;
    }
}