namespace Rawr.Moonkin
{
    partial class CalculationOptionsPanelMoonkin
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
            this.cmbTargetLevel = new System.Windows.Forms.ComboBox();
            this.txtLatency = new System.Windows.Forms.TextBox();
            this.lblLatency = new System.Windows.Forms.Label();
            this.txtFightLength = new System.Windows.Forms.TextBox();
            this.label1 = new System.Windows.Forms.Label();
            this.chkInnervate = new System.Windows.Forms.CheckBox();
            this.lblInnervateOffset = new System.Windows.Forms.Label();
            this.txtInnervateDelay = new System.Windows.Forms.TextBox();
            this.trkReplenishmentUptime = new System.Windows.Forms.TrackBar();
            this.trkTreantLifespan = new System.Windows.Forms.TrackBar();
            this.lblReplenishmentUptime = new System.Windows.Forms.Label();
            this.lblTreantLifespan = new System.Windows.Forms.Label();
            this.lblUptimeValue = new System.Windows.Forms.Label();
            this.lblLifespanValue = new System.Windows.Forms.Label();
            this.lblUserRotation = new System.Windows.Forms.Label();
            this.cmbUserRotation = new System.Windows.Forms.ComboBox();
            this.chkPtrMode = new System.Windows.Forms.CheckBox();
            ((System.ComponentModel.ISupportInitialize)(this.trkReplenishmentUptime)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.trkTreantLifespan)).BeginInit();
            this.SuspendLayout();
            // 
            // lblTargetLevel
            // 
            this.lblTargetLevel.AutoSize = true;
            this.lblTargetLevel.Location = new System.Drawing.Point(3, 6);
            this.lblTargetLevel.Name = "lblTargetLevel";
            this.lblTargetLevel.Size = new System.Drawing.Size(70, 13);
            this.lblTargetLevel.TabIndex = 0;
            this.lblTargetLevel.Text = "Target Level:";
            // 
            // cmbTargetLevel
            // 
            this.cmbTargetLevel.FormattingEnabled = true;
            this.cmbTargetLevel.Items.AddRange(new object[] {
            "80",
            "81",
            "82",
            "83"});
            this.cmbTargetLevel.Location = new System.Drawing.Point(108, 3);
            this.cmbTargetLevel.Name = "cmbTargetLevel";
            this.cmbTargetLevel.Size = new System.Drawing.Size(93, 21);
            this.cmbTargetLevel.TabIndex = 1;
            this.cmbTargetLevel.SelectedIndexChanged += new System.EventHandler(this.cmbTargetLevel_SelectedIndexChanged);
            // 
            // txtLatency
            // 
            this.txtLatency.Location = new System.Drawing.Point(108, 30);
            this.txtLatency.Name = "txtLatency";
            this.txtLatency.Size = new System.Drawing.Size(93, 20);
            this.txtLatency.TabIndex = 2;
            this.txtLatency.Leave += new System.EventHandler(this.txtLatency_TextChanged);
            // 
            // lblLatency
            // 
            this.lblLatency.AutoSize = true;
            this.lblLatency.Location = new System.Drawing.Point(3, 33);
            this.lblLatency.Name = "lblLatency";
            this.lblLatency.Size = new System.Drawing.Size(48, 13);
            this.lblLatency.TabIndex = 3;
            this.lblLatency.Text = "Latency:";
            // 
            // txtFightLength
            // 
            this.txtFightLength.Location = new System.Drawing.Point(108, 57);
            this.txtFightLength.Name = "txtFightLength";
            this.txtFightLength.Size = new System.Drawing.Size(93, 20);
            this.txtFightLength.TabIndex = 3;
            this.txtFightLength.Leave += new System.EventHandler(this.txtFightLength_Leave);
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(3, 60);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(105, 13);
            this.label1.TabIndex = 11;
            this.label1.Text = "Est. Fight Time (min):";
            // 
            // chkInnervate
            // 
            this.chkInnervate.AutoSize = true;
            this.chkInnervate.Location = new System.Drawing.Point(6, 83);
            this.chkInnervate.Name = "chkInnervate";
            this.chkInnervate.Size = new System.Drawing.Size(135, 17);
            this.chkInnervate.TabIndex = 4;
            this.chkInnervate.Text = "Cast Innervate on self?";
            this.chkInnervate.UseVisualStyleBackColor = true;
            this.chkInnervate.CheckedChanged += new System.EventHandler(this.chkInnervate_CheckedChanged);
            // 
            // lblInnervateOffset
            // 
            this.lblInnervateOffset.AutoSize = true;
            this.lblInnervateOffset.Location = new System.Drawing.Point(3, 109);
            this.lblInnervateOffset.Name = "lblInnervateOffset";
            this.lblInnervateOffset.Size = new System.Drawing.Size(85, 13);
            this.lblInnervateOffset.TabIndex = 17;
            this.lblInnervateOffset.Text = "Innervate Delay:";
            // 
            // txtInnervateDelay
            // 
            this.txtInnervateDelay.Location = new System.Drawing.Point(108, 106);
            this.txtInnervateDelay.Name = "txtInnervateDelay";
            this.txtInnervateDelay.Size = new System.Drawing.Size(93, 20);
            this.txtInnervateDelay.TabIndex = 5;
            this.txtInnervateDelay.Leave += new System.EventHandler(this.txtInnervateDelay_Leave);
            // 
            // trkReplenishmentUptime
            // 
            this.trkReplenishmentUptime.Location = new System.Drawing.Point(119, 132);
            this.trkReplenishmentUptime.Maximum = 100;
            this.trkReplenishmentUptime.Name = "trkReplenishmentUptime";
            this.trkReplenishmentUptime.Size = new System.Drawing.Size(76, 45);
            this.trkReplenishmentUptime.TabIndex = 11;
            this.trkReplenishmentUptime.TickFrequency = 10;
            this.trkReplenishmentUptime.Value = 100;
            this.trkReplenishmentUptime.ValueChanged += new System.EventHandler(this.trkReplenishmentUptime_ValueChanged);
            // 
            // trkTreantLifespan
            // 
            this.trkTreantLifespan.Location = new System.Drawing.Point(122, 180);
            this.trkTreantLifespan.Maximum = 100;
            this.trkTreantLifespan.Name = "trkTreantLifespan";
            this.trkTreantLifespan.Size = new System.Drawing.Size(73, 45);
            this.trkTreantLifespan.TabIndex = 12;
            this.trkTreantLifespan.TickFrequency = 10;
            this.trkTreantLifespan.Value = 50;
            this.trkTreantLifespan.ValueChanged += new System.EventHandler(this.trkTreantLifespan_ValueChanged);
            // 
            // lblReplenishmentUptime
            // 
            this.lblReplenishmentUptime.AutoSize = true;
            this.lblReplenishmentUptime.Location = new System.Drawing.Point(-3, 142);
            this.lblReplenishmentUptime.Name = "lblReplenishmentUptime";
            this.lblReplenishmentUptime.Size = new System.Drawing.Size(116, 13);
            this.lblReplenishmentUptime.TabIndex = 31;
            this.lblReplenishmentUptime.Text = "Replenishment Uptime:";
            // 
            // lblTreantLifespan
            // 
            this.lblTreantLifespan.AutoSize = true;
            this.lblTreantLifespan.Location = new System.Drawing.Point(-3, 191);
            this.lblTreantLifespan.Name = "lblTreantLifespan";
            this.lblTreantLifespan.Size = new System.Drawing.Size(84, 13);
            this.lblTreantLifespan.TabIndex = 32;
            this.lblTreantLifespan.Text = "Treant Lifespan:";
            // 
            // lblUptimeValue
            // 
            this.lblUptimeValue.AutoSize = true;
            this.lblUptimeValue.Location = new System.Drawing.Point(122, 164);
            this.lblUptimeValue.Name = "lblUptimeValue";
            this.lblUptimeValue.Size = new System.Drawing.Size(25, 13);
            this.lblUptimeValue.TabIndex = 33;
            this.lblUptimeValue.Text = "100";
            // 
            // lblLifespanValue
            // 
            this.lblLifespanValue.AutoSize = true;
            this.lblLifespanValue.Location = new System.Drawing.Point(122, 212);
            this.lblLifespanValue.Name = "lblLifespanValue";
            this.lblLifespanValue.Size = new System.Drawing.Size(19, 13);
            this.lblLifespanValue.TabIndex = 34;
            this.lblLifespanValue.Text = "50";
            // 
            // lblUserRotation
            // 
            this.lblUserRotation.AutoSize = true;
            this.lblUserRotation.Location = new System.Drawing.Point(-3, 231);
            this.lblUserRotation.Name = "lblUserRotation";
            this.lblUserRotation.Size = new System.Drawing.Size(75, 13);
            this.lblUserRotation.TabIndex = 44;
            this.lblUserRotation.Text = "User Rotation:";
            // 
            // cmbUserRotation
            // 
            this.cmbUserRotation.FormattingEnabled = true;
            this.cmbUserRotation.Items.AddRange(new object[] {
            "None",
            "IS/W",
            "MF/W",
            "IS/SF",
            "MF/SF",
            "IS/MF/W",
            "IS/MF/SF",
            "SF Spam",
            "W Spam"});
            this.cmbUserRotation.Location = new System.Drawing.Point(102, 228);
            this.cmbUserRotation.Name = "cmbUserRotation";
            this.cmbUserRotation.Size = new System.Drawing.Size(93, 21);
            this.cmbUserRotation.TabIndex = 21;
            this.cmbUserRotation.SelectedIndexChanged += new System.EventHandler(this.cmbUserRotation_SelectedIndexChanged);
            // 
            // chkPtrMode
            // 
            this.chkPtrMode.AutoSize = true;
            this.chkPtrMode.Enabled = false;
            this.chkPtrMode.Location = new System.Drawing.Point(0, 256);
            this.chkPtrMode.Name = "chkPtrMode";
            this.chkPtrMode.Size = new System.Drawing.Size(78, 17);
            this.chkPtrMode.TabIndex = 45;
            this.chkPtrMode.Text = "PTR Mode";
            this.chkPtrMode.UseVisualStyleBackColor = true;
            this.chkPtrMode.Visible = false;
            this.chkPtrMode.CheckedChanged += new System.EventHandler(this.chkPtrMode_CheckedChanged);
            // 
            // CalculationOptionsPanelMoonkin
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.Controls.Add(this.chkPtrMode);
            this.Controls.Add(this.cmbUserRotation);
            this.Controls.Add(this.lblUserRotation);
            this.Controls.Add(this.lblLifespanValue);
            this.Controls.Add(this.lblUptimeValue);
            this.Controls.Add(this.lblTreantLifespan);
            this.Controls.Add(this.lblReplenishmentUptime);
            this.Controls.Add(this.trkTreantLifespan);
            this.Controls.Add(this.trkReplenishmentUptime);
            this.Controls.Add(this.txtInnervateDelay);
            this.Controls.Add(this.lblInnervateOffset);
            this.Controls.Add(this.chkInnervate);
            this.Controls.Add(this.label1);
            this.Controls.Add(this.txtFightLength);
            this.Controls.Add(this.lblLatency);
            this.Controls.Add(this.txtLatency);
            this.Controls.Add(this.cmbTargetLevel);
            this.Controls.Add(this.lblTargetLevel);
            this.Name = "CalculationOptionsPanelMoonkin";
            this.Size = new System.Drawing.Size(204, 338);
            ((System.ComponentModel.ISupportInitialize)(this.trkReplenishmentUptime)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.trkTreantLifespan)).EndInit();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Label lblTargetLevel;
        private System.Windows.Forms.ComboBox cmbTargetLevel;
        private System.Windows.Forms.TextBox txtLatency;
        private System.Windows.Forms.Label lblLatency;
        private System.Windows.Forms.TextBox txtFightLength;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.CheckBox chkInnervate;
        private System.Windows.Forms.Label lblInnervateOffset;
        private System.Windows.Forms.TextBox txtInnervateDelay;
        private System.Windows.Forms.TrackBar trkReplenishmentUptime;
        private System.Windows.Forms.TrackBar trkTreantLifespan;
        private System.Windows.Forms.Label lblReplenishmentUptime;
        private System.Windows.Forms.Label lblTreantLifespan;
        private System.Windows.Forms.Label lblUptimeValue;
        private System.Windows.Forms.Label lblLifespanValue;
        private System.Windows.Forms.Label lblUserRotation;
        private System.Windows.Forms.ComboBox cmbUserRotation;
        private System.Windows.Forms.CheckBox chkPtrMode;

    }
}

