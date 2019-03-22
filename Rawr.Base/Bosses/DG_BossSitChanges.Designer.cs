namespace Rawr.Bosses
{
    partial class DG_BossSitChanges
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(DG_BossSitChanges));
            this.BT_Accept = new System.Windows.Forms.Button();
            this.BT_Cancel = new System.Windows.Forms.Button();
            this.LB_TheList = new System.Windows.Forms.ListBox();
            this.CK_Breakable = new System.Windows.Forms.CheckBox();
            this.NUD_Freq = new System.Windows.Forms.NumericUpDown();
            this.NUD_Dur = new System.Windows.Forms.NumericUpDown();
            this.NUD_Chance = new System.Windows.Forms.NumericUpDown();
            this.LB_Freq = new System.Windows.Forms.Label();
            this.LB_Dur = new System.Windows.Forms.Label();
            this.LB_Chance = new System.Windows.Forms.Label();
            this.BT_Delete = new System.Windows.Forms.Button();
            this.LB_Chance2 = new System.Windows.Forms.Label();
            this.LB_Dur2 = new System.Windows.Forms.Label();
            this.LB_Freq2 = new System.Windows.Forms.Label();
            this.BT_Add = new System.Windows.Forms.Button();
            ((System.ComponentModel.ISupportInitialize)(this.NUD_Freq)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.NUD_Dur)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.NUD_Chance)).BeginInit();
            this.SuspendLayout();
            // 
            // BT_Accept
            // 
            this.BT_Accept.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.BT_Accept.DialogResult = System.Windows.Forms.DialogResult.OK;
            this.BT_Accept.Location = new System.Drawing.Point(318, 215);
            this.BT_Accept.Name = "BT_Accept";
            this.BT_Accept.Size = new System.Drawing.Size(75, 23);
            this.BT_Accept.TabIndex = 0;
            this.BT_Accept.Text = "&OK";
            this.BT_Accept.UseVisualStyleBackColor = true;
            // 
            // BT_Cancel
            // 
            this.BT_Cancel.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.BT_Cancel.DialogResult = System.Windows.Forms.DialogResult.Cancel;
            this.BT_Cancel.Location = new System.Drawing.Point(399, 215);
            this.BT_Cancel.Name = "BT_Cancel";
            this.BT_Cancel.Size = new System.Drawing.Size(75, 23);
            this.BT_Cancel.TabIndex = 1;
            this.BT_Cancel.Text = "&Cancel";
            this.BT_Cancel.UseVisualStyleBackColor = true;
            // 
            // LB_TheList
            // 
            this.LB_TheList.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom)
                        | System.Windows.Forms.AnchorStyles.Left)
                        | System.Windows.Forms.AnchorStyles.Right)));
            this.LB_TheList.FormattingEnabled = true;
            this.LB_TheList.Location = new System.Drawing.Point(12, 12);
            this.LB_TheList.Name = "LB_TheList";
            this.LB_TheList.ScrollAlwaysVisible = true;
            this.LB_TheList.Size = new System.Drawing.Size(291, 199);
            this.LB_TheList.TabIndex = 2;
            // 
            // CK_Breakable
            // 
            this.CK_Breakable.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.CK_Breakable.AutoSize = true;
            this.CK_Breakable.Checked = true;
            this.CK_Breakable.CheckState = System.Windows.Forms.CheckState.Checked;
            this.CK_Breakable.Location = new System.Drawing.Point(369, 90);
            this.CK_Breakable.Name = "CK_Breakable";
            this.CK_Breakable.Size = new System.Drawing.Size(74, 17);
            this.CK_Breakable.TabIndex = 13;
            this.CK_Breakable.Text = "&Breakable";
            this.CK_Breakable.UseVisualStyleBackColor = true;
            // 
            // NUD_Freq
            // 
            this.NUD_Freq.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.NUD_Freq.Location = new System.Drawing.Point(369, 12);
            this.NUD_Freq.Maximum = new decimal(new int[] {
            1200,
            0,
            0,
            0});
            this.NUD_Freq.Minimum = new decimal(new int[] {
            1,
            0,
            0,
            0});
            this.NUD_Freq.Name = "NUD_Freq";
            this.NUD_Freq.Size = new System.Drawing.Size(75, 20);
            this.NUD_Freq.TabIndex = 5;
            this.NUD_Freq.TextAlign = System.Windows.Forms.HorizontalAlignment.Right;
            this.NUD_Freq.Value = new decimal(new int[] {
            60,
            0,
            0,
            0});
            // 
            // NUD_Dur
            // 
            this.NUD_Dur.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.NUD_Dur.Location = new System.Drawing.Point(369, 38);
            this.NUD_Dur.Maximum = new decimal(new int[] {
            20000,
            0,
            0,
            0});
            this.NUD_Dur.Minimum = new decimal(new int[] {
            500,
            0,
            0,
            0});
            this.NUD_Dur.Name = "NUD_Dur";
            this.NUD_Dur.Size = new System.Drawing.Size(75, 20);
            this.NUD_Dur.TabIndex = 8;
            this.NUD_Dur.TextAlign = System.Windows.Forms.HorizontalAlignment.Right;
            this.NUD_Dur.Value = new decimal(new int[] {
            5000,
            0,
            0,
            0});
            // 
            // NUD_Chance
            // 
            this.NUD_Chance.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.NUD_Chance.DecimalPlaces = 2;
            this.NUD_Chance.Location = new System.Drawing.Point(369, 64);
            this.NUD_Chance.Minimum = new decimal(new int[] {
            1,
            0,
            0,
            131072});
            this.NUD_Chance.Name = "NUD_Chance";
            this.NUD_Chance.Size = new System.Drawing.Size(75, 20);
            this.NUD_Chance.TabIndex = 11;
            this.NUD_Chance.TextAlign = System.Windows.Forms.HorizontalAlignment.Right;
            this.NUD_Chance.Value = new decimal(new int[] {
            100,
            0,
            0,
            0});
            // 
            // LB_Freq
            // 
            this.LB_Freq.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.LB_Freq.AutoSize = true;
            this.LB_Freq.Location = new System.Drawing.Point(306, 14);
            this.LB_Freq.Name = "LB_Freq";
            this.LB_Freq.Size = new System.Drawing.Size(60, 13);
            this.LB_Freq.TabIndex = 4;
            this.LB_Freq.Text = "&Frequency:";
            // 
            // LB_Dur
            // 
            this.LB_Dur.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.LB_Dur.AutoSize = true;
            this.LB_Dur.Location = new System.Drawing.Point(306, 40);
            this.LB_Dur.Name = "LB_Dur";
            this.LB_Dur.Size = new System.Drawing.Size(50, 13);
            this.LB_Dur.TabIndex = 7;
            this.LB_Dur.Text = "D&uration:";
            // 
            // LB_Chance
            // 
            this.LB_Chance.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.LB_Chance.AutoSize = true;
            this.LB_Chance.Location = new System.Drawing.Point(306, 66);
            this.LB_Chance.Name = "LB_Chance";
            this.LB_Chance.Size = new System.Drawing.Size(47, 13);
            this.LB_Chance.TabIndex = 10;
            this.LB_Chance.Text = "C&hance:";
            // 
            // BT_Delete
            // 
            this.BT_Delete.Location = new System.Drawing.Point(12, 215);
            this.BT_Delete.Name = "BT_Delete";
            this.BT_Delete.Size = new System.Drawing.Size(75, 23);
            this.BT_Delete.TabIndex = 3;
            this.BT_Delete.Text = "&Delete";
            this.BT_Delete.UseVisualStyleBackColor = true;
            this.BT_Delete.Click += new System.EventHandler(this.BT_Delete_Click);
            // 
            // LB_Chance2
            // 
            this.LB_Chance2.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.LB_Chance2.AutoSize = true;
            this.LB_Chance2.Location = new System.Drawing.Point(450, 66);
            this.LB_Chance2.Name = "LB_Chance2";
            this.LB_Chance2.Size = new System.Drawing.Size(15, 13);
            this.LB_Chance2.TabIndex = 12;
            this.LB_Chance2.Text = "%";
            // 
            // LB_Dur2
            // 
            this.LB_Dur2.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.LB_Dur2.AutoSize = true;
            this.LB_Dur2.Location = new System.Drawing.Point(450, 40);
            this.LB_Dur2.Name = "LB_Dur2";
            this.LB_Dur2.Size = new System.Drawing.Size(20, 13);
            this.LB_Dur2.TabIndex = 9;
            this.LB_Dur2.Text = "ms";
            // 
            // LB_Freq2
            // 
            this.LB_Freq2.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.LB_Freq2.AutoSize = true;
            this.LB_Freq2.Location = new System.Drawing.Point(450, 14);
            this.LB_Freq2.Name = "LB_Freq2";
            this.LB_Freq2.Size = new System.Drawing.Size(24, 13);
            this.LB_Freq2.TabIndex = 6;
            this.LB_Freq2.Text = "sec";
            // 
            // BT_Add
            // 
            this.BT_Add.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.BT_Add.Location = new System.Drawing.Point(309, 113);
            this.BT_Add.Name = "BT_Add";
            this.BT_Add.Size = new System.Drawing.Size(165, 23);
            this.BT_Add.TabIndex = 14;
            this.BT_Add.Text = "&Add";
            this.BT_Add.UseVisualStyleBackColor = true;
            this.BT_Add.Click += new System.EventHandler(this.BT_Add_Click);
            // 
            // DG_BossSitChanges
            // 
            this.AcceptButton = this.BT_Accept;
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.CancelButton = this.BT_Cancel;
            this.ClientSize = new System.Drawing.Size(486, 250);
            this.Controls.Add(this.BT_Add);
            this.Controls.Add(this.LB_Freq2);
            this.Controls.Add(this.LB_Dur2);
            this.Controls.Add(this.LB_Chance2);
            this.Controls.Add(this.BT_Delete);
            this.Controls.Add(this.NUD_Dur);
            this.Controls.Add(this.LB_Chance);
            this.Controls.Add(this.LB_Dur);
            this.Controls.Add(this.LB_Freq);
            this.Controls.Add(this.NUD_Chance);
            this.Controls.Add(this.NUD_Freq);
            this.Controls.Add(this.CK_Breakable);
            this.Controls.Add(this.LB_TheList);
            this.Controls.Add(this.BT_Cancel);
            this.Controls.Add(this.BT_Accept);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedToolWindow;
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.KeyPreview = true;
            this.Name = "DG_BossSitChanges";
            this.ShowInTaskbar = false;
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterParent;
            this.Text = "SitChanges - Boss Handler - Rawr";
            this.TopMost = true;
            ((System.ComponentModel.ISupportInitialize)(this.NUD_Freq)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.NUD_Dur)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.NUD_Chance)).EndInit();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Button BT_Accept;
        private System.Windows.Forms.Button BT_Cancel;
        private System.Windows.Forms.ListBox LB_TheList;
        private System.Windows.Forms.CheckBox CK_Breakable;
        private System.Windows.Forms.NumericUpDown NUD_Freq;
        private System.Windows.Forms.NumericUpDown NUD_Dur;
        private System.Windows.Forms.NumericUpDown NUD_Chance;
        private System.Windows.Forms.Label LB_Freq;
        private System.Windows.Forms.Label LB_Dur;
        private System.Windows.Forms.Label LB_Chance;
        private System.Windows.Forms.Button BT_Delete;
        private System.Windows.Forms.Label LB_Chance2;
        private System.Windows.Forms.Label LB_Dur2;
        private System.Windows.Forms.Label LB_Freq2;
        private System.Windows.Forms.Button BT_Add;
    }
}