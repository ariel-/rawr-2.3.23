namespace Rawr
{
    partial class FormItemInstance
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
            this.gem3Button = new Rawr.ItemButton();
            this.gem2Button = new Rawr.ItemButton();
            this.gem1Button = new Rawr.ItemButton();
            this.LB_Info = new System.Windows.Forms.Label();
            this.itemButtonWithEnchant = new Rawr.ItemButtonWithEnchant();
            this.SuspendLayout();
            // 
            // buttonOK
            // 
            this.buttonOK.DialogResult = System.Windows.Forms.DialogResult.OK;
            this.buttonOK.Location = new System.Drawing.Point(235, 119);
            this.buttonOK.Name = "buttonOK";
            this.buttonOK.Size = new System.Drawing.Size(75, 23);
            this.buttonOK.TabIndex = 0;
            this.buttonOK.Text = "OK";
            this.buttonOK.UseVisualStyleBackColor = true;
            // 
            // buttonCancel
            // 
            this.buttonCancel.DialogResult = System.Windows.Forms.DialogResult.Cancel;
            this.buttonCancel.Location = new System.Drawing.Point(154, 119);
            this.buttonCancel.Name = "buttonCancel";
            this.buttonCancel.Size = new System.Drawing.Size(75, 23);
            this.buttonCancel.TabIndex = 6;
            this.buttonCancel.Text = "Cancel";
            this.buttonCancel.UseVisualStyleBackColor = true;
            // 
            // gem3Button
            // 
            this.gem3Button.Character = null;
            this.gem3Button.CharacterSlot = Rawr.CharacterSlot.Gems;
            this.gem3Button.FormItemSelection = null;
            this.gem3Button.ItemHidden = false;
            this.gem3Button.ItemIcon = null;
            this.gem3Button.Location = new System.Drawing.Point(240, 43);
            this.gem3Button.Name = "gem3Button";
            this.gem3Button.ReadOnly = false;
            this.gem3Button.SelectedItem = null;
            this.gem3Button.SelectedItemId = 0;
            this.gem3Button.SelectedItemInstance = null;
            this.gem3Button.Size = new System.Drawing.Size(70, 70);
            this.gem3Button.TabIndex = 5;
            this.gem3Button.UseVisualStyleBackColor = true;
            this.gem3Button.Click += new System.EventHandler(this.gem3Button_Click);
            this.gem3Button.Leave += new System.EventHandler(this.gem3Button_Leave);
            // 
            // gem2Button
            // 
            this.gem2Button.Character = null;
            this.gem2Button.CharacterSlot = Rawr.CharacterSlot.Gems;
            this.gem2Button.FormItemSelection = null;
            this.gem2Button.ItemHidden = false;
            this.gem2Button.ItemIcon = null;
            this.gem2Button.Location = new System.Drawing.Point(164, 43);
            this.gem2Button.Name = "gem2Button";
            this.gem2Button.ReadOnly = false;
            this.gem2Button.SelectedItem = null;
            this.gem2Button.SelectedItemId = 0;
            this.gem2Button.SelectedItemInstance = null;
            this.gem2Button.Size = new System.Drawing.Size(70, 70);
            this.gem2Button.TabIndex = 4;
            this.gem2Button.UseVisualStyleBackColor = true;
            this.gem2Button.Click += new System.EventHandler(this.gem2Button_Click);
            this.gem2Button.Leave += new System.EventHandler(this.gem2Button_Leave);
            // 
            // gem1Button
            // 
            this.gem1Button.Character = null;
            this.gem1Button.CharacterSlot = Rawr.CharacterSlot.Gems;
            this.gem1Button.FormItemSelection = null;
            this.gem1Button.ItemHidden = false;
            this.gem1Button.ItemIcon = null;
            this.gem1Button.Location = new System.Drawing.Point(88, 43);
            this.gem1Button.Name = "gem1Button";
            this.gem1Button.ReadOnly = false;
            this.gem1Button.SelectedItem = null;
            this.gem1Button.SelectedItemId = 0;
            this.gem1Button.SelectedItemInstance = null;
            this.gem1Button.Size = new System.Drawing.Size(70, 70);
            this.gem1Button.TabIndex = 3;
            this.gem1Button.UseVisualStyleBackColor = true;
            this.gem1Button.Click += new System.EventHandler(this.gem1Button_Click);
            this.gem1Button.Leave += new System.EventHandler(this.gem1Button_Leave);
            // 
            // LB_Info
            // 
            this.LB_Info.Location = new System.Drawing.Point(12, 9);
            this.LB_Info.Name = "LB_Info";
            this.LB_Info.Size = new System.Drawing.Size(298, 31);
            this.LB_Info.TabIndex = 1;
            this.LB_Info.Text = "Select gems and enchant for the item.";
            // 
            // itemButtonWithEnchant
            // 
            this.itemButtonWithEnchant.Character = null;
            this.itemButtonWithEnchant.CharacterSlot = Rawr.CharacterSlot.Head;
            this.itemButtonWithEnchant.FormItemSelection = null;
            this.itemButtonWithEnchant.ItemHidden = false;
            this.itemButtonWithEnchant.ItemIcon = null;
            this.itemButtonWithEnchant.Location = new System.Drawing.Point(13, 43);
            this.itemButtonWithEnchant.Name = "itemButtonWithEnchant";
            this.itemButtonWithEnchant.SelectedItem = null;
            this.itemButtonWithEnchant.SelectedItemId = 0;
            this.itemButtonWithEnchant.Size = new System.Drawing.Size(70, 83);
            this.itemButtonWithEnchant.TabIndex = 2;
            this.itemButtonWithEnchant.UseVisualStyleBackColor = true;
            this.itemButtonWithEnchant.Leave += new System.EventHandler(this.itemButtonWithEnchant_Leave);
            // 
            // FormItemInstance
            // 
            this.AcceptButton = this.buttonOK;
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.CancelButton = this.buttonCancel;
            this.ClientSize = new System.Drawing.Size(322, 150);
            this.Controls.Add(this.itemButtonWithEnchant);
            this.Controls.Add(this.buttonCancel);
            this.Controls.Add(this.buttonOK);
            this.Controls.Add(this.LB_Info);
            this.Controls.Add(this.gem3Button);
            this.Controls.Add(this.gem2Button);
            this.Controls.Add(this.gem1Button);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedToolWindow;
            this.Name = "FormItemInstance";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterParent;
            this.Text = "Customize item...";
            this.ResumeLayout(false);

        }

        #endregion

        private ItemButton gem1Button;
        private ItemButton gem2Button;
        private ItemButton gem3Button;
        private System.Windows.Forms.Button buttonOK;
        private System.Windows.Forms.Button buttonCancel;
        private System.Windows.Forms.Label LB_Info;
        private ItemButtonWithEnchant itemButtonWithEnchant;
    }
}