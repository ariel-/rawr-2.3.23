namespace Rawr.DPSDK
{
    partial class CalculationOptionsPanelDPSDK
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
            this.lblTargetLevel = new System.Windows.Forms.Label();
            this.cbTargetLevel = new System.Windows.Forms.ComboBox();
            this.gbFightInfo = new System.Windows.Forms.GroupBox();
            this.nudFightLength = new System.Windows.Forms.NumericUpDown();
            this.lbKMProcUsage = new System.Windows.Forms.Label();
            this.KMProcUsage = new System.Windows.Forms.TrackBar();
            this.label2 = new System.Windows.Forms.Label();
            this.lbGhoulTime = new System.Windows.Forms.Label();
            this.GhoulUptime = new System.Windows.Forms.TrackBar();
            this.label3 = new System.Windows.Forms.Label();
            this.lbBloodwormTime = new System.Windows.Forms.Label();
            this.label1 = new System.Windows.Forms.Label();
            this.BloodwormUptime = new System.Windows.Forms.TrackBar();
            this.nudTargetArmor = new System.Windows.Forms.NumericUpDown();
            this.lblTargetArmor = new System.Windows.Forms.Label();
            this.lblFightLength = new System.Windows.Forms.Label();
            this.rbBloodPresence = new System.Windows.Forms.RadioButton();
            this.rbUnholyPresence = new System.Windows.Forms.RadioButton();
            this.btnGraph = new System.Windows.Forms.Button();
            this.cbGhoul = new System.Windows.Forms.CheckBox();
            this.label18 = new System.Windows.Forms.Label();
            this.cbSignificantChange = new System.Windows.Forms.CheckBox();
            this.cbRefCalcs = new System.Windows.Forms.CheckBox();
            this.cbDisplayCalcs = new System.Windows.Forms.CheckBox();
            this.cbExperimental = new System.Windows.Forms.CheckBox();
            this.gbFightInfo.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.nudFightLength)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.KMProcUsage)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.GhoulUptime)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.BloodwormUptime)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.nudTargetArmor)).BeginInit();
            this.SuspendLayout();
            // 
            // lblTargetLevel
            // 
            this.lblTargetLevel.AutoSize = true;
            this.lblTargetLevel.Font = new System.Drawing.Font("Microsoft Sans Serif", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lblTargetLevel.Location = new System.Drawing.Point(6, 17);
            this.lblTargetLevel.Name = "lblTargetLevel";
            this.lblTargetLevel.Size = new System.Drawing.Size(77, 15);
            this.lblTargetLevel.TabIndex = 0;
            this.lblTargetLevel.Text = "Target Level:";
            // 
            // cbTargetLevel
            // 
            this.cbTargetLevel.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.cbTargetLevel.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.cbTargetLevel.FormattingEnabled = true;
            this.cbTargetLevel.Items.AddRange(new object[] {
            "80",
            "81",
            "82",
            "83"});
            this.cbTargetLevel.Location = new System.Drawing.Point(87, 14);
            this.cbTargetLevel.Name = "cbTargetLevel";
            this.cbTargetLevel.Size = new System.Drawing.Size(49, 21);
            this.cbTargetLevel.TabIndex = 1;
            this.cbTargetLevel.SelectedIndexChanged += new System.EventHandler(this.cbTargetLevel_SelectedIndexChanged);
            // 
            // gbFightInfo
            // 
            this.gbFightInfo.Controls.Add(this.nudFightLength);
            this.gbFightInfo.Controls.Add(this.lbKMProcUsage);
            this.gbFightInfo.Controls.Add(this.KMProcUsage);
            this.gbFightInfo.Controls.Add(this.label2);
            this.gbFightInfo.Controls.Add(this.lbGhoulTime);
            this.gbFightInfo.Controls.Add(this.GhoulUptime);
            this.gbFightInfo.Controls.Add(this.label3);
            this.gbFightInfo.Controls.Add(this.lbBloodwormTime);
            this.gbFightInfo.Controls.Add(this.label1);
            this.gbFightInfo.Controls.Add(this.BloodwormUptime);
            this.gbFightInfo.Controls.Add(this.nudTargetArmor);
            this.gbFightInfo.Controls.Add(this.lblTargetArmor);
            this.gbFightInfo.Controls.Add(this.lblTargetLevel);
            this.gbFightInfo.Controls.Add(this.cbTargetLevel);
            this.gbFightInfo.Controls.Add(this.lblFightLength);
            this.gbFightInfo.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.gbFightInfo.Location = new System.Drawing.Point(13, 3);
            this.gbFightInfo.Name = "gbFightInfo";
            this.gbFightInfo.Size = new System.Drawing.Size(227, 278);
            this.gbFightInfo.TabIndex = 4;
            this.gbFightInfo.TabStop = false;
            this.gbFightInfo.Text = "Fight Info";
            // 
            // nudFightLength
            // 
            this.nudFightLength.Increment = new decimal(new int[] {
            100,
            0,
            0,
            0});
            this.nudFightLength.Location = new System.Drawing.Point(135, 69);
            this.nudFightLength.Maximum = new decimal(new int[] {
            15000,
            0,
            0,
            0});
            this.nudFightLength.Name = "nudFightLength";
            this.nudFightLength.Size = new System.Drawing.Size(64, 20);
            this.nudFightLength.TabIndex = 38;
            this.nudFightLength.Value = new decimal(new int[] {
            300,
            0,
            0,
            0});
            this.nudFightLength.ValueChanged += new System.EventHandler(this.nudFightLength_ValueChanged);
            // 
            // lbKMProcUsage
            // 
            this.lbKMProcUsage.AutoSize = true;
            this.lbKMProcUsage.Location = new System.Drawing.Point(173, 255);
            this.lbKMProcUsage.Name = "lbKMProcUsage";
            this.lbKMProcUsage.Size = new System.Drawing.Size(48, 13);
            this.lbKMProcUsage.TabIndex = 37;
            this.lbKMProcUsage.Text = "100.00%";
            // 
            // KMProcUsage
            // 
            this.KMProcUsage.BackColor = System.Drawing.SystemColors.ControlLightLight;
            this.KMProcUsage.Location = new System.Drawing.Point(135, 223);
            this.KMProcUsage.Maximum = 100;
            this.KMProcUsage.Name = "KMProcUsage";
            this.KMProcUsage.Size = new System.Drawing.Size(86, 45);
            this.KMProcUsage.TabIndex = 36;
            this.KMProcUsage.TickFrequency = 10;
            this.KMProcUsage.Value = 100;
            this.KMProcUsage.Scroll += new System.EventHandler(this.KMProcUsage_Scroll);
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label2.Location = new System.Drawing.Point(6, 223);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(90, 13);
            this.label2.TabIndex = 35;
            this.label2.Text = "% KM Procs used";
            // 
            // lbGhoulTime
            // 
            this.lbGhoulTime.AutoSize = true;
            this.lbGhoulTime.Location = new System.Drawing.Point(173, 204);
            this.lbGhoulTime.Name = "lbGhoulTime";
            this.lbGhoulTime.Size = new System.Drawing.Size(48, 13);
            this.lbGhoulTime.TabIndex = 34;
            this.lbGhoulTime.Text = "100.00%";
            // 
            // GhoulUptime
            // 
            this.GhoulUptime.BackColor = System.Drawing.SystemColors.ControlLightLight;
            this.GhoulUptime.Location = new System.Drawing.Point(135, 172);
            this.GhoulUptime.Maximum = 100;
            this.GhoulUptime.Name = "GhoulUptime";
            this.GhoulUptime.Size = new System.Drawing.Size(86, 45);
            this.GhoulUptime.TabIndex = 33;
            this.GhoulUptime.TickFrequency = 10;
            this.GhoulUptime.Value = 100;
            this.GhoulUptime.Scroll += new System.EventHandler(this.GhoulUptime_Scroll);
            // 
            // label3
            // 
            this.label3.AutoSize = true;
            this.label3.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label3.Location = new System.Drawing.Point(6, 172);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(99, 13);
            this.label3.TabIndex = 32;
            this.label3.Text = "Ghoul survival time:";
            // 
            // lbBloodwormTime
            // 
            this.lbBloodwormTime.AutoSize = true;
            this.lbBloodwormTime.Location = new System.Drawing.Point(173, 153);
            this.lbBloodwormTime.Name = "lbBloodwormTime";
            this.lbBloodwormTime.Size = new System.Drawing.Size(42, 13);
            this.lbBloodwormTime.TabIndex = 31;
            this.lbBloodwormTime.Text = "25.00%";
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label1.Location = new System.Drawing.Point(6, 121);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(123, 13);
            this.label1.TabIndex = 30;
            this.label1.Text = "Bloodworm survival time:";
            // 
            // BloodwormUptime
            // 
            this.BloodwormUptime.BackColor = System.Drawing.SystemColors.ControlLightLight;
            this.BloodwormUptime.Location = new System.Drawing.Point(135, 121);
            this.BloodwormUptime.Maximum = 100;
            this.BloodwormUptime.Name = "BloodwormUptime";
            this.BloodwormUptime.Size = new System.Drawing.Size(86, 45);
            this.BloodwormUptime.TabIndex = 29;
            this.BloodwormUptime.TickFrequency = 10;
            this.BloodwormUptime.Value = 25;
            this.BloodwormUptime.Scroll += new System.EventHandler(this.BloodwormUptime_Scroll);
            // 
            // nudTargetArmor
            // 
            this.nudTargetArmor.Increment = new decimal(new int[] {
            100,
            0,
            0,
            0});
            this.nudTargetArmor.Location = new System.Drawing.Point(87, 43);
            this.nudTargetArmor.Maximum = new decimal(new int[] {
            15000,
            0,
            0,
            0});
            this.nudTargetArmor.Name = "nudTargetArmor";
            this.nudTargetArmor.Size = new System.Drawing.Size(64, 20);
            this.nudTargetArmor.TabIndex = 28;
            this.nudTargetArmor.ValueChanged += new System.EventHandler(this.nudTargetArmor_ValueChanged);
            // 
            // lblTargetArmor
            // 
            this.lblTargetArmor.AutoSize = true;
            this.lblTargetArmor.Location = new System.Drawing.Point(6, 45);
            this.lblTargetArmor.Name = "lblTargetArmor";
            this.lblTargetArmor.Size = new System.Drawing.Size(71, 13);
            this.lblTargetArmor.TabIndex = 24;
            this.lblTargetArmor.Text = "Target Armor:";
            // 
            // lblFightLength
            // 
            this.lblFightLength.AutoSize = true;
            this.lblFightLength.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lblFightLength.Location = new System.Drawing.Point(6, 73);
            this.lblFightLength.Name = "lblFightLength";
            this.lblFightLength.Size = new System.Drawing.Size(115, 13);
            this.lblFightLength.TabIndex = 0;
            this.lblFightLength.Text = "Fight Length (seconds)";
            // 
            // rbBloodPresence
            // 
            this.rbBloodPresence.AutoSize = true;
            this.rbBloodPresence.Checked = true;
            this.rbBloodPresence.Font = new System.Drawing.Font("Microsoft Sans Serif", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.rbBloodPresence.Location = new System.Drawing.Point(67, 392);
            this.rbBloodPresence.Name = "rbBloodPresence";
            this.rbBloodPresence.Size = new System.Drawing.Size(57, 19);
            this.rbBloodPresence.TabIndex = 71;
            this.rbBloodPresence.TabStop = true;
            this.rbBloodPresence.Text = "Blood";
            this.rbBloodPresence.UseVisualStyleBackColor = true;
            this.rbBloodPresence.CheckedChanged += new System.EventHandler(this.rbBloodPresence_CheckedChanged);
            // 
            // rbUnholyPresence
            // 
            this.rbUnholyPresence.AutoSize = true;
            this.rbUnholyPresence.Font = new System.Drawing.Font("Microsoft Sans Serif", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.rbUnholyPresence.Location = new System.Drawing.Point(130, 392);
            this.rbUnholyPresence.Name = "rbUnholyPresence";
            this.rbUnholyPresence.Size = new System.Drawing.Size(63, 19);
            this.rbUnholyPresence.TabIndex = 73;
            this.rbUnholyPresence.Text = "Unholy";
            this.rbUnholyPresence.UseVisualStyleBackColor = true;
            this.rbUnholyPresence.CheckedChanged += new System.EventHandler(this.rbUnholyPresence_CheckedChanged);
            // 
            // btnGraph
            // 
            this.btnGraph.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.btnGraph.Location = new System.Drawing.Point(56, 345);
            this.btnGraph.Name = "btnGraph";
            this.btnGraph.Size = new System.Drawing.Size(75, 23);
            this.btnGraph.TabIndex = 5;
            this.btnGraph.Text = "Stat Graph";
            this.btnGraph.UseVisualStyleBackColor = true;
            this.btnGraph.Click += new System.EventHandler(this.btnGraph_Click);
            // 
            // cbGhoul
            // 
            this.cbGhoul.AutoSize = true;
            this.cbGhoul.Checked = true;
            this.cbGhoul.CheckState = System.Windows.Forms.CheckState.Checked;
            this.cbGhoul.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.cbGhoul.Location = new System.Drawing.Point(13, 322);
            this.cbGhoul.Name = "cbGhoul";
            this.cbGhoul.Size = new System.Drawing.Size(93, 17);
            this.cbGhoul.TabIndex = 41;
            this.cbGhoul.Text = "Ghoul (on CD)";
            this.cbGhoul.UseVisualStyleBackColor = true;
            this.cbGhoul.CheckedChanged += new System.EventHandler(this.cbGhoul_CheckedChanged);
            // 
            // label18
            // 
            this.label18.AutoSize = true;
            this.label18.Location = new System.Drawing.Point(10, 396);
            this.label18.Name = "label18";
            this.label18.Size = new System.Drawing.Size(55, 13);
            this.label18.TabIndex = 72;
            this.label18.Text = "Presence:";
            this.label18.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
            // 
            // cbSignificantChange
            // 
            this.cbSignificantChange.AutoSize = true;
            this.cbSignificantChange.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.cbSignificantChange.Location = new System.Drawing.Point(13, 473);
            this.cbSignificantChange.Name = "cbSignificantChange";
            this.cbSignificantChange.Size = new System.Drawing.Size(170, 17);
            this.cbSignificantChange.TabIndex = 74;
            this.cbSignificantChange.Text = "Refresh for Significant Change";
            this.cbSignificantChange.UseVisualStyleBackColor = true;
            this.cbSignificantChange.CheckedChanged += new System.EventHandler(this.cbSignificantChange_CheckedChanged);
            // 
            // cbRefCalcs
            // 
            this.cbRefCalcs.AutoSize = true;
            this.cbRefCalcs.Checked = true;
            this.cbRefCalcs.CheckState = System.Windows.Forms.CheckState.Checked;
            this.cbRefCalcs.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.cbRefCalcs.Location = new System.Drawing.Point(13, 450);
            this.cbRefCalcs.Name = "cbRefCalcs";
            this.cbRefCalcs.Size = new System.Drawing.Size(159, 17);
            this.cbRefCalcs.TabIndex = 75;
            this.cbRefCalcs.Text = "Refresh for Reference calcs";
            this.cbRefCalcs.UseVisualStyleBackColor = true;
            this.cbRefCalcs.CheckedChanged += new System.EventHandler(this.cbRefCalcs_CheckedChanged);
            // 
            // cbDisplayCalcs
            // 
            this.cbDisplayCalcs.AutoSize = true;
            this.cbDisplayCalcs.Checked = true;
            this.cbDisplayCalcs.CheckState = System.Windows.Forms.CheckState.Checked;
            this.cbDisplayCalcs.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.cbDisplayCalcs.Location = new System.Drawing.Point(13, 427);
            this.cbDisplayCalcs.Name = "cbDisplayCalcs";
            this.cbDisplayCalcs.Size = new System.Drawing.Size(143, 17);
            this.cbDisplayCalcs.TabIndex = 76;
            this.cbDisplayCalcs.Text = "Refresh for Display calcs";
            this.cbDisplayCalcs.UseVisualStyleBackColor = true;
            this.cbDisplayCalcs.CheckedChanged += new System.EventHandler(this.cbDisplayCalcs_CheckedChanged);
            // 
            // cbExperimental
            // 
            this.cbExperimental.AutoSize = true;
            this.cbExperimental.Location = new System.Drawing.Point(13, 496);
            this.cbExperimental.Name = "cbExperimental";
            this.cbExperimental.Size = new System.Drawing.Size(150, 17);
            this.cbExperimental.TabIndex = 77;
            this.cbExperimental.Text = "Enable Experimental Code";
            this.cbExperimental.UseVisualStyleBackColor = true;
            // 
            // CalculationOptionsPanelDPSDK
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.AutoScroll = true;
            this.Controls.Add(this.cbExperimental);
            this.Controls.Add(this.cbDisplayCalcs);
            this.Controls.Add(this.cbRefCalcs);
            this.Controls.Add(this.cbSignificantChange);
            this.Controls.Add(this.rbBloodPresence);
            this.Controls.Add(this.rbUnholyPresence);
            this.Controls.Add(this.label18);
            this.Controls.Add(this.cbGhoul);
            this.Controls.Add(this.btnGraph);
            this.Controls.Add(this.gbFightInfo);
            this.Name = "CalculationOptionsPanelDPSDK";
            this.Size = new System.Drawing.Size(243, 523);
            this.gbFightInfo.ResumeLayout(false);
            this.gbFightInfo.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.nudFightLength)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.KMProcUsage)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.GhoulUptime)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.BloodwormUptime)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.nudTargetArmor)).EndInit();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Label lblTargetLevel;
        private System.Windows.Forms.ComboBox cbTargetLevel;
        private System.Windows.Forms.GroupBox gbFightInfo;
        private System.Windows.Forms.Label lblFightLength;
        private System.Windows.Forms.Button btnGraph;
        private System.Windows.Forms.Label lblTargetArmor;
        private System.Windows.Forms.NumericUpDown nudTargetArmor;
        private System.Windows.Forms.CheckBox cbGhoul;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.TrackBar BloodwormUptime;
        private System.Windows.Forms.Label lbBloodwormTime;
        private System.Windows.Forms.Label lbGhoulTime;
        private System.Windows.Forms.TrackBar GhoulUptime;
        private System.Windows.Forms.Label label3;
        private System.Windows.Forms.Label lbKMProcUsage;
        private System.Windows.Forms.TrackBar KMProcUsage;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.RadioButton rbBloodPresence;
        private System.Windows.Forms.RadioButton rbUnholyPresence;
        private System.Windows.Forms.Label label18;
        private System.Windows.Forms.CheckBox cbSignificantChange;
        private System.Windows.Forms.CheckBox cbRefCalcs;
        private System.Windows.Forms.CheckBox cbDisplayCalcs;
        private System.Windows.Forms.NumericUpDown nudFightLength;
        private System.Windows.Forms.CheckBox cbExperimental;
    }
}