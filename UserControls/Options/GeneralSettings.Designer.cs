namespace Rawr.UserControls.Options
{
	partial class GeneralSettings
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(GeneralSettings));
            this.errorProvider1 = new System.Windows.Forms.ErrorProvider(this.components);
            this.CK_UseMultithreading = new System.Windows.Forms.CheckBox();
            this.label1 = new System.Windows.Forms.Label();
            this.rbGerman = new System.Windows.Forms.RadioButton();
            this.rbRussian = new System.Windows.Forms.RadioButton();
            this.rbSpanish = new System.Windows.Forms.RadioButton();
            this.rbFrench = new System.Windows.Forms.RadioButton();
            this.rbEnglish = new System.Windows.Forms.RadioButton();
            this.CK_BuffSource = new System.Windows.Forms.CheckBox();
            this.CK_GemNames = new System.Windows.Forms.CheckBox();
            this.label2 = new System.Windows.Forms.Label();
            this.CB_ProcEffectCalculationMode = new System.Windows.Forms.ComboBox();
            this.CK_DisplayItemIds = new System.Windows.Forms.CheckBox();
            this.rbZhTW = new System.Windows.Forms.RadioButton();
            this.rbZhCn = new System.Windows.Forms.RadioButton();
            this.rbKr = new System.Windows.Forms.RadioButton();
            this.CB_EffectCombinationsCalculationMode = new System.Windows.Forms.ComboBox();
            this.label3 = new System.Windows.Forms.Label();
            this.CK_HideEnchantsBasedOnProfs = new System.Windows.Forms.CheckBox();
            this.CB_ItemNameWidthSetting = new System.Windows.Forms.ComboBox();
            this.LB_ItemNameWidthSetting = new System.Windows.Forms.Label();
            ((System.ComponentModel.ISupportInitialize)(this.errorProvider1)).BeginInit();
            this.SuspendLayout();
            // 
            // errorProvider1
            // 
            this.errorProvider1.ContainerControl = this;
            // 
            // CK_UseMultithreading
            // 
            resources.ApplyResources(this.CK_UseMultithreading, "CK_UseMultithreading");
            this.CK_UseMultithreading.Name = "CK_UseMultithreading";
            this.CK_UseMultithreading.UseVisualStyleBackColor = true;
            // 
            // label1
            // 
            resources.ApplyResources(this.label1, "label1");
            this.label1.Name = "label1";
            // 
            // rbGerman
            // 
            resources.ApplyResources(this.rbGerman, "rbGerman");
            this.rbGerman.Name = "rbGerman";
            this.rbGerman.UseVisualStyleBackColor = true;
            this.rbGerman.CheckedChanged += new System.EventHandler(this.rbGerman_CheckedChanged);
            // 
            // rbRussian
            // 
            resources.ApplyResources(this.rbRussian, "rbRussian");
            this.rbRussian.Name = "rbRussian";
            this.rbRussian.UseVisualStyleBackColor = true;
            this.rbRussian.CheckedChanged += new System.EventHandler(this.rbRussian_CheckedChanged);
            // 
            // rbSpanish
            // 
            resources.ApplyResources(this.rbSpanish, "rbSpanish");
            this.rbSpanish.Name = "rbSpanish";
            this.rbSpanish.UseVisualStyleBackColor = true;
            this.rbSpanish.CheckedChanged += new System.EventHandler(this.rbSpanish_CheckedChanged);
            // 
            // rbFrench
            // 
            resources.ApplyResources(this.rbFrench, "rbFrench");
            this.rbFrench.Name = "rbFrench";
            this.rbFrench.UseVisualStyleBackColor = true;
            this.rbFrench.CheckedChanged += new System.EventHandler(this.rbFrench_CheckedChanged);
            // 
            // rbEnglish
            // 
            resources.ApplyResources(this.rbEnglish, "rbEnglish");
            this.rbEnglish.Checked = true;
            this.rbEnglish.Name = "rbEnglish";
            this.rbEnglish.TabStop = true;
            this.rbEnglish.UseVisualStyleBackColor = true;
            this.rbEnglish.CheckedChanged += new System.EventHandler(this.rbEnglish_CheckedChanged);
            // 
            // CK_BuffSource
            // 
            resources.ApplyResources(this.CK_BuffSource, "CK_BuffSource");
            this.CK_BuffSource.Name = "CK_BuffSource";
            this.CK_BuffSource.UseVisualStyleBackColor = true;
            // 
            // CK_GemNames
            // 
            resources.ApplyResources(this.CK_GemNames, "CK_GemNames");
            this.CK_GemNames.Name = "CK_GemNames";
            this.CK_GemNames.UseVisualStyleBackColor = true;
            // 
            // label2
            // 
            resources.ApplyResources(this.label2, "label2");
            this.label2.Name = "label2";
            // 
            // comboBoxProcEffectCalculationMode
            // 
            this.CB_ProcEffectCalculationMode.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.CB_ProcEffectCalculationMode.FormattingEnabled = true;
            this.CB_ProcEffectCalculationMode.Items.AddRange(new object[] {
            resources.GetString("comboBoxProcEffectCalculationMode.Items"),
            resources.GetString("comboBoxProcEffectCalculationMode.Items1"),
            resources.GetString("comboBoxProcEffectCalculationMode.Items2"),
            resources.GetString("comboBoxProcEffectCalculationMode.Items3")});
            resources.ApplyResources(this.CB_ProcEffectCalculationMode, "comboBoxProcEffectCalculationMode");
            this.CB_ProcEffectCalculationMode.Name = "comboBoxProcEffectCalculationMode";
            // 
            // CK_DisplayItemIds
            // 
            resources.ApplyResources(this.CK_DisplayItemIds, "CK_DisplayItemIds");
            this.CK_DisplayItemIds.Name = "CK_DisplayItemIds";
            this.CK_DisplayItemIds.UseVisualStyleBackColor = true;
            // 
            // rbZhTW
            // 
            resources.ApplyResources(this.rbZhTW, "rbZhTW");
            this.rbZhTW.Name = "rbZhTW";
            this.rbZhTW.UseVisualStyleBackColor = true;
            this.rbZhTW.CheckedChanged += new System.EventHandler(this.rbZhTW_CheckedChanged);
            // 
            // rbZhCn
            // 
            resources.ApplyResources(this.rbZhCn, "rbZhCn");
            this.rbZhCn.Name = "rbZhCn";
            this.rbZhCn.UseVisualStyleBackColor = true;
            this.rbZhCn.CheckedChanged += new System.EventHandler(this.rbZhCn_CheckedChanged);
            // 
            // rbKr
            // 
            resources.ApplyResources(this.rbKr, "rbKr");
            this.rbKr.Name = "rbKr";
            this.rbKr.UseVisualStyleBackColor = true;
            this.rbKr.CheckedChanged += new System.EventHandler(this.rbKr_CheckedChanged);
            // 
            // comboBoxEffectCombinationsCalculationMode
            // 
            this.CB_EffectCombinationsCalculationMode.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.CB_EffectCombinationsCalculationMode.FormattingEnabled = true;
            this.CB_EffectCombinationsCalculationMode.Items.AddRange(new object[] {
            resources.GetString("comboBoxEffectCombinationsCalculationMode.Items"),
            resources.GetString("comboBoxEffectCombinationsCalculationMode.Items1"),
            resources.GetString("comboBoxEffectCombinationsCalculationMode.Items2")});
            resources.ApplyResources(this.CB_EffectCombinationsCalculationMode, "comboBoxEffectCombinationsCalculationMode");
            this.CB_EffectCombinationsCalculationMode.Name = "comboBoxEffectCombinationsCalculationMode";
            // 
            // label3
            // 
            resources.ApplyResources(this.label3, "label3");
            this.label3.Name = "label3";
            // 
            // CK_HideEnchantsBasedOnProfs
            // 
            resources.ApplyResources(this.CK_HideEnchantsBasedOnProfs, "CK_HideEnchantsBasedOnProfs");
            this.CK_HideEnchantsBasedOnProfs.Name = "CK_HideEnchantsBasedOnProfs";
            this.CK_HideEnchantsBasedOnProfs.UseVisualStyleBackColor = true;
            // 
            // CB_ItemNameWidthSetting
            // 
            this.CB_ItemNameWidthSetting.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.CB_ItemNameWidthSetting.FormattingEnabled = true;
            this.CB_ItemNameWidthSetting.Items.AddRange(new object[] {
            resources.GetString("CB_ItemNameWidthSetting.Items"),
            resources.GetString("CB_ItemNameWidthSetting.Items1"),
            resources.GetString("CB_ItemNameWidthSetting.Items2")});
            resources.ApplyResources(this.CB_ItemNameWidthSetting, "CB_ItemNameWidthSetting");
            this.CB_ItemNameWidthSetting.Name = "CB_ItemNameWidthSetting";
            // 
            // LB_ItemNameWidthSetting
            // 
            resources.ApplyResources(this.LB_ItemNameWidthSetting, "LB_ItemNameWidthSetting");
            this.LB_ItemNameWidthSetting.Name = "LB_ItemNameWidthSetting";
            // 
            // GeneralSettings
            // 
            resources.ApplyResources(this, "$this");
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.Controls.Add(this.CB_ItemNameWidthSetting);
            this.Controls.Add(this.LB_ItemNameWidthSetting);
            this.Controls.Add(this.CK_HideEnchantsBasedOnProfs);
            this.Controls.Add(this.rbKr);
            this.Controls.Add(this.rbZhCn);
            this.Controls.Add(this.rbZhTW);
            this.Controls.Add(this.CB_EffectCombinationsCalculationMode);
            this.Controls.Add(this.label3);
            this.Controls.Add(this.CK_DisplayItemIds);
            this.Controls.Add(this.CB_ProcEffectCalculationMode);
            this.Controls.Add(this.label2);
            this.Controls.Add(this.CK_GemNames);
            this.Controls.Add(this.CK_BuffSource);
            this.Controls.Add(this.rbEnglish);
            this.Controls.Add(this.rbFrench);
            this.Controls.Add(this.rbSpanish);
            this.Controls.Add(this.rbRussian);
            this.Controls.Add(this.rbGerman);
            this.Controls.Add(this.label1);
            this.Controls.Add(this.CK_UseMultithreading);
            this.Name = "GeneralSettings";
            ((System.ComponentModel.ISupportInitialize)(this.errorProvider1)).EndInit();
            this.ResumeLayout(false);
            this.PerformLayout();

		}

		#endregion

		private System.Windows.Forms.ErrorProvider errorProvider1;
		private System.Windows.Forms.CheckBox CK_UseMultithreading;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.RadioButton rbEnglish;
        private System.Windows.Forms.RadioButton rbFrench;
        private System.Windows.Forms.RadioButton rbSpanish;
        private System.Windows.Forms.RadioButton rbRussian;
        private System.Windows.Forms.RadioButton rbGerman;
        private System.Windows.Forms.CheckBox CK_BuffSource;
        private System.Windows.Forms.CheckBox CK_GemNames;
        private System.Windows.Forms.ComboBox CB_ProcEffectCalculationMode;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.CheckBox CK_DisplayItemIds;
        private System.Windows.Forms.RadioButton rbZhCn;
        private System.Windows.Forms.RadioButton rbZhTW;
        private System.Windows.Forms.RadioButton rbKr;
        private System.Windows.Forms.ComboBox CB_EffectCombinationsCalculationMode;
        private System.Windows.Forms.Label label3;
        private System.Windows.Forms.CheckBox CK_HideEnchantsBasedOnProfs;
        private System.Windows.Forms.ComboBox CB_ItemNameWidthSetting;
        private System.Windows.Forms.Label LB_ItemNameWidthSetting;


	}
}
