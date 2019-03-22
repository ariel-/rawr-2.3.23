namespace Rawr
{
    partial class FormEnterId
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
            this.buttonOK = new System.Windows.Forms.Button();
            this.buttonCancel = new System.Windows.Forms.Button();
            this.textItemId = new System.Windows.Forms.TextBox();
            this.lblTextAddItemID = new System.Windows.Forms.Label();
            this.cbArmory = new System.Windows.Forms.CheckBox();
            this.cbWowhead = new System.Windows.Forms.CheckBox();
            this.SuspendLayout();
            // 
            // buttonOK
            // 
            this.buttonOK.DialogResult = System.Windows.Forms.DialogResult.OK;
            this.buttonOK.Location = new System.Drawing.Point(172, 90);
            this.buttonOK.Name = "buttonOK";
            this.buttonOK.Size = new System.Drawing.Size(75, 23);
            this.buttonOK.TabIndex = 0;
            this.buttonOK.Text = "OK";
            this.buttonOK.UseVisualStyleBackColor = true;
            // 
            // buttonCancel
            // 
            this.buttonCancel.DialogResult = System.Windows.Forms.DialogResult.Cancel;
            this.buttonCancel.Location = new System.Drawing.Point(91, 90);
            this.buttonCancel.Name = "buttonCancel";
            this.buttonCancel.Size = new System.Drawing.Size(75, 23);
            this.buttonCancel.TabIndex = 5;
            this.buttonCancel.Text = "Cancel";
            this.buttonCancel.UseVisualStyleBackColor = true;
            // 
            // textItemId
            // 
            this.textItemId.Location = new System.Drawing.Point(12, 39);
            this.textItemId.Name = "textItemId";
            this.textItemId.Size = new System.Drawing.Size(235, 20);
            this.textItemId.TabIndex = 2;
            // 
            // lblTextAddItemID
            // 
            this.lblTextAddItemID.Location = new System.Drawing.Point(12, 10);
            this.lblTextAddItemID.Name = "lblTextAddItemID";
            this.lblTextAddItemID.Size = new System.Drawing.Size(235, 26);
            this.lblTextAddItemID.TabIndex = 1;
            this.lblTextAddItemID.Text = "Enter the Item ID, Item Name, or Database Link (Wowhead, Thottbot):";
            this.lblTextAddItemID.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // cbArmory
            // 
            this.cbArmory.AutoSize = true;
            this.cbArmory.Checked = true;
            this.cbArmory.CheckState = System.Windows.Forms.CheckState.Checked;
            this.cbArmory.Location = new System.Drawing.Point(49, 66);
            this.cbArmory.Name = "cbArmory";
            this.cbArmory.Size = new System.Drawing.Size(58, 17);
            this.cbArmory.TabIndex = 3;
            this.cbArmory.Text = "Armory";
            this.cbArmory.UseVisualStyleBackColor = true;
            // 
            // cbWowhead
            // 
            this.cbWowhead.AutoSize = true;
            this.cbWowhead.Checked = true;
            this.cbWowhead.CheckState = System.Windows.Forms.CheckState.Checked;
            this.cbWowhead.Location = new System.Drawing.Point(135, 66);
            this.cbWowhead.Name = "cbWowhead";
            this.cbWowhead.Size = new System.Drawing.Size(75, 17);
            this.cbWowhead.TabIndex = 4;
            this.cbWowhead.Text = "Wowhead";
            this.cbWowhead.UseVisualStyleBackColor = true;
            // 
            // FormEnterId
            // 
            this.AcceptButton = this.buttonOK;
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(259, 122);
            this.Controls.Add(this.textItemId);
            this.Controls.Add(this.cbWowhead);
            this.Controls.Add(this.cbArmory);
            this.Controls.Add(this.lblTextAddItemID);
            this.Controls.Add(this.buttonCancel);
            this.Controls.Add(this.buttonOK);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedToolWindow;
            this.Name = "FormEnterId";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterParent;
            this.Text = "Add item";
            this.Load += new System.EventHandler(this.FormEnterId_Load);
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Button buttonOK;
        private System.Windows.Forms.Button buttonCancel;
        private System.Windows.Forms.TextBox textItemId;
        private System.Windows.Forms.Label lblTextAddItemID;
        private System.Windows.Forms.CheckBox cbArmory;
        private System.Windows.Forms.CheckBox cbWowhead;
    }
}