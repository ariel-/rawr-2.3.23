namespace Rawr
{
	partial class TalentPicker
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
            this.panelTop = new System.Windows.Forms.Panel();
            this.talentSpecButton = new System.Windows.Forms.Button();
            this.comboBoxTalentSpec = new System.Windows.Forms.ComboBox();
            this.tabPageTree3 = new System.Windows.Forms.TabPage();
            this.talentTree3 = new Rawr.TalentTree();
            this.tabControlMain = new System.Windows.Forms.TabControl();
            this.tabPageTree1 = new System.Windows.Forms.TabPage();
            this.talentTree1 = new Rawr.TalentTree();
            this.tabPageTree2 = new System.Windows.Forms.TabPage();
            this.talentTree2 = new Rawr.TalentTree();
            this.tabPageGlyphs = new System.Windows.Forms.TabPage();
            this.grpMinorGlyph = new System.Windows.Forms.GroupBox();
            this.grpMajorGlyph = new System.Windows.Forms.GroupBox();
            this.tooltipGlyph = new System.Windows.Forms.ToolTip(this.components);
            this.panelTop.SuspendLayout();
            this.tabPageTree3.SuspendLayout();
            this.tabControlMain.SuspendLayout();
            this.tabPageTree1.SuspendLayout();
            this.tabPageTree2.SuspendLayout();
            this.tabPageGlyphs.SuspendLayout();
            this.SuspendLayout();
            // 
            // panelTop
            // 
            this.panelTop.Controls.Add(this.talentSpecButton);
            this.panelTop.Controls.Add(this.comboBoxTalentSpec);
            this.panelTop.Dock = System.Windows.Forms.DockStyle.Top;
            this.panelTop.Location = new System.Drawing.Point(0, 0);
            this.panelTop.Name = "panelTop";
            this.panelTop.Padding = new System.Windows.Forms.Padding(0, 0, 0, 3);
            this.panelTop.Size = new System.Drawing.Size(297, 24);
            this.panelTop.TabIndex = 1;
            // 
            // talentSpecButton
            // 
            this.talentSpecButton.Dock = System.Windows.Forms.DockStyle.Fill;
            this.talentSpecButton.Location = new System.Drawing.Point(241, 0);
            this.talentSpecButton.Name = "talentSpecButton";
            this.talentSpecButton.Size = new System.Drawing.Size(56, 21);
            this.talentSpecButton.TabIndex = 3;
            this.talentSpecButton.Text = "Save";
            this.talentSpecButton.UseVisualStyleBackColor = true;
            this.talentSpecButton.Click += new System.EventHandler(this.talentSpecButton_Click);
            // 
            // comboBoxTalentSpec
            // 
            this.comboBoxTalentSpec.Dock = System.Windows.Forms.DockStyle.Left;
            this.comboBoxTalentSpec.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.comboBoxTalentSpec.FormattingEnabled = true;
            this.comboBoxTalentSpec.Items.AddRange(new object[] {
            "Custom"});
            this.comboBoxTalentSpec.Location = new System.Drawing.Point(0, 0);
            this.comboBoxTalentSpec.Name = "comboBoxTalentSpec";
            this.comboBoxTalentSpec.Size = new System.Drawing.Size(241, 21);
            this.comboBoxTalentSpec.TabIndex = 1;
            this.comboBoxTalentSpec.SelectedIndexChanged += new System.EventHandler(this.comboBoxTalentSpec_SelectedIndexChanged);
            // 
            // tabPageTree3
            // 
            this.tabPageTree3.AutoScroll = true;
            this.tabPageTree3.BackColor = System.Drawing.Color.Transparent;
            this.tabPageTree3.BackgroundImageLayout = System.Windows.Forms.ImageLayout.Stretch;
            this.tabPageTree3.Controls.Add(this.talentTree3);
            this.tabPageTree3.Location = new System.Drawing.Point(4, 25);
            this.tabPageTree3.Name = "tabPageTree3";
            this.tabPageTree3.Size = new System.Drawing.Size(289, 471);
            this.tabPageTree3.TabIndex = 2;
            this.tabPageTree3.Text = "Restoration";
            this.tabPageTree3.UseVisualStyleBackColor = true;
            // 
            // talentTree3
            // 
            this.talentTree3.AutoSize = true;
            this.talentTree3.CharacterClass = Rawr.CharacterClass.Paladin;
            this.talentTree3.Location = new System.Drawing.Point(0, 0);
            this.talentTree3.Name = "talentTree3";
            this.talentTree3.Size = new System.Drawing.Size(269, 717);
            this.talentTree3.TabIndex = 0;
            this.talentTree3.TreeName = "Holy";
            // 
            // tabControlMain
            // 
            this.tabControlMain.Controls.Add(this.tabPageTree1);
            this.tabControlMain.Controls.Add(this.tabPageTree2);
            this.tabControlMain.Controls.Add(this.tabPageTree3);
            this.tabControlMain.Controls.Add(this.tabPageGlyphs);
            this.tabControlMain.Dock = System.Windows.Forms.DockStyle.Fill;
            this.tabControlMain.HotTrack = true;
            this.tabControlMain.ItemSize = new System.Drawing.Size(72, 21);
            this.tabControlMain.Location = new System.Drawing.Point(0, 24);
            this.tabControlMain.Name = "tabControlMain";
            this.tabControlMain.Padding = new System.Drawing.Point(3, 3);
            this.tabControlMain.SelectedIndex = 0;
            this.tabControlMain.Size = new System.Drawing.Size(297, 500);
            this.tabControlMain.SizeMode = System.Windows.Forms.TabSizeMode.Fixed;
            this.tabControlMain.TabIndex = 0;
            // 
            // tabPageTree1
            // 
            this.tabPageTree1.AutoScroll = true;
            this.tabPageTree1.BackColor = System.Drawing.Color.Transparent;
            this.tabPageTree1.BackgroundImageLayout = System.Windows.Forms.ImageLayout.Stretch;
            this.tabPageTree1.Controls.Add(this.talentTree1);
            this.tabPageTree1.Location = new System.Drawing.Point(4, 25);
            this.tabPageTree1.Name = "tabPageTree1";
            this.tabPageTree1.Size = new System.Drawing.Size(289, 471);
            this.tabPageTree1.TabIndex = 0;
            this.tabPageTree1.Text = "Balance";
            this.tabPageTree1.UseVisualStyleBackColor = true;
            // 
            // talentTree1
            // 
            this.talentTree1.AutoSize = true;
            this.talentTree1.CharacterClass = Rawr.CharacterClass.Paladin;
            this.talentTree1.Location = new System.Drawing.Point(0, 0);
            this.talentTree1.Name = "talentTree1";
            this.talentTree1.Size = new System.Drawing.Size(270, 717);
            this.talentTree1.TabIndex = 0;
            this.talentTree1.TreeName = "Holy";
            // 
            // tabPageTree2
            // 
            this.tabPageTree2.AutoScroll = true;
            this.tabPageTree2.BackColor = System.Drawing.Color.Transparent;
            this.tabPageTree2.BackgroundImageLayout = System.Windows.Forms.ImageLayout.Stretch;
            this.tabPageTree2.Controls.Add(this.talentTree2);
            this.tabPageTree2.Location = new System.Drawing.Point(4, 25);
            this.tabPageTree2.Name = "tabPageTree2";
            this.tabPageTree2.Size = new System.Drawing.Size(289, 471);
            this.tabPageTree2.TabIndex = 1;
            this.tabPageTree2.Text = "Feral Combat";
            this.tabPageTree2.UseVisualStyleBackColor = true;
            // 
            // talentTree2
            // 
            this.talentTree2.AutoSize = true;
            this.talentTree2.CharacterClass = Rawr.CharacterClass.Paladin;
            this.talentTree2.Location = new System.Drawing.Point(0, 0);
            this.talentTree2.Name = "talentTree2";
            this.talentTree2.Size = new System.Drawing.Size(269, 717);
            this.talentTree2.TabIndex = 0;
            this.talentTree2.TreeName = "Holy";
            // 
            // tabPageGlyphs
            // 
            this.tabPageGlyphs.AutoScroll = true;
            this.tabPageGlyphs.Controls.Add(this.grpMinorGlyph);
            this.tabPageGlyphs.Controls.Add(this.grpMajorGlyph);
            this.tabPageGlyphs.Location = new System.Drawing.Point(4, 25);
            this.tabPageGlyphs.Name = "tabPageGlyphs";
            this.tabPageGlyphs.Size = new System.Drawing.Size(289, 471);
            this.tabPageGlyphs.TabIndex = 3;
            this.tabPageGlyphs.Text = "Glyphs";
            this.tabPageGlyphs.UseVisualStyleBackColor = true;
            // 
            // grpMinorGlyph
            // 
            this.grpMinorGlyph.AutoSize = true;
            this.grpMinorGlyph.Dock = System.Windows.Forms.DockStyle.Top;
            this.grpMinorGlyph.Location = new System.Drawing.Point(0, 5);
            this.grpMinorGlyph.Name = "grpMinorGlyph";
            this.grpMinorGlyph.Size = new System.Drawing.Size(289, 5);
            this.grpMinorGlyph.TabIndex = 1;
            this.grpMinorGlyph.TabStop = false;
            this.grpMinorGlyph.Text = "Minor";
            // 
            // grpMajorGlyph
            // 
            this.grpMajorGlyph.AutoSize = true;
            this.grpMajorGlyph.Dock = System.Windows.Forms.DockStyle.Top;
            this.grpMajorGlyph.Location = new System.Drawing.Point(0, 0);
            this.grpMajorGlyph.Name = "grpMajorGlyph";
            this.grpMajorGlyph.Size = new System.Drawing.Size(289, 5);
            this.grpMajorGlyph.TabIndex = 0;
            this.grpMajorGlyph.TabStop = false;
            this.grpMajorGlyph.Text = "Major";
            // 
            // TalentPicker
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.Controls.Add(this.tabControlMain);
            this.Controls.Add(this.panelTop);
            this.Name = "TalentPicker";
            this.Size = new System.Drawing.Size(297, 524);
            this.panelTop.ResumeLayout(false);
            this.tabPageTree3.ResumeLayout(false);
            this.tabPageTree3.PerformLayout();
            this.tabControlMain.ResumeLayout(false);
            this.tabPageTree1.ResumeLayout(false);
            this.tabPageTree1.PerformLayout();
            this.tabPageTree2.ResumeLayout(false);
            this.tabPageTree2.PerformLayout();
            this.tabPageGlyphs.ResumeLayout(false);
            this.tabPageGlyphs.PerformLayout();
            this.ResumeLayout(false);

		}

		#endregion

        private System.Windows.Forms.Panel panelTop;
        private System.Windows.Forms.ComboBox comboBoxTalentSpec;
        private System.Windows.Forms.TabPage tabPageTree3;
        private System.Windows.Forms.TabControl tabControlMain;
        private System.Windows.Forms.TabPage tabPageTree1;
        private System.Windows.Forms.TabPage tabPageTree2;
        private TalentTree talentTree1;
        private TalentTree talentTree3;
        private TalentTree talentTree2;
        private System.Windows.Forms.Button talentSpecButton;
        private System.Windows.Forms.TabPage tabPageGlyphs;
        private System.Windows.Forms.GroupBox grpMajorGlyph;
        private System.Windows.Forms.GroupBox grpMinorGlyph;
        private System.Windows.Forms.ToolTip tooltipGlyph;
	}
}
