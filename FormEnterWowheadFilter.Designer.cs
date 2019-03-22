namespace Rawr
{
    partial class FormEnterWowheadFilter
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
            this.textWowheadFilter = new System.Windows.Forms.TextBox();
            this.label1 = new System.Windows.Forms.Label();
            this.lblTextAddItemID = new System.Windows.Forms.Label();
            this.buttonCancel = new System.Windows.Forms.Button();
            this.buttonOK = new System.Windows.Forms.Button();
            this.SuspendLayout();
            // 
            // textWowheadFilter
            // 
            this.textWowheadFilter.Location = new System.Drawing.Point(12, 38);
            this.textWowheadFilter.Name = "textWowheadFilter";
            this.textWowheadFilter.Size = new System.Drawing.Size(296, 20);
            this.textWowheadFilter.TabIndex = 3;
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(12, 22);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(157, 13);
            this.label1.TabIndex = 2;
            this.label1.Text = "qu=4:5;sl=16;cr=82;crs=1;crv=0";
            // 
            // lblTextAddItemID
            // 
            this.lblTextAddItemID.AutoSize = true;
            this.lblTextAddItemID.Location = new System.Drawing.Point(12, 9);
            this.lblTextAddItemID.Name = "lblTextAddItemID";
            this.lblTextAddItemID.Size = new System.Drawing.Size(181, 13);
            this.lblTextAddItemID.TabIndex = 1;
            this.lblTextAddItemID.Text = "Enter the wowhead filter, for example";
            // 
            // buttonCancel
            // 
            this.buttonCancel.DialogResult = System.Windows.Forms.DialogResult.Cancel;
            this.buttonCancel.Location = new System.Drawing.Point(152, 64);
            this.buttonCancel.Name = "buttonCancel";
            this.buttonCancel.Size = new System.Drawing.Size(75, 23);
            this.buttonCancel.TabIndex = 4;
            this.buttonCancel.Text = "Cancel";
            this.buttonCancel.UseVisualStyleBackColor = true;
            // 
            // buttonOK
            // 
            this.buttonOK.DialogResult = System.Windows.Forms.DialogResult.OK;
            this.buttonOK.Location = new System.Drawing.Point(233, 64);
            this.buttonOK.Name = "buttonOK";
            this.buttonOK.Size = new System.Drawing.Size(75, 23);
            this.buttonOK.TabIndex = 0;
            this.buttonOK.Text = "OK";
            this.buttonOK.UseVisualStyleBackColor = true;
            // 
            // FormEnterWowheadFilter
            // 
            this.AcceptButton = this.buttonOK;
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.CancelButton = this.buttonCancel;
            this.ClientSize = new System.Drawing.Size(320, 99);
            this.Controls.Add(this.textWowheadFilter);
            this.Controls.Add(this.label1);
            this.Controls.Add(this.lblTextAddItemID);
            this.Controls.Add(this.buttonCancel);
            this.Controls.Add(this.buttonOK);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedToolWindow;
            this.Name = "FormEnterWowheadFilter";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterParent;
            this.Text = "Import From Wowhead Filter";
            this.Load += new System.EventHandler(this.FormEnterId_Load);
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.TextBox textWowheadFilter;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.Label lblTextAddItemID;
        private System.Windows.Forms.Button buttonCancel;
        private System.Windows.Forms.Button buttonOK;

    }
}