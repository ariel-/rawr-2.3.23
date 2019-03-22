namespace Rawr
{
	partial class ItemButtonWithEnchant
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
			this.itemButtonItem = new Rawr.ItemButton();
			this.buttonEnchant = new System.Windows.Forms.Button();
			this.panelItem = new System.Windows.Forms.Panel();
			this.panelItem.SuspendLayout();
			this.SuspendLayout();
			// 
			// itemButtonItem
			// 
			this.itemButtonItem.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom)
						| System.Windows.Forms.AnchorStyles.Left)
						| System.Windows.Forms.AnchorStyles.Right)));
			this.itemButtonItem.Character = null;
			this.itemButtonItem.CharacterSlot = Rawr.CharacterSlot.Head;
			this.itemButtonItem.ItemIcon = null;
			this.itemButtonItem.Location = new System.Drawing.Point(0, 0);
			this.itemButtonItem.Name = "itemButtonItem";
			this.itemButtonItem.SelectedItem = null;
			this.itemButtonItem.SelectedItemId = 0;
			this.itemButtonItem.Size = new System.Drawing.Size(70, 70);
			this.itemButtonItem.TabIndex = 0;
			this.itemButtonItem.UseVisualStyleBackColor = true;
			// 
			// buttonEnchant
			// 
			this.buttonEnchant.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)
						| System.Windows.Forms.AnchorStyles.Right)));
			this.buttonEnchant.Font = new System.Drawing.Font("Segoe UI", 7F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
			this.buttonEnchant.Location = new System.Drawing.Point(0, 55);
			this.buttonEnchant.Name = "buttonEnchant";
			this.buttonEnchant.Size = new System.Drawing.Size(70, 28);
			this.buttonEnchant.TabIndex = 1;
			this.buttonEnchant.Text = "No Enchant";
			this.buttonEnchant.TextAlign = System.Drawing.ContentAlignment.BottomCenter;
			this.buttonEnchant.UseVisualStyleBackColor = true;
			this.buttonEnchant.Click += new System.EventHandler(this.buttonEnchant_Click);
			// 
			// panelItem
			// 
			this.panelItem.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom)
						| System.Windows.Forms.AnchorStyles.Left)
						| System.Windows.Forms.AnchorStyles.Right)));
			this.panelItem.Controls.Add(this.itemButtonItem);
			this.panelItem.Location = new System.Drawing.Point(0, 0);
			this.panelItem.Name = "panelItem";
			this.panelItem.Size = new System.Drawing.Size(70, 67);
			this.panelItem.TabIndex = 2;
			// 
			// ItemButtonWithEnchant
			// 
			this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
			this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
			this.Controls.Add(this.panelItem);
			this.Controls.Add(this.buttonEnchant);
			this.Name = "ItemButtonWithEnchant";
			this.Size = new System.Drawing.Size(70, 83);
			this.panelItem.ResumeLayout(false);
			this.ResumeLayout(false);

		}

		#endregion

		private ItemButton itemButtonItem;
		private System.Windows.Forms.Button buttonEnchant;
		private System.Windows.Forms.Panel panelItem;
	}
}
