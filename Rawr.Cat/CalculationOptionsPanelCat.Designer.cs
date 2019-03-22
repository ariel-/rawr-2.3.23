namespace Rawr.Cat
{
	partial class CalculationOptionsPanelCat
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(CalculationOptionsPanelCat));
            this.label1 = new System.Windows.Forms.Label();
            this.comboBoxTargetLevel = new System.Windows.Forms.ComboBox();
            this.groupBox1 = new System.Windows.Forms.GroupBox();
            this.checkBoxShred = new System.Windows.Forms.CheckBox();
            this.checkBoxRip = new System.Windows.Forms.CheckBox();
            this.checkBoxRake = new System.Windows.Forms.CheckBox();
            this.comboBoxFerociousBite = new System.Windows.Forms.ComboBox();
            this.comboBoxSavageRoar = new System.Windows.Forms.ComboBox();
            this.label5 = new System.Windows.Forms.Label();
            this.label2 = new System.Windows.Forms.Label();
            this.numericUpDownTargetArmor = new System.Windows.Forms.NumericUpDown();
            this.label3 = new System.Windows.Forms.Label();
            this.label4 = new System.Windows.Forms.Label();
            this.numericUpDownDuration = new System.Windows.Forms.NumericUpDown();
            this.label6 = new System.Windows.Forms.Label();
            this.trackBarTrinketOffset = new System.Windows.Forms.TrackBar();
            this.labelTrinketOffset = new System.Windows.Forms.Label();
            this.label7 = new System.Windows.Forms.Label();
            this.numericUpDownLagVariance = new System.Windows.Forms.NumericUpDown();
            this.groupBox1.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.numericUpDownTargetArmor)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.numericUpDownDuration)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.trackBarTrinketOffset)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.numericUpDownLagVariance)).BeginInit();
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
            this.comboBoxTargetLevel.Location = new System.Drawing.Point(108, 3);
            this.comboBoxTargetLevel.Name = "comboBoxTargetLevel";
            this.comboBoxTargetLevel.Size = new System.Drawing.Size(160, 21);
            this.comboBoxTargetLevel.TabIndex = 1;
            this.comboBoxTargetLevel.SelectedIndexChanged += new System.EventHandler(this.calculationOptionControl_Changed);
            // 
            // groupBox1
            // 
            this.groupBox1.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left)
                        | System.Windows.Forms.AnchorStyles.Right)));
            this.groupBox1.Controls.Add(this.checkBoxShred);
            this.groupBox1.Controls.Add(this.checkBoxRip);
            this.groupBox1.Controls.Add(this.checkBoxRake);
            this.groupBox1.Controls.Add(this.comboBoxFerociousBite);
            this.groupBox1.Controls.Add(this.comboBoxSavageRoar);
            this.groupBox1.Controls.Add(this.label5);
            this.groupBox1.Controls.Add(this.label2);
            this.groupBox1.Location = new System.Drawing.Point(3, 108);
            this.groupBox1.Name = "groupBox1";
            this.groupBox1.Size = new System.Drawing.Size(265, 73);
            this.groupBox1.TabIndex = 7;
            this.groupBox1.TabStop = false;
            this.groupBox1.Text = "Custom Rotation";
            // 
            // checkBoxShred
            // 
            this.checkBoxShred.AutoSize = true;
            this.checkBoxShred.Location = new System.Drawing.Point(6, 48);
            this.checkBoxShred.Name = "checkBoxShred";
            this.checkBoxShred.Size = new System.Drawing.Size(54, 17);
            this.checkBoxShred.TabIndex = 1;
            this.checkBoxShred.Text = "Shred";
            this.checkBoxShred.UseVisualStyleBackColor = true;
            this.checkBoxShred.CheckedChanged += new System.EventHandler(this.calculationOptionControl_Changed);
            // 
            // checkBoxRip
            // 
            this.checkBoxRip.AutoSize = true;
            this.checkBoxRip.Location = new System.Drawing.Point(6, 21);
            this.checkBoxRip.Name = "checkBoxRip";
            this.checkBoxRip.Size = new System.Drawing.Size(42, 17);
            this.checkBoxRip.TabIndex = 1;
            this.checkBoxRip.Text = "Rip";
            this.checkBoxRip.UseVisualStyleBackColor = true;
            this.checkBoxRip.CheckedChanged += new System.EventHandler(this.calculationOptionControl_Changed);
            // 
            // checkBoxRake
            // 
            this.checkBoxRake.AutoSize = true;
            this.checkBoxRake.Location = new System.Drawing.Point(52, 21);
            this.checkBoxRake.Name = "checkBoxRake";
            this.checkBoxRake.Size = new System.Drawing.Size(52, 17);
            this.checkBoxRake.TabIndex = 1;
            this.checkBoxRake.Text = "Rake";
            this.checkBoxRake.UseVisualStyleBackColor = true;
            this.checkBoxRake.CheckedChanged += new System.EventHandler(this.calculationOptionControl_Changed);
            // 
            // comboBoxFerociousBite
            // 
            this.comboBoxFerociousBite.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left)
                        | System.Windows.Forms.AnchorStyles.Right)));
            this.comboBoxFerociousBite.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.comboBoxFerociousBite.FormattingEnabled = true;
            this.comboBoxFerociousBite.Items.AddRange(new object[] {
            "0",
            "1",
            "2",
            "3",
            "4",
            "5"});
            this.comboBoxFerociousBite.Location = new System.Drawing.Point(207, 46);
            this.comboBoxFerociousBite.Name = "comboBoxFerociousBite";
            this.comboBoxFerociousBite.Size = new System.Drawing.Size(52, 21);
            this.comboBoxFerociousBite.TabIndex = 1;
            this.comboBoxFerociousBite.SelectedIndexChanged += new System.EventHandler(this.calculationOptionControl_Changed);
            // 
            // comboBoxSavageRoar
            // 
            this.comboBoxSavageRoar.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left)
                        | System.Windows.Forms.AnchorStyles.Right)));
            this.comboBoxSavageRoar.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.comboBoxSavageRoar.FormattingEnabled = true;
            this.comboBoxSavageRoar.Items.AddRange(new object[] {
            "1",
            "2",
            "3",
            "4",
            "5"});
            this.comboBoxSavageRoar.Location = new System.Drawing.Point(207, 19);
            this.comboBoxSavageRoar.Name = "comboBoxSavageRoar";
            this.comboBoxSavageRoar.Size = new System.Drawing.Size(52, 21);
            this.comboBoxSavageRoar.TabIndex = 1;
            this.comboBoxSavageRoar.SelectedIndexChanged += new System.EventHandler(this.calculationOptionControl_Changed);
            // 
            // label5
            // 
            this.label5.AutoSize = true;
            this.label5.Location = new System.Drawing.Point(107, 49);
            this.label5.Name = "label5";
            this.label5.Size = new System.Drawing.Size(94, 13);
            this.label5.TabIndex = 0;
            this.label5.Text = "Ferocious Bite CP:";
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Location = new System.Drawing.Point(111, 22);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(90, 13);
            this.label2.TabIndex = 0;
            this.label2.Text = "Savage Roar CP:";
            // 
            // numericUpDownTargetArmor
            // 
            this.numericUpDownTargetArmor.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left)
                        | System.Windows.Forms.AnchorStyles.Right)));
            this.numericUpDownTargetArmor.Location = new System.Drawing.Point(108, 30);
            this.numericUpDownTargetArmor.Maximum = new decimal(new int[] {
            15000,
            0,
            0,
            0});
            this.numericUpDownTargetArmor.Name = "numericUpDownTargetArmor";
            this.numericUpDownTargetArmor.Size = new System.Drawing.Size(160, 20);
            this.numericUpDownTargetArmor.TabIndex = 6;
            this.numericUpDownTargetArmor.ThousandsSeparator = true;
            this.numericUpDownTargetArmor.Value = new decimal(new int[] {
            13083,
            0,
            0,
            0});
            this.numericUpDownTargetArmor.ValueChanged += new System.EventHandler(this.calculationOptionControl_Changed);
            // 
            // label3
            // 
            this.label3.AutoSize = true;
            this.label3.Location = new System.Drawing.Point(3, 32);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(71, 13);
            this.label3.TabIndex = 5;
            this.label3.Text = "Target Armor:";
            // 
            // label4
            // 
            this.label4.AutoSize = true;
            this.label4.Location = new System.Drawing.Point(3, 58);
            this.label4.Name = "label4";
            this.label4.Size = new System.Drawing.Size(76, 13);
            this.label4.TabIndex = 5;
            this.label4.Text = "Duration (sec):";
            // 
            // numericUpDownDuration
            // 
            this.numericUpDownDuration.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left)
                        | System.Windows.Forms.AnchorStyles.Right)));
            this.numericUpDownDuration.Location = new System.Drawing.Point(108, 56);
            this.numericUpDownDuration.Maximum = new decimal(new int[] {
            1200,
            0,
            0,
            0});
            this.numericUpDownDuration.Minimum = new decimal(new int[] {
            30,
            0,
            0,
            0});
            this.numericUpDownDuration.Name = "numericUpDownDuration";
            this.numericUpDownDuration.Size = new System.Drawing.Size(160, 20);
            this.numericUpDownDuration.TabIndex = 6;
            this.numericUpDownDuration.ThousandsSeparator = true;
            this.numericUpDownDuration.Value = new decimal(new int[] {
            300,
            0,
            0,
            0});
            this.numericUpDownDuration.ValueChanged += new System.EventHandler(this.calculationOptionControl_Changed);
            // 
            // label6
            // 
            this.label6.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left)
                        | System.Windows.Forms.AnchorStyles.Right)));
            this.label6.Location = new System.Drawing.Point(8, 236);
            this.label6.Name = "label6";
            this.label6.Size = new System.Drawing.Size(254, 98);
            this.label6.TabIndex = 5;
            this.label6.Text = resources.GetString("label6.Text");
            // 
            // trackBarTrinketOffset
            // 
            this.trackBarTrinketOffset.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left)
                        | System.Windows.Forms.AnchorStyles.Right)));
            this.trackBarTrinketOffset.BackColor = System.Drawing.SystemColors.ControlLightLight;
            this.trackBarTrinketOffset.Location = new System.Drawing.Point(3, 200);
            this.trackBarTrinketOffset.Maximum = 90;
            this.trackBarTrinketOffset.Name = "trackBarTrinketOffset";
            this.trackBarTrinketOffset.Size = new System.Drawing.Size(259, 45);
            this.trackBarTrinketOffset.TabIndex = 8;
            this.trackBarTrinketOffset.TickFrequency = 2;
            this.trackBarTrinketOffset.Scroll += new System.EventHandler(this.calculationOptionControl_Changed);
            // 
            // labelTrinketOffset
            // 
            this.labelTrinketOffset.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left)
                        | System.Windows.Forms.AnchorStyles.Right)));
            this.labelTrinketOffset.Location = new System.Drawing.Point(8, 184);
            this.labelTrinketOffset.Name = "labelTrinketOffset";
            this.labelTrinketOffset.Size = new System.Drawing.Size(254, 13);
            this.labelTrinketOffset.TabIndex = 5;
            this.labelTrinketOffset.Tag = "Trinket Offset: {0}sec";
            this.labelTrinketOffset.Text = "Trinket Offset:";
            // 
            // label7
            // 
            this.label7.AutoSize = true;
            this.label7.Location = new System.Drawing.Point(3, 84);
            this.label7.Name = "label7";
            this.label7.Size = new System.Drawing.Size(99, 13);
            this.label7.TabIndex = 5;
            this.label7.Text = "Lag Variance* (ms):";
            // 
            // numericUpDownLagVariance
            // 
            this.numericUpDownLagVariance.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left)
                        | System.Windows.Forms.AnchorStyles.Right)));
            this.numericUpDownLagVariance.Location = new System.Drawing.Point(108, 82);
            this.numericUpDownLagVariance.Maximum = new decimal(new int[] {
            1000,
            0,
            0,
            0});
            this.numericUpDownLagVariance.Name = "numericUpDownLagVariance";
            this.numericUpDownLagVariance.Size = new System.Drawing.Size(160, 20);
            this.numericUpDownLagVariance.TabIndex = 6;
            this.numericUpDownLagVariance.ThousandsSeparator = true;
            this.numericUpDownLagVariance.Value = new decimal(new int[] {
            100,
            0,
            0,
            0});
            this.numericUpDownLagVariance.ValueChanged += new System.EventHandler(this.calculationOptionControl_Changed);
            // 
            // CalculationOptionsPanelCat
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.AutoScroll = true;
            this.Controls.Add(this.label6);
            this.Controls.Add(this.trackBarTrinketOffset);
            this.Controls.Add(this.groupBox1);
            this.Controls.Add(this.numericUpDownLagVariance);
            this.Controls.Add(this.numericUpDownDuration);
            this.Controls.Add(this.label7);
            this.Controls.Add(this.labelTrinketOffset);
            this.Controls.Add(this.label4);
            this.Controls.Add(this.numericUpDownTargetArmor);
            this.Controls.Add(this.label3);
            this.Controls.Add(this.comboBoxTargetLevel);
            this.Controls.Add(this.label1);
            this.Name = "CalculationOptionsPanelCat";
            this.Size = new System.Drawing.Size(271, 534);
            this.groupBox1.ResumeLayout(false);
            this.groupBox1.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.numericUpDownTargetArmor)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.numericUpDownDuration)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.trackBarTrinketOffset)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.numericUpDownLagVariance)).EndInit();
            this.ResumeLayout(false);
            this.PerformLayout();

		}

		#endregion

		private System.Windows.Forms.Label label1;
		private System.Windows.Forms.ComboBox comboBoxTargetLevel;
		private System.Windows.Forms.GroupBox groupBox1;
		private System.Windows.Forms.CheckBox checkBoxShred;
		private System.Windows.Forms.CheckBox checkBoxRip;
		private System.Windows.Forms.ComboBox comboBoxSavageRoar;
		private System.Windows.Forms.Label label2;
		private System.Windows.Forms.NumericUpDown numericUpDownTargetArmor;
		private System.Windows.Forms.Label label3;
		private System.Windows.Forms.Label label4;
		private System.Windows.Forms.NumericUpDown numericUpDownDuration;
		private System.Windows.Forms.CheckBox checkBoxRake;
		private System.Windows.Forms.ComboBox comboBoxFerociousBite;
		private System.Windows.Forms.Label label5;
		private System.Windows.Forms.Label label6;
		private System.Windows.Forms.TrackBar trackBarTrinketOffset;
		private System.Windows.Forms.Label labelTrinketOffset;
		private System.Windows.Forms.Label label7;
		private System.Windows.Forms.NumericUpDown numericUpDownLagVariance;
	}
}
