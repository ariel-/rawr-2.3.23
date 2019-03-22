namespace Rawr
{
	partial class FormOptimize
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(FormOptimize));
            this.buttonOptimize = new System.Windows.Forms.Button();
            this.progressBarMain = new System.Windows.Forms.ProgressBar();
            this.buttonCancel = new System.Windows.Forms.Button();
            this.labelMax = new System.Windows.Forms.Label();
            this.progressBarAlt = new System.Windows.Forms.ProgressBar();
            this.comboBoxCalculationToOptimize = new System.Windows.Forms.ComboBox();
            this.labelCalculationToOptimize = new System.Windows.Forms.Label();
            this.buttonAddRequirement = new System.Windows.Forms.Button();
            this.groupBoxRequirements = new System.Windows.Forms.GroupBox();
            this.labelInfo = new System.Windows.Forms.Label();
            this.groupBoxOptions = new System.Windows.Forms.GroupBox();
            this.checkBoxOptimizeTalents = new System.Windows.Forms.CheckBox();
            this.checkBoxOverrideReenchant = new System.Windows.Forms.CheckBox();
            this.checkBoxMixology = new System.Windows.Forms.CheckBox();
            this.checkBoxOverrideRegem = new System.Windows.Forms.CheckBox();
            this.checkBoxOptimizeElixir = new System.Windows.Forms.CheckBox();
            this.label4 = new System.Windows.Forms.Label();
            this.checkBoxOptimizeFood = new System.Windows.Forms.CheckBox();
            this.label3 = new System.Windows.Forms.Label();
            this.trackBarThoroughness = new System.Windows.Forms.TrackBar();
            this.label2 = new System.Windows.Forms.Label();
            this.label1 = new System.Windows.Forms.Label();
            this.buttonUpgrades = new System.Windows.Forms.Button();
            this.groupBox1 = new System.Windows.Forms.GroupBox();
            this.label5 = new System.Windows.Forms.Label();
            this.groupBox2 = new System.Windows.Forms.GroupBox();
            this.label6 = new System.Windows.Forms.Label();
            this.checkBoxMutateTalents = new System.Windows.Forms.CheckBox();
            this.groupBoxRequirements.SuspendLayout();
            this.groupBoxOptions.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.trackBarThoroughness)).BeginInit();
            this.groupBox1.SuspendLayout();
            this.groupBox2.SuspendLayout();
            this.SuspendLayout();
            // 
            // buttonOptimize
            // 
            this.buttonOptimize.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.buttonOptimize.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.buttonOptimize.Location = new System.Drawing.Point(227, 86);
            this.buttonOptimize.Name = "buttonOptimize";
            this.buttonOptimize.Size = new System.Drawing.Size(75, 23);
            this.buttonOptimize.TabIndex = 1;
            this.buttonOptimize.Text = "Optimize";
            this.buttonOptimize.UseVisualStyleBackColor = true;
            this.buttonOptimize.Click += new System.EventHandler(this.buttonOptimize_Click);
            // 
            // progressBarMain
            // 
            this.progressBarMain.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)
                        | System.Windows.Forms.AnchorStyles.Right)));
            this.progressBarMain.Location = new System.Drawing.Point(12, 431);
            this.progressBarMain.Name = "progressBarMain";
            this.progressBarMain.Size = new System.Drawing.Size(677, 23);
            this.progressBarMain.TabIndex = 1;
            this.progressBarMain.Click += new System.EventHandler(this.progressBarMain_Click);
            // 
            // buttonCancel
            // 
            this.buttonCancel.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.buttonCancel.DialogResult = System.Windows.Forms.DialogResult.Cancel;
            this.buttonCancel.Location = new System.Drawing.Point(614, 460);
            this.buttonCancel.Name = "buttonCancel";
            this.buttonCancel.Size = new System.Drawing.Size(75, 23);
            this.buttonCancel.TabIndex = 0;
            this.buttonCancel.Text = "Close";
            this.buttonCancel.UseVisualStyleBackColor = true;
            this.buttonCancel.Click += new System.EventHandler(this.buttonCancel_Click);
            // 
            // labelMax
            // 
            this.labelMax.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this.labelMax.AutoSize = true;
            this.labelMax.Location = new System.Drawing.Point(12, 467);
            this.labelMax.Name = "labelMax";
            this.labelMax.Size = new System.Drawing.Size(0, 13);
            this.labelMax.TabIndex = 3;
            // 
            // progressBarAlt
            // 
            this.progressBarAlt.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)
                        | System.Windows.Forms.AnchorStyles.Right)));
            this.progressBarAlt.Location = new System.Drawing.Point(12, 424);
            this.progressBarAlt.Name = "progressBarAlt";
            this.progressBarAlt.Size = new System.Drawing.Size(677, 8);
            this.progressBarAlt.TabIndex = 2;
            this.progressBarAlt.Click += new System.EventHandler(this.progressBarAlt_Click);
            // 
            // comboBoxCalculationToOptimize
            // 
            this.comboBoxCalculationToOptimize.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.comboBoxCalculationToOptimize.FormattingEnabled = true;
            this.comboBoxCalculationToOptimize.Location = new System.Drawing.Point(138, 40);
            this.comboBoxCalculationToOptimize.Name = "comboBoxCalculationToOptimize";
            this.comboBoxCalculationToOptimize.Size = new System.Drawing.Size(182, 21);
            this.comboBoxCalculationToOptimize.TabIndex = 6;
            // 
            // labelCalculationToOptimize
            // 
            this.labelCalculationToOptimize.AutoSize = true;
            this.labelCalculationToOptimize.Location = new System.Drawing.Point(12, 43);
            this.labelCalculationToOptimize.Name = "labelCalculationToOptimize";
            this.labelCalculationToOptimize.Size = new System.Drawing.Size(120, 13);
            this.labelCalculationToOptimize.TabIndex = 5;
            this.labelCalculationToOptimize.Text = "Calculation to Optimize: ";
            // 
            // buttonAddRequirement
            // 
            this.buttonAddRequirement.Location = new System.Drawing.Point(6, 19);
            this.buttonAddRequirement.Name = "buttonAddRequirement";
            this.buttonAddRequirement.Size = new System.Drawing.Size(55, 23);
            this.buttonAddRequirement.TabIndex = 0;
            this.buttonAddRequirement.Text = "Add";
            this.buttonAddRequirement.UseVisualStyleBackColor = true;
            this.buttonAddRequirement.Click += new System.EventHandler(this.buttonAddRequirement_Click);
            // 
            // groupBoxRequirements
            // 
            this.groupBoxRequirements.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom)
                        | System.Windows.Forms.AnchorStyles.Left)));
            this.groupBoxRequirements.Controls.Add(this.buttonAddRequirement);
            this.groupBoxRequirements.Location = new System.Drawing.Point(12, 67);
            this.groupBoxRequirements.Name = "groupBoxRequirements";
            this.groupBoxRequirements.Size = new System.Drawing.Size(308, 230);
            this.groupBoxRequirements.TabIndex = 7;
            this.groupBoxRequirements.TabStop = false;
            this.groupBoxRequirements.Text = "Additional Requirements";
            // 
            // labelInfo
            // 
            this.labelInfo.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left)
                        | System.Windows.Forms.AnchorStyles.Right)));
            this.labelInfo.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.labelInfo.Location = new System.Drawing.Point(6, 16);
            this.labelInfo.Name = "labelInfo";
            this.labelInfo.Size = new System.Drawing.Size(296, 67);
            this.labelInfo.TabIndex = 0;
            this.labelInfo.Text = resources.GetString("labelInfo.Text");
            // 
            // groupBoxOptions
            // 
            this.groupBoxOptions.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom)
                        | System.Windows.Forms.AnchorStyles.Left)
                        | System.Windows.Forms.AnchorStyles.Right)));
            this.groupBoxOptions.Controls.Add(this.checkBoxMutateTalents);
            this.groupBoxOptions.Controls.Add(this.checkBoxOptimizeTalents);
            this.groupBoxOptions.Controls.Add(this.checkBoxOverrideReenchant);
            this.groupBoxOptions.Controls.Add(this.checkBoxMixology);
            this.groupBoxOptions.Controls.Add(this.checkBoxOverrideRegem);
            this.groupBoxOptions.Controls.Add(this.checkBoxOptimizeElixir);
            this.groupBoxOptions.Controls.Add(this.label4);
            this.groupBoxOptions.Controls.Add(this.checkBoxOptimizeFood);
            this.groupBoxOptions.Controls.Add(this.label3);
            this.groupBoxOptions.Controls.Add(this.trackBarThoroughness);
            this.groupBoxOptions.Controls.Add(this.label2);
            this.groupBoxOptions.Controls.Add(this.label1);
            this.groupBoxOptions.Location = new System.Drawing.Point(326, 40);
            this.groupBoxOptions.Name = "groupBoxOptions";
            this.groupBoxOptions.Size = new System.Drawing.Size(363, 257);
            this.groupBoxOptions.TabIndex = 8;
            this.groupBoxOptions.TabStop = false;
            this.groupBoxOptions.Text = "Options";
            // 
            // checkBoxOptimizeTalents
            // 
            this.checkBoxOptimizeTalents.AutoSize = true;
            this.checkBoxOptimizeTalents.Location = new System.Drawing.Point(9, 237);
            this.checkBoxOptimizeTalents.Name = "checkBoxOptimizeTalents";
            this.checkBoxOptimizeTalents.Size = new System.Drawing.Size(89, 17);
            this.checkBoxOptimizeTalents.TabIndex = 10;
            this.checkBoxOptimizeTalents.Text = "Talent Specs";
            this.checkBoxOptimizeTalents.UseVisualStyleBackColor = true;
            // 
            // checkBoxOverrideReenchant
            // 
            this.checkBoxOverrideReenchant.AutoSize = true;
            this.checkBoxOverrideReenchant.Location = new System.Drawing.Point(118, 177);
            this.checkBoxOverrideReenchant.Name = "checkBoxOverrideReenchant";
            this.checkBoxOverrideReenchant.Size = new System.Drawing.Size(122, 17);
            this.checkBoxOverrideReenchant.TabIndex = 5;
            this.checkBoxOverrideReenchant.Text = "Override Reenchant";
            this.checkBoxOverrideReenchant.UseVisualStyleBackColor = true;
            // 
            // checkBoxMixology
            // 
            this.checkBoxMixology.AutoSize = true;
            this.checkBoxMixology.Location = new System.Drawing.Point(158, 215);
            this.checkBoxMixology.Name = "checkBoxMixology";
            this.checkBoxMixology.Size = new System.Drawing.Size(67, 17);
            this.checkBoxMixology.TabIndex = 9;
            this.checkBoxMixology.Text = "Mixology";
            this.checkBoxMixology.UseVisualStyleBackColor = true;
            // 
            // checkBoxOverrideRegem
            // 
            this.checkBoxOverrideRegem.AutoSize = true;
            this.checkBoxOverrideRegem.Location = new System.Drawing.Point(9, 177);
            this.checkBoxOverrideRegem.Name = "checkBoxOverrideRegem";
            this.checkBoxOverrideRegem.Size = new System.Drawing.Size(103, 17);
            this.checkBoxOverrideRegem.TabIndex = 4;
            this.checkBoxOverrideRegem.Text = "Override Regem";
            this.checkBoxOverrideRegem.UseVisualStyleBackColor = true;
            // 
            // checkBoxOptimizeElixir
            // 
            this.checkBoxOptimizeElixir.AutoSize = true;
            this.checkBoxOptimizeElixir.Location = new System.Drawing.Point(65, 214);
            this.checkBoxOptimizeElixir.Name = "checkBoxOptimizeElixir";
            this.checkBoxOptimizeElixir.Size = new System.Drawing.Size(87, 17);
            this.checkBoxOptimizeElixir.TabIndex = 8;
            this.checkBoxOptimizeElixir.Text = "Elixirs/Flasks";
            this.checkBoxOptimizeElixir.UseVisualStyleBackColor = true;
            // 
            // label4
            // 
            this.label4.Location = new System.Drawing.Point(6, 199);
            this.label4.Name = "label4";
            this.label4.Size = new System.Drawing.Size(351, 13);
            this.label4.TabIndex = 6;
            this.label4.Text = "Choose additional properties to optimize as well.";
            // 
            // checkBoxOptimizeFood
            // 
            this.checkBoxOptimizeFood.AutoSize = true;
            this.checkBoxOptimizeFood.Location = new System.Drawing.Point(9, 215);
            this.checkBoxOptimizeFood.Name = "checkBoxOptimizeFood";
            this.checkBoxOptimizeFood.Size = new System.Drawing.Size(50, 17);
            this.checkBoxOptimizeFood.TabIndex = 7;
            this.checkBoxOptimizeFood.Text = "Food";
            this.checkBoxOptimizeFood.UseVisualStyleBackColor = true;
            // 
            // label3
            // 
            this.label3.Location = new System.Drawing.Point(6, 134);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(351, 40);
            this.label3.TabIndex = 3;
            this.label3.Text = resources.GetString("label3.Text");
            // 
            // trackBarThoroughness
            // 
            this.trackBarThoroughness.Location = new System.Drawing.Point(90, 86);
            this.trackBarThoroughness.Maximum = 400;
            this.trackBarThoroughness.Minimum = 1;
            this.trackBarThoroughness.Name = "trackBarThoroughness";
            this.trackBarThoroughness.Size = new System.Drawing.Size(267, 45);
            this.trackBarThoroughness.TabIndex = 2;
            this.trackBarThoroughness.TickFrequency = 10;
            this.trackBarThoroughness.Value = 150;
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Location = new System.Drawing.Point(6, 86);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(78, 13);
            this.label2.TabIndex = 1;
            this.label2.Text = "Thoroughness:";
            // 
            // label1
            // 
            this.label1.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left)
                        | System.Windows.Forms.AnchorStyles.Right)));
            this.label1.Location = new System.Drawing.Point(6, 16);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(351, 67);
            this.label1.TabIndex = 0;
            this.label1.Text = resources.GetString("label1.Text");
            // 
            // buttonUpgrades
            // 
            this.buttonUpgrades.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.buttonUpgrades.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.buttonUpgrades.Location = new System.Drawing.Point(254, 86);
            this.buttonUpgrades.Name = "buttonUpgrades";
            this.buttonUpgrades.Size = new System.Drawing.Size(103, 23);
            this.buttonUpgrades.TabIndex = 1;
            this.buttonUpgrades.Text = "Build Upgrade List";
            this.buttonUpgrades.UseVisualStyleBackColor = true;
            this.buttonUpgrades.Click += new System.EventHandler(this.buttonUpgrades_Click);
            // 
            // groupBox1
            // 
            this.groupBox1.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this.groupBox1.Controls.Add(this.buttonOptimize);
            this.groupBox1.Controls.Add(this.labelInfo);
            this.groupBox1.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.groupBox1.Location = new System.Drawing.Point(12, 303);
            this.groupBox1.Name = "groupBox1";
            this.groupBox1.Size = new System.Drawing.Size(308, 115);
            this.groupBox1.TabIndex = 9;
            this.groupBox1.TabStop = false;
            this.groupBox1.Text = "Optimize";
            // 
            // label5
            // 
            this.label5.Location = new System.Drawing.Point(12, 9);
            this.label5.Name = "label5";
            this.label5.Size = new System.Drawing.Size(680, 28);
            this.label5.TabIndex = 4;
            this.label5.Text = resources.GetString("label5.Text");
            // 
            // groupBox2
            // 
            this.groupBox2.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)
                        | System.Windows.Forms.AnchorStyles.Right)));
            this.groupBox2.Controls.Add(this.label6);
            this.groupBox2.Controls.Add(this.buttonUpgrades);
            this.groupBox2.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.groupBox2.Location = new System.Drawing.Point(326, 303);
            this.groupBox2.Name = "groupBox2";
            this.groupBox2.Size = new System.Drawing.Size(363, 115);
            this.groupBox2.TabIndex = 10;
            this.groupBox2.TabStop = false;
            this.groupBox2.Text = "Build Upgrade List";
            // 
            // label6
            // 
            this.label6.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label6.Location = new System.Drawing.Point(6, 16);
            this.label6.Name = "label6";
            this.label6.Size = new System.Drawing.Size(351, 67);
            this.label6.TabIndex = 0;
            this.label6.Text = resources.GetString("label6.Text");
            // 
            // checkBoxMutateTalents
            // 
            this.checkBoxMutateTalents.AutoSize = true;
            this.checkBoxMutateTalents.Location = new System.Drawing.Point(104, 237);
            this.checkBoxMutateTalents.Name = "checkBoxMutateTalents";
            this.checkBoxMutateTalents.Size = new System.Drawing.Size(61, 17);
            this.checkBoxMutateTalents.TabIndex = 11;
            this.checkBoxMutateTalents.Text = "Talents";
            this.checkBoxMutateTalents.UseVisualStyleBackColor = true;
            // 
            // FormOptimize
            // 
            this.AcceptButton = this.buttonOptimize;
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.CancelButton = this.buttonCancel;
            this.ClientSize = new System.Drawing.Size(701, 495);
            this.ControlBox = false;
            this.Controls.Add(this.groupBox2);
            this.Controls.Add(this.groupBox1);
            this.Controls.Add(this.label5);
            this.Controls.Add(this.groupBoxOptions);
            this.Controls.Add(this.groupBoxRequirements);
            this.Controls.Add(this.labelCalculationToOptimize);
            this.Controls.Add(this.comboBoxCalculationToOptimize);
            this.Controls.Add(this.labelMax);
            this.Controls.Add(this.progressBarMain);
            this.Controls.Add(this.buttonCancel);
            this.Controls.Add(this.progressBarAlt);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedDialog;
            this.Name = "FormOptimize";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterParent;
            this.Text = "Rawr Optimizer";
            this.Load += new System.EventHandler(this.FormOptimize_Load);
            this.FormClosed += new System.Windows.Forms.FormClosedEventHandler(this.FormOptimize_FormClosed);
            this.FormClosing += new System.Windows.Forms.FormClosingEventHandler(this.FormOptimize_FormClosing);
            this.groupBoxRequirements.ResumeLayout(false);
            this.groupBoxOptions.ResumeLayout(false);
            this.groupBoxOptions.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.trackBarThoroughness)).EndInit();
            this.groupBox1.ResumeLayout(false);
            this.groupBox2.ResumeLayout(false);
            this.ResumeLayout(false);
            this.PerformLayout();

		}

		#endregion

		private System.Windows.Forms.Button buttonOptimize;
		private System.Windows.Forms.ProgressBar progressBarMain;
		private System.Windows.Forms.Button buttonCancel;
		private System.Windows.Forms.Label labelMax;
		private System.Windows.Forms.ProgressBar progressBarAlt;
		private System.Windows.Forms.ComboBox comboBoxCalculationToOptimize;
		private System.Windows.Forms.Label labelCalculationToOptimize;
		private System.Windows.Forms.Button buttonAddRequirement;
		private System.Windows.Forms.GroupBox groupBoxRequirements;
		private System.Windows.Forms.Label labelInfo;
        private System.Windows.Forms.GroupBox groupBoxOptions;
		private System.Windows.Forms.Label label1;
		private System.Windows.Forms.TrackBar trackBarThoroughness;
        private System.Windows.Forms.Label label2;
		private System.Windows.Forms.Label label3;
        private System.Windows.Forms.Button buttonUpgrades;
        private System.Windows.Forms.CheckBox checkBoxOverrideRegem;
        private System.Windows.Forms.CheckBox checkBoxOverrideReenchant;
        private System.Windows.Forms.CheckBox checkBoxOptimizeFood;
        private System.Windows.Forms.CheckBox checkBoxOptimizeElixir;
        private System.Windows.Forms.CheckBox checkBoxMixology;
        private System.Windows.Forms.CheckBox checkBoxOptimizeTalents;
		private System.Windows.Forms.Label label4;
		private System.Windows.Forms.GroupBox groupBox1;
		private System.Windows.Forms.Label label5;
		private System.Windows.Forms.GroupBox groupBox2;
		private System.Windows.Forms.Label label6;
        private System.Windows.Forms.CheckBox checkBoxMutateTalents;
	}
}