namespace Rawr.RestoSham
{
    partial class CalculationOptionsPanelRestoSham
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
            this.tipRestoSham = new System.Windows.Forms.ToolTip(this.components);
            this.chkManaTide = new System.Windows.Forms.CheckBox();
            this.cboBurstStyle = new System.Windows.Forms.ComboBox();
            this.txtFightLength = new System.Windows.Forms.TextBox();
            this.cboSustStyle = new System.Windows.Forms.ComboBox();
            this.cboHeroism = new System.Windows.Forms.ComboBox();
            this.txtCleanse = new System.Windows.Forms.TextBox();
            this.label16 = new System.Windows.Forms.Label();
            this.txtInnervates = new System.Windows.Forms.TextBox();
            this.cboTargets = new System.Windows.Forms.ComboBox();
            this.txtLatency = new System.Windows.Forms.TextBox();
            this.label10 = new System.Windows.Forms.Label();
            this.tbActivity_Label = new System.Windows.Forms.Label();
            this.txtWSPops = new System.Windows.Forms.TextBox();
            this.label2 = new System.Windows.Forms.Label();
            this.label6 = new System.Windows.Forms.Label();
            this.errorRestoSham = new System.Windows.Forms.ErrorProvider(this.components);
            this.GeneralPage = new System.Windows.Forms.TabPage();
            this.groupBox5 = new System.Windows.Forms.GroupBox();
            this.label8 = new System.Windows.Forms.Label();
            this.label1 = new System.Windows.Forms.Label();
            this.groupBox4 = new System.Windows.Forms.GroupBox();
            this.tbSurvival_Label = new System.Windows.Forms.Label();
            this.tbSurvival = new System.Windows.Forms.TrackBar();
            this.groupBox3 = new System.Windows.Forms.GroupBox();
            this.label14 = new System.Windows.Forms.Label();
            this.groupBox2 = new System.Windows.Forms.GroupBox();
            this.label11 = new System.Windows.Forms.Label();
            this.chkWaterShield = new System.Windows.Forms.CheckBox();
            this.label5 = new System.Windows.Forms.Label();
            this.tbReplenishment = new System.Windows.Forms.TrackBar();
            this.tbReplenishment_Label = new System.Windows.Forms.Label();
            this.groupBox1 = new System.Windows.Forms.GroupBox();
            this.tbActivity = new System.Windows.Forms.TrackBar();
            this.label7 = new System.Windows.Forms.Label();
            this.chkEarthShield = new System.Windows.Forms.CheckBox();
            this.label3 = new System.Windows.Forms.Label();
            this.label4 = new System.Windows.Forms.Label();
            this.tabControl1 = new System.Windows.Forms.TabControl();
            ((System.ComponentModel.ISupportInitialize)(this.errorRestoSham)).BeginInit();
            this.GeneralPage.SuspendLayout();
            this.groupBox5.SuspendLayout();
            this.groupBox4.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.tbSurvival)).BeginInit();
            this.groupBox3.SuspendLayout();
            this.groupBox2.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.tbReplenishment)).BeginInit();
            this.groupBox1.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.tbActivity)).BeginInit();
            this.tabControl1.SuspendLayout();
            this.SuspendLayout();
            // 
            // chkManaTide
            // 
            this.chkManaTide.AutoSize = true;
            this.chkManaTide.Location = new System.Drawing.Point(125, 68);
            this.chkManaTide.Name = "chkManaTide";
            this.chkManaTide.Size = new System.Drawing.Size(155, 17);
            this.chkManaTide.TabIndex = 14;
            this.chkManaTide.Text = "Mana Tide every cooldown";
            this.tipRestoSham.SetToolTip(this.chkManaTide, "Check to indicate you place a Mana Tide totem every time the cooldown is up");
            this.chkManaTide.UseVisualStyleBackColor = true;
            this.chkManaTide.CheckedChanged += new System.EventHandler(this.chkManaTide_CheckedChanged);
            // 
            // cboBurstStyle
            // 
            this.cboBurstStyle.FormattingEnabled = true;
            this.cboBurstStyle.Items.AddRange(new object[] {
            "RT+HW",
            "RT+LHW",
            "RT+CH",
            "HW Spam",
            "LHW Spam",
            "CH Spam"});
            this.cboBurstStyle.Location = new System.Drawing.Point(81, 19);
            this.cboBurstStyle.Name = "cboBurstStyle";
            this.cboBurstStyle.Size = new System.Drawing.Size(83, 21);
            this.cboBurstStyle.TabIndex = 28;
            this.tipRestoSham.SetToolTip(this.cboBurstStyle, "Choose the style of burst healing you do.");
            this.cboBurstStyle.TextChanged += new System.EventHandler(this.cboBurstStyle_TextChanged);
            // 
            // txtFightLength
            // 
            this.txtFightLength.Location = new System.Drawing.Point(81, 13);
            this.txtFightLength.Name = "txtFightLength";
            this.txtFightLength.Size = new System.Drawing.Size(83, 20);
            this.txtFightLength.TabIndex = 25;
            this.tipRestoSham.SetToolTip(this.txtFightLength, "Total length of Fight in Minutes.");
            this.txtFightLength.Validated += new System.EventHandler(this.numericTextBox_Validated);
            this.txtFightLength.Validating += new System.ComponentModel.CancelEventHandler(this.numericTextBox_Validating);
            // 
            // cboSustStyle
            // 
            this.cboSustStyle.FormattingEnabled = true;
            this.cboSustStyle.Items.AddRange(new object[] {
            "RT+HW",
            "RT+LHW",
            "RT+CH",
            "HW Spam",
            "LHW Spam",
            "CH Spam"});
            this.cboSustStyle.Location = new System.Drawing.Point(81, 46);
            this.cboSustStyle.Name = "cboSustStyle";
            this.cboSustStyle.Size = new System.Drawing.Size(83, 21);
            this.cboSustStyle.TabIndex = 31;
            this.tipRestoSham.SetToolTip(this.cboSustStyle, "Choose the style of sustained healing you do.");
            this.cboSustStyle.TextChanged += new System.EventHandler(this.cboSustStyle_TextChanged);
            // 
            // cboHeroism
            // 
            this.cboHeroism.Enabled = false;
            this.cboHeroism.FormattingEnabled = true;
            this.cboHeroism.Items.AddRange(new object[] {
            "No",
            "Yes",
            "Me"});
            this.cboHeroism.Location = new System.Drawing.Point(75, 19);
            this.cboHeroism.Name = "cboHeroism";
            this.cboHeroism.Size = new System.Drawing.Size(83, 21);
            this.cboHeroism.TabIndex = 47;
            this.tipRestoSham.SetToolTip(this.cboHeroism, "Note, if you cast heroism, select \"me\".");
            this.cboHeroism.TextChanged += new System.EventHandler(this.cboHeroism_TextChanged);
            // 
            // txtCleanse
            // 
            this.txtCleanse.Location = new System.Drawing.Point(75, 46);
            this.txtCleanse.Name = "txtCleanse";
            this.txtCleanse.Size = new System.Drawing.Size(83, 20);
            this.txtCleanse.TabIndex = 49;
            this.tipRestoSham.SetToolTip(this.txtCleanse, "The number of times per fight you use Cleanse Spirit.");
            this.txtCleanse.Validated += new System.EventHandler(this.numericTextBox_Validated);
            this.txtCleanse.Validating += new System.ComponentModel.CancelEventHandler(this.numericTextBox_Validating);
            // 
            // label16
            // 
            this.label16.AutoSize = true;
            this.label16.Location = new System.Drawing.Point(14, 49);
            this.label16.Name = "label16";
            this.label16.Size = new System.Drawing.Size(55, 13);
            this.label16.TabIndex = 48;
            this.label16.Text = "Decurses:";
            this.tipRestoSham.SetToolTip(this.label16, "The number of times per fight you use Cleanse Spirit.");
            // 
            // txtInnervates
            // 
            this.txtInnervates.Location = new System.Drawing.Point(75, 16);
            this.txtInnervates.Name = "txtInnervates";
            this.txtInnervates.Size = new System.Drawing.Size(83, 20);
            this.txtInnervates.TabIndex = 52;
            this.tipRestoSham.SetToolTip(this.txtInnervates, "Count of Innervates you get.");
            this.txtInnervates.Validated += new System.EventHandler(this.numericTextBox_Validated);
            this.txtInnervates.Validating += new System.ComponentModel.CancelEventHandler(this.numericTextBox_Validating);
            // 
            // cboTargets
            // 
            this.cboTargets.FormattingEnabled = true;
            this.cboTargets.Items.AddRange(new object[] {
            "Raid",
            "Tank",
            "Self",
            "Heavy Raid"});
            this.cboTargets.Location = new System.Drawing.Point(81, 73);
            this.cboTargets.Name = "cboTargets";
            this.cboTargets.Size = new System.Drawing.Size(83, 21);
            this.cboTargets.TabIndex = 51;
            this.tipRestoSham.SetToolTip(this.cboTargets, "Choose main healing targets.");
            this.cboTargets.TextChanged += new System.EventHandler(this.cboDamageReceivers_TextChanged);
            // 
            // txtLatency
            // 
            this.txtLatency.Location = new System.Drawing.Point(81, 39);
            this.txtLatency.Name = "txtLatency";
            this.txtLatency.Size = new System.Drawing.Size(83, 20);
            this.txtLatency.TabIndex = 49;
            this.tipRestoSham.SetToolTip(this.txtLatency, "Your normal latency.");
            this.txtLatency.Validated += new System.EventHandler(this.numericTextBox_Validated);
            this.txtLatency.Validating += new System.ComponentModel.CancelEventHandler(this.numericTextBox_Validating);
            // 
            // label10
            // 
            this.label10.AutoSize = true;
            this.label10.Location = new System.Drawing.Point(38, 42);
            this.label10.Name = "label10";
            this.label10.Size = new System.Drawing.Size(31, 13);
            this.label10.TabIndex = 48;
            this.label10.Text = "Ping:";
            this.tipRestoSham.SetToolTip(this.label10, "Ping in milliseconds.");
            // 
            // tbActivity_Label
            // 
            this.tbActivity_Label.AutoSize = true;
            this.tbActivity_Label.Location = new System.Drawing.Point(6, 96);
            this.tbActivity_Label.Name = "tbActivity_Label";
            this.tbActivity_Label.Size = new System.Drawing.Size(119, 13);
            this.tbActivity_Label.TabIndex = 54;
            this.tbActivity_Label.Text = "Time Spent Casting (%):";
            this.tipRestoSham.SetToolTip(this.tbActivity_Label, "Do not include canceled spells, 85% default includes canceled spells.");
            // 
            // txtWSPops
            // 
            this.txtWSPops.Location = new System.Drawing.Point(75, 42);
            this.txtWSPops.Name = "txtWSPops";
            this.txtWSPops.Size = new System.Drawing.Size(83, 20);
            this.txtWSPops.TabIndex = 54;
            this.tipRestoSham.SetToolTip(this.txtWSPops, "Number of times per minute Water Shield is popped");
            this.txtWSPops.Validated += new System.EventHandler(this.numericTextBox_Validated);
            this.txtWSPops.Validating += new System.ComponentModel.CancelEventHandler(this.numericTextBox_Validating);
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Location = new System.Drawing.Point(170, 16);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(43, 13);
            this.label2.TabIndex = 26;
            this.label2.Text = "minutes";
            // 
            // label6
            // 
            this.label6.AutoSize = true;
            this.label6.Location = new System.Drawing.Point(164, 49);
            this.label6.Name = "label6";
            this.label6.Size = new System.Drawing.Size(31, 13);
            this.label6.TabIndex = 54;
            this.label6.Text = "times";
            // 
            // errorRestoSham
            // 
            this.errorRestoSham.ContainerControl = this;
            // 
            // GeneralPage
            // 
            this.GeneralPage.AutoScroll = true;
            this.GeneralPage.Controls.Add(this.groupBox5);
            this.GeneralPage.Controls.Add(this.groupBox4);
            this.GeneralPage.Controls.Add(this.groupBox3);
            this.GeneralPage.Controls.Add(this.groupBox2);
            this.GeneralPage.Controls.Add(this.groupBox1);
            this.GeneralPage.Location = new System.Drawing.Point(4, 22);
            this.GeneralPage.Name = "GeneralPage";
            this.GeneralPage.Padding = new System.Windows.Forms.Padding(3);
            this.GeneralPage.Size = new System.Drawing.Size(292, 579);
            this.GeneralPage.TabIndex = 0;
            this.GeneralPage.Text = "General";
            this.GeneralPage.UseVisualStyleBackColor = true;
            // 
            // groupBox5
            // 
            this.groupBox5.Controls.Add(this.label8);
            this.groupBox5.Controls.Add(this.txtLatency);
            this.groupBox5.Controls.Add(this.label10);
            this.groupBox5.Controls.Add(this.label1);
            this.groupBox5.Controls.Add(this.txtFightLength);
            this.groupBox5.Controls.Add(this.label2);
            this.groupBox5.Location = new System.Drawing.Point(3, 3);
            this.groupBox5.Name = "groupBox5";
            this.groupBox5.Size = new System.Drawing.Size(286, 69);
            this.groupBox5.TabIndex = 55;
            this.groupBox5.TabStop = false;
            this.groupBox5.Text = "Settings";
            // 
            // label8
            // 
            this.label8.AutoSize = true;
            this.label8.Location = new System.Drawing.Point(170, 42);
            this.label8.Name = "label8";
            this.label8.Size = new System.Drawing.Size(20, 13);
            this.label8.TabIndex = 54;
            this.label8.Text = "ms";
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(6, 16);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(69, 13);
            this.label1.TabIndex = 0;
            this.label1.Text = "Fight Length:";
            // 
            // groupBox4
            // 
            this.groupBox4.Controls.Add(this.tbSurvival_Label);
            this.groupBox4.Controls.Add(this.tbSurvival);
            this.groupBox4.Location = new System.Drawing.Point(3, 491);
            this.groupBox4.Name = "groupBox4";
            this.groupBox4.Size = new System.Drawing.Size(286, 85);
            this.groupBox4.TabIndex = 55;
            this.groupBox4.TabStop = false;
            this.groupBox4.Text = "Miscellaneous";
            // 
            // tbSurvival_Label
            // 
            this.tbSurvival_Label.AutoSize = true;
            this.tbSurvival_Label.Location = new System.Drawing.Point(6, 16);
            this.tbSurvival_Label.Name = "tbSurvival_Label";
            this.tbSurvival_Label.Size = new System.Drawing.Size(102, 13);
            this.tbSurvival_Label.TabIndex = 5;
            this.tbSurvival_Label.Text = "Survival Weight (%):";
            // 
            // tbSurvival
            // 
            this.tbSurvival.Location = new System.Drawing.Point(6, 32);
            this.tbSurvival.Maximum = 100;
            this.tbSurvival.Name = "tbSurvival";
            this.tbSurvival.Size = new System.Drawing.Size(274, 45);
            this.tbSurvival.TabIndex = 4;
            this.tbSurvival.Scroll += new System.EventHandler(this.OnTrackBarScroll);
            // 
            // groupBox3
            // 
            this.groupBox3.Controls.Add(this.label6);
            this.groupBox3.Controls.Add(this.cboHeroism);
            this.groupBox3.Controls.Add(this.label14);
            this.groupBox3.Controls.Add(this.txtCleanse);
            this.groupBox3.Controls.Add(this.label16);
            this.groupBox3.Location = new System.Drawing.Point(3, 410);
            this.groupBox3.Name = "groupBox3";
            this.groupBox3.Size = new System.Drawing.Size(286, 75);
            this.groupBox3.TabIndex = 53;
            this.groupBox3.TabStop = false;
            this.groupBox3.Text = "Utility";
            // 
            // label14
            // 
            this.label14.AutoSize = true;
            this.label14.Location = new System.Drawing.Point(21, 22);
            this.label14.Name = "label14";
            this.label14.Size = new System.Drawing.Size(48, 13);
            this.label14.TabIndex = 46;
            this.label14.Text = "Heroism:";
            // 
            // groupBox2
            // 
            this.groupBox2.Controls.Add(this.txtWSPops);
            this.groupBox2.Controls.Add(this.label11);
            this.groupBox2.Controls.Add(this.chkWaterShield);
            this.groupBox2.Controls.Add(this.label5);
            this.groupBox2.Controls.Add(this.tbReplenishment);
            this.groupBox2.Controls.Add(this.tbReplenishment_Label);
            this.groupBox2.Controls.Add(this.txtInnervates);
            this.groupBox2.Controls.Add(this.chkManaTide);
            this.groupBox2.Location = new System.Drawing.Point(3, 247);
            this.groupBox2.Name = "groupBox2";
            this.groupBox2.Size = new System.Drawing.Size(286, 157);
            this.groupBox2.TabIndex = 28;
            this.groupBox2.TabStop = false;
            this.groupBox2.Text = "Mana";
            // 
            // label11
            // 
            this.label11.AutoSize = true;
            this.label11.Location = new System.Drawing.Point(9, 45);
            this.label11.Name = "label11";
            this.label11.Size = new System.Drawing.Size(55, 13);
            this.label11.TabIndex = 53;
            this.label11.Text = "WS Pops:";
            // 
            // chkWaterShield
            // 
            this.chkWaterShield.AutoSize = true;
            this.chkWaterShield.Location = new System.Drawing.Point(12, 68);
            this.chkWaterShield.Name = "chkWaterShield";
            this.chkWaterShield.Size = new System.Drawing.Size(87, 17);
            this.chkWaterShield.TabIndex = 15;
            this.chkWaterShield.Text = "Water Shield";
            this.chkWaterShield.UseVisualStyleBackColor = true;
            this.chkWaterShield.CheckedChanged += new System.EventHandler(this.chkWaterShield_CheckedChanged);
            // 
            // label5
            // 
            this.label5.AutoSize = true;
            this.label5.Location = new System.Drawing.Point(9, 19);
            this.label5.Name = "label5";
            this.label5.Size = new System.Drawing.Size(60, 13);
            this.label5.TabIndex = 51;
            this.label5.Text = "Innervates:";
            // 
            // tbReplenishment
            // 
            this.tbReplenishment.Location = new System.Drawing.Point(6, 104);
            this.tbReplenishment.Maximum = 100;
            this.tbReplenishment.Name = "tbReplenishment";
            this.tbReplenishment.Size = new System.Drawing.Size(274, 45);
            this.tbReplenishment.TabIndex = 0;
            this.tbReplenishment.Scroll += new System.EventHandler(this.OnTrackBarScroll);
            // 
            // tbReplenishment_Label
            // 
            this.tbReplenishment_Label.AutoSize = true;
            this.tbReplenishment_Label.Location = new System.Drawing.Point(6, 88);
            this.tbReplenishment_Label.Name = "tbReplenishment_Label";
            this.tbReplenishment_Label.Size = new System.Drawing.Size(133, 13);
            this.tbReplenishment_Label.TabIndex = 1;
            this.tbReplenishment_Label.Text = "Replenishment Uptime (%):";
            // 
            // groupBox1
            // 
            this.groupBox1.Controls.Add(this.tbActivity);
            this.groupBox1.Controls.Add(this.tbActivity_Label);
            this.groupBox1.Controls.Add(this.label7);
            this.groupBox1.Controls.Add(this.cboTargets);
            this.groupBox1.Controls.Add(this.chkEarthShield);
            this.groupBox1.Controls.Add(this.label3);
            this.groupBox1.Controls.Add(this.cboSustStyle);
            this.groupBox1.Controls.Add(this.cboBurstStyle);
            this.groupBox1.Controls.Add(this.label4);
            this.groupBox1.Location = new System.Drawing.Point(3, 78);
            this.groupBox1.Name = "groupBox1";
            this.groupBox1.Size = new System.Drawing.Size(286, 163);
            this.groupBox1.TabIndex = 27;
            this.groupBox1.TabStop = false;
            this.groupBox1.Text = "Healing";
            // 
            // tbActivity
            // 
            this.tbActivity.Location = new System.Drawing.Point(6, 112);
            this.tbActivity.Maximum = 100;
            this.tbActivity.Name = "tbActivity";
            this.tbActivity.Size = new System.Drawing.Size(274, 45);
            this.tbActivity.TabIndex = 53;
            this.tbActivity.Scroll += new System.EventHandler(this.OnTrackBarScroll);
            // 
            // label7
            // 
            this.label7.AutoSize = true;
            this.label7.Location = new System.Drawing.Point(27, 76);
            this.label7.Name = "label7";
            this.label7.Size = new System.Drawing.Size(46, 13);
            this.label7.TabIndex = 52;
            this.label7.Text = "Targets:";
            // 
            // chkEarthShield
            // 
            this.chkEarthShield.AutoSize = true;
            this.chkEarthShield.Location = new System.Drawing.Point(173, 48);
            this.chkEarthShield.Name = "chkEarthShield";
            this.chkEarthShield.Size = new System.Drawing.Size(83, 17);
            this.chkEarthShield.TabIndex = 50;
            this.chkEarthShield.Text = "Earth Shield";
            this.chkEarthShield.UseVisualStyleBackColor = true;
            this.chkEarthShield.CheckedChanged += new System.EventHandler(this.chkEarthShield_CheckedChanged);
            // 
            // label3
            // 
            this.label3.AutoSize = true;
            this.label3.Location = new System.Drawing.Point(15, 22);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(60, 13);
            this.label3.TabIndex = 29;
            this.label3.Text = "Burst Style:";
            // 
            // label4
            // 
            this.label4.AutoSize = true;
            this.label4.Location = new System.Drawing.Point(15, 49);
            this.label4.Name = "label4";
            this.label4.Size = new System.Drawing.Size(60, 13);
            this.label4.TabIndex = 30;
            this.label4.Text = "Sust. Style:";
            // 
            // tabControl1
            // 
            this.tabControl1.Controls.Add(this.GeneralPage);
            this.tabControl1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.tabControl1.Location = new System.Drawing.Point(0, 0);
            this.tabControl1.Name = "tabControl1";
            this.tabControl1.SelectedIndex = 0;
            this.tabControl1.Size = new System.Drawing.Size(300, 605);
            this.tabControl1.TabIndex = 14;
            this.tabControl1.Tag = "";
            // 
            // CalculationOptionsPanelRestoSham
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.Controls.Add(this.tabControl1);
            this.Name = "CalculationOptionsPanelRestoSham";
            this.Size = new System.Drawing.Size(300, 605);
            ((System.ComponentModel.ISupportInitialize)(this.errorRestoSham)).EndInit();
            this.GeneralPage.ResumeLayout(false);
            this.groupBox5.ResumeLayout(false);
            this.groupBox5.PerformLayout();
            this.groupBox4.ResumeLayout(false);
            this.groupBox4.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.tbSurvival)).EndInit();
            this.groupBox3.ResumeLayout(false);
            this.groupBox3.PerformLayout();
            this.groupBox2.ResumeLayout(false);
            this.groupBox2.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.tbReplenishment)).EndInit();
            this.groupBox1.ResumeLayout(false);
            this.groupBox1.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.tbActivity)).EndInit();
            this.tabControl1.ResumeLayout(false);
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.ToolTip tipRestoSham;
        private System.Windows.Forms.ErrorProvider errorRestoSham;
        private System.Windows.Forms.TabControl tabControl1;
        private System.Windows.Forms.TabPage GeneralPage;
        private System.Windows.Forms.Label label3;
        private System.Windows.Forms.ComboBox cboBurstStyle;
        private System.Windows.Forms.GroupBox groupBox1;
        private System.Windows.Forms.Label tbSurvival_Label;
        private System.Windows.Forms.TrackBar tbSurvival;
        private System.Windows.Forms.Label tbReplenishment_Label;
        private System.Windows.Forms.TrackBar tbReplenishment;
        private System.Windows.Forms.CheckBox chkWaterShield;
        private System.Windows.Forms.CheckBox chkManaTide;
        private System.Windows.Forms.TextBox txtFightLength;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.ComboBox cboSustStyle;
        private System.Windows.Forms.Label label4;
        private System.Windows.Forms.ComboBox cboHeroism;
        private System.Windows.Forms.Label label14;
        private System.Windows.Forms.TextBox txtCleanse;
        private System.Windows.Forms.Label label16;
        private System.Windows.Forms.CheckBox chkEarthShield;
        private System.Windows.Forms.TextBox txtInnervates;
        private System.Windows.Forms.Label label5;
        private System.Windows.Forms.GroupBox groupBox2;
        private System.Windows.Forms.GroupBox groupBox3;
        private System.Windows.Forms.Label label6;
        private System.Windows.Forms.GroupBox groupBox4;
        private System.Windows.Forms.Label label7;
        private System.Windows.Forms.ComboBox cboTargets;
        private System.Windows.Forms.GroupBox groupBox5;
        private System.Windows.Forms.Label label8;
        private System.Windows.Forms.TextBox txtLatency;
        private System.Windows.Forms.Label label10;
        private System.Windows.Forms.TrackBar tbActivity;
        private System.Windows.Forms.Label tbActivity_Label;
        private System.Windows.Forms.Label label11;
        private System.Windows.Forms.TextBox txtWSPops;

    }
}
