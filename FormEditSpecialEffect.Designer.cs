namespace Rawr
{
    partial class FormEditSpecialEffect
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
            this.butCancel = new System.Windows.Forms.Button();
            this.propertyGridStats = new System.Windows.Forms.PropertyGrid();
            this.nudChance = new System.Windows.Forms.NumericUpDown();
            this.nudDuration = new System.Windows.Forms.NumericUpDown();
            this.nudCooldown = new System.Windows.Forms.NumericUpDown();
            this.nudStacks = new System.Windows.Forms.NumericUpDown();
            this.label1 = new System.Windows.Forms.Label();
            this.label2 = new System.Windows.Forms.Label();
            this.label3 = new System.Windows.Forms.Label();
            this.label4 = new System.Windows.Forms.Label();
            this.cmbTrigger = new System.Windows.Forms.ComboBox();
            this.label5 = new System.Windows.Forms.Label();
            this.butOkay = new System.Windows.Forms.Button();
            this.cmbPPM = new System.Windows.Forms.ComboBox();
            ((System.ComponentModel.ISupportInitialize)(this.nudChance)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.nudDuration)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.nudCooldown)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.nudStacks)).BeginInit();
            this.SuspendLayout();
            // 
            // butCancel
            // 
            this.butCancel.DialogResult = System.Windows.Forms.DialogResult.Cancel;
            this.butCancel.Location = new System.Drawing.Point(85, 336);
            this.butCancel.Name = "butCancel";
            this.butCancel.Size = new System.Drawing.Size(75, 23);
            this.butCancel.TabIndex = 13;
            this.butCancel.Text = "Cancel";
            this.butCancel.UseVisualStyleBackColor = true;
            // 
            // propertyGridStats
            // 
            this.propertyGridStats.HelpVisible = false;
            this.propertyGridStats.Location = new System.Drawing.Point(12, 12);
            this.propertyGridStats.Name = "propertyGridStats";
            this.propertyGridStats.Size = new System.Drawing.Size(229, 187);
            this.propertyGridStats.TabIndex = 1;
            this.propertyGridStats.ToolbarVisible = false;
            // 
            // nudChance
            // 
            this.nudChance.DecimalPlaces = 2;
            this.nudChance.Location = new System.Drawing.Point(85, 284);
            this.nudChance.Name = "nudChance";
            this.nudChance.Size = new System.Drawing.Size(61, 20);
            this.nudChance.TabIndex = 9;
            // 
            // nudDuration
            // 
            this.nudDuration.Location = new System.Drawing.Point(85, 232);
            this.nudDuration.Name = "nudDuration";
            this.nudDuration.Size = new System.Drawing.Size(61, 20);
            this.nudDuration.TabIndex = 5;
            // 
            // nudCooldown
            // 
            this.nudCooldown.DecimalPlaces = 2;
            this.nudCooldown.Location = new System.Drawing.Point(85, 258);
            this.nudCooldown.Maximum = new decimal(new int[] {
            3600,
            0,
            0,
            0});
            this.nudCooldown.Name = "nudCooldown";
            this.nudCooldown.Size = new System.Drawing.Size(61, 20);
            this.nudCooldown.TabIndex = 7;
            // 
            // nudStacks
            // 
            this.nudStacks.Location = new System.Drawing.Point(85, 310);
            this.nudStacks.Name = "nudStacks";
            this.nudStacks.Size = new System.Drawing.Size(61, 20);
            this.nudStacks.TabIndex = 12;
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(29, 234);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(50, 13);
            this.label1.TabIndex = 4;
            this.label1.Text = "Duration:";
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Location = new System.Drawing.Point(22, 260);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(57, 13);
            this.label2.TabIndex = 6;
            this.label2.Text = "Cooldown:";
            // 
            // label3
            // 
            this.label3.AutoSize = true;
            this.label3.Location = new System.Drawing.Point(32, 286);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(47, 13);
            this.label3.TabIndex = 8;
            this.label3.Text = "Chance:";
            // 
            // label4
            // 
            this.label4.AutoSize = true;
            this.label4.Location = new System.Drawing.Point(36, 312);
            this.label4.Name = "label4";
            this.label4.Size = new System.Drawing.Size(43, 13);
            this.label4.TabIndex = 11;
            this.label4.Text = "Stacks:";
            // 
            // cmbTrigger
            // 
            this.cmbTrigger.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.cmbTrigger.DropDownWidth = 250;
            this.cmbTrigger.FormattingEnabled = true;
            this.cmbTrigger.Location = new System.Drawing.Point(85, 205);
            this.cmbTrigger.Name = "cmbTrigger";
            this.cmbTrigger.Size = new System.Drawing.Size(156, 21);
            this.cmbTrigger.TabIndex = 3;
            // 
            // label5
            // 
            this.label5.AutoSize = true;
            this.label5.Location = new System.Drawing.Point(36, 208);
            this.label5.Name = "label5";
            this.label5.Size = new System.Drawing.Size(43, 13);
            this.label5.TabIndex = 2;
            this.label5.Text = "Trigger:";
            // 
            // butOkay
            // 
            this.butOkay.Location = new System.Drawing.Point(166, 336);
            this.butOkay.Name = "butOkay";
            this.butOkay.Size = new System.Drawing.Size(75, 23);
            this.butOkay.TabIndex = 0;
            this.butOkay.Text = "OK";
            this.butOkay.UseVisualStyleBackColor = true;
            this.butOkay.Click += new System.EventHandler(this.butOkay_Click);
            // 
            // cmbPPM
            // 
            this.cmbPPM.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.cmbPPM.FormattingEnabled = true;
            this.cmbPPM.Items.AddRange(new object[] {
            "%",
            "PPM"});
            this.cmbPPM.Location = new System.Drawing.Point(152, 283);
            this.cmbPPM.Name = "cmbPPM";
            this.cmbPPM.Size = new System.Drawing.Size(49, 21);
            this.cmbPPM.TabIndex = 10;
            // 
            // FormEditSpecialEffect
            // 
            this.AcceptButton = this.butOkay;
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.CancelButton = this.butCancel;
            this.ClientSize = new System.Drawing.Size(253, 371);
            this.Controls.Add(this.cmbPPM);
            this.Controls.Add(this.butOkay);
            this.Controls.Add(this.label5);
            this.Controls.Add(this.cmbTrigger);
            this.Controls.Add(this.label4);
            this.Controls.Add(this.label3);
            this.Controls.Add(this.label2);
            this.Controls.Add(this.label1);
            this.Controls.Add(this.nudStacks);
            this.Controls.Add(this.nudCooldown);
            this.Controls.Add(this.nudDuration);
            this.Controls.Add(this.nudChance);
            this.Controls.Add(this.propertyGridStats);
            this.Controls.Add(this.butCancel);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedToolWindow;
            this.Name = "FormEditSpecialEffect";
            this.ShowInTaskbar = false;
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterParent;
            this.Text = "Edit Special Effect";
            ((System.ComponentModel.ISupportInitialize)(this.nudChance)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.nudDuration)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.nudCooldown)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.nudStacks)).EndInit();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Button butCancel;
        private System.Windows.Forms.PropertyGrid propertyGridStats;
        private System.Windows.Forms.NumericUpDown nudChance;
        private System.Windows.Forms.NumericUpDown nudDuration;
        private System.Windows.Forms.NumericUpDown nudCooldown;
        private System.Windows.Forms.NumericUpDown nudStacks;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.Label label3;
        private System.Windows.Forms.Label label4;
        private System.Windows.Forms.ComboBox cmbTrigger;
        private System.Windows.Forms.Label label5;
        private System.Windows.Forms.Button butOkay;
        private System.Windows.Forms.ComboBox cmbPPM;
    }
}