namespace Rawr.Bear
{
	partial class CalculationOptionsPanelBear
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
			System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(CalculationOptionsPanelBear));
			this.label1 = new System.Windows.Forms.Label();
			this.comboBoxTargetLevel = new System.Windows.Forms.ComboBox();
			this.label2 = new System.Windows.Forms.Label();
			this.numericUpDownThreatValue = new System.Windows.Forms.NumericUpDown();
			this.comboBoxThreatValue = new System.Windows.Forms.ComboBox();
			this.numericUpDownTargetArmor = new System.Windows.Forms.NumericUpDown();
			this.label3 = new System.Windows.Forms.Label();
			this.groupBox1 = new System.Windows.Forms.GroupBox();
			this.checkBoxMaul = new System.Windows.Forms.CheckBox();
			this.checkBoxLacerate = new System.Windows.Forms.CheckBox();
			this.checkBoxMangle = new System.Windows.Forms.CheckBox();
			this.checkBoxFaerieFire = new System.Windows.Forms.CheckBox();
			this.checkBoxSwipe = new System.Windows.Forms.CheckBox();
			this.label4 = new System.Windows.Forms.Label();
			this.numericUpDownSurvivalSoftCap = new System.Windows.Forms.NumericUpDown();
			this.label5 = new System.Windows.Forms.Label();
			this.numericUpDownTargetDamage = new System.Windows.Forms.NumericUpDown();
			this.label6 = new System.Windows.Forms.Label();
			this.numericUpDownTargetAttackSpeed = new System.Windows.Forms.NumericUpDown();
			this.comboBoxTargetDamage = new System.Windows.Forms.ComboBox();
			this.comboBoxSurvivalSoftCap = new System.Windows.Forms.ComboBox();
			this.checkBoxTargetParryHastes = new System.Windows.Forms.CheckBox();
			this.groupBox2 = new System.Windows.Forms.GroupBox();
			this.label7 = new System.Windows.Forms.Label();
			this.trackBarTemporarySurvivalEffectValue = new System.Windows.Forms.TrackBar();
			this.label8 = new System.Windows.Forms.Label();
			this.label9 = new System.Windows.Forms.Label();
			this.label10 = new System.Windows.Forms.Label();
			this.panel1 = new System.Windows.Forms.Panel();
			this.panel2 = new System.Windows.Forms.Panel();
			this.panel3 = new System.Windows.Forms.Panel();
			((System.ComponentModel.ISupportInitialize)(this.numericUpDownThreatValue)).BeginInit();
			((System.ComponentModel.ISupportInitialize)(this.numericUpDownTargetArmor)).BeginInit();
			this.groupBox1.SuspendLayout();
			((System.ComponentModel.ISupportInitialize)(this.numericUpDownSurvivalSoftCap)).BeginInit();
			((System.ComponentModel.ISupportInitialize)(this.numericUpDownTargetDamage)).BeginInit();
			((System.ComponentModel.ISupportInitialize)(this.numericUpDownTargetAttackSpeed)).BeginInit();
			this.groupBox2.SuspendLayout();
			((System.ComponentModel.ISupportInitialize)(this.trackBarTemporarySurvivalEffectValue)).BeginInit();
			this.SuspendLayout();
			// 
			// label1
			// 
			this.label1.AutoSize = true;
			this.label1.Location = new System.Drawing.Point(3, 6);
			this.label1.Name = "label1";
			this.label1.Size = new System.Drawing.Size(73, 13);
			this.label1.TabIndex = 0;
			this.label1.Text = "Target Level: ";
			// 
			// comboBoxTargetLevel
			// 
			this.comboBoxTargetLevel.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left)
						| System.Windows.Forms.AnchorStyles.Right)));
			this.comboBoxTargetLevel.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
			this.comboBoxTargetLevel.FormattingEnabled = true;
			this.comboBoxTargetLevel.Items.AddRange(new object[] {
            "80",
            "81",
            "82",
            "83"});
			this.comboBoxTargetLevel.Location = new System.Drawing.Point(146, 3);
			this.comboBoxTargetLevel.Name = "comboBoxTargetLevel";
			this.comboBoxTargetLevel.Size = new System.Drawing.Size(111, 21);
			this.comboBoxTargetLevel.TabIndex = 1;
			this.comboBoxTargetLevel.SelectedIndexChanged += new System.EventHandler(this.calculationOptionControl_Changed);
			// 
			// label2
			// 
			this.label2.AutoSize = true;
			this.label2.Location = new System.Drawing.Point(3, 33);
			this.label2.Name = "label2";
			this.label2.Size = new System.Drawing.Size(71, 13);
			this.label2.TabIndex = 0;
			this.label2.Text = "Threat Value:";
			// 
			// numericUpDownThreatValue
			// 
			this.numericUpDownThreatValue.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left)
						| System.Windows.Forms.AnchorStyles.Right)));
			this.numericUpDownThreatValue.DecimalPlaces = 3;
			this.numericUpDownThreatValue.Enabled = false;
			this.numericUpDownThreatValue.Location = new System.Drawing.Point(146, 58);
			this.numericUpDownThreatValue.Maximum = new decimal(new int[] {
            1000,
            0,
            0,
            0});
			this.numericUpDownThreatValue.Name = "numericUpDownThreatValue";
			this.numericUpDownThreatValue.Size = new System.Drawing.Size(111, 20);
			this.numericUpDownThreatValue.TabIndex = 3;
			this.numericUpDownThreatValue.Value = new decimal(new int[] {
            1,
            0,
            0,
            0});
			this.numericUpDownThreatValue.ValueChanged += new System.EventHandler(this.calculationOptionControl_Changed);
			// 
			// comboBoxThreatValue
			// 
			this.comboBoxThreatValue.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left)
						| System.Windows.Forms.AnchorStyles.Right)));
			this.comboBoxThreatValue.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
			this.comboBoxThreatValue.FormattingEnabled = true;
			this.comboBoxThreatValue.Items.AddRange(new object[] {
            "None",
            "MT",
            "OT",
            "Crazy About Threat",
            "Custom Factor"});
			this.comboBoxThreatValue.Location = new System.Drawing.Point(146, 30);
			this.comboBoxThreatValue.Name = "comboBoxThreatValue";
			this.comboBoxThreatValue.Size = new System.Drawing.Size(111, 21);
			this.comboBoxThreatValue.TabIndex = 1;
			this.comboBoxThreatValue.SelectedIndexChanged += new System.EventHandler(this.comboBoxThreatValue_SelectedIndexChanged);
			// 
			// numericUpDownTargetArmor
			// 
			this.numericUpDownTargetArmor.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left)
						| System.Windows.Forms.AnchorStyles.Right)));
			this.numericUpDownTargetArmor.Location = new System.Drawing.Point(146, 137);
			this.numericUpDownTargetArmor.Maximum = new decimal(new int[] {
            15000,
            0,
            0,
            0});
			this.numericUpDownTargetArmor.Name = "numericUpDownTargetArmor";
			this.numericUpDownTargetArmor.Size = new System.Drawing.Size(111, 20);
			this.numericUpDownTargetArmor.TabIndex = 3;
			this.numericUpDownTargetArmor.ThousandsSeparator = true;
			this.numericUpDownTargetArmor.Value = new decimal(new int[] {
            10643,
            0,
            0,
            0});
			this.numericUpDownTargetArmor.ValueChanged += new System.EventHandler(this.calculationOptionControl_Changed);
			// 
			// label3
			// 
			this.label3.AutoSize = true;
			this.label3.Location = new System.Drawing.Point(3, 139);
			this.label3.Name = "label3";
			this.label3.Size = new System.Drawing.Size(71, 13);
			this.label3.TabIndex = 0;
			this.label3.Text = "Target Armor:";
			// 
			// groupBox1
			// 
			this.groupBox1.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left)
						| System.Windows.Forms.AnchorStyles.Right)));
			this.groupBox1.Controls.Add(this.checkBoxMaul);
			this.groupBox1.Controls.Add(this.checkBoxLacerate);
			this.groupBox1.Controls.Add(this.checkBoxMangle);
			this.groupBox1.Controls.Add(this.checkBoxFaerieFire);
			this.groupBox1.Controls.Add(this.checkBoxSwipe);
			this.groupBox1.Location = new System.Drawing.Point(7, 163);
			this.groupBox1.Name = "groupBox1";
			this.groupBox1.Size = new System.Drawing.Size(250, 66);
			this.groupBox1.TabIndex = 4;
			this.groupBox1.TabStop = false;
			this.groupBox1.Text = "Custom Rotation";
			// 
			// checkBoxMaul
			// 
			this.checkBoxMaul.AutoSize = true;
			this.checkBoxMaul.Location = new System.Drawing.Point(6, 19);
			this.checkBoxMaul.Name = "checkBoxMaul";
			this.checkBoxMaul.Size = new System.Drawing.Size(49, 17);
			this.checkBoxMaul.TabIndex = 1;
			this.checkBoxMaul.Text = "Maul";
			this.checkBoxMaul.UseVisualStyleBackColor = true;
			this.checkBoxMaul.CheckedChanged += new System.EventHandler(this.calculationOptionControl_Changed);
			// 
			// checkBoxLacerate
			// 
			this.checkBoxLacerate.AutoSize = true;
			this.checkBoxLacerate.Location = new System.Drawing.Point(159, 42);
			this.checkBoxLacerate.Name = "checkBoxLacerate";
			this.checkBoxLacerate.Size = new System.Drawing.Size(68, 17);
			this.checkBoxLacerate.TabIndex = 1;
			this.checkBoxLacerate.Text = "Lacerate";
			this.checkBoxLacerate.UseVisualStyleBackColor = true;
			this.checkBoxLacerate.CheckedChanged += new System.EventHandler(this.calculationOptionControl_Changed);
			// 
			// checkBoxMangle
			// 
			this.checkBoxMangle.AutoSize = true;
			this.checkBoxMangle.Location = new System.Drawing.Point(78, 19);
			this.checkBoxMangle.Name = "checkBoxMangle";
			this.checkBoxMangle.Size = new System.Drawing.Size(61, 17);
			this.checkBoxMangle.TabIndex = 1;
			this.checkBoxMangle.Text = "Mangle";
			this.checkBoxMangle.UseVisualStyleBackColor = true;
			this.checkBoxMangle.CheckedChanged += new System.EventHandler(this.calculationOptionControl_Changed);
			// 
			// checkBoxFaerieFire
			// 
			this.checkBoxFaerieFire.AutoSize = true;
			this.checkBoxFaerieFire.Location = new System.Drawing.Point(78, 42);
			this.checkBoxFaerieFire.Name = "checkBoxFaerieFire";
			this.checkBoxFaerieFire.Size = new System.Drawing.Size(75, 17);
			this.checkBoxFaerieFire.TabIndex = 1;
			this.checkBoxFaerieFire.Text = "Faerie Fire";
			this.checkBoxFaerieFire.UseVisualStyleBackColor = true;
			this.checkBoxFaerieFire.CheckedChanged += new System.EventHandler(this.calculationOptionControl_Changed);
			// 
			// checkBoxSwipe
			// 
			this.checkBoxSwipe.AutoSize = true;
			this.checkBoxSwipe.Location = new System.Drawing.Point(159, 19);
			this.checkBoxSwipe.Name = "checkBoxSwipe";
			this.checkBoxSwipe.Size = new System.Drawing.Size(55, 17);
			this.checkBoxSwipe.TabIndex = 1;
			this.checkBoxSwipe.Text = "Swipe";
			this.checkBoxSwipe.UseVisualStyleBackColor = true;
			this.checkBoxSwipe.CheckedChanged += new System.EventHandler(this.calculationOptionControl_Changed);
			// 
			// label4
			// 
			this.label4.AutoSize = true;
			this.label4.Location = new System.Drawing.Point(3, 87);
			this.label4.Name = "label4";
			this.label4.Size = new System.Drawing.Size(92, 13);
			this.label4.TabIndex = 0;
			this.label4.Text = "Survival Soft Cap:";
			// 
			// numericUpDownSurvivalSoftCap
			// 
			this.numericUpDownSurvivalSoftCap.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left)
						| System.Windows.Forms.AnchorStyles.Right)));
			this.numericUpDownSurvivalSoftCap.Increment = new decimal(new int[] {
            1000,
            0,
            0,
            0});
			this.numericUpDownSurvivalSoftCap.Location = new System.Drawing.Point(146, 111);
			this.numericUpDownSurvivalSoftCap.Maximum = new decimal(new int[] {
            999999,
            0,
            0,
            0});
			this.numericUpDownSurvivalSoftCap.Name = "numericUpDownSurvivalSoftCap";
			this.numericUpDownSurvivalSoftCap.Size = new System.Drawing.Size(111, 20);
			this.numericUpDownSurvivalSoftCap.TabIndex = 3;
			this.numericUpDownSurvivalSoftCap.ThousandsSeparator = true;
			this.numericUpDownSurvivalSoftCap.Value = new decimal(new int[] {
            160000,
            0,
            0,
            0});
			this.numericUpDownSurvivalSoftCap.ValueChanged += new System.EventHandler(this.calculationOptionControl_Changed);
			// 
			// label5
			// 
			this.label5.AutoSize = true;
			this.label5.Location = new System.Drawing.Point(3, 238);
			this.label5.Name = "label5";
			this.label5.Size = new System.Drawing.Size(115, 13);
			this.label5.TabIndex = 0;
			this.label5.Text = "Target Damage (Raw):";
			// 
			// numericUpDownTargetDamage
			// 
			this.numericUpDownTargetDamage.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left)
						| System.Windows.Forms.AnchorStyles.Right)));
			this.numericUpDownTargetDamage.Increment = new decimal(new int[] {
            1000,
            0,
            0,
            0});
			this.numericUpDownTargetDamage.Location = new System.Drawing.Point(146, 262);
			this.numericUpDownTargetDamage.Maximum = new decimal(new int[] {
            999999,
            0,
            0,
            0});
			this.numericUpDownTargetDamage.Name = "numericUpDownTargetDamage";
			this.numericUpDownTargetDamage.Size = new System.Drawing.Size(111, 20);
			this.numericUpDownTargetDamage.TabIndex = 3;
			this.numericUpDownTargetDamage.ThousandsSeparator = true;
			this.numericUpDownTargetDamage.Value = new decimal(new int[] {
            140000,
            0,
            0,
            0});
			this.numericUpDownTargetDamage.ValueChanged += new System.EventHandler(this.calculationOptionControl_Changed);
			// 
			// label6
			// 
			this.label6.AutoSize = true;
			this.label6.Location = new System.Drawing.Point(3, 290);
			this.label6.Name = "label6";
			this.label6.Size = new System.Drawing.Size(137, 13);
			this.label6.TabIndex = 0;
			this.label6.Text = "Target Attack Speed (Sec):";
			// 
			// numericUpDownTargetAttackSpeed
			// 
			this.numericUpDownTargetAttackSpeed.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left)
						| System.Windows.Forms.AnchorStyles.Right)));
			this.numericUpDownTargetAttackSpeed.DecimalPlaces = 2;
			this.numericUpDownTargetAttackSpeed.Increment = new decimal(new int[] {
            1,
            0,
            0,
            65536});
			this.numericUpDownTargetAttackSpeed.Location = new System.Drawing.Point(146, 288);
			this.numericUpDownTargetAttackSpeed.Maximum = new decimal(new int[] {
            10,
            0,
            0,
            0});
			this.numericUpDownTargetAttackSpeed.Name = "numericUpDownTargetAttackSpeed";
			this.numericUpDownTargetAttackSpeed.Size = new System.Drawing.Size(111, 20);
			this.numericUpDownTargetAttackSpeed.TabIndex = 3;
			this.numericUpDownTargetAttackSpeed.Value = new decimal(new int[] {
            2,
            0,
            0,
            0});
			this.numericUpDownTargetAttackSpeed.ValueChanged += new System.EventHandler(this.calculationOptionControl_Changed);
			// 
			// comboBoxTargetDamage
			// 
			this.comboBoxTargetDamage.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left)
						| System.Windows.Forms.AnchorStyles.Right)));
			this.comboBoxTargetDamage.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
			this.comboBoxTargetDamage.FormattingEnabled = true;
			this.comboBoxTargetDamage.Items.AddRange(new object[] {
            "Normal Dungeons",
            "Heroic Dungeons",
            "T7 Raids (10)",
            "T7 Raids\r (25)",
            "T8 Raids (10)",
            "T8 Raids (10, Hard)",
            "T8 Raids (25)",
            "T8 Raids (25, Hard)",
            "T9 Raids (10)",
            "T9 Raids (10, Heroic)",
            "T9 Raids (25)",
            "T9 Raids (25, Heroic)",
            "T10 Raids (10)",
            "T10 Raids (10, Heroic)",
            "T10 Raids (25)",
            "T10 Raids (25, Heroic)",
            "Lich King (10)",
            "Lich King (10, Heroic)",
            "Lich King (25)",
            "Lich King (25, Heroic)",
            "Custom..."});
			this.comboBoxTargetDamage.Location = new System.Drawing.Point(146, 235);
			this.comboBoxTargetDamage.Name = "comboBoxTargetDamage";
			this.comboBoxTargetDamage.Size = new System.Drawing.Size(111, 21);
			this.comboBoxTargetDamage.TabIndex = 1;
			this.comboBoxTargetDamage.SelectedIndexChanged += new System.EventHandler(this.comboBoxTargetDamage_SelectedIndexChanged);
			// 
			// comboBoxSurvivalSoftCap
			// 
			this.comboBoxSurvivalSoftCap.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left)
						| System.Windows.Forms.AnchorStyles.Right)));
			this.comboBoxSurvivalSoftCap.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
			this.comboBoxSurvivalSoftCap.FormattingEnabled = true;
			this.comboBoxSurvivalSoftCap.Items.AddRange(new object[] {
            "Normal Dungeons",
            "Heroic Dungeons",
            "T7 Raids (10)",
            "T7 Raids\r (25)",
            "T8 Raids (10)",
            "T8 Raids (10, Hard)",
            "T8 Raids (25)",
            "T8 Raids (25, Hard)",
            "T9 Raids (10)",
            "T9 Raids (10, Heroic)",
            "T9 Raids (25)",
            "T9 Raids (25, Heroic)",
            "T10 Raids (10)",
            "T10 Raids (10, Heroic)",
            "T10 Raids (25)",
            "T10 Raids (25, Heroic)",
            "Lich King (10)",
            "Lich King (10, Heroic)",
            "Lich King (25)",
            "Lich King (25, Heroic)",
            "Custom..."});
			this.comboBoxSurvivalSoftCap.Location = new System.Drawing.Point(146, 84);
			this.comboBoxSurvivalSoftCap.Name = "comboBoxSurvivalSoftCap";
			this.comboBoxSurvivalSoftCap.Size = new System.Drawing.Size(111, 21);
			this.comboBoxSurvivalSoftCap.TabIndex = 1;
			this.comboBoxSurvivalSoftCap.SelectedIndexChanged += new System.EventHandler(this.comboBoxSurvivalSoftCap_SelectedIndexChanged);
			// 
			// checkBoxTargetParryHastes
			// 
			this.checkBoxTargetParryHastes.AutoSize = true;
			this.checkBoxTargetParryHastes.Location = new System.Drawing.Point(10, 314);
			this.checkBoxTargetParryHastes.Name = "checkBoxTargetParryHastes";
			this.checkBoxTargetParryHastes.Size = new System.Drawing.Size(120, 17);
			this.checkBoxTargetParryHastes.TabIndex = 1;
			this.checkBoxTargetParryHastes.Text = "Target Parry Hastes";
			this.checkBoxTargetParryHastes.UseVisualStyleBackColor = true;
			this.checkBoxTargetParryHastes.CheckedChanged += new System.EventHandler(this.calculationOptionControl_Changed);
			// 
			// groupBox2
			// 
			this.groupBox2.Controls.Add(this.panel3);
			this.groupBox2.Controls.Add(this.panel2);
			this.groupBox2.Controls.Add(this.panel1);
			this.groupBox2.Controls.Add(this.label10);
			this.groupBox2.Controls.Add(this.label9);
			this.groupBox2.Controls.Add(this.label8);
			this.groupBox2.Controls.Add(this.trackBarTemporarySurvivalEffectValue);
			this.groupBox2.Controls.Add(this.label7);
			this.groupBox2.Location = new System.Drawing.Point(3, 337);
			this.groupBox2.Name = "groupBox2";
			this.groupBox2.Size = new System.Drawing.Size(254, 218);
			this.groupBox2.TabIndex = 5;
			this.groupBox2.TabStop = false;
			this.groupBox2.Text = "Temporary Survival Effect Value";
			this.groupBox2.Visible = false;
			// 
			// label7
			// 
			this.label7.Location = new System.Drawing.Point(6, 16);
			this.label7.Name = "label7";
			this.label7.Size = new System.Drawing.Size(242, 148);
			this.label7.TabIndex = 6;
			this.label7.Text = resources.GetString("label7.Text");
			// 
			// trackBarTemporarySurvivalEffectValue
			// 
			this.trackBarTemporarySurvivalEffectValue.LargeChange = 50;
			this.trackBarTemporarySurvivalEffectValue.Location = new System.Drawing.Point(6, 167);
			this.trackBarTemporarySurvivalEffectValue.Maximum = 200;
			this.trackBarTemporarySurvivalEffectValue.Name = "trackBarTemporarySurvivalEffectValue";
			this.trackBarTemporarySurvivalEffectValue.Size = new System.Drawing.Size(242, 45);
			this.trackBarTemporarySurvivalEffectValue.SmallChange = 5;
			this.trackBarTemporarySurvivalEffectValue.TabIndex = 7;
			this.trackBarTemporarySurvivalEffectValue.TickFrequency = 5;
			// 
			// label8
			// 
			this.label8.AutoSize = true;
			this.label8.Location = new System.Drawing.Point(3, 199);
			this.label8.Name = "label8";
			this.label8.Size = new System.Drawing.Size(33, 13);
			this.label8.TabIndex = 0;
			this.label8.Text = "None";
			// 
			// label9
			// 
			this.label9.Location = new System.Drawing.Point(104, 199);
			this.label9.Name = "label9";
			this.label9.Size = new System.Drawing.Size(47, 13);
			this.label9.TabIndex = 0;
			this.label9.Text = "Average";
			// 
			// label10
			// 
			this.label10.AutoSize = true;
			this.label10.Location = new System.Drawing.Point(223, 199);
			this.label10.Name = "label10";
			this.label10.Size = new System.Drawing.Size(23, 13);
			this.label10.TabIndex = 0;
			this.label10.Text = "Full";
			// 
			// panel1
			// 
			this.panel1.BackColor = System.Drawing.SystemColors.ControlDarkDark;
			this.panel1.Location = new System.Drawing.Point(19, 192);
			this.panel1.Name = "panel1";
			this.panel1.Size = new System.Drawing.Size(1, 8);
			this.panel1.TabIndex = 6;
			// 
			// panel2
			// 
			this.panel2.BackColor = System.Drawing.SystemColors.ControlDarkDark;
			this.panel2.Location = new System.Drawing.Point(127, 192);
			this.panel2.Name = "panel2";
			this.panel2.Size = new System.Drawing.Size(1, 8);
			this.panel2.TabIndex = 7;
			// 
			// panel3
			// 
			this.panel3.BackColor = System.Drawing.SystemColors.ControlDarkDark;
			this.panel3.Location = new System.Drawing.Point(234, 192);
			this.panel3.Name = "panel3";
			this.panel3.Size = new System.Drawing.Size(1, 8);
			this.panel3.TabIndex = 7;
			// 
			// CalculationOptionsPanelBear
			// 
			this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
			this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
			this.Controls.Add(this.groupBox2);
			this.Controls.Add(this.groupBox1);
			this.Controls.Add(this.checkBoxTargetParryHastes);
			this.Controls.Add(this.numericUpDownSurvivalSoftCap);
			this.Controls.Add(this.numericUpDownTargetAttackSpeed);
			this.Controls.Add(this.numericUpDownTargetDamage);
			this.Controls.Add(this.numericUpDownTargetArmor);
			this.Controls.Add(this.numericUpDownThreatValue);
			this.Controls.Add(this.comboBoxSurvivalSoftCap);
			this.Controls.Add(this.comboBoxTargetDamage);
			this.Controls.Add(this.comboBoxThreatValue);
			this.Controls.Add(this.label6);
			this.Controls.Add(this.label4);
			this.Controls.Add(this.label5);
			this.Controls.Add(this.comboBoxTargetLevel);
			this.Controls.Add(this.label3);
			this.Controls.Add(this.label2);
			this.Controls.Add(this.label1);
			this.Name = "CalculationOptionsPanelBear";
			this.Size = new System.Drawing.Size(260, 559);
			((System.ComponentModel.ISupportInitialize)(this.numericUpDownThreatValue)).EndInit();
			((System.ComponentModel.ISupportInitialize)(this.numericUpDownTargetArmor)).EndInit();
			this.groupBox1.ResumeLayout(false);
			this.groupBox1.PerformLayout();
			((System.ComponentModel.ISupportInitialize)(this.numericUpDownSurvivalSoftCap)).EndInit();
			((System.ComponentModel.ISupportInitialize)(this.numericUpDownTargetDamage)).EndInit();
			((System.ComponentModel.ISupportInitialize)(this.numericUpDownTargetAttackSpeed)).EndInit();
			this.groupBox2.ResumeLayout(false);
			this.groupBox2.PerformLayout();
			((System.ComponentModel.ISupportInitialize)(this.trackBarTemporarySurvivalEffectValue)).EndInit();
			this.ResumeLayout(false);
			this.PerformLayout();

		}

		#endregion

		private System.Windows.Forms.Label label1;
		private System.Windows.Forms.ComboBox comboBoxTargetLevel;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.NumericUpDown numericUpDownThreatValue;
		private System.Windows.Forms.ComboBox comboBoxThreatValue;
		private System.Windows.Forms.NumericUpDown numericUpDownTargetArmor;
		private System.Windows.Forms.Label label3;
		private System.Windows.Forms.GroupBox groupBox1;
		private System.Windows.Forms.CheckBox checkBoxLacerate;
		private System.Windows.Forms.CheckBox checkBoxMangle;
		private System.Windows.Forms.CheckBox checkBoxFaerieFire;
        private System.Windows.Forms.CheckBox checkBoxSwipe;
		private System.Windows.Forms.Label label4;
		private System.Windows.Forms.NumericUpDown numericUpDownSurvivalSoftCap;
		private System.Windows.Forms.Label label5;
		private System.Windows.Forms.NumericUpDown numericUpDownTargetDamage;
		private System.Windows.Forms.Label label6;
		private System.Windows.Forms.NumericUpDown numericUpDownTargetAttackSpeed;
		private System.Windows.Forms.ComboBox comboBoxTargetDamage;
		private System.Windows.Forms.ComboBox comboBoxSurvivalSoftCap;
		private System.Windows.Forms.CheckBox checkBoxTargetParryHastes;
        private System.Windows.Forms.CheckBox checkBoxMaul;
		private System.Windows.Forms.GroupBox groupBox2;
		private System.Windows.Forms.TrackBar trackBarTemporarySurvivalEffectValue;
		private System.Windows.Forms.Label label7;
		private System.Windows.Forms.Label label10;
		private System.Windows.Forms.Label label9;
		private System.Windows.Forms.Label label8;
		private System.Windows.Forms.Panel panel3;
		private System.Windows.Forms.Panel panel2;
		private System.Windows.Forms.Panel panel1;
	}
}
