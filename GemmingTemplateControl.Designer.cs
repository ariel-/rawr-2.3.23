namespace Rawr
{
	partial class GemmingTemplateControl
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
			this.checkBoxEnabled = new System.Windows.Forms.CheckBox();
			this.itemButtonRed = new Rawr.ItemButton();
			this.itemButtonYellow = new Rawr.ItemButton();
			this.itemButtonBlue = new Rawr.ItemButton();
			this.itemButtonPrismatic = new Rawr.ItemButton();
			this.itemButtonMeta = new Rawr.ItemButton();
			this.buttonDelete = new System.Windows.Forms.Button();
			this.SuspendLayout();
			// 
			// checkBoxEnabled
			// 
			this.checkBoxEnabled.AutoSize = true;
			this.checkBoxEnabled.Location = new System.Drawing.Point(61, 19);
			this.checkBoxEnabled.Name = "checkBoxEnabled";
			this.checkBoxEnabled.Size = new System.Drawing.Size(15, 14);
			this.checkBoxEnabled.TabIndex = 1;
			this.checkBoxEnabled.UseVisualStyleBackColor = true;
			this.checkBoxEnabled.CheckedChanged += new System.EventHandler(this.checkBoxEnabled_CheckedChanged);
			// 
			// itemButtonRed
			// 
			this.itemButtonRed.Character = null;
			this.itemButtonRed.CharacterSlot = Rawr.CharacterSlot.Gems;
			this.itemButtonRed.ItemIcon = null;
			this.itemButtonRed.Location = new System.Drawing.Point(88, 6);
			this.itemButtonRed.Name = "itemButtonRed";
			this.itemButtonRed.SelectedItem = null;
			this.itemButtonRed.SelectedItemId = 0;
			this.itemButtonRed.SelectedItemInstance = null;
			this.itemButtonRed.Size = new System.Drawing.Size(38, 38);
			this.itemButtonRed.TabIndex = 0;
			this.itemButtonRed.Text = "itemButton1";
			this.itemButtonRed.UseVisualStyleBackColor = true;
			this.itemButtonRed.SelectedItemChanged += new System.EventHandler(this.itemButton_SelectedItemChanged);
			// 
			// itemButtonYellow
			// 
			this.itemButtonYellow.Character = null;
			this.itemButtonYellow.CharacterSlot = Rawr.CharacterSlot.Gems;
			this.itemButtonYellow.ItemIcon = null;
			this.itemButtonYellow.Location = new System.Drawing.Point(130, 6);
			this.itemButtonYellow.Name = "itemButtonYellow";
			this.itemButtonYellow.SelectedItem = null;
			this.itemButtonYellow.SelectedItemId = 0;
			this.itemButtonYellow.SelectedItemInstance = null;
			this.itemButtonYellow.Size = new System.Drawing.Size(38, 38);
			this.itemButtonYellow.TabIndex = 0;
			this.itemButtonYellow.Text = "itemButton1";
			this.itemButtonYellow.UseVisualStyleBackColor = true;
			this.itemButtonYellow.SelectedItemChanged += new System.EventHandler(this.itemButton_SelectedItemChanged);
			// 
			// itemButtonBlue
			// 
			this.itemButtonBlue.Character = null;
			this.itemButtonBlue.CharacterSlot = Rawr.CharacterSlot.Gems;
			this.itemButtonBlue.ItemIcon = null;
			this.itemButtonBlue.Location = new System.Drawing.Point(172, 6);
			this.itemButtonBlue.Name = "itemButtonBlue";
			this.itemButtonBlue.SelectedItem = null;
			this.itemButtonBlue.SelectedItemId = 0;
			this.itemButtonBlue.SelectedItemInstance = null;
			this.itemButtonBlue.Size = new System.Drawing.Size(38, 38);
			this.itemButtonBlue.TabIndex = 0;
			this.itemButtonBlue.Text = "itemButton1";
			this.itemButtonBlue.UseVisualStyleBackColor = true;
			this.itemButtonBlue.SelectedItemChanged += new System.EventHandler(this.itemButton_SelectedItemChanged);
			// 
			// itemButtonPrismatic
			// 
			this.itemButtonPrismatic.Character = null;
			this.itemButtonPrismatic.CharacterSlot = Rawr.CharacterSlot.Gems;
			this.itemButtonPrismatic.ItemIcon = null;
			this.itemButtonPrismatic.Location = new System.Drawing.Point(214, 6);
			this.itemButtonPrismatic.Name = "itemButtonPrismatic";
			this.itemButtonPrismatic.SelectedItem = null;
			this.itemButtonPrismatic.SelectedItemId = 0;
			this.itemButtonPrismatic.SelectedItemInstance = null;
			this.itemButtonPrismatic.Size = new System.Drawing.Size(38, 38);
			this.itemButtonPrismatic.TabIndex = 0;
			this.itemButtonPrismatic.Text = "itemButton1";
			this.itemButtonPrismatic.UseVisualStyleBackColor = true;
			this.itemButtonPrismatic.SelectedItemChanged += new System.EventHandler(this.itemButton_SelectedItemChanged);
			// 
			// itemButtonMeta
			// 
			this.itemButtonMeta.Character = null;
			this.itemButtonMeta.CharacterSlot = Rawr.CharacterSlot.Metas;
			this.itemButtonMeta.ItemIcon = null;
			this.itemButtonMeta.Location = new System.Drawing.Point(256, 6);
			this.itemButtonMeta.Name = "itemButtonMeta";
			this.itemButtonMeta.SelectedItem = null;
			this.itemButtonMeta.SelectedItemId = 0;
			this.itemButtonMeta.SelectedItemInstance = null;
			this.itemButtonMeta.Size = new System.Drawing.Size(38, 38);
			this.itemButtonMeta.TabIndex = 0;
			this.itemButtonMeta.Text = "itemButton1";
			this.itemButtonMeta.UseVisualStyleBackColor = true;
			this.itemButtonMeta.SelectedItemChanged += new System.EventHandler(this.itemButton_SelectedItemChanged);
			// 
			// buttonDelete
			// 
			this.buttonDelete.Location = new System.Drawing.Point(3, 14);
			this.buttonDelete.Name = "buttonDelete";
			this.buttonDelete.Size = new System.Drawing.Size(46, 23);
			this.buttonDelete.TabIndex = 2;
			this.buttonDelete.Text = "Delete";
			this.buttonDelete.UseVisualStyleBackColor = true;
			this.buttonDelete.Click += new System.EventHandler(this.buttonDelete_Click);
			// 
			// GemmingTemplateControl
			// 
			this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
			this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
			this.Controls.Add(this.buttonDelete);
			this.Controls.Add(this.checkBoxEnabled);
			this.Controls.Add(this.itemButtonRed);
			this.Controls.Add(this.itemButtonYellow);
			this.Controls.Add(this.itemButtonBlue);
			this.Controls.Add(this.itemButtonPrismatic);
			this.Controls.Add(this.itemButtonMeta);
			this.Name = "GemmingTemplateControl";
			this.Size = new System.Drawing.Size(300, 50);
			this.ResumeLayout(false);
			this.PerformLayout();

		}

		#endregion

		private ItemButton itemButtonMeta;
		private ItemButton itemButtonPrismatic;
		private ItemButton itemButtonBlue;
		private ItemButton itemButtonYellow;
		private ItemButton itemButtonRed;
		private System.Windows.Forms.CheckBox checkBoxEnabled;
		private System.Windows.Forms.Button buttonDelete;
	}
}
