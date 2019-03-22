namespace Rawr
{
    partial class FormBatchTools
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
            this.components = new System.ComponentModel.Container();
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(FormBatchTools));
            this.menuStrip1 = new System.Windows.Forms.MenuStrip();
            this.fileToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.newToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.importToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.openToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.saveToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.saveAsToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.toolStripSeparator1 = new System.Windows.Forms.ToolStripSeparator();
            this.exitToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.toolsToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.setAvailableItemsToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.replaceUnavailableToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.optimizeToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.buildUpgradeListToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.bathcOptimizeToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.buildBatchUpgradeListToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.progressiveOptimizeToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.buildProgressiveUpgradeListToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.toolStripSeparator2 = new System.Windows.Forms.ToolStripSeparator();
            this.saveCharactersToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.saveCharactersAsCopyToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.dataGridView = new System.Windows.Forms.DataGridView();
            this.characterColumn = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.Locked = new System.Windows.Forms.DataGridViewCheckBoxColumn();
            this.weightColumn = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.scoreColumn = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.newScoreColumn = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.loadBatchCharacterColumn = new Rawr.FormBatchTools.MyDataGridViewButtonColumn();
            this.showBatchCharacterColumn = new Rawr.FormBatchTools.MyDataGridViewButtonColumn();
            this.diffBatchCharacterColumn = new Rawr.FormBatchTools.MyDataGridViewButtonColumn();
            this.batchCharacterListBindingSource = new System.Windows.Forms.BindingSource(this.components);
            this.statusStrip1 = new Rawr.FormBatchTools.MyStatusStrip();
            this.statusLabel = new System.Windows.Forms.ToolStripStatusLabel();
            this.statusProgressBar = new System.Windows.Forms.ToolStripProgressBar();
            this.panel1 = new System.Windows.Forms.Panel();
            this.textBoxSingleItemUpgrade = new System.Windows.Forms.TextBox();
            this.label3 = new System.Windows.Forms.Label();
            this.buttonDown = new System.Windows.Forms.Button();
            this.buttonUp = new System.Windows.Forms.Button();
            this.buttonCancel = new System.Windows.Forms.Button();
            this.trackBarMaxRounds = new System.Windows.Forms.TrackBar();
            this.batchToolsBindingSource = new System.Windows.Forms.BindingSource(this.components);
            this.label1 = new System.Windows.Forms.Label();
            this.label2 = new System.Windows.Forms.Label();
            this.checkBoxOverrideReenchant = new System.Windows.Forms.CheckBox();
            this.checkBoxOverrideRegem = new System.Windows.Forms.CheckBox();
            this.trackBarThoroughness = new System.Windows.Forms.TrackBar();
            this.menuStrip1.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.dataGridView)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.batchCharacterListBindingSource)).BeginInit();
            this.statusStrip1.SuspendLayout();
            this.panel1.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.trackBarMaxRounds)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.batchToolsBindingSource)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.trackBarThoroughness)).BeginInit();
            this.SuspendLayout();
            // 
            // menuStrip1
            // 
            this.menuStrip1.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.fileToolStripMenuItem,
            this.toolsToolStripMenuItem});
            this.menuStrip1.Location = new System.Drawing.Point(0, 0);
            this.menuStrip1.Name = "menuStrip1";
            this.menuStrip1.Size = new System.Drawing.Size(674, 24);
            this.menuStrip1.TabIndex = 0;
            this.menuStrip1.Text = "menuStripBatch";
            // 
            // fileToolStripMenuItem
            // 
            this.fileToolStripMenuItem.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.newToolStripMenuItem,
            this.importToolStripMenuItem,
            this.openToolStripMenuItem,
            this.saveToolStripMenuItem,
            this.saveAsToolStripMenuItem,
            this.toolStripSeparator1,
            this.exitToolStripMenuItem});
            this.fileToolStripMenuItem.Name = "fileToolStripMenuItem";
            this.fileToolStripMenuItem.Size = new System.Drawing.Size(35, 20);
            this.fileToolStripMenuItem.Text = "&File";
            // 
            // newToolStripMenuItem
            // 
            this.newToolStripMenuItem.Name = "newToolStripMenuItem";
            this.newToolStripMenuItem.Size = new System.Drawing.Size(136, 22);
            this.newToolStripMenuItem.Text = "&New";
            this.newToolStripMenuItem.Click += new System.EventHandler(this.newToolStripMenuItem_Click);
            // 
            // importToolStripMenuItem
            // 
            this.importToolStripMenuItem.Name = "importToolStripMenuItem";
            this.importToolStripMenuItem.Size = new System.Drawing.Size(136, 22);
            this.importToolStripMenuItem.Text = "&Import";
            this.importToolStripMenuItem.Click += new System.EventHandler(this.importToolStripMenuItem_Click);
            // 
            // openToolStripMenuItem
            // 
            this.openToolStripMenuItem.Name = "openToolStripMenuItem";
            this.openToolStripMenuItem.Size = new System.Drawing.Size(136, 22);
            this.openToolStripMenuItem.Text = "&Open";
            this.openToolStripMenuItem.Click += new System.EventHandler(this.openToolStripMenuItem_Click);
            // 
            // saveToolStripMenuItem
            // 
            this.saveToolStripMenuItem.Name = "saveToolStripMenuItem";
            this.saveToolStripMenuItem.Size = new System.Drawing.Size(136, 22);
            this.saveToolStripMenuItem.Text = "&Save";
            this.saveToolStripMenuItem.Click += new System.EventHandler(this.saveToolStripMenuItem_Click);
            // 
            // saveAsToolStripMenuItem
            // 
            this.saveAsToolStripMenuItem.Name = "saveAsToolStripMenuItem";
            this.saveAsToolStripMenuItem.Size = new System.Drawing.Size(136, 22);
            this.saveAsToolStripMenuItem.Text = "Save &As...";
            this.saveAsToolStripMenuItem.Click += new System.EventHandler(this.saveAsToolStripMenuItem_Click);
            // 
            // toolStripSeparator1
            // 
            this.toolStripSeparator1.Name = "toolStripSeparator1";
            this.toolStripSeparator1.Size = new System.Drawing.Size(133, 6);
            // 
            // exitToolStripMenuItem
            // 
            this.exitToolStripMenuItem.Name = "exitToolStripMenuItem";
            this.exitToolStripMenuItem.Size = new System.Drawing.Size(136, 22);
            this.exitToolStripMenuItem.Text = "E&xit";
            this.exitToolStripMenuItem.Click += new System.EventHandler(this.exitToolStripMenuItem_Click);
            // 
            // toolsToolStripMenuItem
            // 
            this.toolsToolStripMenuItem.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.setAvailableItemsToolStripMenuItem,
            this.replaceUnavailableToolStripMenuItem,
            this.optimizeToolStripMenuItem,
            this.buildUpgradeListToolStripMenuItem,
            this.bathcOptimizeToolStripMenuItem,
            this.buildBatchUpgradeListToolStripMenuItem,
            this.progressiveOptimizeToolStripMenuItem,
            this.buildProgressiveUpgradeListToolStripMenuItem,
            this.toolStripSeparator2,
            this.saveCharactersToolStripMenuItem,
            this.saveCharactersAsCopyToolStripMenuItem});
            this.toolsToolStripMenuItem.Name = "toolsToolStripMenuItem";
            this.toolsToolStripMenuItem.Size = new System.Drawing.Size(44, 20);
            this.toolsToolStripMenuItem.Text = "&Tools";
            // 
            // setAvailableItemsToolStripMenuItem
            // 
            this.setAvailableItemsToolStripMenuItem.Name = "setAvailableItemsToolStripMenuItem";
            this.setAvailableItemsToolStripMenuItem.Size = new System.Drawing.Size(229, 22);
            this.setAvailableItemsToolStripMenuItem.Text = "Set Available Items";
            this.setAvailableItemsToolStripMenuItem.Click += new System.EventHandler(this.setAvailableGearToolStripMenuItem_Click);
            // 
            // replaceUnavailableToolStripMenuItem
            // 
            this.replaceUnavailableToolStripMenuItem.Name = "replaceUnavailableToolStripMenuItem";
            this.replaceUnavailableToolStripMenuItem.Size = new System.Drawing.Size(229, 22);
            this.replaceUnavailableToolStripMenuItem.Text = "Replace Unavailable";
            this.replaceUnavailableToolStripMenuItem.Click += new System.EventHandler(this.replaceUnavailableToolStripMenuItem_Click);
            // 
            // optimizeToolStripMenuItem
            // 
            this.optimizeToolStripMenuItem.Name = "optimizeToolStripMenuItem";
            this.optimizeToolStripMenuItem.Size = new System.Drawing.Size(229, 22);
            this.optimizeToolStripMenuItem.Text = "Optimize";
            this.optimizeToolStripMenuItem.Click += new System.EventHandler(this.optimizeToolStripMenuItem_Click);
            // 
            // buildUpgradeListToolStripMenuItem
            // 
            this.buildUpgradeListToolStripMenuItem.Name = "buildUpgradeListToolStripMenuItem";
            this.buildUpgradeListToolStripMenuItem.Size = new System.Drawing.Size(229, 22);
            this.buildUpgradeListToolStripMenuItem.Text = "Build Upgrade List";
            this.buildUpgradeListToolStripMenuItem.Click += new System.EventHandler(this.buildUpgradeListToolStripMenuItem_Click);
            // 
            // bathcOptimizeToolStripMenuItem
            // 
            this.bathcOptimizeToolStripMenuItem.Name = "bathcOptimizeToolStripMenuItem";
            this.bathcOptimizeToolStripMenuItem.Size = new System.Drawing.Size(229, 22);
            this.bathcOptimizeToolStripMenuItem.Text = "Batch Optimize";
            this.bathcOptimizeToolStripMenuItem.Click += new System.EventHandler(this.bathcOptimizeToolStripMenuItem_Click);
            // 
            // buildBatchUpgradeListToolStripMenuItem
            // 
            this.buildBatchUpgradeListToolStripMenuItem.Name = "buildBatchUpgradeListToolStripMenuItem";
            this.buildBatchUpgradeListToolStripMenuItem.Size = new System.Drawing.Size(229, 22);
            this.buildBatchUpgradeListToolStripMenuItem.Text = "Build Batch Upgrade List";
            this.buildBatchUpgradeListToolStripMenuItem.Click += new System.EventHandler(this.buildBatchUpgradeListToolStripMenuItem_Click);
            // 
            // progressiveOptimizeToolStripMenuItem
            // 
            this.progressiveOptimizeToolStripMenuItem.Name = "progressiveOptimizeToolStripMenuItem";
            this.progressiveOptimizeToolStripMenuItem.Size = new System.Drawing.Size(229, 22);
            this.progressiveOptimizeToolStripMenuItem.Text = "Progressive Optimize";
            this.progressiveOptimizeToolStripMenuItem.Click += new System.EventHandler(this.progressiveOptimizeToolStripMenuItem_Click);
            // 
            // buildProgressiveUpgradeListToolStripMenuItem
            // 
            this.buildProgressiveUpgradeListToolStripMenuItem.Name = "buildProgressiveUpgradeListToolStripMenuItem";
            this.buildProgressiveUpgradeListToolStripMenuItem.Size = new System.Drawing.Size(229, 22);
            this.buildProgressiveUpgradeListToolStripMenuItem.Text = "Build Progressive Upgrade List";
            this.buildProgressiveUpgradeListToolStripMenuItem.Click += new System.EventHandler(this.buildProgressiveUpgradeListToolStripMenuItem_Click);
            // 
            // toolStripSeparator2
            // 
            this.toolStripSeparator2.Name = "toolStripSeparator2";
            this.toolStripSeparator2.Size = new System.Drawing.Size(226, 6);
            // 
            // saveCharactersToolStripMenuItem
            // 
            this.saveCharactersToolStripMenuItem.Name = "saveCharactersToolStripMenuItem";
            this.saveCharactersToolStripMenuItem.Size = new System.Drawing.Size(229, 22);
            this.saveCharactersToolStripMenuItem.Text = "Save Characters";
            this.saveCharactersToolStripMenuItem.Click += new System.EventHandler(this.saveCharactersToolStripMenuItem_Click);
            // 
            // saveCharactersAsCopyToolStripMenuItem
            // 
            this.saveCharactersAsCopyToolStripMenuItem.Name = "saveCharactersAsCopyToolStripMenuItem";
            this.saveCharactersAsCopyToolStripMenuItem.Size = new System.Drawing.Size(229, 22);
            this.saveCharactersAsCopyToolStripMenuItem.Text = "Save Characters As Copy";
            this.saveCharactersAsCopyToolStripMenuItem.Click += new System.EventHandler(this.saveCharactersAsCopyToolStripMenuItem_Click);
            // 
            // dataGridView
            // 
            this.dataGridView.AutoGenerateColumns = false;
            this.dataGridView.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            this.dataGridView.Columns.AddRange(new System.Windows.Forms.DataGridViewColumn[] {
            this.characterColumn,
            this.Locked,
            this.weightColumn,
            this.scoreColumn,
            this.newScoreColumn,
            this.loadBatchCharacterColumn,
            this.showBatchCharacterColumn,
            this.diffBatchCharacterColumn});
            this.dataGridView.DataSource = this.batchCharacterListBindingSource;
            this.dataGridView.Dock = System.Windows.Forms.DockStyle.Fill;
            this.dataGridView.Location = new System.Drawing.Point(0, 24);
            this.dataGridView.Margin = new System.Windows.Forms.Padding(0);
            this.dataGridView.Name = "dataGridView";
            this.dataGridView.SelectionMode = System.Windows.Forms.DataGridViewSelectionMode.FullRowSelect;
            this.dataGridView.Size = new System.Drawing.Size(506, 336);
            this.dataGridView.TabIndex = 1;
            this.dataGridView.CellClick += new System.Windows.Forms.DataGridViewCellEventHandler(this.dataGridView_CellClick);
            this.dataGridView.SelectionChanged += new System.EventHandler(this.dataGridView_SelectionChanged);
            // 
            // characterColumn
            // 
            this.characterColumn.AutoSizeMode = System.Windows.Forms.DataGridViewAutoSizeColumnMode.Fill;
            this.characterColumn.DataPropertyName = "Name";
            this.characterColumn.HeaderText = "Character";
            this.characterColumn.Name = "characterColumn";
            this.characterColumn.ReadOnly = true;
            this.characterColumn.SortMode = System.Windows.Forms.DataGridViewColumnSortMode.NotSortable;
            // 
            // Locked
            // 
            this.Locked.AutoSizeMode = System.Windows.Forms.DataGridViewAutoSizeColumnMode.ColumnHeader;
            this.Locked.DataPropertyName = "Locked";
            this.Locked.HeaderText = "Locked";
            this.Locked.Name = "Locked";
            this.Locked.Width = 49;
            // 
            // weightColumn
            // 
            this.weightColumn.DataPropertyName = "Weight";
            this.weightColumn.HeaderText = "Weight";
            this.weightColumn.Name = "weightColumn";
            this.weightColumn.SortMode = System.Windows.Forms.DataGridViewColumnSortMode.NotSortable;
            this.weightColumn.Width = 50;
            // 
            // scoreColumn
            // 
            this.scoreColumn.DataPropertyName = "Score";
            this.scoreColumn.HeaderText = "Score";
            this.scoreColumn.Name = "scoreColumn";
            this.scoreColumn.ReadOnly = true;
            this.scoreColumn.SortMode = System.Windows.Forms.DataGridViewColumnSortMode.NotSortable;
            this.scoreColumn.Width = 66;
            // 
            // newScoreColumn
            // 
            this.newScoreColumn.DataPropertyName = "NewScore";
            this.newScoreColumn.HeaderText = "New Score";
            this.newScoreColumn.Name = "newScoreColumn";
            this.newScoreColumn.ReadOnly = true;
            this.newScoreColumn.SortMode = System.Windows.Forms.DataGridViewColumnSortMode.NotSortable;
            this.newScoreColumn.Width = 66;
            // 
            // loadBatchCharacterColumn
            // 
            this.loadBatchCharacterColumn.DataPropertyName = "RelativePath";
            this.loadBatchCharacterColumn.HeaderText = "";
            this.loadBatchCharacterColumn.Name = "loadBatchCharacterColumn";
            this.loadBatchCharacterColumn.NewRowButtonVisible = true;
            this.loadBatchCharacterColumn.Resizable = System.Windows.Forms.DataGridViewTriState.False;
            this.loadBatchCharacterColumn.Text = "...";
            this.loadBatchCharacterColumn.UseColumnTextForButtonValue = true;
            this.loadBatchCharacterColumn.Width = 20;
            // 
            // showBatchCharacterColumn
            // 
            this.showBatchCharacterColumn.HeaderText = "";
            this.showBatchCharacterColumn.Name = "showBatchCharacterColumn";
            this.showBatchCharacterColumn.Resizable = System.Windows.Forms.DataGridViewTriState.False;
            this.showBatchCharacterColumn.Text = "Show";
            this.showBatchCharacterColumn.UseColumnTextForButtonValue = true;
            this.showBatchCharacterColumn.Width = 50;
            // 
            // diffBatchCharacterColumn
            // 
            this.diffBatchCharacterColumn.HeaderText = "";
            this.diffBatchCharacterColumn.Name = "diffBatchCharacterColumn";
            this.diffBatchCharacterColumn.Resizable = System.Windows.Forms.DataGridViewTriState.False;
            this.diffBatchCharacterColumn.Text = "Diff";
            this.diffBatchCharacterColumn.UseColumnTextForButtonValue = true;
            this.diffBatchCharacterColumn.Width = 50;
            // 
            // batchCharacterListBindingSource
            // 
            this.batchCharacterListBindingSource.DataSource = typeof(Rawr.BatchCharacterList);
            this.batchCharacterListBindingSource.ListChanged += new System.ComponentModel.ListChangedEventHandler(this.batchCharacterListBindingSource_ListChanged);
            // 
            // statusStrip1
            // 
            this.statusStrip1.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.statusLabel,
            this.statusProgressBar});
            this.statusStrip1.Location = new System.Drawing.Point(0, 360);
            this.statusStrip1.Name = "statusStrip1";
            this.statusStrip1.Size = new System.Drawing.Size(674, 22);
            this.statusStrip1.TabIndex = 2;
            // 
            // statusLabel
            // 
            this.statusLabel.AutoSize = false;
            this.statusLabel.Name = "statusLabel";
            this.statusLabel.Size = new System.Drawing.Size(250, 17);
            this.statusLabel.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            // 
            // statusProgressBar
            // 
            this.statusProgressBar.Alignment = System.Windows.Forms.ToolStripItemAlignment.Right;
            this.statusProgressBar.Name = "statusProgressBar";
            this.statusProgressBar.Size = new System.Drawing.Size(100, 16);
            // 
            // panel1
            // 
            this.panel1.Controls.Add(this.textBoxSingleItemUpgrade);
            this.panel1.Controls.Add(this.label3);
            this.panel1.Controls.Add(this.buttonDown);
            this.panel1.Controls.Add(this.buttonUp);
            this.panel1.Controls.Add(this.buttonCancel);
            this.panel1.Controls.Add(this.trackBarMaxRounds);
            this.panel1.Controls.Add(this.label1);
            this.panel1.Controls.Add(this.label2);
            this.panel1.Controls.Add(this.checkBoxOverrideReenchant);
            this.panel1.Controls.Add(this.checkBoxOverrideRegem);
            this.panel1.Controls.Add(this.trackBarThoroughness);
            this.panel1.Dock = System.Windows.Forms.DockStyle.Right;
            this.panel1.Location = new System.Drawing.Point(506, 24);
            this.panel1.Name = "panel1";
            this.panel1.Size = new System.Drawing.Size(168, 336);
            this.panel1.TabIndex = 3;
            // 
            // textBoxSingleItemUpgrade
            // 
            this.textBoxSingleItemUpgrade.Location = new System.Drawing.Point(6, 186);
            this.textBoxSingleItemUpgrade.Name = "textBoxSingleItemUpgrade";
            this.textBoxSingleItemUpgrade.Size = new System.Drawing.Size(150, 20);
            this.textBoxSingleItemUpgrade.TabIndex = 35;
            // 
            // label3
            // 
            this.label3.AutoSize = true;
            this.label3.Location = new System.Drawing.Point(3, 170);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(108, 13);
            this.label3.TabIndex = 34;
            this.label3.Text = "Single Item Upgrades";
            // 
            // buttonDown
            // 
            this.buttonDown.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this.buttonDown.Enabled = false;
            this.buttonDown.Location = new System.Drawing.Point(33, 310);
            this.buttonDown.Name = "buttonDown";
            this.buttonDown.Size = new System.Drawing.Size(23, 23);
            this.buttonDown.TabIndex = 33;
            this.buttonDown.Text = "↓";
            this.buttonDown.UseVisualStyleBackColor = true;
            this.buttonDown.Click += new System.EventHandler(this.buttonDown_Click);
            // 
            // buttonUp
            // 
            this.buttonUp.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this.buttonUp.Enabled = false;
            this.buttonUp.Location = new System.Drawing.Point(6, 310);
            this.buttonUp.Name = "buttonUp";
            this.buttonUp.Size = new System.Drawing.Size(23, 23);
            this.buttonUp.TabIndex = 32;
            this.buttonUp.Text = "↑";
            this.buttonUp.UseVisualStyleBackColor = true;
            this.buttonUp.Click += new System.EventHandler(this.buttonUp_Click);
            // 
            // buttonCancel
            // 
            this.buttonCancel.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this.buttonCancel.DialogResult = System.Windows.Forms.DialogResult.Cancel;
            this.buttonCancel.Enabled = false;
            this.buttonCancel.Location = new System.Drawing.Point(90, 310);
            this.buttonCancel.Name = "buttonCancel";
            this.buttonCancel.Size = new System.Drawing.Size(75, 23);
            this.buttonCancel.TabIndex = 14;
            this.buttonCancel.Text = "Cancel";
            this.buttonCancel.UseVisualStyleBackColor = true;
            this.buttonCancel.Click += new System.EventHandler(this.buttonCancel_Click);
            // 
            // trackBarMaxRounds
            // 
            this.trackBarMaxRounds.DataBindings.Add(new System.Windows.Forms.Binding("Value", this.batchToolsBindingSource, "MaxRounds", true, System.Windows.Forms.DataSourceUpdateMode.OnPropertyChanged));
            this.trackBarMaxRounds.LargeChange = 1;
            this.trackBarMaxRounds.Location = new System.Drawing.Point(79, 122);
            this.trackBarMaxRounds.Maximum = 5;
            this.trackBarMaxRounds.Minimum = 1;
            this.trackBarMaxRounds.Name = "trackBarMaxRounds";
            this.trackBarMaxRounds.Size = new System.Drawing.Size(77, 45);
            this.trackBarMaxRounds.TabIndex = 13;
            this.trackBarMaxRounds.Value = 3;
            // 
            // batchToolsBindingSource
            // 
            this.batchToolsBindingSource.DataSource = typeof(Rawr.BatchTools);
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(3, 129);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(70, 13);
            this.label1.TabIndex = 12;
            this.label1.Text = "Max Rounds:";
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Location = new System.Drawing.Point(3, 9);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(78, 13);
            this.label2.TabIndex = 11;
            this.label2.Text = "Thoroughness:";
            // 
            // checkBoxOverrideReenchant
            // 
            this.checkBoxOverrideReenchant.AutoSize = true;
            this.checkBoxOverrideReenchant.DataBindings.Add(new System.Windows.Forms.Binding("Checked", this.batchToolsBindingSource, "OverrideReenchant", true, System.Windows.Forms.DataSourceUpdateMode.OnPropertyChanged));
            this.checkBoxOverrideReenchant.Location = new System.Drawing.Point(6, 99);
            this.checkBoxOverrideReenchant.Name = "checkBoxOverrideReenchant";
            this.checkBoxOverrideReenchant.Size = new System.Drawing.Size(122, 17);
            this.checkBoxOverrideReenchant.TabIndex = 10;
            this.checkBoxOverrideReenchant.Text = "Override Reenchant";
            this.checkBoxOverrideReenchant.UseVisualStyleBackColor = true;
            // 
            // checkBoxOverrideRegem
            // 
            this.checkBoxOverrideRegem.AutoSize = true;
            this.checkBoxOverrideRegem.DataBindings.Add(new System.Windows.Forms.Binding("Checked", this.batchToolsBindingSource, "OverrideRegem", true, System.Windows.Forms.DataSourceUpdateMode.OnPropertyChanged));
            this.checkBoxOverrideRegem.Location = new System.Drawing.Point(6, 76);
            this.checkBoxOverrideRegem.Name = "checkBoxOverrideRegem";
            this.checkBoxOverrideRegem.Size = new System.Drawing.Size(103, 17);
            this.checkBoxOverrideRegem.TabIndex = 9;
            this.checkBoxOverrideRegem.Text = "Override Regem";
            this.checkBoxOverrideRegem.UseVisualStyleBackColor = true;
            // 
            // trackBarThoroughness
            // 
            this.trackBarThoroughness.DataBindings.Add(new System.Windows.Forms.Binding("Value", this.batchToolsBindingSource, "Thoroughness", true, System.Windows.Forms.DataSourceUpdateMode.OnPropertyChanged));
            this.trackBarThoroughness.Location = new System.Drawing.Point(6, 25);
            this.trackBarThoroughness.Maximum = 400;
            this.trackBarThoroughness.Minimum = 1;
            this.trackBarThoroughness.Name = "trackBarThoroughness";
            this.trackBarThoroughness.Size = new System.Drawing.Size(150, 45);
            this.trackBarThoroughness.TabIndex = 8;
            this.trackBarThoroughness.TickFrequency = 10;
            this.trackBarThoroughness.Value = 150;
            // 
            // FormBatchTools
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(674, 382);
            this.Controls.Add(this.dataGridView);
            this.Controls.Add(this.panel1);
            this.Controls.Add(this.statusStrip1);
            this.Controls.Add(this.menuStrip1);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.SizableToolWindow;
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.MainMenuStrip = this.menuStrip1;
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.MinimumSize = new System.Drawing.Size(682, 408);
            this.Name = "FormBatchTools";
            this.ShowIcon = false;
            this.Text = "Batch Tools";
            this.FormClosed += new System.Windows.Forms.FormClosedEventHandler(this.FormBatchTools_FormClosed);
            this.FormClosing += new System.Windows.Forms.FormClosingEventHandler(this.FormBatchTools_FormClosing);
            this.menuStrip1.ResumeLayout(false);
            this.menuStrip1.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.dataGridView)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.batchCharacterListBindingSource)).EndInit();
            this.statusStrip1.ResumeLayout(false);
            this.statusStrip1.PerformLayout();
            this.panel1.ResumeLayout(false);
            this.panel1.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.trackBarMaxRounds)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.batchToolsBindingSource)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.trackBarThoroughness)).EndInit();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.MenuStrip menuStrip1;
        private System.Windows.Forms.ToolStripMenuItem fileToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem newToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem openToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem saveToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem saveAsToolStripMenuItem;
        private System.Windows.Forms.ToolStripSeparator toolStripSeparator1;
        private System.Windows.Forms.ToolStripMenuItem exitToolStripMenuItem;
        private System.Windows.Forms.DataGridView dataGridView;
        internal System.Windows.Forms.BindingSource batchCharacterListBindingSource;
        private System.Windows.Forms.ToolStripMenuItem importToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem toolsToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem setAvailableItemsToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem saveCharactersToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem saveCharactersAsCopyToolStripMenuItem;
        private System.Windows.Forms.ToolStripSeparator toolStripSeparator2;
        private MyStatusStrip statusStrip1;
        private System.Windows.Forms.ToolStripStatusLabel statusLabel;
        private System.Windows.Forms.ToolStripProgressBar statusProgressBar;
        private System.Windows.Forms.ToolStripMenuItem optimizeToolStripMenuItem;
        private System.Windows.Forms.Panel panel1;
        private System.Windows.Forms.CheckBox checkBoxOverrideReenchant;
        private System.Windows.Forms.CheckBox checkBoxOverrideRegem;
        private System.Windows.Forms.TrackBar trackBarThoroughness;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.TrackBar trackBarMaxRounds;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.Button buttonCancel;
        private System.Windows.Forms.ToolStripMenuItem buildUpgradeListToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem replaceUnavailableToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem bathcOptimizeToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem buildBatchUpgradeListToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem progressiveOptimizeToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem buildProgressiveUpgradeListToolStripMenuItem;
        private System.Windows.Forms.Button buttonDown;
        private System.Windows.Forms.Button buttonUp;
        private System.Windows.Forms.Label label3;
        private System.Windows.Forms.TextBox textBoxSingleItemUpgrade;
        private System.Windows.Forms.DataGridViewTextBoxColumn characterColumn;
        private System.Windows.Forms.DataGridViewCheckBoxColumn Locked;
        private System.Windows.Forms.DataGridViewTextBoxColumn weightColumn;
        private System.Windows.Forms.DataGridViewTextBoxColumn scoreColumn;
        private System.Windows.Forms.DataGridViewTextBoxColumn newScoreColumn;
        private FormBatchTools.MyDataGridViewButtonColumn loadBatchCharacterColumn;
        private FormBatchTools.MyDataGridViewButtonColumn showBatchCharacterColumn;
        private FormBatchTools.MyDataGridViewButtonColumn diffBatchCharacterColumn;
        private System.Windows.Forms.BindingSource batchToolsBindingSource;
    }
}