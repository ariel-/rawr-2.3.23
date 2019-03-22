namespace Rawr.Elemental
{
    partial class CalculationOptionsPanelElemental
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
            this.tabPage4 = new System.Windows.Forms.TabPage();
            this.tbModuleNotes = new System.Windows.Forms.TextBox();
            this.tabPage1 = new System.Windows.Forms.TabPage();
            this.groupBox1 = new System.Windows.Forms.GroupBox();
            this.cbFireEle = new System.Windows.Forms.CheckBox();
            this.cbFireDpsTotem = new System.Windows.Forms.CheckBox();
            this.cbChainLightning = new System.Windows.Forms.CheckBox();
            this.cbFireNova = new System.Windows.Forms.CheckBox();
            this.groupBox3 = new System.Windows.Forms.GroupBox();
            this.cbThunderstorm = new System.Windows.Forms.CheckBox();
            this.groupBox7 = new System.Windows.Forms.GroupBox();
            this.textBoxGCDLatency = new System.Windows.Forms.TextBox();
            this.lblLatencyGcd = new System.Windows.Forms.Label();
            this.textBoxCastLatency = new System.Windows.Forms.TextBox();
            this.lblLatencyCast = new System.Windows.Forms.Label();
            this.trkTargets = new System.Windows.Forms.TrackBar();
            this.lbTargets = new System.Windows.Forms.Label();
            this.trkFightLength = new System.Windows.Forms.TrackBar();
            this.lblFightLength = new System.Windows.Forms.Label();
            this.groupBox2 = new System.Windows.Forms.GroupBox();
            this.lblBSRatio = new System.Windows.Forms.Label();
            this.tbBSRatio = new System.Windows.Forms.TrackBar();
            this.tabControl1 = new System.Windows.Forms.TabControl();
            this.tabPage4.SuspendLayout();
            this.tabPage1.SuspendLayout();
            this.groupBox1.SuspendLayout();
            this.groupBox3.SuspendLayout();
            this.groupBox7.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.trkTargets)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.trkFightLength)).BeginInit();
            this.groupBox2.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.tbBSRatio)).BeginInit();
            this.tabControl1.SuspendLayout();
            this.SuspendLayout();
            // 
            // tabPage4
            // 
            this.tabPage4.Controls.Add(this.tbModuleNotes);
            this.tabPage4.Location = new System.Drawing.Point(4, 22);
            this.tabPage4.Name = "tabPage4";
            this.tabPage4.Padding = new System.Windows.Forms.Padding(3);
            this.tabPage4.Size = new System.Drawing.Size(289, 527);
            this.tabPage4.TabIndex = 3;
            this.tabPage4.Text = "Module Notes";
            this.tabPage4.UseVisualStyleBackColor = true;
            // 
            // tbModuleNotes
            // 
            this.tbModuleNotes.AcceptsReturn = true;
            this.tbModuleNotes.AcceptsTab = true;
            this.tbModuleNotes.Location = new System.Drawing.Point(7, 7);
            this.tbModuleNotes.Multiline = true;
            this.tbModuleNotes.Name = "tbModuleNotes";
            this.tbModuleNotes.ReadOnly = true;
            this.tbModuleNotes.ScrollBars = System.Windows.Forms.ScrollBars.Vertical;
            this.tbModuleNotes.Size = new System.Drawing.Size(276, 537);
            this.tbModuleNotes.TabIndex = 0;
            // 
            // tabPage1
            // 
            this.tabPage1.Controls.Add(this.groupBox1);
            this.tabPage1.Controls.Add(this.groupBox3);
            this.tabPage1.Controls.Add(this.groupBox7);
            this.tabPage1.Controls.Add(this.groupBox2);
            this.tabPage1.Location = new System.Drawing.Point(4, 22);
            this.tabPage1.Name = "tabPage1";
            this.tabPage1.Padding = new System.Windows.Forms.Padding(3);
            this.tabPage1.Size = new System.Drawing.Size(289, 527);
            this.tabPage1.TabIndex = 0;
            this.tabPage1.Text = "Settings";
            this.tabPage1.UseVisualStyleBackColor = true;
            // 
            // groupBox1
            // 
            this.groupBox1.Controls.Add(this.cbFireEle);
            this.groupBox1.Controls.Add(this.cbFireDpsTotem);
            this.groupBox1.Controls.Add(this.cbChainLightning);
            this.groupBox1.Controls.Add(this.cbFireNova);
            this.groupBox1.Location = new System.Drawing.Point(6, 308);
            this.groupBox1.Name = "groupBox1";
            this.groupBox1.Size = new System.Drawing.Size(277, 61);
            this.groupBox1.TabIndex = 55;
            this.groupBox1.TabStop = false;
            this.groupBox1.Text = "Rotation options";
            // 
            // cbFireEle
            // 
            this.cbFireEle.AutoSize = true;
            this.cbFireEle.Location = new System.Drawing.Point(116, 38);
            this.cbFireEle.Name = "cbFireEle";
            this.cbFireEle.Size = new System.Drawing.Size(114, 17);
            this.cbFireEle.TabIndex = 54;
            this.cbFireEle.Text = "Use Fire Elemental";
            this.cbFireEle.UseVisualStyleBackColor = true;
            this.cbFireEle.Visible = false;
            this.cbFireEle.CheckedChanged += new System.EventHandler(this.cbFireEle_CheckedChanged);
            // 
            // cbFireDpsTotem
            // 
            this.cbFireDpsTotem.AutoSize = true;
            this.cbFireDpsTotem.Location = new System.Drawing.Point(9, 38);
            this.cbFireDpsTotem.Name = "cbFireDpsTotem";
            this.cbFireDpsTotem.Size = new System.Drawing.Size(103, 17);
            this.cbFireDpsTotem.TabIndex = 53;
            this.cbFireDpsTotem.Text = "Use DPS Totem";
            this.cbFireDpsTotem.UseVisualStyleBackColor = true;
            this.cbFireDpsTotem.CheckedChanged += new System.EventHandler(this.cbFireTotem_CheckedChanged);
            // 
            // cbChainLightning
            // 
            this.cbChainLightning.AutoSize = true;
            this.cbChainLightning.Location = new System.Drawing.Point(116, 19);
            this.cbChainLightning.Name = "cbChainLightning";
            this.cbChainLightning.Size = new System.Drawing.Size(121, 17);
            this.cbChainLightning.TabIndex = 52;
            this.cbChainLightning.Text = "Use Chain Lightning";
            this.cbChainLightning.UseVisualStyleBackColor = true;
            this.cbChainLightning.CheckedChanged += new System.EventHandler(this.cbChainLightning_CheckedChanged);
            // 
            // cbFireNova
            // 
            this.cbFireNova.AutoSize = true;
            this.cbFireNova.Location = new System.Drawing.Point(9, 19);
            this.cbFireNova.Name = "cbFireNova";
            this.cbFireNova.Size = new System.Drawing.Size(94, 17);
            this.cbFireNova.TabIndex = 51;
            this.cbFireNova.Text = "Use Fire Nova";
            this.cbFireNova.UseVisualStyleBackColor = true;
            this.cbFireNova.CheckedChanged += new System.EventHandler(this.cbFireNova_CheckedChanged);
            // 
            // groupBox3
            // 
            this.groupBox3.Controls.Add(this.cbThunderstorm);
            this.groupBox3.Location = new System.Drawing.Point(6, 374);
            this.groupBox3.Name = "groupBox3";
            this.groupBox3.Size = new System.Drawing.Size(277, 41);
            this.groupBox3.TabIndex = 54;
            this.groupBox3.TabStop = false;
            this.groupBox3.Text = "Estimation details";
            // 
            // cbThunderstorm
            // 
            this.cbThunderstorm.AutoSize = true;
            this.cbThunderstorm.Location = new System.Drawing.Point(9, 19);
            this.cbThunderstorm.Name = "cbThunderstorm";
            this.cbThunderstorm.Size = new System.Drawing.Size(208, 17);
            this.cbThunderstorm.TabIndex = 51;
            this.cbThunderstorm.Text = "Use Thunderstorm whenever available";
            this.cbThunderstorm.UseVisualStyleBackColor = true;
            this.cbThunderstorm.CheckedChanged += new System.EventHandler(this.checkBox1_CheckedChanged);
            // 
            // groupBox7
            // 
            this.groupBox7.Controls.Add(this.textBoxGCDLatency);
            this.groupBox7.Controls.Add(this.lblLatencyGcd);
            this.groupBox7.Controls.Add(this.textBoxCastLatency);
            this.groupBox7.Controls.Add(this.lblLatencyCast);
            this.groupBox7.Controls.Add(this.trkTargets);
            this.groupBox7.Controls.Add(this.lbTargets);
            this.groupBox7.Controls.Add(this.trkFightLength);
            this.groupBox7.Controls.Add(this.lblFightLength);
            this.groupBox7.Location = new System.Drawing.Point(6, 93);
            this.groupBox7.Name = "groupBox7";
            this.groupBox7.Size = new System.Drawing.Size(277, 209);
            this.groupBox7.TabIndex = 33;
            this.groupBox7.TabStop = false;
            this.groupBox7.Text = "General Fight Details";
            // 
            // textBoxGCDLatency
            // 
            this.textBoxGCDLatency.Location = new System.Drawing.Point(116, 176);
            this.textBoxGCDLatency.Name = "textBoxGCDLatency";
            this.textBoxGCDLatency.Size = new System.Drawing.Size(75, 20);
            this.textBoxGCDLatency.TabIndex = 161;
            this.textBoxGCDLatency.TextChanged += new System.EventHandler(this.textBoxGCDLatency_TextChanged);
            // 
            // lblLatencyGcd
            // 
            this.lblLatencyGcd.AutoSize = true;
            this.lblLatencyGcd.Location = new System.Drawing.Point(5, 179);
            this.lblLatencyGcd.Name = "lblLatencyGcd";
            this.lblLatencyGcd.Size = new System.Drawing.Size(74, 13);
            this.lblLatencyGcd.TabIndex = 160;
            this.lblLatencyGcd.Text = "GCD Latency:";
            // 
            // textBoxCastLatency
            // 
            this.textBoxCastLatency.Location = new System.Drawing.Point(116, 150);
            this.textBoxCastLatency.Name = "textBoxCastLatency";
            this.textBoxCastLatency.Size = new System.Drawing.Size(75, 20);
            this.textBoxCastLatency.TabIndex = 159;
            this.textBoxCastLatency.TextChanged += new System.EventHandler(this.textBoxCastLatency_TextChanged);
            // 
            // lblLatencyCast
            // 
            this.lblLatencyCast.AutoSize = true;
            this.lblLatencyCast.Location = new System.Drawing.Point(5, 153);
            this.lblLatencyCast.Name = "lblLatencyCast";
            this.lblLatencyCast.Size = new System.Drawing.Size(72, 13);
            this.lblLatencyCast.TabIndex = 158;
            this.lblLatencyCast.Text = "Cast Latency:";
            // 
            // trkTargets
            // 
            this.trkTargets.LargeChange = 1;
            this.trkTargets.Location = new System.Drawing.Point(9, 99);
            this.trkTargets.Minimum = 1;
            this.trkTargets.Name = "trkTargets";
            this.trkTargets.Size = new System.Drawing.Size(262, 45);
            this.trkTargets.TabIndex = 41;
            this.trkTargets.Value = 1;
            this.trkTargets.Scroll += new System.EventHandler(this.trkTargets_Scroll);
            // 
            // lbTargets
            // 
            this.lbTargets.AutoSize = true;
            this.lbTargets.Location = new System.Drawing.Point(6, 83);
            this.lbTargets.Name = "lbTargets";
            this.lbTargets.Size = new System.Drawing.Size(94, 13);
            this.lbTargets.TabIndex = 40;
            this.lbTargets.Text = "Number of targets:";
            // 
            // trkFightLength
            // 
            this.trkFightLength.Location = new System.Drawing.Point(9, 34);
            this.trkFightLength.Maximum = 60;
            this.trkFightLength.Minimum = 1;
            this.trkFightLength.Name = "trkFightLength";
            this.trkFightLength.Size = new System.Drawing.Size(262, 45);
            this.trkFightLength.TabIndex = 39;
            this.trkFightLength.TickFrequency = 4;
            this.trkFightLength.Value = 1;
            this.trkFightLength.Scroll += new System.EventHandler(this.trkFightLength_Scroll);
            // 
            // lblFightLength
            // 
            this.lblFightLength.AutoSize = true;
            this.lblFightLength.Location = new System.Drawing.Point(6, 18);
            this.lblFightLength.Name = "lblFightLength";
            this.lblFightLength.Size = new System.Drawing.Size(69, 13);
            this.lblFightLength.TabIndex = 38;
            this.lblFightLength.Text = "Fight Length:";
            // 
            // groupBox2
            // 
            this.groupBox2.Controls.Add(this.lblBSRatio);
            this.groupBox2.Controls.Add(this.tbBSRatio);
            this.groupBox2.Location = new System.Drawing.Point(6, 6);
            this.groupBox2.Name = "groupBox2";
            this.groupBox2.Size = new System.Drawing.Size(277, 87);
            this.groupBox2.TabIndex = 41;
            this.groupBox2.TabStop = false;
            this.groupBox2.Text = "Burst : Sustained Ratio";
            // 
            // lblBSRatio
            // 
            this.lblBSRatio.AutoSize = true;
            this.lblBSRatio.Location = new System.Drawing.Point(6, 20);
            this.lblBSRatio.Name = "lblBSRatio";
            this.lblBSRatio.Size = new System.Drawing.Size(32, 13);
            this.lblBSRatio.TabIndex = 41;
            this.lblBSRatio.Text = "Ratio";
            // 
            // tbBSRatio
            // 
            this.tbBSRatio.Location = new System.Drawing.Point(9, 36);
            this.tbBSRatio.Maximum = 100;
            this.tbBSRatio.Name = "tbBSRatio";
            this.tbBSRatio.Size = new System.Drawing.Size(262, 45);
            this.tbBSRatio.TabIndex = 40;
            this.tbBSRatio.TickFrequency = 5;
            this.tbBSRatio.Value = 50;
            this.tbBSRatio.Scroll += new System.EventHandler(this.tbBSRatio_Scroll);
            // 
            // tabControl1
            // 
            this.tabControl1.Controls.Add(this.tabPage1);
            this.tabControl1.Controls.Add(this.tabPage4);
            this.tabControl1.Location = new System.Drawing.Point(3, 3);
            this.tabControl1.Name = "tabControl1";
            this.tabControl1.SelectedIndex = 0;
            this.tabControl1.Size = new System.Drawing.Size(297, 553);
            this.tabControl1.TabIndex = 23;
            // 
            // CalculationOptionsPanelElemental
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.BackColor = System.Drawing.Color.White;
            this.Controls.Add(this.tabControl1);
            this.Name = "CalculationOptionsPanelElemental";
            this.Size = new System.Drawing.Size(303, 582);
            this.tabPage4.ResumeLayout(false);
            this.tabPage4.PerformLayout();
            this.tabPage1.ResumeLayout(false);
            this.groupBox1.ResumeLayout(false);
            this.groupBox1.PerformLayout();
            this.groupBox3.ResumeLayout(false);
            this.groupBox3.PerformLayout();
            this.groupBox7.ResumeLayout(false);
            this.groupBox7.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.trkTargets)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.trkFightLength)).EndInit();
            this.groupBox2.ResumeLayout(false);
            this.groupBox2.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.tbBSRatio)).EndInit();
            this.tabControl1.ResumeLayout(false);
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.TabPage tabPage4;
        private System.Windows.Forms.TextBox tbModuleNotes;
        private System.Windows.Forms.TabPage tabPage1;
        private System.Windows.Forms.TabControl tabControl1;
        private System.Windows.Forms.GroupBox groupBox2;
        private System.Windows.Forms.Label lblBSRatio;
        private System.Windows.Forms.TrackBar tbBSRatio;
        private System.Windows.Forms.GroupBox groupBox3;
        private System.Windows.Forms.CheckBox cbThunderstorm;
        private System.Windows.Forms.GroupBox groupBox7;
        private System.Windows.Forms.TrackBar trkFightLength;
        private System.Windows.Forms.Label lblFightLength;
        private System.Windows.Forms.TrackBar trkTargets;
        private System.Windows.Forms.Label lbTargets;
        private System.Windows.Forms.TextBox textBoxGCDLatency;
        private System.Windows.Forms.Label lblLatencyGcd;
        private System.Windows.Forms.TextBox textBoxCastLatency;
        private System.Windows.Forms.Label lblLatencyCast;
        private System.Windows.Forms.GroupBox groupBox1;
        private System.Windows.Forms.CheckBox cbChainLightning;
        private System.Windows.Forms.CheckBox cbFireNova;
        private System.Windows.Forms.CheckBox cbFireDpsTotem;
        private System.Windows.Forms.CheckBox cbFireEle;

    }
}