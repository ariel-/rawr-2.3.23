namespace Rawr.Bosses
{
    partial class DG_BossAttacks
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(DG_BossAttacks));
            this.LB_TheList = new System.Windows.Forms.ListBox();
            this.BT_Delete = new System.Windows.Forms.Button();
            this.BT_Accept = new System.Windows.Forms.Button();
            this.BT_Cancel = new System.Windows.Forms.Button();
            this.BT_Add = new System.Windows.Forms.Button();
            this.LB_Freq = new System.Windows.Forms.Label();
            this.LB_DMG = new System.Windows.Forms.Label();
            this.LB_Chance = new System.Windows.Forms.Label();
            this.NUD_Freq = new System.Windows.Forms.NumericUpDown();
            this.NUD_DMG = new System.Windows.Forms.NumericUpDown();
            this.NUD_Chance = new System.Windows.Forms.NumericUpDown();
            this.LB_Freq2 = new System.Windows.Forms.Label();
            this.LB_Chance2 = new System.Windows.Forms.Label();
            this.CK_ParryHaste = new System.Windows.Forms.CheckBox();
            ((System.ComponentModel.ISupportInitialize)(this.NUD_Freq)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.NUD_DMG)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.NUD_Chance)).BeginInit();
            this.SuspendLayout();
            // 
            // LB_TheList
            // 
            this.LB_TheList.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom)
                        | System.Windows.Forms.AnchorStyles.Left)
                        | System.Windows.Forms.AnchorStyles.Right)));
            this.LB_TheList.FormattingEnabled = true;
            this.LB_TheList.Location = new System.Drawing.Point(12, 12);
            this.LB_TheList.Name = "LB_TheList";
            this.LB_TheList.Size = new System.Drawing.Size(284, 199);
            this.LB_TheList.TabIndex = 0;
            // 
            // BT_Delete
            // 
            this.BT_Delete.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this.BT_Delete.Location = new System.Drawing.Point(12, 214);
            this.BT_Delete.Name = "BT_Delete";
            this.BT_Delete.Size = new System.Drawing.Size(75, 23);
            this.BT_Delete.TabIndex = 1;
            this.BT_Delete.Text = "&Delete";
            this.BT_Delete.UseVisualStyleBackColor = true;
            // 
            // BT_Accept
            // 
            this.BT_Accept.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.BT_Accept.Location = new System.Drawing.Point(318, 214);
            this.BT_Accept.Name = "BT_Accept";
            this.BT_Accept.Size = new System.Drawing.Size(75, 23);
            this.BT_Accept.TabIndex = 2;
            this.BT_Accept.Text = "&OK";
            this.BT_Accept.UseVisualStyleBackColor = true;
            // 
            // BT_Cancel
            // 
            this.BT_Cancel.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.BT_Cancel.DialogResult = System.Windows.Forms.DialogResult.Cancel;
            this.BT_Cancel.Location = new System.Drawing.Point(399, 214);
            this.BT_Cancel.Name = "BT_Cancel";
            this.BT_Cancel.Size = new System.Drawing.Size(75, 23);
            this.BT_Cancel.TabIndex = 3;
            this.BT_Cancel.Text = "&Cancel";
            this.BT_Cancel.UseVisualStyleBackColor = true;
            // 
            // BT_Add
            // 
            this.BT_Add.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.BT_Add.Location = new System.Drawing.Point(302, 113);
            this.BT_Add.Name = "BT_Add";
            this.BT_Add.Size = new System.Drawing.Size(172, 23);
            this.BT_Add.TabIndex = 4;
            this.BT_Add.Text = "&Add";
            this.BT_Add.UseVisualStyleBackColor = true;
            // 
            // LB_Freq
            // 
            this.LB_Freq.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.LB_Freq.AutoSize = true;
            this.LB_Freq.Location = new System.Drawing.Point(302, 14);
            this.LB_Freq.Name = "LB_Freq";
            this.LB_Freq.Size = new System.Drawing.Size(60, 13);
            this.LB_Freq.TabIndex = 5;
            this.LB_Freq.Text = "&Frequency:";
            // 
            // LB_DMG
            // 
            this.LB_DMG.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.LB_DMG.AutoSize = true;
            this.LB_DMG.Location = new System.Drawing.Point(302, 40);
            this.LB_DMG.Name = "LB_DMG";
            this.LB_DMG.Size = new System.Drawing.Size(50, 13);
            this.LB_DMG.TabIndex = 6;
            this.LB_DMG.Text = "Da&mage:";
            // 
            // LB_Chance
            // 
            this.LB_Chance.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.LB_Chance.AutoSize = true;
            this.LB_Chance.Location = new System.Drawing.Point(302, 66);
            this.LB_Chance.Name = "LB_Chance";
            this.LB_Chance.Size = new System.Drawing.Size(47, 13);
            this.LB_Chance.TabIndex = 7;
            this.LB_Chance.Text = "C&hance:";
            // 
            // NUD_Freq
            // 
            this.NUD_Freq.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.NUD_Freq.DecimalPlaces = 2;
            this.NUD_Freq.Increment = new decimal(new int[] {
            1,
            0,
            0,
            65536});
            this.NUD_Freq.Location = new System.Drawing.Point(365, 12);
            this.NUD_Freq.Maximum = new decimal(new int[] {
            1200,
            0,
            0,
            0});
            this.NUD_Freq.Minimum = new decimal(new int[] {
            1,
            0,
            0,
            131072});
            this.NUD_Freq.Name = "NUD_Freq";
            this.NUD_Freq.Size = new System.Drawing.Size(72, 20);
            this.NUD_Freq.TabIndex = 8;
            this.NUD_Freq.TextAlign = System.Windows.Forms.HorizontalAlignment.Right;
            this.NUD_Freq.Value = new decimal(new int[] {
            25,
            0,
            0,
            65536});
            // 
            // NUD_DMG
            // 
            this.NUD_DMG.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.NUD_DMG.Increment = new decimal(new int[] {
            1000,
            0,
            0,
            0});
            this.NUD_DMG.Location = new System.Drawing.Point(365, 38);
            this.NUD_DMG.Maximum = new decimal(new int[] {
            250000,
            0,
            0,
            0});
            this.NUD_DMG.Name = "NUD_DMG";
            this.NUD_DMG.Size = new System.Drawing.Size(72, 20);
            this.NUD_DMG.TabIndex = 9;
            this.NUD_DMG.TextAlign = System.Windows.Forms.HorizontalAlignment.Right;
            this.NUD_DMG.Value = new decimal(new int[] {
            80000,
            0,
            0,
            0});
            // 
            // NUD_Chance
            // 
            this.NUD_Chance.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.NUD_Chance.DecimalPlaces = 2;
            this.NUD_Chance.Location = new System.Drawing.Point(365, 64);
            this.NUD_Chance.Name = "NUD_Chance";
            this.NUD_Chance.Size = new System.Drawing.Size(72, 20);
            this.NUD_Chance.TabIndex = 10;
            this.NUD_Chance.TextAlign = System.Windows.Forms.HorizontalAlignment.Right;
            this.NUD_Chance.Value = new decimal(new int[] {
            100,
            0,
            0,
            0});
            // 
            // LB_Freq2
            // 
            this.LB_Freq2.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.LB_Freq2.AutoSize = true;
            this.LB_Freq2.Location = new System.Drawing.Point(443, 14);
            this.LB_Freq2.Name = "LB_Freq2";
            this.LB_Freq2.Size = new System.Drawing.Size(24, 13);
            this.LB_Freq2.TabIndex = 11;
            this.LB_Freq2.Text = "sec";
            // 
            // LB_Chance2
            // 
            this.LB_Chance2.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.LB_Chance2.AutoSize = true;
            this.LB_Chance2.Location = new System.Drawing.Point(443, 66);
            this.LB_Chance2.Name = "LB_Chance2";
            this.LB_Chance2.Size = new System.Drawing.Size(15, 13);
            this.LB_Chance2.TabIndex = 13;
            this.LB_Chance2.Text = "%";
            // 
            // CK_ParryHaste
            // 
            this.CK_ParryHaste.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.CK_ParryHaste.AutoSize = true;
            this.CK_ParryHaste.Location = new System.Drawing.Point(365, 90);
            this.CK_ParryHaste.Name = "CK_ParryHaste";
            this.CK_ParryHaste.Size = new System.Drawing.Size(103, 17);
            this.CK_ParryHaste.TabIndex = 14;
            this.CK_ParryHaste.Text = "&Use Parry Haste";
            this.CK_ParryHaste.UseVisualStyleBackColor = true;
            // 
            // DG_BossAttacks
            // 
            this.AcceptButton = this.BT_Accept;
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.CancelButton = this.BT_Cancel;
            this.ClientSize = new System.Drawing.Size(486, 250);
            this.Controls.Add(this.CK_ParryHaste);
            this.Controls.Add(this.LB_Chance2);
            this.Controls.Add(this.LB_Freq2);
            this.Controls.Add(this.NUD_Chance);
            this.Controls.Add(this.NUD_DMG);
            this.Controls.Add(this.NUD_Freq);
            this.Controls.Add(this.LB_Chance);
            this.Controls.Add(this.LB_DMG);
            this.Controls.Add(this.LB_Freq);
            this.Controls.Add(this.BT_Add);
            this.Controls.Add(this.BT_Cancel);
            this.Controls.Add(this.BT_Accept);
            this.Controls.Add(this.BT_Delete);
            this.Controls.Add(this.LB_TheList);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedToolWindow;
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.KeyPreview = true;
            this.Name = "DG_BossAttacks";
            this.ShowInTaskbar = false;
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterParent;
            this.Text = "Boss Attacks - Boss Handler - Rawr";
            this.TopMost = true;
            ((System.ComponentModel.ISupportInitialize)(this.NUD_Freq)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.NUD_DMG)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.NUD_Chance)).EndInit();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.ListBox LB_TheList;
        private System.Windows.Forms.Button BT_Delete;
        private System.Windows.Forms.Button BT_Accept;
        private System.Windows.Forms.Button BT_Cancel;
        private System.Windows.Forms.Button BT_Add;
        private System.Windows.Forms.Label LB_Freq;
        private System.Windows.Forms.Label LB_DMG;
        private System.Windows.Forms.Label LB_Chance;
        private System.Windows.Forms.NumericUpDown NUD_Freq;
        private System.Windows.Forms.NumericUpDown NUD_DMG;
        private System.Windows.Forms.NumericUpDown NUD_Chance;
        private System.Windows.Forms.Label LB_Freq2;
        private System.Windows.Forms.Label LB_Chance2;
        private System.Windows.Forms.CheckBox CK_ParryHaste;
    }
}