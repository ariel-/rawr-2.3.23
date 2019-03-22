namespace Rawr
{
    partial class DG_BossHandler
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(DG_BossHandler));
            this.BT_Disarm = new System.Windows.Forms.Button();
            this.BT_Root = new System.Windows.Forms.Button();
            this.BT_Fear = new System.Windows.Forms.Button();
            this.BT_Move = new System.Windows.Forms.Button();
            this.BT_Stun = new System.Windows.Forms.Button();
            this.NUD_TargHP = new System.Windows.Forms.NumericUpDown();
            this.LB_Under35Perc2 = new System.Windows.Forms.Label();
            this.LB_InBack = new System.Windows.Forms.Label();
            this.LB_Freq2 = new System.Windows.Forms.Label();
            this.LB_UnmitDmg = new System.Windows.Forms.Label();
            this.NUD_AoEDMG = new System.Windows.Forms.NumericUpDown();
            this.NUD_AoEFreq = new System.Windows.Forms.NumericUpDown();
            this.CK_AoETargs = new Rawr.CustomControls.ExtendedToolTipCheckBox();
            this.LB_MultiTargsPerc = new System.Windows.Forms.Label();
            this.NUD_Under35Perc = new System.Windows.Forms.NumericUpDown();
            this.CK_RootingTargs = new Rawr.CustomControls.ExtendedToolTipCheckBox();
            this.CK_FearingTargs = new Rawr.CustomControls.ExtendedToolTipCheckBox();
            this.LB_Where = new System.Windows.Forms.Label();
            this.LB_Is = new System.Windows.Forms.Label();
            this.CB_BL_FilterType = new System.Windows.Forms.ComboBox();
            this.CB_BL_Filter = new System.Windows.Forms.ComboBox();
            this.TB_BossInfo = new System.Windows.Forms.TextBox();
            this.CB_BossList = new System.Windows.Forms.ComboBox();
            this.LB_Max = new System.Windows.Forms.Label();
            this.CB_MultiTargsMax = new System.Windows.Forms.NumericUpDown();
            this.CK_MultiTargs = new Rawr.CustomControls.ExtendedToolTipCheckBox();
            this.CB_InBackPerc = new System.Windows.Forms.NumericUpDown();
            this.CK_MovingTargs = new Rawr.CustomControls.ExtendedToolTipCheckBox();
            this.CB_MultiTargsPerc = new System.Windows.Forms.NumericUpDown();
            this.CK_DisarmTargs = new Rawr.CustomControls.ExtendedToolTipCheckBox();
            this.CK_StunningTargs = new Rawr.CustomControls.ExtendedToolTipCheckBox();
            this.LB_Duration = new System.Windows.Forms.Label();
            this.CB_Duration = new System.Windows.Forms.NumericUpDown();
            this.LB_TargLvl = new System.Windows.Forms.Label();
            this.CB_TargLvl = new System.Windows.Forms.ComboBox();
            this.CB_TargArmor = new System.Windows.Forms.ComboBox();
            this.LB_TargArmor = new System.Windows.Forms.Label();
            this.LB_Under35Perc = new Rawr.CustomControls.ExtendedToolTipLabel();
            this.LB_Notice = new System.Windows.Forms.Label();
            this.GB_Presets = new System.Windows.Forms.GroupBox();
            this.TLP_Stats = new System.Windows.Forms.TableLayoutPanel();
            this.LB_Under20Perc = new Rawr.CustomControls.ExtendedToolTipLabel();
            this.NUD_Under20Perc = new System.Windows.Forms.NumericUpDown();
            this.LB_Under20Perc2 = new System.Windows.Forms.Label();
            this.PN_MultiTargs = new System.Windows.Forms.Panel();
            this.LB_TargHP = new System.Windows.Forms.Label();
            this.LB_Col1 = new System.Windows.Forms.Label();
            this.LB_Col2 = new System.Windows.Forms.Label();
            this.LB_Col3 = new System.Windows.Forms.Label();
            this.CK_InBack = new Rawr.CustomControls.ExtendedToolTipCheckBox();
            ((System.ComponentModel.ISupportInitialize)(this.NUD_TargHP)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.NUD_AoEDMG)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.NUD_AoEFreq)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.NUD_Under35Perc)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.CB_MultiTargsMax)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.CB_InBackPerc)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.CB_MultiTargsPerc)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.CB_Duration)).BeginInit();
            this.GB_Presets.SuspendLayout();
            this.TLP_Stats.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.NUD_Under20Perc)).BeginInit();
            this.PN_MultiTargs.SuspendLayout();
            this.SuspendLayout();
            // 
            // BT_Disarm
            // 
            this.BT_Disarm.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left)
                        | System.Windows.Forms.AnchorStyles.Right)));
            this.TLP_Stats.SetColumnSpan(this.BT_Disarm, 2);
            this.BT_Disarm.Enabled = false;
            this.BT_Disarm.Font = new System.Drawing.Font("Microsoft Sans Serif", 6.5F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.BT_Disarm.Location = new System.Drawing.Point(166, 335);
            this.BT_Disarm.Name = "BT_Disarm";
            this.BT_Disarm.Size = new System.Drawing.Size(197, 20);
            this.BT_Disarm.TabIndex = 107;
            this.BT_Disarm.Text = "F: 120sec D: 5000ms C: 100% : B";
            this.BT_Disarm.UseVisualStyleBackColor = true;
            // 
            // BT_Root
            // 
            this.BT_Root.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left)
                        | System.Windows.Forms.AnchorStyles.Right)));
            this.TLP_Stats.SetColumnSpan(this.BT_Root, 2);
            this.BT_Root.Font = new System.Drawing.Font("Microsoft Sans Serif", 6.5F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.BT_Root.Location = new System.Drawing.Point(166, 309);
            this.BT_Root.Name = "BT_Root";
            this.BT_Root.Size = new System.Drawing.Size(197, 20);
            this.BT_Root.TabIndex = 106;
            this.BT_Root.Text = "F: 120sec D: 5000ms C: 100% : B";
            this.BT_Root.UseVisualStyleBackColor = true;
            // 
            // BT_Fear
            // 
            this.BT_Fear.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left)
                        | System.Windows.Forms.AnchorStyles.Right)));
            this.TLP_Stats.SetColumnSpan(this.BT_Fear, 2);
            this.BT_Fear.Font = new System.Drawing.Font("Microsoft Sans Serif", 6.5F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.BT_Fear.Location = new System.Drawing.Point(166, 283);
            this.BT_Fear.Name = "BT_Fear";
            this.BT_Fear.Size = new System.Drawing.Size(197, 20);
            this.BT_Fear.TabIndex = 105;
            this.BT_Fear.Text = "F: 120sec D: 5000ms C: 100% : B";
            this.BT_Fear.UseVisualStyleBackColor = true;
            // 
            // BT_Move
            // 
            this.BT_Move.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left)
                        | System.Windows.Forms.AnchorStyles.Right)));
            this.TLP_Stats.SetColumnSpan(this.BT_Move, 2);
            this.BT_Move.Font = new System.Drawing.Font("Microsoft Sans Serif", 6.5F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.BT_Move.Location = new System.Drawing.Point(166, 257);
            this.BT_Move.Name = "BT_Move";
            this.BT_Move.Size = new System.Drawing.Size(197, 20);
            this.BT_Move.TabIndex = 104;
            this.BT_Move.Text = "F: 120sec D: 5000ms C: 100% : B";
            this.BT_Move.UseVisualStyleBackColor = true;
            // 
            // BT_Stun
            // 
            this.BT_Stun.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left)
                        | System.Windows.Forms.AnchorStyles.Right)));
            this.TLP_Stats.SetColumnSpan(this.BT_Stun, 2);
            this.BT_Stun.Font = new System.Drawing.Font("Microsoft Sans Serif", 6.5F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.BT_Stun.Location = new System.Drawing.Point(166, 231);
            this.BT_Stun.Name = "BT_Stun";
            this.BT_Stun.Size = new System.Drawing.Size(197, 20);
            this.BT_Stun.TabIndex = 103;
            this.BT_Stun.Text = "F: 120sec D: 5000ms C: 100% : B";
            this.BT_Stun.UseVisualStyleBackColor = true;
            // 
            // NUD_TargHP
            // 
            this.NUD_TargHP.Dock = System.Windows.Forms.DockStyle.Fill;
            this.NUD_TargHP.Location = new System.Drawing.Point(166, 101);
            this.NUD_TargHP.Maximum = new decimal(new int[] {
            100000000,
            0,
            0,
            0});
            this.NUD_TargHP.Minimum = new decimal(new int[] {
            1,
            0,
            0,
            0});
            this.NUD_TargHP.Name = "NUD_TargHP";
            this.NUD_TargHP.Size = new System.Drawing.Size(157, 20);
            this.NUD_TargHP.TabIndex = 102;
            this.NUD_TargHP.TextAlign = System.Windows.Forms.HorizontalAlignment.Right;
            this.NUD_TargHP.ThousandsSeparator = true;
            this.NUD_TargHP.Value = new decimal(new int[] {
            1000000,
            0,
            0,
            0});
            // 
            // LB_Under35Perc2
            // 
            this.LB_Under35Perc2.BackColor = System.Drawing.Color.Transparent;
            this.LB_Under35Perc2.Dock = System.Windows.Forms.DockStyle.Fill;
            this.LB_Under35Perc2.Location = new System.Drawing.Point(329, 124);
            this.LB_Under35Perc2.Name = "LB_Under35Perc2";
            this.LB_Under35Perc2.Size = new System.Drawing.Size(34, 26);
            this.LB_Under35Perc2.TabIndex = 101;
            this.LB_Under35Perc2.Text = "%";
            this.LB_Under35Perc2.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            // 
            // LB_InBack
            // 
            this.LB_InBack.BackColor = System.Drawing.Color.Transparent;
            this.LB_InBack.Dock = System.Windows.Forms.DockStyle.Fill;
            this.LB_InBack.Location = new System.Drawing.Point(329, 176);
            this.LB_InBack.Name = "LB_InBack";
            this.LB_InBack.Size = new System.Drawing.Size(34, 26);
            this.LB_InBack.TabIndex = 100;
            this.LB_InBack.Text = "%";
            this.LB_InBack.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            // 
            // LB_Freq2
            // 
            this.LB_Freq2.BackColor = System.Drawing.Color.Transparent;
            this.LB_Freq2.Location = new System.Drawing.Point(175, 488);
            this.LB_Freq2.Name = "LB_Freq2";
            this.LB_Freq2.Size = new System.Drawing.Size(54, 20);
            this.LB_Freq2.TabIndex = 94;
            this.LB_Freq2.Text = "Freq (sec)";
            this.LB_Freq2.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // LB_UnmitDmg
            // 
            this.LB_UnmitDmg.Anchor = System.Windows.Forms.AnchorStyles.Top;
            this.LB_UnmitDmg.BackColor = System.Drawing.Color.Transparent;
            this.LB_UnmitDmg.Location = new System.Drawing.Point(236, 488);
            this.LB_UnmitDmg.Name = "LB_UnmitDmg";
            this.LB_UnmitDmg.Size = new System.Drawing.Size(67, 20);
            this.LB_UnmitDmg.TabIndex = 95;
            this.LB_UnmitDmg.Text = "Unmit DMG";
            this.LB_UnmitDmg.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // NUD_AoEDMG
            // 
            this.NUD_AoEDMG.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left)
                        | System.Windows.Forms.AnchorStyles.Right)));
            this.NUD_AoEDMG.Enabled = false;
            this.NUD_AoEDMG.Location = new System.Drawing.Point(233, 510);
            this.NUD_AoEDMG.Maximum = new decimal(new int[] {
            500000,
            0,
            0,
            0});
            this.NUD_AoEDMG.Minimum = new decimal(new int[] {
            1000,
            0,
            0,
            0});
            this.NUD_AoEDMG.Name = "NUD_AoEDMG";
            this.NUD_AoEDMG.Size = new System.Drawing.Size(73, 20);
            this.NUD_AoEDMG.TabIndex = 98;
            this.NUD_AoEDMG.TextAlign = System.Windows.Forms.HorizontalAlignment.Right;
            this.NUD_AoEDMG.Value = new decimal(new int[] {
            5000,
            0,
            0,
            0});
            // 
            // NUD_AoEFreq
            // 
            this.NUD_AoEFreq.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left)
                        | System.Windows.Forms.AnchorStyles.Right)));
            this.NUD_AoEFreq.Enabled = false;
            this.NUD_AoEFreq.Location = new System.Drawing.Point(177, 510);
            this.NUD_AoEFreq.Maximum = new decimal(new int[] {
            1200,
            0,
            0,
            0});
            this.NUD_AoEFreq.Name = "NUD_AoEFreq";
            this.NUD_AoEFreq.Size = new System.Drawing.Size(56, 20);
            this.NUD_AoEFreq.TabIndex = 97;
            this.NUD_AoEFreq.TextAlign = System.Windows.Forms.HorizontalAlignment.Right;
            this.NUD_AoEFreq.Value = new decimal(new int[] {
            20,
            0,
            0,
            0});
            // 
            // CK_AoETargs
            // 
            this.CK_AoETargs.Location = new System.Drawing.Point(16, 510);
            this.CK_AoETargs.Name = "CK_AoETargs";
            this.CK_AoETargs.Size = new System.Drawing.Size(155, 20);
            this.CK_AoETargs.TabIndex = 96;
            this.CK_AoETargs.Text = "Boss Does AoE Damage *";
            this.CK_AoETargs.ToolTipText = "";
            // 
            // LB_MultiTargsPerc
            // 
            this.LB_MultiTargsPerc.BackColor = System.Drawing.Color.Transparent;
            this.LB_MultiTargsPerc.Dock = System.Windows.Forms.DockStyle.Fill;
            this.LB_MultiTargsPerc.Location = new System.Drawing.Point(329, 202);
            this.LB_MultiTargsPerc.Name = "LB_MultiTargsPerc";
            this.LB_MultiTargsPerc.Size = new System.Drawing.Size(34, 26);
            this.LB_MultiTargsPerc.TabIndex = 74;
            this.LB_MultiTargsPerc.Text = "%";
            this.LB_MultiTargsPerc.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            // 
            // NUD_Under35Perc
            // 
            this.NUD_Under35Perc.Dock = System.Windows.Forms.DockStyle.Fill;
            this.NUD_Under35Perc.Location = new System.Drawing.Point(166, 127);
            this.NUD_Under35Perc.Maximum = new decimal(new int[] {
            25,
            0,
            0,
            0});
            this.NUD_Under35Perc.Minimum = new decimal(new int[] {
            10,
            0,
            0,
            0});
            this.NUD_Under35Perc.Name = "NUD_Under35Perc";
            this.NUD_Under35Perc.Size = new System.Drawing.Size(157, 20);
            this.NUD_Under35Perc.TabIndex = 73;
            this.NUD_Under35Perc.TextAlign = System.Windows.Forms.HorizontalAlignment.Right;
            this.NUD_Under35Perc.Value = new decimal(new int[] {
            17,
            0,
            0,
            0});
            // 
            // CK_RootingTargs
            // 
            this.CK_RootingTargs.BackColor = System.Drawing.Color.Transparent;
            this.CK_RootingTargs.Location = new System.Drawing.Point(3, 309);
            this.CK_RootingTargs.Name = "CK_RootingTargs";
            this.CK_RootingTargs.Size = new System.Drawing.Size(155, 20);
            this.CK_RootingTargs.TabIndex = 88;
            this.CK_RootingTargs.Text = "Boss Roots You *";
            this.CK_RootingTargs.ToolTipText = "";
            this.CK_RootingTargs.UseVisualStyleBackColor = false;
            // 
            // CK_FearingTargs
            // 
            this.CK_FearingTargs.Location = new System.Drawing.Point(3, 283);
            this.CK_FearingTargs.Name = "CK_FearingTargs";
            this.CK_FearingTargs.Size = new System.Drawing.Size(155, 20);
            this.CK_FearingTargs.TabIndex = 87;
            this.CK_FearingTargs.Text = "Boss Fears You *";
            this.CK_FearingTargs.ToolTipText = "";
            this.CK_FearingTargs.UseVisualStyleBackColor = false;
            // 
            // LB_Where
            // 
            this.LB_Where.BackColor = System.Drawing.Color.Transparent;
            this.LB_Where.Location = new System.Drawing.Point(6, 19);
            this.LB_Where.Name = "LB_Where";
            this.LB_Where.Size = new System.Drawing.Size(40, 21);
            this.LB_Where.TabIndex = 61;
            this.LB_Where.Text = "Where";
            this.LB_Where.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            // 
            // LB_Is
            // 
            this.LB_Is.BackColor = System.Drawing.Color.Transparent;
            this.LB_Is.Location = new System.Drawing.Point(133, 19);
            this.LB_Is.Name = "LB_Is";
            this.LB_Is.Size = new System.Drawing.Size(14, 21);
            this.LB_Is.TabIndex = 63;
            this.LB_Is.Text = "is";
            this.LB_Is.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // CB_BL_FilterType
            // 
            this.CB_BL_FilterType.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.CB_BL_FilterType.FormattingEnabled = true;
            this.CB_BL_FilterType.Items.AddRange(new object[] {
            "Content",
            "Instance",
            "Version",
            "Name"});
            this.CB_BL_FilterType.Location = new System.Drawing.Point(49, 19);
            this.CB_BL_FilterType.Name = "CB_BL_FilterType";
            this.CB_BL_FilterType.Size = new System.Drawing.Size(81, 21);
            this.CB_BL_FilterType.TabIndex = 62;
            // 
            // CB_BL_Filter
            // 
            this.CB_BL_Filter.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left)
                        | System.Windows.Forms.AnchorStyles.Right)));
            this.CB_BL_Filter.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.CB_BL_Filter.FormattingEnabled = true;
            this.CB_BL_Filter.Items.AddRange(new object[] {
            "All"});
            this.CB_BL_Filter.Location = new System.Drawing.Point(150, 19);
            this.CB_BL_Filter.Name = "CB_BL_Filter";
            this.CB_BL_Filter.Size = new System.Drawing.Size(210, 21);
            this.CB_BL_Filter.TabIndex = 64;
            // 
            // TB_BossInfo
            // 
            this.TB_BossInfo.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom)
                        | System.Windows.Forms.AnchorStyles.Right)));
            this.TB_BossInfo.Location = new System.Drawing.Point(385, 12);
            this.TB_BossInfo.MaxLength = 65536;
            this.TB_BossInfo.Multiline = true;
            this.TB_BossInfo.Name = "TB_BossInfo";
            this.TB_BossInfo.ReadOnly = true;
            this.TB_BossInfo.ScrollBars = System.Windows.Forms.ScrollBars.Vertical;
            this.TB_BossInfo.Size = new System.Drawing.Size(280, 532);
            this.TB_BossInfo.TabIndex = 99;
            this.TB_BossInfo.Text = "Boss Information would normally be displayed here";
            // 
            // CB_BossList
            // 
            this.CB_BossList.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left)
                        | System.Windows.Forms.AnchorStyles.Right)));
            this.CB_BossList.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.CB_BossList.DropDownWidth = 250;
            this.CB_BossList.FormattingEnabled = true;
            this.CB_BossList.Items.AddRange(new object[] {
            "Custom"});
            this.CB_BossList.Location = new System.Drawing.Point(6, 43);
            this.CB_BossList.Name = "CB_BossList";
            this.CB_BossList.Size = new System.Drawing.Size(354, 21);
            this.CB_BossList.TabIndex = 65;
            // 
            // LB_Max
            // 
            this.LB_Max.BackColor = System.Drawing.Color.Transparent;
            this.LB_Max.Enabled = false;
            this.LB_Max.Location = new System.Drawing.Point(4, 1);
            this.LB_Max.Name = "LB_Max";
            this.LB_Max.Size = new System.Drawing.Size(30, 20);
            this.LB_Max.TabIndex = 78;
            this.LB_Max.Text = "Max:";
            this.LB_Max.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // CB_MultiTargsMax
            // 
            this.CB_MultiTargsMax.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left)
                        | System.Windows.Forms.AnchorStyles.Right)));
            this.CB_MultiTargsMax.Enabled = false;
            this.CB_MultiTargsMax.Location = new System.Drawing.Point(40, 2);
            this.CB_MultiTargsMax.Maximum = new decimal(new int[] {
            8,
            0,
            0,
            0});
            this.CB_MultiTargsMax.Minimum = new decimal(new int[] {
            1,
            0,
            0,
            0});
            this.CB_MultiTargsMax.Name = "CB_MultiTargsMax";
            this.CB_MultiTargsMax.Size = new System.Drawing.Size(52, 20);
            this.CB_MultiTargsMax.TabIndex = 79;
            this.CB_MultiTargsMax.TextAlign = System.Windows.Forms.HorizontalAlignment.Center;
            this.CB_MultiTargsMax.Value = new decimal(new int[] {
            4,
            0,
            0,
            0});
            // 
            // CK_MultiTargs
            // 
            this.CK_MultiTargs.BackColor = System.Drawing.Color.Transparent;
            this.CK_MultiTargs.Dock = System.Windows.Forms.DockStyle.Fill;
            this.CK_MultiTargs.Location = new System.Drawing.Point(3, 205);
            this.CK_MultiTargs.Name = "CK_MultiTargs";
            this.CK_MultiTargs.Size = new System.Drawing.Size(157, 20);
            this.CK_MultiTargs.TabIndex = 77;
            this.CK_MultiTargs.Text = "Multiple Targets *";
            this.CK_MultiTargs.ToolTipText = "";
            this.CK_MultiTargs.UseVisualStyleBackColor = false;
            // 
            // CB_InBackPerc
            // 
            this.CB_InBackPerc.Dock = System.Windows.Forms.DockStyle.Fill;
            this.CB_InBackPerc.Location = new System.Drawing.Point(166, 179);
            this.CB_InBackPerc.Name = "CB_InBackPerc";
            this.CB_InBackPerc.Size = new System.Drawing.Size(157, 20);
            this.CB_InBackPerc.TabIndex = 76;
            this.CB_InBackPerc.TextAlign = System.Windows.Forms.HorizontalAlignment.Right;
            this.CB_InBackPerc.Value = new decimal(new int[] {
            100,
            0,
            0,
            0});
            // 
            // CK_MovingTargs
            // 
            this.CK_MovingTargs.BackColor = System.Drawing.Color.Transparent;
            this.CK_MovingTargs.Location = new System.Drawing.Point(3, 257);
            this.CK_MovingTargs.Name = "CK_MovingTargs";
            this.CK_MovingTargs.Size = new System.Drawing.Size(155, 20);
            this.CK_MovingTargs.TabIndex = 86;
            this.CK_MovingTargs.Text = "You have to Move *";
            this.CK_MovingTargs.ToolTipText = "";
            this.CK_MovingTargs.UseVisualStyleBackColor = false;
            // 
            // CB_MultiTargsPerc
            // 
            this.CB_MultiTargsPerc.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.CB_MultiTargsPerc.Enabled = false;
            this.CB_MultiTargsPerc.Location = new System.Drawing.Point(98, 2);
            this.CB_MultiTargsPerc.Name = "CB_MultiTargsPerc";
            this.CB_MultiTargsPerc.Size = new System.Drawing.Size(62, 20);
            this.CB_MultiTargsPerc.TabIndex = 80;
            this.CB_MultiTargsPerc.TextAlign = System.Windows.Forms.HorizontalAlignment.Right;
            this.CB_MultiTargsPerc.Value = new decimal(new int[] {
            100,
            0,
            0,
            0});
            // 
            // CK_DisarmTargs
            // 
            this.CK_DisarmTargs.Enabled = false;
            this.CK_DisarmTargs.Location = new System.Drawing.Point(3, 335);
            this.CK_DisarmTargs.Name = "CK_DisarmTargs";
            this.CK_DisarmTargs.Size = new System.Drawing.Size(155, 20);
            this.CK_DisarmTargs.TabIndex = 91;
            this.CK_DisarmTargs.Text = "Boss Disarms You *";
            this.CK_DisarmTargs.ToolTipText = "";
            this.CK_DisarmTargs.UseVisualStyleBackColor = false;
            // 
            // CK_StunningTargs
            // 
            this.CK_StunningTargs.BackColor = System.Drawing.Color.Transparent;
            this.CK_StunningTargs.Location = new System.Drawing.Point(3, 231);
            this.CK_StunningTargs.Name = "CK_StunningTargs";
            this.CK_StunningTargs.Size = new System.Drawing.Size(155, 20);
            this.CK_StunningTargs.TabIndex = 83;
            this.CK_StunningTargs.Text = "Boss Stuns You *";
            this.CK_StunningTargs.ToolTipText = "";
            this.CK_StunningTargs.UseVisualStyleBackColor = false;
            // 
            // LB_Duration
            // 
            this.LB_Duration.BackColor = System.Drawing.Color.Transparent;
            this.LB_Duration.Dock = System.Windows.Forms.DockStyle.Fill;
            this.LB_Duration.Location = new System.Drawing.Point(3, 72);
            this.LB_Duration.Name = "LB_Duration";
            this.LB_Duration.Size = new System.Drawing.Size(157, 26);
            this.LB_Duration.TabIndex = 70;
            this.LB_Duration.Text = "Duration(sec)";
            this.LB_Duration.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            // 
            // CB_Duration
            // 
            this.CB_Duration.Dock = System.Windows.Forms.DockStyle.Fill;
            this.CB_Duration.Location = new System.Drawing.Point(166, 75);
            this.CB_Duration.Maximum = new decimal(new int[] {
            1200,
            0,
            0,
            0});
            this.CB_Duration.Minimum = new decimal(new int[] {
            30,
            0,
            0,
            0});
            this.CB_Duration.Name = "CB_Duration";
            this.CB_Duration.Size = new System.Drawing.Size(157, 20);
            this.CB_Duration.TabIndex = 71;
            this.CB_Duration.TextAlign = System.Windows.Forms.HorizontalAlignment.Right;
            this.CB_Duration.ThousandsSeparator = true;
            this.CB_Duration.Value = new decimal(new int[] {
            300,
            0,
            0,
            0});
            // 
            // LB_TargLvl
            // 
            this.LB_TargLvl.BackColor = System.Drawing.Color.Transparent;
            this.LB_TargLvl.Dock = System.Windows.Forms.DockStyle.Fill;
            this.LB_TargLvl.Location = new System.Drawing.Point(3, 20);
            this.LB_TargLvl.Name = "LB_TargLvl";
            this.LB_TargLvl.Size = new System.Drawing.Size(157, 26);
            this.LB_TargLvl.TabIndex = 66;
            this.LB_TargLvl.Text = "Targ Level:";
            this.LB_TargLvl.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            // 
            // CB_TargLvl
            // 
            this.CB_TargLvl.Dock = System.Windows.Forms.DockStyle.Fill;
            this.CB_TargLvl.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.CB_TargLvl.Items.AddRange(new object[] {
            "80",
            "81",
            "82",
            "83"});
            this.CB_TargLvl.Location = new System.Drawing.Point(166, 23);
            this.CB_TargLvl.Name = "CB_TargLvl";
            this.CB_TargLvl.Size = new System.Drawing.Size(157, 21);
            this.CB_TargLvl.Sorted = true;
            this.CB_TargLvl.TabIndex = 67;
            // 
            // CB_TargArmor
            // 
            this.CB_TargArmor.Dock = System.Windows.Forms.DockStyle.Fill;
            this.CB_TargArmor.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.CB_TargArmor.Items.AddRange(new object[] {
            "10643",
            "10338",
            "10034",
            "9729"});
            this.CB_TargArmor.Location = new System.Drawing.Point(166, 49);
            this.CB_TargArmor.Name = "CB_TargArmor";
            this.CB_TargArmor.Size = new System.Drawing.Size(157, 21);
            this.CB_TargArmor.TabIndex = 69;
            // 
            // LB_TargArmor
            // 
            this.LB_TargArmor.BackColor = System.Drawing.Color.Transparent;
            this.LB_TargArmor.Dock = System.Windows.Forms.DockStyle.Fill;
            this.LB_TargArmor.Location = new System.Drawing.Point(3, 46);
            this.LB_TargArmor.Name = "LB_TargArmor";
            this.LB_TargArmor.Size = new System.Drawing.Size(157, 26);
            this.LB_TargArmor.TabIndex = 68;
            this.LB_TargArmor.Text = "Targ Armor:";
            this.LB_TargArmor.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            // 
            // LB_Under35Perc
            // 
            this.LB_Under35Perc.BackColor = System.Drawing.Color.Transparent;
            this.LB_Under35Perc.Dock = System.Windows.Forms.DockStyle.Fill;
            this.LB_Under35Perc.Location = new System.Drawing.Point(3, 124);
            this.LB_Under35Perc.Name = "LB_Under35Perc";
            this.LB_Under35Perc.Size = new System.Drawing.Size(157, 26);
            this.LB_Under35Perc.TabIndex = 72;
            this.LB_Under35Perc.Text = "% of Time Spent HP 20-35%: *";
            this.LB_Under35Perc.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            this.LB_Under35Perc.ToolTipText = "";
            // 
            // LB_Notice
            // 
            this.LB_Notice.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left)
                        | System.Windows.Forms.AnchorStyles.Right)));
            this.LB_Notice.Location = new System.Drawing.Point(12, 15);
            this.LB_Notice.Name = "LB_Notice";
            this.LB_Notice.Size = new System.Drawing.Size(367, 30);
            this.LB_Notice.TabIndex = 108;
            this.LB_Notice.Text = "NOTICE: Many models do not yet support the Boss Handler so changes here will not " +
                "have any effect on your character in those models.";
            this.LB_Notice.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // GB_Presets
            // 
            this.GB_Presets.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left)
                        | System.Windows.Forms.AnchorStyles.Right)));
            this.GB_Presets.Controls.Add(this.CB_BL_Filter);
            this.GB_Presets.Controls.Add(this.CB_BossList);
            this.GB_Presets.Controls.Add(this.CB_BL_FilterType);
            this.GB_Presets.Controls.Add(this.LB_Is);
            this.GB_Presets.Controls.Add(this.LB_Where);
            this.GB_Presets.Location = new System.Drawing.Point(13, 49);
            this.GB_Presets.Name = "GB_Presets";
            this.GB_Presets.Size = new System.Drawing.Size(366, 72);
            this.GB_Presets.TabIndex = 109;
            this.GB_Presets.TabStop = false;
            this.GB_Presets.Text = "Select a Preset";
            // 
            // TLP_Stats
            // 
            this.TLP_Stats.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left)
                        | System.Windows.Forms.AnchorStyles.Right)));
            this.TLP_Stats.ColumnCount = 3;
            this.TLP_Stats.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 50F));
            this.TLP_Stats.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 50F));
            this.TLP_Stats.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Absolute, 40F));
            this.TLP_Stats.Controls.Add(this.LB_MultiTargsPerc, 2, 8);
            this.TLP_Stats.Controls.Add(this.LB_InBack, 2, 7);
            this.TLP_Stats.Controls.Add(this.PN_MultiTargs, 1, 8);
            this.TLP_Stats.Controls.Add(this.BT_Disarm, 1, 13);
            this.TLP_Stats.Controls.Add(this.CK_InBack, 0, 7);
            this.TLP_Stats.Controls.Add(this.BT_Root, 1, 12);
            this.TLP_Stats.Controls.Add(this.CK_MultiTargs, 0, 8);
            this.TLP_Stats.Controls.Add(this.BT_Fear, 1, 11);
            this.TLP_Stats.Controls.Add(this.CB_InBackPerc, 1, 7);
            this.TLP_Stats.Controls.Add(this.BT_Move, 1, 10);
            this.TLP_Stats.Controls.Add(this.LB_Under20Perc2, 2, 6);
            this.TLP_Stats.Controls.Add(this.CK_DisarmTargs, 0, 13);
            this.TLP_Stats.Controls.Add(this.NUD_Under20Perc, 1, 6);
            this.TLP_Stats.Controls.Add(this.CK_RootingTargs, 0, 12);
            this.TLP_Stats.Controls.Add(this.LB_TargLvl, 0, 1);
            this.TLP_Stats.Controls.Add(this.CB_TargLvl, 1, 1);
            this.TLP_Stats.Controls.Add(this.CB_TargArmor, 1, 2);
            this.TLP_Stats.Controls.Add(this.CK_FearingTargs, 0, 11);
            this.TLP_Stats.Controls.Add(this.LB_TargArmor, 0, 2);
            this.TLP_Stats.Controls.Add(this.LB_Duration, 0, 3);
            this.TLP_Stats.Controls.Add(this.CK_MovingTargs, 0, 10);
            this.TLP_Stats.Controls.Add(this.CB_Duration, 1, 3);
            this.TLP_Stats.Controls.Add(this.LB_Under35Perc, 0, 5);
            this.TLP_Stats.Controls.Add(this.NUD_Under35Perc, 1, 5);
            this.TLP_Stats.Controls.Add(this.LB_Under35Perc2, 2, 5);
            this.TLP_Stats.Controls.Add(this.LB_Under20Perc, 0, 6);
            this.TLP_Stats.Controls.Add(this.CK_StunningTargs, 0, 9);
            this.TLP_Stats.Controls.Add(this.LB_TargHP, 0, 4);
            this.TLP_Stats.Controls.Add(this.NUD_TargHP, 1, 4);
            this.TLP_Stats.Controls.Add(this.LB_Col1, 0, 0);
            this.TLP_Stats.Controls.Add(this.LB_Col2, 1, 0);
            this.TLP_Stats.Controls.Add(this.LB_Col3, 2, 0);
            this.TLP_Stats.Controls.Add(this.BT_Stun, 1, 9);
            this.TLP_Stats.Location = new System.Drawing.Point(13, 127);
            this.TLP_Stats.Name = "TLP_Stats";
            this.TLP_Stats.RowCount = 14;
            this.TLP_Stats.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 20F));
            this.TLP_Stats.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 26F));
            this.TLP_Stats.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 26F));
            this.TLP_Stats.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 26F));
            this.TLP_Stats.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 26F));
            this.TLP_Stats.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 26F));
            this.TLP_Stats.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 26F));
            this.TLP_Stats.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 26F));
            this.TLP_Stats.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 26F));
            this.TLP_Stats.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 26F));
            this.TLP_Stats.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 26F));
            this.TLP_Stats.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 26F));
            this.TLP_Stats.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 26F));
            this.TLP_Stats.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 26F));
            this.TLP_Stats.Size = new System.Drawing.Size(366, 358);
            this.TLP_Stats.TabIndex = 110;
            // 
            // LB_Under20Perc
            // 
            this.LB_Under20Perc.BackColor = System.Drawing.Color.Transparent;
            this.LB_Under20Perc.Dock = System.Windows.Forms.DockStyle.Fill;
            this.LB_Under20Perc.Location = new System.Drawing.Point(3, 150);
            this.LB_Under20Perc.Name = "LB_Under20Perc";
            this.LB_Under20Perc.Size = new System.Drawing.Size(157, 26);
            this.LB_Under20Perc.TabIndex = 102;
            this.LB_Under20Perc.Text = "% of Time Spent HP <20%: *";
            this.LB_Under20Perc.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            this.LB_Under20Perc.ToolTipText = "";
            // 
            // NUD_Under20Perc
            // 
            this.NUD_Under20Perc.Dock = System.Windows.Forms.DockStyle.Fill;
            this.NUD_Under20Perc.Location = new System.Drawing.Point(166, 153);
            this.NUD_Under20Perc.Maximum = new decimal(new int[] {
            25,
            0,
            0,
            0});
            this.NUD_Under20Perc.Minimum = new decimal(new int[] {
            10,
            0,
            0,
            0});
            this.NUD_Under20Perc.Name = "NUD_Under20Perc";
            this.NUD_Under20Perc.Size = new System.Drawing.Size(157, 20);
            this.NUD_Under20Perc.TabIndex = 111;
            this.NUD_Under20Perc.TextAlign = System.Windows.Forms.HorizontalAlignment.Right;
            this.NUD_Under20Perc.Value = new decimal(new int[] {
            17,
            0,
            0,
            0});
            // 
            // LB_Under20Perc2
            // 
            this.LB_Under20Perc2.BackColor = System.Drawing.Color.Transparent;
            this.LB_Under20Perc2.Dock = System.Windows.Forms.DockStyle.Fill;
            this.LB_Under20Perc2.Location = new System.Drawing.Point(329, 150);
            this.LB_Under20Perc2.Name = "LB_Under20Perc2";
            this.LB_Under20Perc2.Size = new System.Drawing.Size(34, 26);
            this.LB_Under20Perc2.TabIndex = 111;
            this.LB_Under20Perc2.Text = "%";
            this.LB_Under20Perc2.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            // 
            // PN_MultiTargs
            // 
            this.PN_MultiTargs.Controls.Add(this.LB_Max);
            this.PN_MultiTargs.Controls.Add(this.CB_MultiTargsMax);
            this.PN_MultiTargs.Controls.Add(this.CB_MultiTargsPerc);
            this.PN_MultiTargs.Dock = System.Windows.Forms.DockStyle.Fill;
            this.PN_MultiTargs.Location = new System.Drawing.Point(163, 202);
            this.PN_MultiTargs.Margin = new System.Windows.Forms.Padding(0);
            this.PN_MultiTargs.Name = "PN_MultiTargs";
            this.PN_MultiTargs.Size = new System.Drawing.Size(163, 26);
            this.PN_MultiTargs.TabIndex = 111;
            // 
            // LB_TargHP
            // 
            this.LB_TargHP.BackColor = System.Drawing.Color.Transparent;
            this.LB_TargHP.Dock = System.Windows.Forms.DockStyle.Fill;
            this.LB_TargHP.Location = new System.Drawing.Point(3, 98);
            this.LB_TargHP.Name = "LB_TargHP";
            this.LB_TargHP.Size = new System.Drawing.Size(157, 26);
            this.LB_TargHP.TabIndex = 111;
            this.LB_TargHP.Text = "Targ HP:";
            this.LB_TargHP.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            // 
            // LB_Col1
            // 
            this.LB_Col1.AutoSize = true;
            this.LB_Col1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.LB_Col1.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.LB_Col1.Location = new System.Drawing.Point(3, 0);
            this.LB_Col1.Name = "LB_Col1";
            this.LB_Col1.Size = new System.Drawing.Size(157, 20);
            this.LB_Col1.TabIndex = 112;
            this.LB_Col1.Text = "Field";
            this.LB_Col1.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // LB_Col2
            // 
            this.LB_Col2.AutoSize = true;
            this.LB_Col2.Dock = System.Windows.Forms.DockStyle.Fill;
            this.LB_Col2.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.LB_Col2.Location = new System.Drawing.Point(166, 0);
            this.LB_Col2.Name = "LB_Col2";
            this.LB_Col2.Size = new System.Drawing.Size(157, 20);
            this.LB_Col2.TabIndex = 113;
            this.LB_Col2.Text = "Value";
            this.LB_Col2.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // LB_Col3
            // 
            this.LB_Col3.AutoSize = true;
            this.LB_Col3.Dock = System.Windows.Forms.DockStyle.Fill;
            this.LB_Col3.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.LB_Col3.Location = new System.Drawing.Point(329, 0);
            this.LB_Col3.Name = "LB_Col3";
            this.LB_Col3.Size = new System.Drawing.Size(34, 20);
            this.LB_Col3.TabIndex = 114;
            this.LB_Col3.Text = "Unit";
            this.LB_Col3.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // CK_InBack
            // 
            this.CK_InBack.BackColor = System.Drawing.Color.Transparent;
            this.CK_InBack.Checked = true;
            this.CK_InBack.CheckState = System.Windows.Forms.CheckState.Checked;
            this.CK_InBack.Dock = System.Windows.Forms.DockStyle.Fill;
            this.CK_InBack.Location = new System.Drawing.Point(3, 179);
            this.CK_InBack.Name = "CK_InBack";
            this.CK_InBack.Size = new System.Drawing.Size(157, 20);
            this.CK_InBack.TabIndex = 75;
            this.CK_InBack.Text = "You stand behind boss *";
            this.CK_InBack.ToolTipText = "";
            this.CK_InBack.UseVisualStyleBackColor = false;
            // 
            // DG_BossHandler
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(677, 556);
            this.Controls.Add(this.TLP_Stats);
            this.Controls.Add(this.GB_Presets);
            this.Controls.Add(this.LB_Notice);
            this.Controls.Add(this.LB_Freq2);
            this.Controls.Add(this.LB_UnmitDmg);
            this.Controls.Add(this.NUD_AoEDMG);
            this.Controls.Add(this.NUD_AoEFreq);
            this.Controls.Add(this.CK_AoETargs);
            this.Controls.Add(this.TB_BossInfo);
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.Name = "DG_BossHandler";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterParent;
            this.Text = "Boss Handler";
            ((System.ComponentModel.ISupportInitialize)(this.NUD_TargHP)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.NUD_AoEDMG)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.NUD_AoEFreq)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.NUD_Under35Perc)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.CB_MultiTargsMax)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.CB_InBackPerc)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.CB_MultiTargsPerc)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.CB_Duration)).EndInit();
            this.GB_Presets.ResumeLayout(false);
            this.TLP_Stats.ResumeLayout(false);
            this.TLP_Stats.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.NUD_Under20Perc)).EndInit();
            this.PN_MultiTargs.ResumeLayout(false);
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Button BT_Disarm;
        private System.Windows.Forms.Button BT_Root;
        private System.Windows.Forms.Button BT_Fear;
        private System.Windows.Forms.Button BT_Move;
        private System.Windows.Forms.Button BT_Stun;
        private System.Windows.Forms.NumericUpDown NUD_TargHP;
        private System.Windows.Forms.Label LB_Under35Perc2;
        private System.Windows.Forms.Label LB_InBack;
        private System.Windows.Forms.Label LB_Freq2;
        private System.Windows.Forms.Label LB_UnmitDmg;
        private System.Windows.Forms.NumericUpDown NUD_AoEDMG;
        private System.Windows.Forms.NumericUpDown NUD_AoEFreq;
        private Rawr.CustomControls.ExtendedToolTipCheckBox CK_AoETargs;
        private System.Windows.Forms.Label LB_MultiTargsPerc;
        private System.Windows.Forms.NumericUpDown NUD_Under35Perc;
        private Rawr.CustomControls.ExtendedToolTipCheckBox CK_RootingTargs;
        private Rawr.CustomControls.ExtendedToolTipCheckBox CK_FearingTargs;
        private System.Windows.Forms.Label LB_Where;
        private System.Windows.Forms.Label LB_Is;
        private System.Windows.Forms.ComboBox CB_BL_FilterType;
        private System.Windows.Forms.ComboBox CB_BL_Filter;
        private System.Windows.Forms.TextBox TB_BossInfo;
        private System.Windows.Forms.ComboBox CB_BossList;
        private System.Windows.Forms.Label LB_Max;
        private System.Windows.Forms.NumericUpDown CB_MultiTargsMax;
        private Rawr.CustomControls.ExtendedToolTipCheckBox CK_MultiTargs;
        private System.Windows.Forms.NumericUpDown CB_InBackPerc;
        private Rawr.CustomControls.ExtendedToolTipCheckBox CK_MovingTargs;
        private System.Windows.Forms.NumericUpDown CB_MultiTargsPerc;
        private Rawr.CustomControls.ExtendedToolTipCheckBox CK_DisarmTargs;
        private Rawr.CustomControls.ExtendedToolTipCheckBox CK_StunningTargs;
        private System.Windows.Forms.Label LB_Duration;
        private System.Windows.Forms.NumericUpDown CB_Duration;
        public System.Windows.Forms.Label LB_TargLvl;
        public System.Windows.Forms.ComboBox CB_TargLvl;
        public System.Windows.Forms.ComboBox CB_TargArmor;
        public System.Windows.Forms.Label LB_TargArmor;
        public Rawr.CustomControls.ExtendedToolTipLabel LB_Under35Perc;
        private System.Windows.Forms.Label LB_Notice;
        private System.Windows.Forms.GroupBox GB_Presets;
        private System.Windows.Forms.TableLayoutPanel TLP_Stats;
        private System.Windows.Forms.Panel PN_MultiTargs;
        private System.Windows.Forms.Label LB_Under20Perc2;
        private System.Windows.Forms.NumericUpDown NUD_Under20Perc;
        public Rawr.CustomControls.ExtendedToolTipLabel LB_Under20Perc;
        public System.Windows.Forms.Label LB_TargHP;
        private System.Windows.Forms.Label LB_Col1;
        private System.Windows.Forms.Label LB_Col2;
        private System.Windows.Forms.Label LB_Col3;
        private Rawr.CustomControls.ExtendedToolTipCheckBox CK_InBack;
    }
}