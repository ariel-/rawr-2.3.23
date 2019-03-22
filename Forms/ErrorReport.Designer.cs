namespace Rawr.Forms
{
    partial class ErrorReport
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
            this.CloseButton = new System.Windows.Forms.Button();
            this.label1 = new System.Windows.Forms.Label();
            this.label2 = new System.Windows.Forms.Label();
            this.StepName = new System.Windows.Forms.TextBox();
            this.Description = new System.Windows.Forms.TextBox();
            this.groupBox1 = new System.Windows.Forms.GroupBox();
            this.StepExceptionMessage = new System.Windows.Forms.TextBox();
            this.label3 = new System.Windows.Forms.Label();
            this.groupBox2 = new System.Windows.Forms.GroupBox();
            this.WebClientExceptionMessage = new System.Windows.Forms.TextBox();
            this.label4 = new System.Windows.Forms.Label();
            this.CopyToClipboard = new System.Windows.Forms.Button();
            this.groupBox1.SuspendLayout();
            this.groupBox2.SuspendLayout();
            this.SuspendLayout();
            // 
            // CloseButton
            // 
            this.CloseButton.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.CloseButton.DialogResult = System.Windows.Forms.DialogResult.OK;
            this.CloseButton.Location = new System.Drawing.Point(287, 234);
            this.CloseButton.Name = "CloseButton";
            this.CloseButton.Size = new System.Drawing.Size(75, 23);
            this.CloseButton.TabIndex = 0;
            this.CloseButton.Text = "Close";
            this.CloseButton.UseVisualStyleBackColor = true;
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(15, 22);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(63, 13);
            this.label1.TabIndex = 1;
            this.label1.Text = "Step Name:";
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Location = new System.Drawing.Point(15, 48);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(78, 13);
            this.label2.TabIndex = 3;
            this.label2.Text = "Error Message:";
            // 
            // StepName
            // 
            this.StepName.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left)
                        | System.Windows.Forms.AnchorStyles.Right)));
            this.StepName.BackColor = System.Drawing.SystemColors.ActiveCaptionText;
            this.StepName.Enabled = false;
            this.StepName.Location = new System.Drawing.Point(99, 19);
            this.StepName.Name = "StepName";
            this.StepName.ReadOnly = true;
            this.StepName.Size = new System.Drawing.Size(245, 20);
            this.StepName.TabIndex = 4;
            // 
            // Description
            // 
            this.Description.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left)
                        | System.Windows.Forms.AnchorStyles.Right)));
            this.Description.BackColor = System.Drawing.SystemColors.ActiveCaptionText;
            this.Description.Enabled = false;
            this.Description.Location = new System.Drawing.Point(99, 45);
            this.Description.Name = "Description";
            this.Description.ReadOnly = true;
            this.Description.Size = new System.Drawing.Size(245, 20);
            this.Description.TabIndex = 5;
            // 
            // groupBox1
            // 
            this.groupBox1.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left)
                        | System.Windows.Forms.AnchorStyles.Right)));
            this.groupBox1.Controls.Add(this.StepExceptionMessage);
            this.groupBox1.Controls.Add(this.label3);
            this.groupBox1.Controls.Add(this.Description);
            this.groupBox1.Controls.Add(this.label1);
            this.groupBox1.Controls.Add(this.StepName);
            this.groupBox1.Controls.Add(this.label2);
            this.groupBox1.Location = new System.Drawing.Point(12, 12);
            this.groupBox1.Name = "groupBox1";
            this.groupBox1.Size = new System.Drawing.Size(350, 126);
            this.groupBox1.TabIndex = 6;
            this.groupBox1.TabStop = false;
            this.groupBox1.Text = "Step Specific";
            // 
            // StepExceptionMessage
            // 
            this.StepExceptionMessage.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left)
                        | System.Windows.Forms.AnchorStyles.Right)));
            this.StepExceptionMessage.BackColor = System.Drawing.SystemColors.ActiveCaptionText;
            this.StepExceptionMessage.Enabled = false;
            this.StepExceptionMessage.Location = new System.Drawing.Point(99, 72);
            this.StepExceptionMessage.Multiline = true;
            this.StepExceptionMessage.Name = "StepExceptionMessage";
            this.StepExceptionMessage.ReadOnly = true;
            this.StepExceptionMessage.Size = new System.Drawing.Size(245, 39);
            this.StepExceptionMessage.TabIndex = 7;
            // 
            // label3
            // 
            this.label3.AutoSize = true;
            this.label3.Location = new System.Drawing.Point(15, 75);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(57, 13);
            this.label3.TabIndex = 6;
            this.label3.Text = "Exception:";
            // 
            // groupBox2
            // 
            this.groupBox2.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left)
                        | System.Windows.Forms.AnchorStyles.Right)));
            this.groupBox2.Controls.Add(this.WebClientExceptionMessage);
            this.groupBox2.Controls.Add(this.label4);
            this.groupBox2.Location = new System.Drawing.Point(12, 144);
            this.groupBox2.Name = "groupBox2";
            this.groupBox2.Size = new System.Drawing.Size(350, 84);
            this.groupBox2.TabIndex = 7;
            this.groupBox2.TabStop = false;
            this.groupBox2.Text = "Web Client";
            // 
            // WebClientExceptionMessage
            // 
            this.WebClientExceptionMessage.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left)
                        | System.Windows.Forms.AnchorStyles.Right)));
            this.WebClientExceptionMessage.BackColor = System.Drawing.SystemColors.ActiveCaptionText;
            this.WebClientExceptionMessage.Enabled = false;
            this.WebClientExceptionMessage.Location = new System.Drawing.Point(99, 19);
            this.WebClientExceptionMessage.Multiline = true;
            this.WebClientExceptionMessage.Name = "WebClientExceptionMessage";
            this.WebClientExceptionMessage.ReadOnly = true;
            this.WebClientExceptionMessage.Size = new System.Drawing.Size(245, 41);
            this.WebClientExceptionMessage.TabIndex = 8;
            // 
            // label4
            // 
            this.label4.AutoSize = true;
            this.label4.Location = new System.Drawing.Point(15, 22);
            this.label4.Name = "label4";
            this.label4.Size = new System.Drawing.Size(57, 13);
            this.label4.TabIndex = 0;
            this.label4.Text = "Exception:";
            // 
            // CopyToClipboard
            // 
            this.CopyToClipboard.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this.CopyToClipboard.Location = new System.Drawing.Point(12, 234);
            this.CopyToClipboard.Name = "CopyToClipboard";
            this.CopyToClipboard.Size = new System.Drawing.Size(110, 23);
            this.CopyToClipboard.TabIndex = 8;
            this.CopyToClipboard.Text = "Copy To Clipboard";
            this.CopyToClipboard.UseVisualStyleBackColor = true;
            this.CopyToClipboard.Click += new System.EventHandler(this.CopyToClipboard_Click);
            // 
            // ErrorReport
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(374, 269);
            this.Controls.Add(this.CopyToClipboard);
            this.Controls.Add(this.groupBox2);
            this.Controls.Add(this.groupBox1);
            this.Controls.Add(this.CloseButton);
            this.MinimumSize = new System.Drawing.Size(382, 303);
            this.Name = "ErrorReport";
            this.ShowIcon = false;
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterParent;
            this.Text = "Error Report";
            this.Load += new System.EventHandler(this.ErrorReport_Load);
            this.groupBox1.ResumeLayout(false);
            this.groupBox1.PerformLayout();
            this.groupBox2.ResumeLayout(false);
            this.groupBox2.PerformLayout();
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.Button CloseButton;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.TextBox StepName;
        private System.Windows.Forms.TextBox Description;
        private System.Windows.Forms.GroupBox groupBox1;
        private System.Windows.Forms.TextBox StepExceptionMessage;
        private System.Windows.Forms.Label label3;
        private System.Windows.Forms.GroupBox groupBox2;
        private System.Windows.Forms.TextBox WebClientExceptionMessage;
        private System.Windows.Forms.Label label4;
        private System.Windows.Forms.Button CopyToClipboard;
    }
}