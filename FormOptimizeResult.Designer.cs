namespace Rawr
{
    partial class FormOptimizeResult
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(FormOptimizeResult));
            this.buttonKeep = new System.Windows.Forms.Button();
            this.buttonChange = new System.Windows.Forms.Button();
            this.paperDollAfter = new Rawr.PaperDoll();
            this.paperDollBefore = new Rawr.PaperDoll();
            this.SuspendLayout();
            // 
            // buttonKeep
            // 
            this.buttonKeep.DialogResult = System.Windows.Forms.DialogResult.No;
            this.buttonKeep.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.buttonKeep.Location = new System.Drawing.Point(164, 688);
            this.buttonKeep.Name = "buttonKeep";
            this.buttonKeep.Size = new System.Drawing.Size(179, 23);
            this.buttonKeep.TabIndex = 2;
            this.buttonKeep.Text = "Keep Existing Gear Setup";
            this.buttonKeep.UseVisualStyleBackColor = true;
            // 
            // buttonChange
            // 
            this.buttonChange.DialogResult = System.Windows.Forms.DialogResult.Yes;
            this.buttonChange.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.buttonChange.Location = new System.Drawing.Point(682, 688);
            this.buttonChange.Name = "buttonChange";
            this.buttonChange.Size = new System.Drawing.Size(179, 23);
            this.buttonChange.TabIndex = 3;
            this.buttonChange.Text = "Load Optimized Gear Setup";
            this.buttonChange.UseVisualStyleBackColor = true;
            // 
            // paperDollAfter
            // 
            this.paperDollAfter.BackHidden = false;
            this.paperDollAfter.ChestHidden = false;
            this.paperDollAfter.Cursor = System.Windows.Forms.Cursors.Default;
            this.paperDollAfter.FeetHidden = false;
            this.paperDollAfter.Finger1Hidden = false;
            this.paperDollAfter.Finger2Hidden = false;
            this.paperDollAfter.HandsHidden = false;
            this.paperDollAfter.HeadHidden = false;
            this.paperDollAfter.LegsHidden = false;
            this.paperDollAfter.Location = new System.Drawing.Point(519, 4);
            this.paperDollAfter.MainHandHidden = false;
            this.paperDollAfter.Name = "paperDollAfter";
            this.paperDollAfter.NeckHidden = false;
            this.paperDollAfter.OffHandHidden = false;
            this.paperDollAfter.ProjectileBagHidden = false;
            this.paperDollAfter.ProjectileHidden = false;
            this.paperDollAfter.RangedHidden = false;
            this.paperDollAfter.ShirtHidden = false;
            this.paperDollAfter.ShouldersHidden = false;
            this.paperDollAfter.Size = new System.Drawing.Size(476, 678);
            this.paperDollAfter.TabardHidden = false;
            this.paperDollAfter.TabIndex = 1;
            this.paperDollAfter.Trinket1Hidden = false;
            this.paperDollAfter.Trinket2Hidden = false;
            this.paperDollAfter.WaistHidden = false;
            this.paperDollAfter.WristHidden = false;
            // 
            // paperDollBefore
            // 
            this.paperDollBefore.BackHidden = false;
            this.paperDollBefore.ChestHidden = false;
            this.paperDollBefore.Cursor = System.Windows.Forms.Cursors.Default;
            this.paperDollBefore.FeetHidden = false;
            this.paperDollBefore.Finger1Hidden = false;
            this.paperDollBefore.Finger2Hidden = false;
            this.paperDollBefore.HandsHidden = false;
            this.paperDollBefore.HeadHidden = false;
            this.paperDollBefore.LegsHidden = false;
            this.paperDollBefore.Location = new System.Drawing.Point(3, 4);
            this.paperDollBefore.MainHandHidden = false;
            this.paperDollBefore.Name = "paperDollBefore";
            this.paperDollBefore.NeckHidden = false;
            this.paperDollBefore.OffHandHidden = false;
            this.paperDollBefore.ProjectileBagHidden = false;
            this.paperDollBefore.ProjectileHidden = false;
            this.paperDollBefore.RangedHidden = false;
            this.paperDollBefore.ShirtHidden = false;
            this.paperDollBefore.ShouldersHidden = false;
            this.paperDollBefore.Size = new System.Drawing.Size(476, 678);
            this.paperDollBefore.TabardHidden = false;
            this.paperDollBefore.TabIndex = 0;
            this.paperDollBefore.Trinket1Hidden = false;
            this.paperDollBefore.Trinket2Hidden = false;
            this.paperDollBefore.WaistHidden = false;
            this.paperDollBefore.WristHidden = false;
            // 
            // FormOptimizeResult
            // 
            this.AcceptButton = this.buttonChange;
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.AutoScroll = true;
            this.CancelButton = this.buttonKeep;
            this.ClientSize = new System.Drawing.Size(1003, 719);
            this.Controls.Add(this.buttonChange);
            this.Controls.Add(this.buttonKeep);
            this.Controls.Add(this.paperDollAfter);
            this.Controls.Add(this.paperDollBefore);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedToolWindow;
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.Name = "FormOptimizeResult";
            this.Text = "Optimizer Results";
            this.ResumeLayout(false);

        }

        #endregion

        private PaperDoll paperDollBefore;
        private PaperDoll paperDollAfter;
        private System.Windows.Forms.Button buttonKeep;
        private System.Windows.Forms.Button buttonChange;


    }
}