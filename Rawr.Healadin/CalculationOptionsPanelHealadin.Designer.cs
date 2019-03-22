namespace Rawr.Healadin
{
    partial class CalculationOptionsPanelHealadin
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
            this.components = new System.ComponentModel.Container();
            this.cmbLength = new System.Windows.Forms.NumericUpDown();
            this.label3 = new System.Windows.Forms.Label();
            this.groupBox2 = new System.Windows.Forms.GroupBox();
            this.chkLoHSelf = new System.Windows.Forms.CheckBox();
            this.label11 = new System.Windows.Forms.Label();
            this.label2 = new System.Windows.Forms.Label();
            this.lblReplenishment = new System.Windows.Forms.Label();
            this.trkReplenishment = new System.Windows.Forms.TrackBar();
            this.nudDivinePlea = new System.Windows.Forms.NumericUpDown();
            this.lblDivinePlea = new System.Windows.Forms.Label();
            this.trkActivity = new System.Windows.Forms.TrackBar();
            this.lblActivity = new System.Windows.Forms.Label();
            this.label9 = new System.Windows.Forms.Label();
            this.label5 = new System.Windows.Forms.Label();
            this.lblBoLUp = new System.Windows.Forms.Label();
            this.trkBoLUp = new System.Windows.Forms.TrackBar();
            this.trkHS = new System.Windows.Forms.TrackBar();
            this.lblHS = new System.Windows.Forms.Label();
            this.label7 = new System.Windows.Forms.Label();
            this.chkJotP = new System.Windows.Forms.CheckBox();
            this.label12 = new System.Windows.Forms.Label();
            this.nudGHL = new System.Windows.Forms.NumericUpDown();
            this.label19 = new System.Windows.Forms.Label();
            this.lblBurstScale = new System.Windows.Forms.Label();
            this.trkBurstScale = new System.Windows.Forms.TrackBar();
            this.trkIoLRatio = new System.Windows.Forms.TrackBar();
            this.chkIoL = new System.Windows.Forms.CheckBox();
            this.trkSacredShield = new System.Windows.Forms.TrackBar();
            this.label1 = new System.Windows.Forms.Label();
            this.lblSacredShield = new System.Windows.Forms.Label();
            this.toolTip = new System.Windows.Forms.ToolTip(this.components);
            this.trkFlashOfLightOnTank = new System.Windows.Forms.TrackBar();
            this.lblIoLFoL = new System.Windows.Forms.Label();
            this.lblIoLHL = new System.Windows.Forms.Label();
            this.chkJudgement = new System.Windows.Forms.CheckBox();
            this.chkSpiritIrrelevant = new System.Windows.Forms.CheckBox();
            this.chkHitIrrelevant = new System.Windows.Forms.CheckBox();
            this.label8 = new System.Windows.Forms.Label();
            this.lblFlashOfLightOnTank = new System.Windows.Forms.Label();
            ((System.ComponentModel.ISupportInitialize)(this.cmbLength)).BeginInit();
            this.groupBox2.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.trkReplenishment)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.nudDivinePlea)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.trkActivity)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.trkBoLUp)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.trkHS)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.nudGHL)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.trkBurstScale)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.trkIoLRatio)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.trkSacredShield)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.trkFlashOfLightOnTank)).BeginInit();
            this.SuspendLayout();
            // 
            // cmbLength
            // 
            this.cmbLength.DecimalPlaces = 1;
            this.cmbLength.Increment = new decimal(new int[] {
            5,
            0,
            0,
            65536});
            this.cmbLength.Location = new System.Drawing.Point(221, 196);
            this.cmbLength.Maximum = new decimal(new int[] {
            60,
            0,
            0,
            0});
            this.cmbLength.Minimum = new decimal(new int[] {
            1,
            0,
            0,
            0});
            this.cmbLength.Name = "cmbLength";
            this.cmbLength.Size = new System.Drawing.Size(76, 20);
            this.cmbLength.TabIndex = 20;
            this.cmbLength.Value = new decimal(new int[] {
            5,
            0,
            0,
            0});
            this.cmbLength.ValueChanged += new System.EventHandler(this.cmbLength_ValueChanged);
            // 
            // label3
            // 
            this.label3.AutoSize = true;
            this.label3.Location = new System.Drawing.Point(146, 198);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(69, 13);
            this.label3.TabIndex = 21;
            this.label3.Text = "Fight Length:";
            // 
            // groupBox2
            // 
            this.groupBox2.Controls.Add(this.chkLoHSelf);
            this.groupBox2.Controls.Add(this.label11);
            this.groupBox2.Controls.Add(this.label2);
            this.groupBox2.Controls.Add(this.lblReplenishment);
            this.groupBox2.Controls.Add(this.trkReplenishment);
            this.groupBox2.Controls.Add(this.nudDivinePlea);
            this.groupBox2.Controls.Add(this.lblDivinePlea);
            this.groupBox2.Location = new System.Drawing.Point(6, 248);
            this.groupBox2.Name = "groupBox2";
            this.groupBox2.Size = new System.Drawing.Size(294, 86);
            this.groupBox2.TabIndex = 22;
            this.groupBox2.TabStop = false;
            this.groupBox2.Text = "Mana Buffs";
            // 
            // chkLoHSelf
            // 
            this.chkLoHSelf.AutoSize = true;
            this.chkLoHSelf.Location = new System.Drawing.Point(151, 58);
            this.chkLoHSelf.Name = "chkLoHSelf";
            this.chkLoHSelf.Size = new System.Drawing.Size(106, 17);
            this.chkLoHSelf.TabIndex = 31;
            this.chkLoHSelf.Text = "Lay on Hand self";
            this.chkLoHSelf.UseVisualStyleBackColor = true;
            this.chkLoHSelf.CheckedChanged += new System.EventHandler(this.chkLoHSelf_CheckedChanged);
            // 
            // label11
            // 
            this.label11.AutoSize = true;
            this.label11.Location = new System.Drawing.Point(224, 34);
            this.label11.Name = "label11";
            this.label11.Size = new System.Drawing.Size(41, 13);
            this.label11.TabIndex = 30;
            this.label11.Text = "min CD";
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Location = new System.Drawing.Point(6, 16);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(116, 13);
            this.label2.TabIndex = 27;
            this.label2.Text = "Replenishment Uptime:";
            this.label2.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
            // 
            // lblReplenishment
            // 
            this.lblReplenishment.AutoSize = true;
            this.lblReplenishment.Location = new System.Drawing.Point(110, 64);
            this.lblReplenishment.Name = "lblReplenishment";
            this.lblReplenishment.Size = new System.Drawing.Size(33, 13);
            this.lblReplenishment.TabIndex = 26;
            this.lblReplenishment.Text = "100%";
            // 
            // trkReplenishment
            // 
            this.trkReplenishment.BackColor = System.Drawing.SystemColors.ControlLightLight;
            this.trkReplenishment.Location = new System.Drawing.Point(6, 32);
            this.trkReplenishment.Maximum = 100;
            this.trkReplenishment.Name = "trkReplenishment";
            this.trkReplenishment.Size = new System.Drawing.Size(137, 45);
            this.trkReplenishment.TabIndex = 25;
            this.trkReplenishment.TickFrequency = 10;
            this.trkReplenishment.Value = 90;
            this.trkReplenishment.Scroll += new System.EventHandler(this.trkReplenishment_Scroll);
            // 
            // nudDivinePlea
            // 
            this.nudDivinePlea.DecimalPlaces = 1;
            this.nudDivinePlea.Increment = new decimal(new int[] {
            1,
            0,
            0,
            65536});
            this.nudDivinePlea.Location = new System.Drawing.Point(151, 32);
            this.nudDivinePlea.Maximum = new decimal(new int[] {
            20,
            0,
            0,
            0});
            this.nudDivinePlea.Minimum = new decimal(new int[] {
            1,
            0,
            0,
            0});
            this.nudDivinePlea.Name = "nudDivinePlea";
            this.nudDivinePlea.Size = new System.Drawing.Size(71, 20);
            this.nudDivinePlea.TabIndex = 10;
            this.nudDivinePlea.Value = new decimal(new int[] {
            1,
            0,
            0,
            0});
            this.nudDivinePlea.ValueChanged += new System.EventHandler(this.nudDivinePlea_ValueChanged);
            // 
            // lblDivinePlea
            // 
            this.lblDivinePlea.AutoSize = true;
            this.lblDivinePlea.Location = new System.Drawing.Point(148, 16);
            this.lblDivinePlea.Name = "lblDivinePlea";
            this.lblDivinePlea.Size = new System.Drawing.Size(64, 13);
            this.lblDivinePlea.TabIndex = 9;
            this.lblDivinePlea.Text = "Divine Plea:";
            // 
            // trkActivity
            // 
            this.trkActivity.BackColor = System.Drawing.SystemColors.ControlLightLight;
            this.trkActivity.Location = new System.Drawing.Point(151, 19);
            this.trkActivity.Maximum = 100;
            this.trkActivity.Minimum = 10;
            this.trkActivity.Name = "trkActivity";
            this.trkActivity.Size = new System.Drawing.Size(137, 45);
            this.trkActivity.TabIndex = 23;
            this.trkActivity.TickFrequency = 10;
            this.toolTip.SetToolTip(this.trkActivity, "Overall activity.  100% means you\'re always casting with no latency.");
            this.trkActivity.Value = 90;
            this.trkActivity.Scroll += new System.EventHandler(this.trkActivity_Scroll);
            // 
            // lblActivity
            // 
            this.lblActivity.AutoSize = true;
            this.lblActivity.Location = new System.Drawing.Point(255, 53);
            this.lblActivity.Name = "lblActivity";
            this.lblActivity.Size = new System.Drawing.Size(33, 13);
            this.lblActivity.TabIndex = 24;
            this.lblActivity.Text = "100%";
            // 
            // label9
            // 
            this.label9.AutoSize = true;
            this.label9.Location = new System.Drawing.Point(151, 3);
            this.label9.Name = "label9";
            this.label9.Size = new System.Drawing.Size(44, 13);
            this.label9.TabIndex = 25;
            this.label9.Text = "Activity:";
            this.label9.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
            // 
            // label5
            // 
            this.label5.AutoSize = true;
            this.label5.Location = new System.Drawing.Point(12, 337);
            this.label5.Name = "label5";
            this.label5.Size = new System.Drawing.Size(121, 13);
            this.label5.TabIndex = 30;
            this.label5.Text = "Beacon of Light Uptime:";
            this.label5.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
            // 
            // lblBoLUp
            // 
            this.lblBoLUp.AutoSize = true;
            this.lblBoLUp.Location = new System.Drawing.Point(116, 387);
            this.lblBoLUp.Name = "lblBoLUp";
            this.lblBoLUp.Size = new System.Drawing.Size(33, 13);
            this.lblBoLUp.TabIndex = 29;
            this.lblBoLUp.Text = "100%";
            // 
            // trkBoLUp
            // 
            this.trkBoLUp.BackColor = System.Drawing.SystemColors.ControlLightLight;
            this.trkBoLUp.Location = new System.Drawing.Point(12, 353);
            this.trkBoLUp.Maximum = 100;
            this.trkBoLUp.Name = "trkBoLUp";
            this.trkBoLUp.Size = new System.Drawing.Size(137, 45);
            this.trkBoLUp.TabIndex = 28;
            this.trkBoLUp.TickFrequency = 10;
            this.trkBoLUp.Value = 90;
            this.trkBoLUp.Scroll += new System.EventHandler(this.trkBoLUp_Scroll);
            // 
            // trkHS
            // 
            this.trkHS.BackColor = System.Drawing.SystemColors.ControlLightLight;
            this.trkHS.LargeChange = 10;
            this.trkHS.Location = new System.Drawing.Point(3, 83);
            this.trkHS.Maximum = 100;
            this.trkHS.Name = "trkHS";
            this.trkHS.Size = new System.Drawing.Size(137, 45);
            this.trkHS.TabIndex = 29;
            this.trkHS.TickFrequency = 10;
            this.toolTip.SetToolTip(this.trkHS, "Percentage of casts of Holy Shock possible.  ie: In a 1 minute fight, you can cas" +
                    "t up to 10 holy shocks.  50% would mean you only cast 5 holy shocks.");
            this.trkHS.Value = 20;
            this.trkHS.Scroll += new System.EventHandler(this.trkHS_Scroll);
            // 
            // lblHS
            // 
            this.lblHS.Location = new System.Drawing.Point(107, 115);
            this.lblHS.Name = "lblHS";
            this.lblHS.Size = new System.Drawing.Size(33, 13);
            this.lblHS.TabIndex = 34;
            this.lblHS.Text = "100%";
            this.lblHS.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // label7
            // 
            this.label7.AutoSize = true;
            this.label7.Location = new System.Drawing.Point(3, 67);
            this.label7.Name = "label7";
            this.label7.Size = new System.Drawing.Size(65, 13);
            this.label7.TabIndex = 35;
            this.label7.Text = "Holy Shock:";
            this.label7.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
            // 
            // chkJotP
            // 
            this.chkJotP.AutoSize = true;
            this.chkJotP.Location = new System.Drawing.Point(6, 198);
            this.chkJotP.Name = "chkJotP";
            this.chkJotP.Size = new System.Drawing.Size(90, 17);
            this.chkJotP.TabIndex = 37;
            this.chkJotP.Text = "Maintain JotP";
            this.chkJotP.UseVisualStyleBackColor = true;
            this.chkJotP.CheckedChanged += new System.EventHandler(this.chkJotP_CheckedChanged);
            // 
            // label12
            // 
            this.label12.AutoSize = true;
            this.label12.Location = new System.Drawing.Point(148, 224);
            this.label12.Name = "label12";
            this.label12.Size = new System.Drawing.Size(67, 13);
            this.label12.TabIndex = 38;
            this.label12.Text = "GHL targets:";
            // 
            // nudGHL
            // 
            this.nudGHL.DecimalPlaces = 1;
            this.nudGHL.Increment = new decimal(new int[] {
            1,
            0,
            0,
            65536});
            this.nudGHL.Location = new System.Drawing.Point(221, 222);
            this.nudGHL.Maximum = new decimal(new int[] {
            5,
            0,
            0,
            0});
            this.nudGHL.Name = "nudGHL";
            this.nudGHL.Size = new System.Drawing.Size(76, 20);
            this.nudGHL.TabIndex = 39;
            this.nudGHL.ValueChanged += new System.EventHandler(this.nudGHL_ValueChanged);
            // 
            // label19
            // 
            this.label19.AutoSize = true;
            this.label19.Location = new System.Drawing.Point(3, 3);
            this.label19.Name = "label19";
            this.label19.Size = new System.Drawing.Size(64, 13);
            this.label19.TabIndex = 44;
            this.label19.Text = "Burst Scale:";
            this.label19.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
            // 
            // lblBurstScale
            // 
            this.lblBurstScale.AutoSize = true;
            this.lblBurstScale.Location = new System.Drawing.Point(107, 51);
            this.lblBurstScale.Name = "lblBurstScale";
            this.lblBurstScale.Size = new System.Drawing.Size(33, 13);
            this.lblBurstScale.TabIndex = 43;
            this.lblBurstScale.Text = "100%";
            // 
            // trkBurstScale
            // 
            this.trkBurstScale.BackColor = System.Drawing.SystemColors.ControlLightLight;
            this.trkBurstScale.Location = new System.Drawing.Point(3, 19);
            this.trkBurstScale.Maximum = 100;
            this.trkBurstScale.Name = "trkBurstScale";
            this.trkBurstScale.Size = new System.Drawing.Size(137, 45);
            this.trkBurstScale.TabIndex = 42;
            this.trkBurstScale.TickFrequency = 10;
            this.trkBurstScale.Value = 90;
            this.trkBurstScale.Scroll += new System.EventHandler(this.trkBurstScale_Scroll);
            // 
            // trkIoLRatio
            // 
            this.trkIoLRatio.BackColor = System.Drawing.SystemColors.ControlLightLight;
            this.trkIoLRatio.Location = new System.Drawing.Point(151, 83);
            this.trkIoLRatio.Maximum = 100;
            this.trkIoLRatio.Name = "trkIoLRatio";
            this.trkIoLRatio.Size = new System.Drawing.Size(137, 45);
            this.trkIoLRatio.TabIndex = 46;
            this.trkIoLRatio.TickFrequency = 10;
            this.toolTip.SetToolTip(this.trkIoLRatio, "Ratio of spells cast with Infusion of Light proc.");
            this.trkIoLRatio.Value = 90;
            this.trkIoLRatio.Scroll += new System.EventHandler(this.trkIoLRatio_Scroll);
            // 
            // chkIoL
            // 
            this.chkIoL.AutoSize = true;
            this.chkIoL.Location = new System.Drawing.Point(151, 67);
            this.chkIoL.Name = "chkIoL";
            this.chkIoL.Size = new System.Drawing.Size(133, 17);
            this.chkIoL.TabIndex = 48;
            this.chkIoL.Text = "Model Infusion of Light";
            this.chkIoL.UseVisualStyleBackColor = true;
            this.chkIoL.CheckedChanged += new System.EventHandler(this.chkIoL_CheckedChanged);
            // 
            // trkSacredShield
            // 
            this.trkSacredShield.BackColor = System.Drawing.SystemColors.ControlLightLight;
            this.trkSacredShield.Location = new System.Drawing.Point(3, 147);
            this.trkSacredShield.Maximum = 100;
            this.trkSacredShield.Name = "trkSacredShield";
            this.trkSacredShield.Size = new System.Drawing.Size(137, 45);
            this.trkSacredShield.TabIndex = 49;
            this.trkSacredShield.TickFrequency = 10;
            this.toolTip.SetToolTip(this.trkSacredShield, "Amount of time you have Sacred Shield active on a target.");
            this.trkSacredShield.Value = 90;
            this.trkSacredShield.Scroll += new System.EventHandler(this.trkSacredShield_Scroll);
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(3, 131);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(112, 13);
            this.label1.TabIndex = 50;
            this.label1.Text = "Sacred Shield Uptime:";
            this.label1.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
            // 
            // lblSacredShield
            // 
            this.lblSacredShield.AutoSize = true;
            this.lblSacredShield.Location = new System.Drawing.Point(107, 179);
            this.lblSacredShield.Name = "lblSacredShield";
            this.lblSacredShield.Size = new System.Drawing.Size(33, 13);
            this.lblSacredShield.TabIndex = 51;
            this.lblSacredShield.Text = "100%";
            // 
            // trkFlashOfLightOnTank
            // 
            this.trkFlashOfLightOnTank.BackColor = System.Drawing.SystemColors.ControlLightLight;
            this.trkFlashOfLightOnTank.Location = new System.Drawing.Point(151, 147);
            this.trkFlashOfLightOnTank.Maximum = 100;
            this.trkFlashOfLightOnTank.Name = "trkFlashOfLightOnTank";
            this.trkFlashOfLightOnTank.Size = new System.Drawing.Size(137, 45);
            this.trkFlashOfLightOnTank.TabIndex = 58;
            this.trkFlashOfLightOnTank.TickFrequency = 10;
            this.toolTip.SetToolTip(this.trkFlashOfLightOnTank, "Percentage of Flash of Lights you directly apply to your Sacred Shield target.");
            this.trkFlashOfLightOnTank.Value = 90;
            this.trkFlashOfLightOnTank.Scroll += new System.EventHandler(this.trkFlashOfLightOnTank_Scroll);
            // 
            // lblIoLFoL
            // 
            this.lblIoLFoL.AutoSize = true;
            this.lblIoLFoL.Location = new System.Drawing.Point(151, 115);
            this.lblIoLFoL.Name = "lblIoLFoL";
            this.lblIoLFoL.Size = new System.Drawing.Size(25, 13);
            this.lblIoLFoL.TabIndex = 52;
            this.lblIoLFoL.Text = "FoL";
            // 
            // lblIoLHL
            // 
            this.lblIoLHL.Location = new System.Drawing.Point(230, 115);
            this.lblIoLHL.Name = "lblIoLHL";
            this.lblIoLHL.Size = new System.Drawing.Size(58, 13);
            this.lblIoLHL.TabIndex = 53;
            this.lblIoLHL.Text = "100% HL";
            this.lblIoLHL.TextAlign = System.Drawing.ContentAlignment.BottomRight;
            // 
            // chkJudgement
            // 
            this.chkJudgement.AutoSize = true;
            this.chkJudgement.Location = new System.Drawing.Point(6, 221);
            this.chkJudgement.Name = "chkJudgement";
            this.chkJudgement.Size = new System.Drawing.Size(121, 17);
            this.chkJudgement.TabIndex = 55;
            this.chkJudgement.Text = "Maintain Judgement";
            this.chkJudgement.UseVisualStyleBackColor = true;
            this.chkJudgement.CheckedChanged += new System.EventHandler(this.chkJudgement_CheckedChanged);
            // 
            // chkSpiritIrrelevant
            // 
            this.chkSpiritIrrelevant.AutoSize = true;
            this.chkSpiritIrrelevant.Location = new System.Drawing.Point(12, 404);
            this.chkSpiritIrrelevant.Name = "chkSpiritIrrelevant";
            this.chkSpiritIrrelevant.Size = new System.Drawing.Size(110, 17);
            this.chkSpiritIrrelevant.TabIndex = 56;
            this.chkSpiritIrrelevant.Text = "Ignore Spirit Items";
            this.chkSpiritIrrelevant.UseVisualStyleBackColor = true;
            this.chkSpiritIrrelevant.CheckedChanged += new System.EventHandler(this.chkSpiritIrrelevant_CheckedChanged);
            // 
            // chkHitIrrelevant
            // 
            this.chkHitIrrelevant.AutoSize = true;
            this.chkHitIrrelevant.Location = new System.Drawing.Point(157, 404);
            this.chkHitIrrelevant.Name = "chkHitIrrelevant";
            this.chkHitIrrelevant.Size = new System.Drawing.Size(100, 17);
            this.chkHitIrrelevant.TabIndex = 57;
            this.chkHitIrrelevant.Text = "Ignore Hit Items";
            this.chkHitIrrelevant.UseVisualStyleBackColor = true;
            this.chkHitIrrelevant.CheckedChanged += new System.EventHandler(this.chkHitIrrelevant_CheckedChanged);
            // 
            // label8
            // 
            this.label8.AutoSize = true;
            this.label8.Location = new System.Drawing.Point(151, 131);
            this.label8.Name = "label8";
            this.label8.Size = new System.Drawing.Size(135, 13);
            this.label8.TabIndex = 59;
            this.label8.Text = "Flash of Light on SS target:";
            this.label8.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
            // 
            // lblFlashOfLightOnTank
            // 
            this.lblFlashOfLightOnTank.AutoSize = true;
            this.lblFlashOfLightOnTank.Location = new System.Drawing.Point(255, 180);
            this.lblFlashOfLightOnTank.Name = "lblFlashOfLightOnTank";
            this.lblFlashOfLightOnTank.Size = new System.Drawing.Size(33, 13);
            this.lblFlashOfLightOnTank.TabIndex = 60;
            this.lblFlashOfLightOnTank.Text = "100%";
            // 
            // CalculationOptionsPanelHealadin
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.Controls.Add(this.label5);
            this.Controls.Add(this.lblBoLUp);
            this.Controls.Add(this.lblFlashOfLightOnTank);
            this.Controls.Add(this.trkBoLUp);
            this.Controls.Add(this.label8);
            this.Controls.Add(this.trkFlashOfLightOnTank);
            this.Controls.Add(this.chkHitIrrelevant);
            this.Controls.Add(this.chkSpiritIrrelevant);
            this.Controls.Add(this.chkJudgement);
            this.Controls.Add(this.lblIoLHL);
            this.Controls.Add(this.lblIoLFoL);
            this.Controls.Add(this.lblSacredShield);
            this.Controls.Add(this.label1);
            this.Controls.Add(this.trkSacredShield);
            this.Controls.Add(this.chkIoL);
            this.Controls.Add(this.trkIoLRatio);
            this.Controls.Add(this.label19);
            this.Controls.Add(this.lblBurstScale);
            this.Controls.Add(this.trkBurstScale);
            this.Controls.Add(this.nudGHL);
            this.Controls.Add(this.label12);
            this.Controls.Add(this.chkJotP);
            this.Controls.Add(this.label7);
            this.Controls.Add(this.lblHS);
            this.Controls.Add(this.trkHS);
            this.Controls.Add(this.label9);
            this.Controls.Add(this.groupBox2);
            this.Controls.Add(this.lblActivity);
            this.Controls.Add(this.trkActivity);
            this.Controls.Add(this.label3);
            this.Controls.Add(this.cmbLength);
            this.Name = "CalculationOptionsPanelHealadin";
            this.Size = new System.Drawing.Size(300, 456);
            ((System.ComponentModel.ISupportInitialize)(this.cmbLength)).EndInit();
            this.groupBox2.ResumeLayout(false);
            this.groupBox2.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.trkReplenishment)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.nudDivinePlea)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.trkActivity)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.trkBoLUp)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.trkHS)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.nudGHL)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.trkBurstScale)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.trkIoLRatio)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.trkSacredShield)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.trkFlashOfLightOnTank)).EndInit();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.NumericUpDown cmbLength;
        private System.Windows.Forms.Label label3;
        private System.Windows.Forms.GroupBox groupBox2;
        private System.Windows.Forms.TrackBar trkActivity;
        private System.Windows.Forms.Label lblActivity;
        private System.Windows.Forms.Label label9;
        private System.Windows.Forms.NumericUpDown nudDivinePlea;
        private System.Windows.Forms.Label lblDivinePlea;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.Label lblReplenishment;
        private System.Windows.Forms.TrackBar trkReplenishment;
        private System.Windows.Forms.Label label5;
        private System.Windows.Forms.Label lblBoLUp;
        private System.Windows.Forms.TrackBar trkBoLUp;
        private System.Windows.Forms.TrackBar trkHS;
        private System.Windows.Forms.Label lblHS;
        private System.Windows.Forms.Label label7;
        private System.Windows.Forms.Label label11;
        private System.Windows.Forms.CheckBox chkLoHSelf;
        private System.Windows.Forms.CheckBox chkJotP;
        private System.Windows.Forms.Label label12;
        private System.Windows.Forms.NumericUpDown nudGHL;
        private System.Windows.Forms.Label label19;
        private System.Windows.Forms.Label lblBurstScale;
        private System.Windows.Forms.TrackBar trkBurstScale;
        private System.Windows.Forms.TrackBar trkIoLRatio;
        private System.Windows.Forms.CheckBox chkIoL;
        private System.Windows.Forms.TrackBar trkSacredShield;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.Label lblSacredShield;
        private System.Windows.Forms.ToolTip toolTip;
        private System.Windows.Forms.Label lblIoLFoL;
        private System.Windows.Forms.Label lblIoLHL;
        private System.Windows.Forms.CheckBox chkJudgement;
        private System.Windows.Forms.CheckBox chkSpiritIrrelevant;
        private System.Windows.Forms.CheckBox chkHitIrrelevant;
        private System.Windows.Forms.Label label8;
        private System.Windows.Forms.TrackBar trkFlashOfLightOnTank;
        private System.Windows.Forms.Label lblFlashOfLightOnTank;
    }
}
