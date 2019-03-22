namespace Rawr.TankDK
{
    partial class CalculationOptionsPanelTankDK
    {
        /// <summary>
        /// Erforderliche Designervariable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        /// Verwendete Ressourcen bereinigen.
        /// </summary>
        /// <param name="disposing">True, wenn verwaltete Ressourcen gelöscht werden sollen; andernfalls False.</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Vom Windows Form-Designer generierter Code

        /// <summary>
        /// Erforderliche Methode für die Designerunterstützung.
        /// Der Inhalt der Methode darf nicht mit dem Code-Editor geändert werden.
        /// </summary>
        private void InitializeComponent()
        {
            this.label1 = new System.Windows.Forms.Label();
            this.cmbAttackerLevel = new System.Windows.Forms.ComboBox();
            this.label2 = new System.Windows.Forms.Label();
            this.numThreatWeight = new System.Windows.Forms.NumericUpDown();
            this.label3 = new System.Windows.Forms.Label();
            this.numSurvivalWeight = new System.Windows.Forms.NumericUpDown();
            this.numIncomingDamage = new System.Windows.Forms.NumericUpDown();
            this.label4 = new System.Windows.Forms.Label();
            this.numBossAttackSpeed = new System.Windows.Forms.NumericUpDown();
            this.label5 = new System.Windows.Forms.Label();
            this.numIncFromMagicFrequency = new System.Windows.Forms.NumericUpDown();
            this.label6 = new System.Windows.Forms.Label();
            this.nudTargetArmor = new System.Windows.Forms.NumericUpDown();
            this.lblTargetArmor = new System.Windows.Forms.Label();
            this.tbFightLength = new System.Windows.Forms.TrackBar();
            this.btnRotation = new System.Windows.Forms.Button();
            this.gbFightInfo = new System.Windows.Forms.GroupBox();
            this.numTargets = new System.Windows.Forms.NumericUpDown();
            this.label7 = new System.Windows.Forms.Label();
            this.lblFightLengthNum = new System.Windows.Forms.Label();
            this.lblFightLength = new System.Windows.Forms.Label();
            this.comboChartType = new System.Windows.Forms.ComboBox();
            this.label8 = new System.Windows.Forms.Label();
            this.btnStatsGraph = new System.Windows.Forms.Button();
            this.cb_AdditiveMitigation = new System.Windows.Forms.CheckBox();
            this.cbExperimental = new System.Windows.Forms.CheckBox();
            this.cb_ParryHaste = new System.Windows.Forms.CheckBox();
            this.numIncMagicDamage = new System.Windows.Forms.NumericUpDown();
            this.label9 = new System.Windows.Forms.Label();
            this.numBleedPerTick = new System.Windows.Forms.NumericUpDown();
            this.label10 = new System.Windows.Forms.Label();
            this.numBleedTickFreq = new System.Windows.Forms.NumericUpDown();
            this.label11 = new System.Windows.Forms.Label();
            this.numMitigationWeight = new System.Windows.Forms.NumericUpDown();
            this.label12 = new System.Windows.Forms.Label();
            this.cbUseOnUse = new System.Windows.Forms.CheckBox();
            ((System.ComponentModel.ISupportInitialize)(this.numThreatWeight)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.numSurvivalWeight)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.numIncomingDamage)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.numBossAttackSpeed)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.numIncFromMagicFrequency)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.nudTargetArmor)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.tbFightLength)).BeginInit();
            this.gbFightInfo.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.numTargets)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.numIncMagicDamage)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.numBleedPerTick)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.numBleedTickFreq)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.numMitigationWeight)).BeginInit();
            this.SuspendLayout();
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(3, 70);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(76, 13);
            this.label1.TabIndex = 0;
            this.label1.Text = "Attacker Level";
            // 
            // cmbAttackerLevel
            // 
            this.cmbAttackerLevel.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.cmbAttackerLevel.FormattingEnabled = true;
            this.cmbAttackerLevel.Items.AddRange(new object[] {
            "80",
            "81",
            "82",
            "83"});
            this.cmbAttackerLevel.Location = new System.Drawing.Point(150, 70);
            this.cmbAttackerLevel.Name = "cmbAttackerLevel";
            this.cmbAttackerLevel.Size = new System.Drawing.Size(105, 21);
            this.cmbAttackerLevel.TabIndex = 5;
            this.cmbAttackerLevel.SelectedIndexChanged += new System.EventHandler(this.cmbAttackerLevel_SelectedIndexChanged);
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Location = new System.Drawing.Point(3, 4);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(75, 13);
            this.label2.TabIndex = 2;
            this.label2.Text = "Threat Weight";
            // 
            // numThreatWeight
            // 
            this.numThreatWeight.DecimalPlaces = 2;
            this.numThreatWeight.Increment = new decimal(new int[] {
            5,
            0,
            0,
            131072});
            this.numThreatWeight.Location = new System.Drawing.Point(150, 4);
            this.numThreatWeight.Name = "numThreatWeight";
            this.numThreatWeight.Size = new System.Drawing.Size(105, 20);
            this.numThreatWeight.TabIndex = 1;
            this.numThreatWeight.Value = new decimal(new int[] {
            10,
            0,
            0,
            65536});
            this.numThreatWeight.ValueChanged += new System.EventHandler(this.numThreatWeight_ValueChanged);
            // 
            // label3
            // 
            this.label3.AutoSize = true;
            this.label3.Location = new System.Drawing.Point(3, 23);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(82, 13);
            this.label3.TabIndex = 4;
            this.label3.Text = "Survival Weight";
            // 
            // numSurvivalWeight
            // 
            this.numSurvivalWeight.DecimalPlaces = 2;
            this.numSurvivalWeight.Increment = new decimal(new int[] {
            5,
            0,
            0,
            131072});
            this.numSurvivalWeight.Location = new System.Drawing.Point(150, 23);
            this.numSurvivalWeight.Maximum = new decimal(new int[] {
            10,
            0,
            0,
            0});
            this.numSurvivalWeight.Name = "numSurvivalWeight";
            this.numSurvivalWeight.Size = new System.Drawing.Size(105, 20);
            this.numSurvivalWeight.TabIndex = 3;
            this.numSurvivalWeight.Value = new decimal(new int[] {
            10,
            0,
            0,
            65536});
            this.numSurvivalWeight.ValueChanged += new System.EventHandler(this.numSurvivalWeight_ValueChanged);
            // 
            // numIncomingDamage
            // 
            this.numIncomingDamage.Increment = new decimal(new int[] {
            1000,
            0,
            0,
            0});
            this.numIncomingDamage.Location = new System.Drawing.Point(150, 116);
            this.numIncomingDamage.Maximum = new decimal(new int[] {
            500000,
            0,
            0,
            0});
            this.numIncomingDamage.Minimum = new decimal(new int[] {
            1000,
            0,
            0,
            0});
            this.numIncomingDamage.Name = "numIncomingDamage";
            this.numIncomingDamage.Size = new System.Drawing.Size(105, 20);
            this.numIncomingDamage.TabIndex = 9;
            this.numIncomingDamage.ThousandsSeparator = true;
            this.numIncomingDamage.Value = new decimal(new int[] {
            10000,
            0,
            0,
            0});
            this.numIncomingDamage.ValueChanged += new System.EventHandler(this.numIncomingDamage_ValueChanged);
            // 
            // label4
            // 
            this.label4.AutoSize = true;
            this.label4.Location = new System.Drawing.Point(3, 118);
            this.label4.Name = "label4";
            this.label4.Size = new System.Drawing.Size(137, 13);
            this.label4.TabIndex = 6;
            this.label4.Text = "Incoming Damage Per Shot";
            // 
            // numBossAttackSpeed
            // 
            this.numBossAttackSpeed.DecimalPlaces = 2;
            this.numBossAttackSpeed.Increment = new decimal(new int[] {
            5,
            0,
            0,
            65536});
            this.numBossAttackSpeed.Location = new System.Drawing.Point(150, 97);
            this.numBossAttackSpeed.Maximum = new decimal(new int[] {
            5,
            0,
            0,
            0});
            this.numBossAttackSpeed.Minimum = new decimal(new int[] {
            1,
            0,
            0,
            0});
            this.numBossAttackSpeed.Name = "numBossAttackSpeed";
            this.numBossAttackSpeed.Size = new System.Drawing.Size(105, 20);
            this.numBossAttackSpeed.TabIndex = 7;
            this.numBossAttackSpeed.Value = new decimal(new int[] {
            25,
            0,
            0,
            65536});
            this.numBossAttackSpeed.ValueChanged += new System.EventHandler(this.numBossAttackSpeed_ValueChanged);
            // 
            // label5
            // 
            this.label5.AutoSize = true;
            this.label5.Location = new System.Drawing.Point(3, 99);
            this.label5.Name = "label5";
            this.label5.Size = new System.Drawing.Size(140, 13);
            this.label5.TabIndex = 8;
            this.label5.Text = "Boss Physical Attack Speed";
            // 
            // numIncFromMagicFrequency
            // 
            this.numIncFromMagicFrequency.DecimalPlaces = 2;
            this.numIncFromMagicFrequency.Increment = new decimal(new int[] {
            5,
            0,
            0,
            65536});
            this.numIncFromMagicFrequency.Location = new System.Drawing.Point(150, 143);
            this.numIncFromMagicFrequency.Maximum = new decimal(new int[] {
            120,
            0,
            0,
            0});
            this.numIncFromMagicFrequency.Name = "numIncFromMagicFrequency";
            this.numIncFromMagicFrequency.Size = new System.Drawing.Size(105, 20);
            this.numIncFromMagicFrequency.TabIndex = 11;
            this.numIncFromMagicFrequency.ValueChanged += new System.EventHandler(this.numPercIncFromMagic_ValueChanged);
            // 
            // label6
            // 
            this.label6.AutoSize = true;
            this.label6.Location = new System.Drawing.Point(3, 145);
            this.label6.Name = "label6";
            this.label6.Size = new System.Drawing.Size(123, 13);
            this.label6.TabIndex = 10;
            this.label6.Text = "Magic Attack Frequency";
            // 
            // nudTargetArmor
            // 
            this.nudTargetArmor.Increment = new decimal(new int[] {
            100,
            0,
            0,
            0});
            this.nudTargetArmor.Location = new System.Drawing.Point(135, 17);
            this.nudTargetArmor.Maximum = new decimal(new int[] {
            30000,
            0,
            0,
            0});
            this.nudTargetArmor.Name = "nudTargetArmor";
            this.nudTargetArmor.Size = new System.Drawing.Size(84, 20);
            this.nudTargetArmor.TabIndex = 13;
            this.nudTargetArmor.Value = new decimal(new int[] {
            10643,
            0,
            0,
            0});
            // 
            // lblTargetArmor
            // 
            this.lblTargetArmor.AutoSize = true;
            this.lblTargetArmor.Location = new System.Drawing.Point(6, 20);
            this.lblTargetArmor.Name = "lblTargetArmor";
            this.lblTargetArmor.Size = new System.Drawing.Size(71, 13);
            this.lblTargetArmor.TabIndex = 24;
            this.lblTargetArmor.Text = "Target Armor:";
            // 
            // tbFightLength
            // 
            this.tbFightLength.BackColor = System.Drawing.SystemColors.ControlLightLight;
            this.tbFightLength.Location = new System.Drawing.Point(150, 510);
            this.tbFightLength.Name = "tbFightLength";
            this.tbFightLength.Size = new System.Drawing.Size(86, 45);
            this.tbFightLength.TabIndex = 15;
            this.tbFightLength.Value = 10;
            this.tbFightLength.Visible = false;
            this.tbFightLength.Scroll += new System.EventHandler(this.tbFightLength_Scroll);
            // 
            // btnRotation
            // 
            this.btnRotation.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.btnRotation.Location = new System.Drawing.Point(47, 73);
            this.btnRotation.Name = "btnRotation";
            this.btnRotation.Size = new System.Drawing.Size(125, 23);
            this.btnRotation.TabIndex = 17;
            this.btnRotation.Text = "Rotation Details";
            this.btnRotation.UseVisualStyleBackColor = true;
            this.btnRotation.Click += new System.EventHandler(this.btnRotation_Click);
            // 
            // gbFightInfo
            // 
            this.gbFightInfo.Controls.Add(this.numTargets);
            this.gbFightInfo.Controls.Add(this.btnRotation);
            this.gbFightInfo.Controls.Add(this.label7);
            this.gbFightInfo.Controls.Add(this.nudTargetArmor);
            this.gbFightInfo.Controls.Add(this.lblTargetArmor);
            this.gbFightInfo.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.gbFightInfo.Location = new System.Drawing.Point(14, 241);
            this.gbFightInfo.Name = "gbFightInfo";
            this.gbFightInfo.Size = new System.Drawing.Size(227, 110);
            this.gbFightInfo.TabIndex = 37;
            this.gbFightInfo.TabStop = false;
            this.gbFightInfo.Text = "Fight Info";
            // 
            // numTargets
            // 
            this.numTargets.Location = new System.Drawing.Point(136, 44);
            this.numTargets.Maximum = new decimal(new int[] {
            5,
            0,
            0,
            0});
            this.numTargets.Minimum = new decimal(new int[] {
            1,
            0,
            0,
            0});
            this.numTargets.Name = "numTargets";
            this.numTargets.Size = new System.Drawing.Size(84, 20);
            this.numTargets.TabIndex = 15;
            this.numTargets.Value = new decimal(new int[] {
            1,
            0,
            0,
            0});
            this.numTargets.ValueChanged += new System.EventHandler(this.numTargets_ValueChanged);
            // 
            // label7
            // 
            this.label7.AutoSize = true;
            this.label7.Location = new System.Drawing.Point(7, 46);
            this.label7.Name = "label7";
            this.label7.Size = new System.Drawing.Size(53, 13);
            this.label7.TabIndex = 38;
            this.label7.Text = "# Targets";
            // 
            // lblFightLengthNum
            // 
            this.lblFightLengthNum.AutoSize = true;
            this.lblFightLengthNum.Location = new System.Drawing.Point(217, 542);
            this.lblFightLengthNum.Name = "lblFightLengthNum";
            this.lblFightLengthNum.Size = new System.Drawing.Size(19, 13);
            this.lblFightLengthNum.TabIndex = 2;
            this.lblFightLengthNum.Text = "10";
            this.lblFightLengthNum.Visible = false;
            // 
            // lblFightLength
            // 
            this.lblFightLength.AutoSize = true;
            this.lblFightLength.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lblFightLength.Location = new System.Drawing.Point(21, 510);
            this.lblFightLength.Name = "lblFightLength";
            this.lblFightLength.Size = new System.Drawing.Size(114, 13);
            this.lblFightLength.TabIndex = 0;
            this.lblFightLength.Text = "Fight Length (minutes):";
            this.lblFightLength.Visible = false;
            // 
            // comboChartType
            // 
            this.comboChartType.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.comboChartType.FormattingEnabled = true;
            this.comboChartType.Items.AddRange(new object[] {
            "Default",
            "Burst/Reaction"});
            this.comboChartType.Location = new System.Drawing.Point(147, 345);
            this.comboChartType.Name = "comboChartType";
            this.comboChartType.Size = new System.Drawing.Size(105, 21);
            this.comboChartType.TabIndex = 39;
            this.comboChartType.SelectedIndexChanged += new System.EventHandler(this.comboChartType_SelectedIndexChanged);
            // 
            // label8
            // 
            this.label8.AutoSize = true;
            this.label8.Location = new System.Drawing.Point(0, 345);
            this.label8.Name = "label8";
            this.label8.Size = new System.Drawing.Size(89, 13);
            this.label8.TabIndex = 38;
            this.label8.Text = "Main Graph Type";
            // 
            // btnStatsGraph
            // 
            this.btnStatsGraph.Location = new System.Drawing.Point(61, 474);
            this.btnStatsGraph.Name = "btnStatsGraph";
            this.btnStatsGraph.Size = new System.Drawing.Size(113, 30);
            this.btnStatsGraph.TabIndex = 62;
            this.btnStatsGraph.Text = "Display Stats Graph";
            this.btnStatsGraph.UseVisualStyleBackColor = true;
            this.btnStatsGraph.Click += new System.EventHandler(this.btnStatsGraph_Click_1);
            // 
            // cb_AdditiveMitigation
            // 
            this.cb_AdditiveMitigation.AutoSize = true;
            this.cb_AdditiveMitigation.Location = new System.Drawing.Point(3, 397);
            this.cb_AdditiveMitigation.Name = "cb_AdditiveMitigation";
            this.cb_AdditiveMitigation.Size = new System.Drawing.Size(180, 17);
            this.cb_AdditiveMitigation.TabIndex = 63;
            this.cb_AdditiveMitigation.Text = "Additive Mitigation (experimental)";
            this.cb_AdditiveMitigation.UseVisualStyleBackColor = true;
            this.cb_AdditiveMitigation.CheckedChanged += new System.EventHandler(this.cb_AdditiveMitigation_CheckedChanged);
            // 
            // cbExperimental
            // 
            this.cbExperimental.AutoSize = true;
            this.cbExperimental.Location = new System.Drawing.Point(3, 374);
            this.cbExperimental.Name = "cbExperimental";
            this.cbExperimental.Size = new System.Drawing.Size(150, 17);
            this.cbExperimental.TabIndex = 64;
            this.cbExperimental.Text = "Enable Experimental Code";
            this.cbExperimental.UseVisualStyleBackColor = true;
            this.cbExperimental.CheckedChanged += new System.EventHandler(this.cbExperimental_CheckedChanged);
            // 
            // cb_ParryHaste
            // 
            this.cb_ParryHaste.AutoSize = true;
            this.cb_ParryHaste.Checked = true;
            this.cb_ParryHaste.CheckState = System.Windows.Forms.CheckState.Checked;
            this.cb_ParryHaste.Location = new System.Drawing.Point(3, 420);
            this.cb_ParryHaste.Name = "cb_ParryHaste";
            this.cb_ParryHaste.Size = new System.Drawing.Size(117, 17);
            this.cb_ParryHaste.TabIndex = 65;
            this.cb_ParryHaste.Text = "Enable Parry Haste";
            this.cb_ParryHaste.UseVisualStyleBackColor = true;
            this.cb_ParryHaste.CheckedChanged += new System.EventHandler(this.cb_ParryHaste_CheckedChanged);
            // 
            // numIncMagicDamage
            // 
            this.numIncMagicDamage.Increment = new decimal(new int[] {
            1000,
            0,
            0,
            0});
            this.numIncMagicDamage.Location = new System.Drawing.Point(150, 162);
            this.numIncMagicDamage.Maximum = new decimal(new int[] {
            500000,
            0,
            0,
            0});
            this.numIncMagicDamage.Name = "numIncMagicDamage";
            this.numIncMagicDamage.Size = new System.Drawing.Size(105, 20);
            this.numIncMagicDamage.TabIndex = 67;
            this.numIncMagicDamage.ThousandsSeparator = true;
            this.numIncMagicDamage.ValueChanged += new System.EventHandler(this.numIncMagicDamage_ValueChanged);
            // 
            // label9
            // 
            this.label9.AutoSize = true;
            this.label9.Location = new System.Drawing.Point(3, 164);
            this.label9.Name = "label9";
            this.label9.Size = new System.Drawing.Size(123, 13);
            this.label9.TabIndex = 66;
            this.label9.Text = "Inc Magic Dmg Per Shot";
            // 
            // numBleedPerTick
            // 
            this.numBleedPerTick.Increment = new decimal(new int[] {
            1000,
            0,
            0,
            0});
            this.numBleedPerTick.Location = new System.Drawing.Point(150, 210);
            this.numBleedPerTick.Maximum = new decimal(new int[] {
            50000,
            0,
            0,
            0});
            this.numBleedPerTick.Name = "numBleedPerTick";
            this.numBleedPerTick.Size = new System.Drawing.Size(105, 20);
            this.numBleedPerTick.TabIndex = 71;
            this.numBleedPerTick.ThousandsSeparator = true;
            this.numBleedPerTick.ValueChanged += new System.EventHandler(this.numBleedPerTick_ValueChanged);
            // 
            // label10
            // 
            this.label10.AutoSize = true;
            this.label10.Location = new System.Drawing.Point(3, 212);
            this.label10.Name = "label10";
            this.label10.Size = new System.Drawing.Size(120, 13);
            this.label10.TabIndex = 70;
            this.label10.Text = "Inc Bleed Dmg Per Tick";
            // 
            // numBleedTickFreq
            // 
            this.numBleedTickFreq.DecimalPlaces = 2;
            this.numBleedTickFreq.Increment = new decimal(new int[] {
            5,
            0,
            0,
            65536});
            this.numBleedTickFreq.Location = new System.Drawing.Point(150, 191);
            this.numBleedTickFreq.Maximum = new decimal(new int[] {
            10,
            0,
            0,
            0});
            this.numBleedTickFreq.Name = "numBleedTickFreq";
            this.numBleedTickFreq.Size = new System.Drawing.Size(105, 20);
            this.numBleedTickFreq.TabIndex = 69;
            this.numBleedTickFreq.ValueChanged += new System.EventHandler(this.numBleedTickFreq_ValueChanged);
            // 
            // label11
            // 
            this.label11.AutoSize = true;
            this.label11.Location = new System.Drawing.Point(3, 193);
            this.label11.Name = "label11";
            this.label11.Size = new System.Drawing.Size(111, 13);
            this.label11.TabIndex = 68;
            this.label11.Text = "Bleed Tick Frequency";
            // 
            // numMitigationWeight
            // 
            this.numMitigationWeight.DecimalPlaces = 2;
            this.numMitigationWeight.Increment = new decimal(new int[] {
            5,
            0,
            0,
            65536});
            this.numMitigationWeight.Location = new System.Drawing.Point(150, 41);
            this.numMitigationWeight.Maximum = new decimal(new int[] {
            10,
            0,
            0,
            0});
            this.numMitigationWeight.Name = "numMitigationWeight";
            this.numMitigationWeight.Size = new System.Drawing.Size(105, 20);
            this.numMitigationWeight.TabIndex = 72;
            this.numMitigationWeight.Value = new decimal(new int[] {
            4,
            0,
            0,
            0});
            this.numMitigationWeight.ValueChanged += new System.EventHandler(this.numMitigationWeight_ValueChanged);
            // 
            // label12
            // 
            this.label12.AutoSize = true;
            this.label12.Location = new System.Drawing.Point(3, 41);
            this.label12.Name = "label12";
            this.label12.Size = new System.Drawing.Size(120, 13);
            this.label12.TabIndex = 73;
            this.label12.Text = "Mitigation Weight (secs)";
            // 
            // cbUseOnUse
            // 
            this.cbUseOnUse.AutoSize = true;
            this.cbUseOnUse.Checked = true;
            this.cbUseOnUse.CheckState = System.Windows.Forms.CheckState.Checked;
            this.cbUseOnUse.Location = new System.Drawing.Point(3, 443);
            this.cbUseOnUse.Name = "cbUseOnUse";
            this.cbUseOnUse.Size = new System.Drawing.Size(136, 17);
            this.cbUseOnUse.TabIndex = 74;
            this.cbUseOnUse.Text = "Enable On Use Abilities";
            this.cbUseOnUse.UseVisualStyleBackColor = true;
            this.cbUseOnUse.CheckedChanged += new System.EventHandler(this.cbUseOnUse_CheckedChanged);
            // 
            // CalculationOptionsPanelTankDK
            // 
            this.Controls.Add(this.cbUseOnUse);
            this.Controls.Add(this.numMitigationWeight);
            this.Controls.Add(this.label12);
            this.Controls.Add(this.numBleedPerTick);
            this.Controls.Add(this.label10);
            this.Controls.Add(this.numBleedTickFreq);
            this.Controls.Add(this.label11);
            this.Controls.Add(this.numIncMagicDamage);
            this.Controls.Add(this.label9);
            this.Controls.Add(this.cb_ParryHaste);
            this.Controls.Add(this.cbExperimental);
            this.Controls.Add(this.cb_AdditiveMitigation);
            this.Controls.Add(this.btnStatsGraph);
            this.Controls.Add(this.comboChartType);
            this.Controls.Add(this.label8);
            this.Controls.Add(this.gbFightInfo);
            this.Controls.Add(this.numIncFromMagicFrequency);
            this.Controls.Add(this.label6);
            this.Controls.Add(this.numBossAttackSpeed);
            this.Controls.Add(this.lblFightLengthNum);
            this.Controls.Add(this.label5);
            this.Controls.Add(this.numIncomingDamage);
            this.Controls.Add(this.tbFightLength);
            this.Controls.Add(this.lblFightLength);
            this.Controls.Add(this.label4);
            this.Controls.Add(this.numSurvivalWeight);
            this.Controls.Add(this.label3);
            this.Controls.Add(this.numThreatWeight);
            this.Controls.Add(this.label2);
            this.Controls.Add(this.cmbAttackerLevel);
            this.Controls.Add(this.label1);
            this.Name = "CalculationOptionsPanelTankDK";
            this.Size = new System.Drawing.Size(258, 642);
            ((System.ComponentModel.ISupportInitialize)(this.numThreatWeight)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.numSurvivalWeight)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.numIncomingDamage)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.numBossAttackSpeed)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.numIncFromMagicFrequency)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.nudTargetArmor)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.tbFightLength)).EndInit();
            this.gbFightInfo.ResumeLayout(false);
            this.gbFightInfo.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.numTargets)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.numIncMagicDamage)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.numBleedPerTick)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.numBleedTickFreq)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.numMitigationWeight)).EndInit();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.ComboBox cmbAttackerLevel;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.NumericUpDown numThreatWeight;
        private System.Windows.Forms.Label label3;
        private System.Windows.Forms.NumericUpDown numSurvivalWeight;
        private System.Windows.Forms.NumericUpDown numIncomingDamage;
        private System.Windows.Forms.Label label4;
        private System.Windows.Forms.NumericUpDown numBossAttackSpeed;
        private System.Windows.Forms.Label label5;
        private System.Windows.Forms.NumericUpDown numIncFromMagicFrequency;
        private System.Windows.Forms.Label label6;
        private System.Windows.Forms.NumericUpDown nudTargetArmor;
        private System.Windows.Forms.Label lblTargetArmor;
        private System.Windows.Forms.TrackBar tbFightLength;
        private System.Windows.Forms.Button btnRotation;
        private System.Windows.Forms.GroupBox gbFightInfo;
        private System.Windows.Forms.Label lblFightLengthNum;
        private System.Windows.Forms.Label lblFightLength;
        private System.Windows.Forms.NumericUpDown numTargets;
        private System.Windows.Forms.Label label7;
        private System.Windows.Forms.ComboBox comboChartType;
        private System.Windows.Forms.Label label8;
        private System.Windows.Forms.Button btnStatsGraph;
        private System.Windows.Forms.CheckBox cb_AdditiveMitigation;
        private System.Windows.Forms.CheckBox cbExperimental;
        private System.Windows.Forms.CheckBox cb_ParryHaste;
        private System.Windows.Forms.NumericUpDown numIncMagicDamage;
        private System.Windows.Forms.Label label9;
        private System.Windows.Forms.NumericUpDown numBleedPerTick;
        private System.Windows.Forms.Label label10;
        private System.Windows.Forms.NumericUpDown numBleedTickFreq;
        private System.Windows.Forms.Label label11;
        private System.Windows.Forms.NumericUpDown numMitigationWeight;
        private System.Windows.Forms.Label label12;
        private System.Windows.Forms.CheckBox cbUseOnUse;
    }
}
