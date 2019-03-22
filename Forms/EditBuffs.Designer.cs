namespace Rawr.Forms
{
    partial class EditBuffs
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
			this.Add = new System.Windows.Forms.Button();
			this.Delete = new System.Windows.Forms.Button();
			this.Save = new System.Windows.Forms.Button();
			this.Cancel = new System.Windows.Forms.Button();
			this.listView1 = new System.Windows.Forms.ListView();
			this.NameColumn = new System.Windows.Forms.ColumnHeader();
			this.label1 = new System.Windows.Forms.Label();
			this.Filter = new System.Windows.Forms.TextBox();
			this.label2 = new System.Windows.Forms.Label();
			this.BuffName = new System.Windows.Forms.TextBox();
			this.Stats = new System.Windows.Forms.PropertyGrid();
			this.SuspendLayout();
			// 
			// Add
			// 
			this.Add.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
			this.Add.Location = new System.Drawing.Point(12, 391);
			this.Add.Name = "Add";
			this.Add.Size = new System.Drawing.Size(75, 23);
			this.Add.TabIndex = 0;
			this.Add.Text = "Add";
			this.Add.UseVisualStyleBackColor = true;
			// 
			// Delete
			// 
			this.Delete.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
			this.Delete.Location = new System.Drawing.Point(93, 391);
			this.Delete.Name = "Delete";
			this.Delete.Size = new System.Drawing.Size(75, 23);
			this.Delete.TabIndex = 1;
			this.Delete.Text = "Delete";
			this.Delete.UseVisualStyleBackColor = true;
			// 
			// Save
			// 
			this.Save.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
			this.Save.Location = new System.Drawing.Point(514, 391);
			this.Save.Name = "Save";
			this.Save.Size = new System.Drawing.Size(75, 23);
			this.Save.TabIndex = 2;
			this.Save.Text = "Save";
			this.Save.UseVisualStyleBackColor = true;
			this.Save.Click += new System.EventHandler(this.Save_Click);
			// 
			// Cancel
			// 
			this.Cancel.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
			this.Cancel.Location = new System.Drawing.Point(433, 391);
			this.Cancel.Name = "Cancel";
			this.Cancel.Size = new System.Drawing.Size(75, 23);
			this.Cancel.TabIndex = 3;
			this.Cancel.Text = "Cancel";
			this.Cancel.UseVisualStyleBackColor = true;
			this.Cancel.Click += new System.EventHandler(this.CancelButton_Click);
			// 
			// listView1
			// 
			this.listView1.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom)
						| System.Windows.Forms.AnchorStyles.Left)
						| System.Windows.Forms.AnchorStyles.Right)));
			this.listView1.Columns.AddRange(new System.Windows.Forms.ColumnHeader[] {
            this.NameColumn});
			this.listView1.FullRowSelect = true;
			this.listView1.HideSelection = false;
			this.listView1.Location = new System.Drawing.Point(12, 38);
			this.listView1.MultiSelect = false;
			this.listView1.Name = "listView1";
			this.listView1.Size = new System.Drawing.Size(252, 347);
			this.listView1.TabIndex = 4;
			this.listView1.UseCompatibleStateImageBehavior = false;
			this.listView1.View = System.Windows.Forms.View.Details;
			this.listView1.SelectedIndexChanged += new System.EventHandler(this.listView1_SelectedIndexChanged);
			// 
			// NameColumn
			// 
			this.NameColumn.Text = "Name";
			this.NameColumn.Width = 230;
			// 
			// label1
			// 
			this.label1.AutoSize = true;
			this.label1.Location = new System.Drawing.Point(9, 15);
			this.label1.Name = "label1";
			this.label1.Size = new System.Drawing.Size(32, 13);
			this.label1.TabIndex = 5;
			this.label1.Text = "Filter:";
			// 
			// Filter
			// 
			this.Filter.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left)
						| System.Windows.Forms.AnchorStyles.Right)));
			this.Filter.Location = new System.Drawing.Point(47, 12);
			this.Filter.Name = "Filter";
			this.Filter.Size = new System.Drawing.Size(217, 20);
			this.Filter.TabIndex = 6;
			// 
			// label2
			// 
			this.label2.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
			this.label2.AutoSize = true;
			this.label2.Location = new System.Drawing.Point(322, 15);
			this.label2.Name = "label2";
			this.label2.Size = new System.Drawing.Size(38, 13);
			this.label2.TabIndex = 7;
			this.label2.Text = "Name:";
			// 
			// BuffName
			// 
			this.BuffName.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
			this.BuffName.Location = new System.Drawing.Point(366, 12);
			this.BuffName.Name = "BuffName";
			this.BuffName.Size = new System.Drawing.Size(223, 20);
			this.BuffName.TabIndex = 8;
			// 
			// Stats
			// 
			this.Stats.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom)
						| System.Windows.Forms.AnchorStyles.Right)));
			this.Stats.HelpVisible = false;
			this.Stats.Location = new System.Drawing.Point(325, 38);
			this.Stats.Name = "Stats";
			this.Stats.Size = new System.Drawing.Size(264, 347);
			this.Stats.TabIndex = 9;
			// 
			// EditBuffs
			// 
			this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
			this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
			this.ClientSize = new System.Drawing.Size(601, 425);
			this.Controls.Add(this.Stats);
			this.Controls.Add(this.BuffName);
			this.Controls.Add(this.label2);
			this.Controls.Add(this.Filter);
			this.Controls.Add(this.label1);
			this.Controls.Add(this.listView1);
			this.Controls.Add(this.Cancel);
			this.Controls.Add(this.Save);
			this.Controls.Add(this.Delete);
			this.Controls.Add(this.Add);
			this.Name = "EditBuffs";
			this.Text = "Edit";
			this.Load += new System.EventHandler(this.Edit_Load);
			this.ResumeLayout(false);
			this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Button Add;
        private System.Windows.Forms.Button Delete;
        private System.Windows.Forms.Button Save;
        private System.Windows.Forms.Button Cancel;
        private System.Windows.Forms.ListView listView1;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.TextBox Filter;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.TextBox BuffName;
        private System.Windows.Forms.PropertyGrid Stats;
        private System.Windows.Forms.ColumnHeader NameColumn;
    }
}