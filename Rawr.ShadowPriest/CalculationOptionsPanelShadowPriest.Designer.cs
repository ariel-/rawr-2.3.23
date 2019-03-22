namespace Rawr.ShadowPriest
{
    partial class CalculationOptionsPanelShadowPriest
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(CalculationOptionsPanelShadowPriest));
            this.lblFightLength = new System.Windows.Forms.Label();
            this.label4 = new System.Windows.Forms.Label();
            this.cmbManaAmt = new System.Windows.Forms.ComboBox();
            this.gbSpellPriority = new System.Windows.Forms.GroupBox();
            this.bChangePriority = new System.Windows.Forms.Button();
            this.lsSpellPriopity = new System.Windows.Forms.ListBox();
            this.cbTargetLevel = new System.Windows.Forms.ComboBox();
            this.label15 = new System.Windows.Forms.Label();
            this.toolTip1 = new System.Windows.Forms.ToolTip(this.components);
            this.trkFightLength = new System.Windows.Forms.TrackBar();
            this.trkShadowfiend = new System.Windows.Forms.TrackBar();
            this.trkReplenishment = new System.Windows.Forms.TrackBar();
            this.trkJoW = new System.Windows.Forms.TrackBar();
            this.trkSurvivability = new System.Windows.Forms.TrackBar();
            this.trkFSR = new System.Windows.Forms.TrackBar();
            this.trkDelay = new System.Windows.Forms.TrackBar();
            this.trkMoveFrequency = new System.Windows.Forms.TrackBar();
            this.trkMoveDuration = new System.Windows.Forms.TrackBar();
            this.lblShadowfiend = new System.Windows.Forms.Label();
            this.lblReplenishment = new System.Windows.Forms.Label();
            this.lblJoW = new System.Windows.Forms.Label();
            this.lblSurvivability = new System.Windows.Forms.Label();
            this.lblFSR = new System.Windows.Forms.Label();
            this.lblDelay = new System.Windows.Forms.Label();
            this.lblMoveFrequency = new System.Windows.Forms.Label();
            this.lblMoveDuration = new System.Windows.Forms.Label();
            this.cbPtr = new System.Windows.Forms.CheckBox();
            this.gbSpellPriority.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.trkFightLength)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.trkShadowfiend)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.trkReplenishment)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.trkJoW)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.trkSurvivability)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.trkFSR)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.trkDelay)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.trkMoveFrequency)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.trkMoveDuration)).BeginInit();
            this.SuspendLayout();
            // 
            // lblFightLength
            // 
            this.lblFightLength.AutoSize = true;
            this.lblFightLength.Location = new System.Drawing.Point(9, 112);
            this.lblFightLength.Name = "lblFightLength";
            this.lblFightLength.Size = new System.Drawing.Size(66, 13);
            this.lblFightLength.TabIndex = 21;
            this.lblFightLength.Text = "Fight Length";
            // 
            // label4
            // 
            this.label4.AutoSize = true;
            this.label4.Location = new System.Drawing.Point(9, 41);
            this.label4.Name = "label4";
            this.label4.Size = new System.Drawing.Size(75, 13);
            this.label4.TabIndex = 2;
            this.label4.Text = "Mana Potions:";
            // 
            // cmbManaAmt
            // 
            this.cmbManaAmt.DisplayMember = "2400";
            this.cmbManaAmt.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.cmbManaAmt.Items.AddRange(new object[] {
            "(None) 0",
            "(Major) 1350-2250, Avg 1800",
            "(Mad) 1650-2750, Avg 2200",
            "(Super) 1800-3000, Avg 2400",
            "(Runic) 4200-4400, Avg 4300"});
            this.cmbManaAmt.Location = new System.Drawing.Point(90, 41);
            this.cmbManaAmt.Name = "cmbManaAmt";
            this.cmbManaAmt.Size = new System.Drawing.Size(192, 21);
            this.cmbManaAmt.TabIndex = 0;
            this.cmbManaAmt.ValueMember = "2400";
            this.cmbManaAmt.SelectedIndexChanged += new System.EventHandler(this.cmbManaAmt_SelectedIndexChanged);
            // 
            // gbSpellPriority
            // 
            this.gbSpellPriority.Controls.Add(this.bChangePriority);
            this.gbSpellPriority.Controls.Add(this.lsSpellPriopity);
            this.gbSpellPriority.Location = new System.Drawing.Point(12, 503);
            this.gbSpellPriority.Name = "gbSpellPriority";
            this.gbSpellPriority.Size = new System.Drawing.Size(270, 186);
            this.gbSpellPriority.TabIndex = 29;
            this.gbSpellPriority.TabStop = false;
            this.gbSpellPriority.Text = "Spell Priority";
            // 
            // bChangePriority
            // 
            this.bChangePriority.Location = new System.Drawing.Point(5, 151);
            this.bChangePriority.Name = "bChangePriority";
            this.bChangePriority.Size = new System.Drawing.Size(75, 23);
            this.bChangePriority.TabIndex = 11;
            this.bChangePriority.Text = "Change";
            this.bChangePriority.UseVisualStyleBackColor = true;
            this.bChangePriority.Click += new System.EventHandler(this.bChangePriority_Click);
            // 
            // lsSpellPriopity
            // 
            this.lsSpellPriopity.FormattingEnabled = true;
            this.lsSpellPriopity.Location = new System.Drawing.Point(5, 20);
            this.lsSpellPriopity.Name = "lsSpellPriopity";
            this.lsSpellPriopity.Size = new System.Drawing.Size(259, 121);
            this.lsSpellPriopity.TabIndex = 10;
            // 
            // cbTargetLevel
            // 
            this.cbTargetLevel.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.cbTargetLevel.FormattingEnabled = true;
            this.cbTargetLevel.Items.AddRange(new object[] {
            "+0",
            "+1",
            "+2 (PvP)",
            "+3 (Boss)"});
            this.cbTargetLevel.Location = new System.Drawing.Point(131, 14);
            this.cbTargetLevel.Name = "cbTargetLevel";
            this.cbTargetLevel.Size = new System.Drawing.Size(151, 21);
            this.cbTargetLevel.TabIndex = 42;
            this.cbTargetLevel.SelectedIndexChanged += new System.EventHandler(this.cbTargetLevel_SelectedIndexChanged);
            // 
            // label15
            // 
            this.label15.AutoSize = true;
            this.label15.Location = new System.Drawing.Point(9, 14);
            this.label15.Name = "label15";
            this.label15.Size = new System.Drawing.Size(99, 13);
            this.label15.TabIndex = 43;
            this.label15.Text = "Relative Mob Level";
            // 
            // trkFightLength
            // 
            this.trkFightLength.Location = new System.Drawing.Point(12, 128);
            this.trkFightLength.Maximum = 20;
            this.trkFightLength.Minimum = 1;
            this.trkFightLength.Name = "trkFightLength";
            this.trkFightLength.Size = new System.Drawing.Size(270, 42);
            this.trkFightLength.TabIndex = 44;
            this.toolTip1.SetToolTip(this.trkFightLength, "Estimated duration of the fight. Important for sustainability calculations.");
            this.trkFightLength.Value = 1;
            this.trkFightLength.Scroll += new System.EventHandler(this.trkFightLength_Scroll);
            // 
            // trkShadowfiend
            // 
            this.trkShadowfiend.Location = new System.Drawing.Point(12, 226);
            this.trkShadowfiend.Maximum = 150;
            this.trkShadowfiend.Name = "trkShadowfiend";
            this.trkShadowfiend.Size = new System.Drawing.Size(270, 42);
            this.trkShadowfiend.TabIndex = 46;
            this.trkShadowfiend.TickFrequency = 5;
            this.toolTip1.SetToolTip(this.trkShadowfiend, resources.GetString("trkShadowfiend.ToolTip"));
            this.trkShadowfiend.Scroll += new System.EventHandler(this.trkShadowfiend_Scroll);
            // 
            // trkReplenishment
            // 
            this.trkReplenishment.Location = new System.Drawing.Point(12, 275);
            this.trkReplenishment.Maximum = 100;
            this.trkReplenishment.Name = "trkReplenishment";
            this.trkReplenishment.Size = new System.Drawing.Size(270, 42);
            this.trkReplenishment.TabIndex = 48;
            this.trkReplenishment.TickFrequency = 5;
            this.toolTip1.SetToolTip(this.trkReplenishment, "How much uptime do you expect on Replenishment?");
            this.trkReplenishment.Scroll += new System.EventHandler(this.trkReplenishment_Scroll);
            // 
            // trkJoW
            // 
            this.trkJoW.Location = new System.Drawing.Point(12, 320);
            this.trkJoW.Maximum = 100;
            this.trkJoW.Name = "trkJoW";
            this.trkJoW.Size = new System.Drawing.Size(270, 42);
            this.trkJoW.TabIndex = 50;
            this.trkJoW.TickFrequency = 5;
            this.toolTip1.SetToolTip(this.trkJoW, "Uptime of Judgment of Wisdom on Target.");
            this.trkJoW.Scroll += new System.EventHandler(this.trkJoW_Scroll);
            // 
            // trkSurvivability
            // 
            this.trkSurvivability.Location = new System.Drawing.Point(12, 368);
            this.trkSurvivability.Maximum = 100;
            this.trkSurvivability.Name = "trkSurvivability";
            this.trkSurvivability.Size = new System.Drawing.Size(270, 42);
            this.trkSurvivability.TabIndex = 51;
            this.trkSurvivability.TickFrequency = 5;
            this.toolTip1.SetToolTip(this.trkSurvivability, "Tell Rawr how much you value your life. Use 0-5% for PvE, 10-15% for PvP");
            this.trkSurvivability.Scroll += new System.EventHandler(this.trackBar1_Scroll);
            // 
            // trkFSR
            // 
            this.trkFSR.Location = new System.Drawing.Point(12, 80);
            this.trkFSR.Maximum = 100;
            this.trkFSR.Name = "trkFSR";
            this.trkFSR.Size = new System.Drawing.Size(270, 42);
            this.trkFSR.TabIndex = 54;
            this.trkFSR.TickFrequency = 5;
            this.toolTip1.SetToolTip(this.trkFSR, resources.GetString("trkFSR.ToolTip"));
            this.trkFSR.Scroll += new System.EventHandler(this.trkFSR_Scroll);
            // 
            // trkDelay
            // 
            this.trkDelay.Location = new System.Drawing.Point(12, 177);
            this.trkDelay.Maximum = 1000;
            this.trkDelay.Name = "trkDelay";
            this.trkDelay.Size = new System.Drawing.Size(270, 42);
            this.trkDelay.TabIndex = 55;
            this.trkDelay.TickFrequency = 50;
            this.toolTip1.SetToolTip(this.trkDelay, "Change this value to adjust how much lag from latency, finger twitching and gener" +
                    "al brain farts you expect to have.");
            this.trkDelay.Scroll += new System.EventHandler(this.trkDelay_Scroll);
            // 
            // trkMoveFrequency
            // 
            this.trkMoveFrequency.Location = new System.Drawing.Point(12, 413);
            this.trkMoveFrequency.Maximum = 300;
            this.trkMoveFrequency.Minimum = 5;
            this.trkMoveFrequency.Name = "trkMoveFrequency";
            this.trkMoveFrequency.Size = new System.Drawing.Size(270, 42);
            this.trkMoveFrequency.SmallChange = 5;
            this.trkMoveFrequency.TabIndex = 57;
            this.trkMoveFrequency.TickFrequency = 5;
            this.toolTip1.SetToolTip(this.trkMoveFrequency, "In average, how often you need to move during a fight in Seconds.");
            this.trkMoveFrequency.Value = 60;
            this.trkMoveFrequency.Scroll += new System.EventHandler(this.trkMoveFrequency_Scroll);
            // 
            // trkMoveDuration
            // 
            this.trkMoveDuration.Location = new System.Drawing.Point(12, 458);
            this.trkMoveDuration.Maximum = 60;
            this.trkMoveDuration.Name = "trkMoveDuration";
            this.trkMoveDuration.RightToLeft = System.Windows.Forms.RightToLeft.No;
            this.trkMoveDuration.Size = new System.Drawing.Size(270, 42);
            this.trkMoveDuration.TabIndex = 60;
            this.toolTip1.SetToolTip(this.trkMoveDuration, "In average, how long you move each time you need to move during a fight in second" +
                    "s.");
            this.trkMoveDuration.Value = 5;
            this.trkMoveDuration.Scroll += new System.EventHandler(this.trkMoveDuration_Scroll);
            // 
            // lblShadowfiend
            // 
            this.lblShadowfiend.AutoSize = true;
            this.lblShadowfiend.Location = new System.Drawing.Point(9, 206);
            this.lblShadowfiend.Name = "lblShadowfiend";
            this.lblShadowfiend.Size = new System.Drawing.Size(80, 13);
            this.lblShadowfiend.TabIndex = 45;
            this.lblShadowfiend.Text = "% Shadowfiend";
            // 
            // lblReplenishment
            // 
            this.lblReplenishment.AutoSize = true;
            this.lblReplenishment.Location = new System.Drawing.Point(9, 255);
            this.lblReplenishment.Name = "lblReplenishment";
            this.lblReplenishment.Size = new System.Drawing.Size(88, 13);
            this.lblReplenishment.TabIndex = 47;
            this.lblReplenishment.Text = "% Replenishment";
            // 
            // lblJoW
            // 
            this.lblJoW.AutoSize = true;
            this.lblJoW.Location = new System.Drawing.Point(9, 304);
            this.lblJoW.Name = "lblJoW";
            this.lblJoW.Size = new System.Drawing.Size(123, 13);
            this.lblJoW.TabIndex = 49;
            this.lblJoW.Text = "% Judgement of Wisdom";
            // 
            // lblSurvivability
            // 
            this.lblSurvivability.AutoSize = true;
            this.lblSurvivability.Location = new System.Drawing.Point(9, 352);
            this.lblSurvivability.Name = "lblSurvivability";
            this.lblSurvivability.Size = new System.Drawing.Size(121, 13);
            this.lblSurvivability.TabIndex = 52;
            this.lblSurvivability.Text = "% Focus on Survivability";
            // 
            // lblFSR
            // 
            this.lblFSR.AutoSize = true;
            this.lblFSR.Location = new System.Drawing.Point(9, 64);
            this.lblFSR.Name = "lblFSR";
            this.lblFSR.Size = new System.Drawing.Size(101, 13);
            this.lblFSR.TabIndex = 53;
            this.lblFSR.Text = "% time spent in FSR";
            // 
            // lblDelay
            // 
            this.lblDelay.AutoSize = true;
            this.lblDelay.Location = new System.Drawing.Point(9, 157);
            this.lblDelay.Name = "lblDelay";
            this.lblDelay.Size = new System.Drawing.Size(34, 13);
            this.lblDelay.TabIndex = 56;
            this.lblDelay.Text = "Delay";
            // 
            // lblMoveFrequency
            // 
            this.lblMoveFrequency.AutoSize = true;
            this.lblMoveFrequency.Location = new System.Drawing.Point(9, 397);
            this.lblMoveFrequency.Name = "lblMoveFrequency";
            this.lblMoveFrequency.Size = new System.Drawing.Size(110, 13);
            this.lblMoveFrequency.TabIndex = 58;
            this.lblMoveFrequency.Text = "Movement Frequency";
            // 
            // lblMoveDuration
            // 
            this.lblMoveDuration.AutoSize = true;
            this.lblMoveDuration.Location = new System.Drawing.Point(12, 441);
            this.lblMoveDuration.Name = "lblMoveDuration";
            this.lblMoveDuration.Size = new System.Drawing.Size(100, 13);
            this.lblMoveDuration.TabIndex = 59;
            this.lblMoveDuration.Text = "Movement Duration";
            // 
            // cbPtr
            // 
            this.cbPtr.AutoSize = true;
            this.cbPtr.Location = new System.Drawing.Point(146, 64);
            this.cbPtr.Name = "cbPtr";
            this.cbPtr.Size = new System.Drawing.Size(72, 17);
            this.cbPtr.TabIndex = 61;
            this.cbPtr.Text = "PTR (3.3)";
            this.cbPtr.UseVisualStyleBackColor = true;
            this.cbPtr.Visible = false;
            this.cbPtr.CheckedChanged += new System.EventHandler(this.cbPtr_CheckedChanged);
            // 
            // CalculationOptionsPanelShadowPriest
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.AutoScroll = true;
            this.Controls.Add(this.cbPtr);
            this.Controls.Add(this.trkMoveDuration);
            this.Controls.Add(this.lblMoveDuration);
            this.Controls.Add(this.lblMoveFrequency);
            this.Controls.Add(this.trkMoveFrequency);
            this.Controls.Add(this.lblDelay);
            this.Controls.Add(this.lblFSR);
            this.Controls.Add(this.lblSurvivability);
            this.Controls.Add(this.trkJoW);
            this.Controls.Add(this.lblJoW);
            this.Controls.Add(this.label4);
            this.Controls.Add(this.trkReplenishment);
            this.Controls.Add(this.cmbManaAmt);
            this.Controls.Add(this.lblReplenishment);
            this.Controls.Add(this.trkShadowfiend);
            this.Controls.Add(this.lblShadowfiend);
            this.Controls.Add(this.trkFightLength);
            this.Controls.Add(this.label15);
            this.Controls.Add(this.cbTargetLevel);
            this.Controls.Add(this.gbSpellPriority);
            this.Controls.Add(this.lblFightLength);
            this.Controls.Add(this.trkFSR);
            this.Controls.Add(this.trkDelay);
            this.Controls.Add(this.trkSurvivability);
            this.Name = "CalculationOptionsPanelShadowPriest";
            this.Size = new System.Drawing.Size(300, 692);
            this.gbSpellPriority.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.trkFightLength)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.trkShadowfiend)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.trkReplenishment)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.trkJoW)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.trkSurvivability)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.trkFSR)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.trkDelay)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.trkMoveFrequency)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.trkMoveDuration)).EndInit();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Label lblFightLength;
        private System.Windows.Forms.ComboBox cmbManaAmt;
        private System.Windows.Forms.Label label4;
        private System.Windows.Forms.GroupBox gbSpellPriority;
        private System.Windows.Forms.ListBox lsSpellPriopity;
        private System.Windows.Forms.Button bChangePriority;
        private System.Windows.Forms.ComboBox cbTargetLevel;
        private System.Windows.Forms.Label label15;
        private System.Windows.Forms.ToolTip toolTip1;
        private System.Windows.Forms.TrackBar trkFightLength;
        private System.Windows.Forms.Label lblShadowfiend;
        private System.Windows.Forms.TrackBar trkShadowfiend;
        private System.Windows.Forms.Label lblReplenishment;
        private System.Windows.Forms.TrackBar trkReplenishment;
        private System.Windows.Forms.Label lblJoW;
        private System.Windows.Forms.TrackBar trkJoW;
        private System.Windows.Forms.TrackBar trkSurvivability;
        private System.Windows.Forms.Label lblSurvivability;
        private System.Windows.Forms.Label lblFSR;
        private System.Windows.Forms.TrackBar trkFSR;
        private System.Windows.Forms.TrackBar trkDelay;
        private System.Windows.Forms.Label lblDelay;
        private System.Windows.Forms.TrackBar trkMoveFrequency;
        private System.Windows.Forms.Label lblMoveFrequency;
        private System.Windows.Forms.Label lblMoveDuration;
        private System.Windows.Forms.TrackBar trkMoveDuration;
        private System.Windows.Forms.CheckBox cbPtr;
    }
}
