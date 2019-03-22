namespace Rawr.UserControls.Options
{
	partial class OptimizerSettings
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(OptimizerSettings));
            this.groupBox1 = new System.Windows.Forms.GroupBox();
            this.comboBoxGreedyOptimizationMethod = new System.Windows.Forms.ComboBox();
            this.checkBoxTemplateGemsEnabled = new System.Windows.Forms.CheckBox();
            this.comboBoxOptimizationMethod = new System.Windows.Forms.ComboBox();
            this.label1 = new System.Windows.Forms.Label();
            this.WarningsEnabledCheckBox = new System.Windows.Forms.CheckBox();
            this.errorProvider1 = new System.Windows.Forms.ErrorProvider(this.components);
            this.groupBox1.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.errorProvider1)).BeginInit();
            this.SuspendLayout();
            // 
            // groupBox1
            // 
            this.groupBox1.Controls.Add(this.comboBoxGreedyOptimizationMethod);
            this.groupBox1.Controls.Add(this.checkBoxTemplateGemsEnabled);
            this.groupBox1.Controls.Add(this.comboBoxOptimizationMethod);
            this.groupBox1.Controls.Add(this.label1);
            this.groupBox1.Controls.Add(this.WarningsEnabledCheckBox);
            resources.ApplyResources(this.groupBox1, "groupBox1");
            this.groupBox1.Name = "groupBox1";
            this.groupBox1.TabStop = false;
            // 
            // comboBoxGreedyOptimizationMethod
            // 
            this.comboBoxGreedyOptimizationMethod.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.comboBoxGreedyOptimizationMethod.FormattingEnabled = true;
            this.comboBoxGreedyOptimizationMethod.Items.AddRange(new object[] {
            resources.GetString("comboBoxGreedyOptimizationMethod.Items"),
            resources.GetString("comboBoxGreedyOptimizationMethod.Items1"),
            resources.GetString("comboBoxGreedyOptimizationMethod.Items2")});
            resources.ApplyResources(this.comboBoxGreedyOptimizationMethod, "comboBoxGreedyOptimizationMethod");
            this.comboBoxGreedyOptimizationMethod.Name = "comboBoxGreedyOptimizationMethod";
            // 
            // checkBoxTemplateGemsEnabled
            // 
            resources.ApplyResources(this.checkBoxTemplateGemsEnabled, "checkBoxTemplateGemsEnabled");
            this.checkBoxTemplateGemsEnabled.Name = "checkBoxTemplateGemsEnabled";
            this.checkBoxTemplateGemsEnabled.UseVisualStyleBackColor = true;
            // 
            // comboBoxOptimizationMethod
            // 
            this.comboBoxOptimizationMethod.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.comboBoxOptimizationMethod.FormattingEnabled = true;
            this.comboBoxOptimizationMethod.Items.AddRange(new object[] {
            resources.GetString("comboBoxOptimizationMethod.Items"),
            resources.GetString("comboBoxOptimizationMethod.Items1")});
            resources.ApplyResources(this.comboBoxOptimizationMethod, "comboBoxOptimizationMethod");
            this.comboBoxOptimizationMethod.Name = "comboBoxOptimizationMethod";
            // 
            // label1
            // 
            resources.ApplyResources(this.label1, "label1");
            this.label1.Name = "label1";
            // 
            // WarningsEnabledCheckBox
            // 
            resources.ApplyResources(this.WarningsEnabledCheckBox, "WarningsEnabledCheckBox");
            this.WarningsEnabledCheckBox.Name = "WarningsEnabledCheckBox";
            this.WarningsEnabledCheckBox.UseVisualStyleBackColor = true;
            // 
            // errorProvider1
            // 
            this.errorProvider1.ContainerControl = this;
            // 
            // OptimizerSettings
            // 
            resources.ApplyResources(this, "$this");
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.Controls.Add(this.groupBox1);
            this.Name = "OptimizerSettings";
            this.groupBox1.ResumeLayout(false);
            this.groupBox1.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.errorProvider1)).EndInit();
            this.ResumeLayout(false);

		}

		#endregion

        private System.Windows.Forms.GroupBox groupBox1;
        private System.Windows.Forms.ErrorProvider errorProvider1;
        private System.Windows.Forms.CheckBox WarningsEnabledCheckBox;
        private System.Windows.Forms.ComboBox comboBoxOptimizationMethod;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.CheckBox checkBoxTemplateGemsEnabled;
        private System.Windows.Forms.ComboBox comboBoxGreedyOptimizationMethod;


	}
}
