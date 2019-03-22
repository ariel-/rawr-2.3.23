namespace Rawr.Forms
{
	partial class Status
	{
		/// <summary>
		/// Required designer variable.
		/// </summary>
		private System.ComponentModel.IContainer components = null;

		#region Windows Form Designer generated code

		/// <summary>
		/// Required method for Designer support - do not modify
		/// the contents of this method with the code editor.
		/// </summary>
		private void InitializeComponent()
		{
            this.progressBar1 = new System.Windows.Forms.ProgressBar();
            this.Cancel = new System.Windows.Forms.Button();
            this.ShowHideDetails = new System.Windows.Forms.Button();
            this.label1 = new System.Windows.Forms.Label();
            this.groupBox1 = new System.Windows.Forms.GroupBox();
            this.tabControl1 = new System.Windows.Forms.TabControl();
            this.Tasks = new System.Windows.Forms.TabPage();
            this.TaskListView = new System.Windows.Forms.ListView();
            this.Key = new System.Windows.Forms.ColumnHeader();
            this.Details = new System.Windows.Forms.ColumnHeader();
            this.Errors = new System.Windows.Forms.TabPage();
            this.ErrorListView = new System.Windows.Forms.ListView();
            this.columnHeader1 = new System.Windows.Forms.ColumnHeader();
            this.columnHeader2 = new System.Windows.Forms.ColumnHeader();
            this.tabControl1.SuspendLayout();
            this.Tasks.SuspendLayout();
            this.Errors.SuspendLayout();
            this.SuspendLayout();
            // 
            // progressBar1
            // 
            this.progressBar1.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left)
                        | System.Windows.Forms.AnchorStyles.Right)));
            this.progressBar1.Location = new System.Drawing.Point(12, 41);
            this.progressBar1.Name = "progressBar1";
            this.progressBar1.Size = new System.Drawing.Size(395, 23);
            this.progressBar1.TabIndex = 0;
            // 
            // Cancel
            // 
            this.Cancel.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.Cancel.Location = new System.Drawing.Point(418, 12);
            this.Cancel.Name = "Cancel";
            this.Cancel.Size = new System.Drawing.Size(75, 23);
            this.Cancel.TabIndex = 1;
            this.Cancel.Text = "Cancell All";
            this.Cancel.UseVisualStyleBackColor = true;
            this.Cancel.Click += new System.EventHandler(this.Cancel_Click);
            // 
            // ShowHideDetails
            // 
            this.ShowHideDetails.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.ShowHideDetails.Location = new System.Drawing.Point(418, 41);
            this.ShowHideDetails.Name = "ShowHideDetails";
            this.ShowHideDetails.Size = new System.Drawing.Size(75, 23);
            this.ShowHideDetails.TabIndex = 2;
            this.ShowHideDetails.Text = "<< Details";
            this.ShowHideDetails.UseVisualStyleBackColor = true;
            this.ShowHideDetails.Click += new System.EventHandler(this.ShowHideDetails_Click);
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(13, 13);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(221, 13);
            this.label1.TabIndex = 3;
            this.label1.Text = "{0} of {1} Tasks have completed successfully";
            // 
            // groupBox1
            // 
            this.groupBox1.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left)
                        | System.Windows.Forms.AnchorStyles.Right)));
            this.groupBox1.Location = new System.Drawing.Point(12, 90);
            this.groupBox1.Name = "groupBox1";
            this.groupBox1.Size = new System.Drawing.Size(481, 10);
            this.groupBox1.TabIndex = 5;
            this.groupBox1.TabStop = false;
            // 
            // tabControl1
            // 
            this.tabControl1.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom)
                        | System.Windows.Forms.AnchorStyles.Left)
                        | System.Windows.Forms.AnchorStyles.Right)));
            this.tabControl1.Controls.Add(this.Tasks);
            this.tabControl1.Controls.Add(this.Errors);
            this.tabControl1.Location = new System.Drawing.Point(16, 110);
            this.tabControl1.Name = "tabControl1";
            this.tabControl1.SelectedIndex = 0;
            this.tabControl1.Size = new System.Drawing.Size(477, 171);
            this.tabControl1.TabIndex = 6;
            // 
            // Tasks
            // 
            this.Tasks.Controls.Add(this.TaskListView);
            this.Tasks.Location = new System.Drawing.Point(4, 22);
            this.Tasks.Name = "Tasks";
            this.Tasks.Padding = new System.Windows.Forms.Padding(3);
            this.Tasks.Size = new System.Drawing.Size(469, 145);
            this.Tasks.TabIndex = 0;
            this.Tasks.Text = "Tasks";
            this.Tasks.UseVisualStyleBackColor = true;
            // 
            // TaskListView
            // 
            this.TaskListView.Columns.AddRange(new System.Windows.Forms.ColumnHeader[] {
            this.Key,
            this.Details});
            this.TaskListView.Dock = System.Windows.Forms.DockStyle.Fill;
            this.TaskListView.HeaderStyle = System.Windows.Forms.ColumnHeaderStyle.Nonclickable;
            this.TaskListView.Location = new System.Drawing.Point(3, 3);
            this.TaskListView.MultiSelect = false;
            this.TaskListView.Name = "TaskListView";
            this.TaskListView.Size = new System.Drawing.Size(463, 139);
            this.TaskListView.TabIndex = 0;
            this.TaskListView.UseCompatibleStateImageBehavior = false;
            this.TaskListView.View = System.Windows.Forms.View.Details;
            // 
            // Key
            // 
            this.Key.Text = "Step";
            this.Key.Width = 150;
            // 
            // Details
            // 
            this.Details.Text = "Detail";
            this.Details.Width = 288;
            // 
            // Errors
            // 
            this.Errors.Controls.Add(this.ErrorListView);
            this.Errors.Location = new System.Drawing.Point(4, 22);
            this.Errors.Name = "Errors";
            this.Errors.Padding = new System.Windows.Forms.Padding(3);
            this.Errors.Size = new System.Drawing.Size(469, 145);
            this.Errors.TabIndex = 1;
            this.Errors.Text = "Errors";
            this.Errors.UseVisualStyleBackColor = true;
            // 
            // ErrorListView
            // 
            this.ErrorListView.Columns.AddRange(new System.Windows.Forms.ColumnHeader[] {
            this.columnHeader1,
            this.columnHeader2});
            this.ErrorListView.Dock = System.Windows.Forms.DockStyle.Fill;
            this.ErrorListView.FullRowSelect = true;
            this.ErrorListView.HeaderStyle = System.Windows.Forms.ColumnHeaderStyle.Nonclickable;
            this.ErrorListView.Location = new System.Drawing.Point(3, 3);
            this.ErrorListView.MultiSelect = false;
            this.ErrorListView.Name = "ErrorListView";
            this.ErrorListView.Size = new System.Drawing.Size(463, 139);
            this.ErrorListView.TabIndex = 1;
            this.ErrorListView.UseCompatibleStateImageBehavior = false;
            this.ErrorListView.View = System.Windows.Forms.View.Details;
            this.ErrorListView.DoubleClick += new System.EventHandler(this.ErrorListView_DoubleClick);
            // 
            // columnHeader1
            // 
            this.columnHeader1.Text = "Step";
            this.columnHeader1.Width = 150;
            // 
            // columnHeader2
            // 
            this.columnHeader2.Text = "Detail";
            this.columnHeader2.Width = 210;
            // 
            // Status
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(505, 300);
            this.Controls.Add(this.tabControl1);
            this.Controls.Add(this.groupBox1);
            this.Controls.Add(this.label1);
            this.Controls.Add(this.ShowHideDetails);
            this.Controls.Add(this.Cancel);
            this.Controls.Add(this.progressBar1);
            this.MaximizeBox = false;
            this.MinimumSize = new System.Drawing.Size(440, 327);
            this.Name = "Status";
            this.ShowIcon = false;
            this.SizeGripStyle = System.Windows.Forms.SizeGripStyle.Hide;
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterParent;
            this.Text = "Status";
            this.FormClosing += new System.Windows.Forms.FormClosingEventHandler(this.Status_FormClosing);
            this.tabControl1.ResumeLayout(false);
            this.Tasks.ResumeLayout(false);
            this.Errors.ResumeLayout(false);
            this.ResumeLayout(false);
            this.PerformLayout();

		}

		#endregion

		private System.Windows.Forms.ProgressBar progressBar1;
		private System.Windows.Forms.Button Cancel;
		private System.Windows.Forms.Button ShowHideDetails;
		private System.Windows.Forms.Label label1;
		private System.Windows.Forms.GroupBox groupBox1;
		private System.Windows.Forms.TabControl tabControl1;
		private System.Windows.Forms.TabPage Tasks;
        private System.Windows.Forms.TabPage Errors;
        private System.Windows.Forms.ListView TaskListView;
        private System.Windows.Forms.ColumnHeader Key;
        private System.Windows.Forms.ColumnHeader Details;
        private System.Windows.Forms.ListView ErrorListView;
        private System.Windows.Forms.ColumnHeader columnHeader1;
        private System.Windows.Forms.ColumnHeader columnHeader2;
	}
}