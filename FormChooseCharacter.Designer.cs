namespace Rawr
{
    partial class FormChooseCharacter
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(FormChooseCharacter));
            this.buttonCancel = new System.Windows.Forms.Button();
            this.buttonOK = new System.Windows.Forms.Button();
            this.label1 = new System.Windows.Forms.Label();
            this.label2 = new System.Windows.Forms.Label();
            this.comboBoxRealm = new System.Windows.Forms.ComboBox();
            this.comboBoxCharacter = new System.Windows.Forms.ComboBox();
            this.labelDescription = new System.Windows.Forms.Label();
            this.importFailureBox = new System.Windows.Forms.GroupBox();
            this.label3 = new System.Windows.Forms.Label();
            this.dataGridFailedImport = new System.Windows.Forms.DataGridView();
            this.FailedImportRealm = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.FailedImportCharacter = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.FailedImportReason = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.importFailureBox.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.dataGridFailedImport)).BeginInit();
            this.SuspendLayout();
            // 
            // buttonCancel
            // 
            this.buttonCancel.DialogResult = System.Windows.Forms.DialogResult.Cancel;
            this.buttonCancel.Location = new System.Drawing.Point(183, 101);
            this.buttonCancel.Name = "buttonCancel";
            this.buttonCancel.Size = new System.Drawing.Size(75, 23);
            this.buttonCancel.TabIndex = 4;
            this.buttonCancel.Text = "Cancel";
            this.buttonCancel.UseVisualStyleBackColor = true;
            // 
            // buttonOK
            // 
            this.buttonOK.DialogResult = System.Windows.Forms.DialogResult.OK;
            this.buttonOK.Location = new System.Drawing.Point(264, 101);
            this.buttonOK.Name = "buttonOK";
            this.buttonOK.Size = new System.Drawing.Size(75, 23);
            this.buttonOK.TabIndex = 3;
            this.buttonOK.Text = "OK";
            this.buttonOK.UseVisualStyleBackColor = true;
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(13, 15);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(40, 13);
            this.label1.TabIndex = 8;
            this.label1.Text = "Realm:";
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Location = new System.Drawing.Point(13, 42);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(87, 13);
            this.label2.TabIndex = 9;
            this.label2.Text = "Character Name:";
            // 
            // comboBoxRealm
            // 
            this.comboBoxRealm.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.comboBoxRealm.FormattingEnabled = true;
            this.comboBoxRealm.Location = new System.Drawing.Point(106, 12);
            this.comboBoxRealm.Name = "comboBoxRealm";
            this.comboBoxRealm.Size = new System.Drawing.Size(233, 21);
            this.comboBoxRealm.TabIndex = 1;
            this.comboBoxRealm.SelectedIndexChanged += new System.EventHandler(this.comboBoxRealm_SelectedIndexChanged);
            // 
            // comboBoxCharacter
            // 
            this.comboBoxCharacter.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.comboBoxCharacter.FormattingEnabled = true;
            this.comboBoxCharacter.Location = new System.Drawing.Point(106, 39);
            this.comboBoxCharacter.Name = "comboBoxCharacter";
            this.comboBoxCharacter.Size = new System.Drawing.Size(233, 21);
            this.comboBoxCharacter.TabIndex = 2;
            this.comboBoxCharacter.SelectedIndexChanged += new System.EventHandler(this.comboBoxCharacter_SelectedIndexChanged);
            // 
            // labelDescription
            // 
            this.labelDescription.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left)
                        | System.Windows.Forms.AnchorStyles.Right)));
            this.labelDescription.Location = new System.Drawing.Point(13, 85);
            this.labelDescription.Name = "labelDescription";
            this.labelDescription.Size = new System.Drawing.Size(597, 13);
            this.labelDescription.TabIndex = 12;
            this.labelDescription.Text = "(none selected)";
            // 
            // importFailureBox
            // 
            this.importFailureBox.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom)
                        | System.Windows.Forms.AnchorStyles.Left)
                        | System.Windows.Forms.AnchorStyles.Right)));
            this.importFailureBox.Controls.Add(this.label3);
            this.importFailureBox.Controls.Add(this.dataGridFailedImport);
            this.importFailureBox.Location = new System.Drawing.Point(11, 130);
            this.importFailureBox.Name = "importFailureBox";
            this.importFailureBox.Size = new System.Drawing.Size(599, 203);
            this.importFailureBox.TabIndex = 13;
            this.importFailureBox.TabStop = false;
            this.importFailureBox.Text = "The characters below failed to import: ";
            // 
            // label3
            // 
            this.label3.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this.label3.AutoSize = true;
            this.label3.Location = new System.Drawing.Point(6, 174);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(482, 26);
            this.label3.TabIndex = 1;
            this.label3.Text = resources.GetString("label3.Text");
            // 
            // dataGridFailedImport
            // 
            this.dataGridFailedImport.AllowUserToAddRows = false;
            this.dataGridFailedImport.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom)
                        | System.Windows.Forms.AnchorStyles.Left)
                        | System.Windows.Forms.AnchorStyles.Right)));
            this.dataGridFailedImport.AutoSizeColumnsMode = System.Windows.Forms.DataGridViewAutoSizeColumnsMode.Fill;
            this.dataGridFailedImport.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            this.dataGridFailedImport.Columns.AddRange(new System.Windows.Forms.DataGridViewColumn[] {
            this.FailedImportRealm,
            this.FailedImportCharacter,
            this.FailedImportReason});
            this.dataGridFailedImport.Location = new System.Drawing.Point(2, 19);
            this.dataGridFailedImport.MultiSelect = false;
            this.dataGridFailedImport.Name = "dataGridFailedImport";
            this.dataGridFailedImport.ReadOnly = true;
            this.dataGridFailedImport.ShowCellErrors = false;
            this.dataGridFailedImport.ShowCellToolTips = false;
            this.dataGridFailedImport.ShowEditingIcon = false;
            this.dataGridFailedImport.ShowRowErrors = false;
            this.dataGridFailedImport.Size = new System.Drawing.Size(591, 152);
            this.dataGridFailedImport.TabIndex = 0;
            // 
            // FailedImportRealm
            // 
            this.FailedImportRealm.AutoSizeMode = System.Windows.Forms.DataGridViewAutoSizeColumnMode.None;
            this.FailedImportRealm.HeaderText = "Realm";
            this.FailedImportRealm.Name = "FailedImportRealm";
            this.FailedImportRealm.ReadOnly = true;
            this.FailedImportRealm.Width = 144;
            // 
            // FailedImportCharacter
            // 
            this.FailedImportCharacter.AutoSizeMode = System.Windows.Forms.DataGridViewAutoSizeColumnMode.None;
            this.FailedImportCharacter.HeaderText = "Character";
            this.FailedImportCharacter.Name = "FailedImportCharacter";
            this.FailedImportCharacter.ReadOnly = true;
            this.FailedImportCharacter.Width = 145;
            // 
            // FailedImportReason
            // 
            this.FailedImportReason.AutoSizeMode = System.Windows.Forms.DataGridViewAutoSizeColumnMode.Fill;
            this.FailedImportReason.FillWeight = 200F;
            this.FailedImportReason.HeaderText = "Reason";
            this.FailedImportReason.Name = "FailedImportReason";
            this.FailedImportReason.ReadOnly = true;
            // 
            // FormChooseCharacter
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(622, 345);
            this.Controls.Add(this.labelDescription);
            this.Controls.Add(this.buttonCancel);
            this.Controls.Add(this.comboBoxCharacter);
            this.Controls.Add(this.comboBoxRealm);
            this.Controls.Add(this.label2);
            this.Controls.Add(this.label1);
            this.Controls.Add(this.buttonOK);
            this.Controls.Add(this.importFailureBox);
            this.Name = "FormChooseCharacter";
            this.ShowIcon = false;
            this.Text = "Choose Realm and Character";
            this.importFailureBox.ResumeLayout(false);
            this.importFailureBox.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.dataGridFailedImport)).EndInit();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Button buttonCancel;
        private System.Windows.Forms.Button buttonOK;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.ComboBox comboBoxRealm;
        private System.Windows.Forms.ComboBox comboBoxCharacter;
        private System.Windows.Forms.Label labelDescription;
        private System.Windows.Forms.GroupBox importFailureBox;
        private System.Windows.Forms.DataGridView dataGridFailedImport;
        private System.Windows.Forms.DataGridViewTextBoxColumn FailedImportRealm;
        private System.Windows.Forms.DataGridViewTextBoxColumn FailedImportCharacter;
        private System.Windows.Forms.DataGridViewTextBoxColumn FailedImportReason;
        private System.Windows.Forms.Label label3;
    }
}