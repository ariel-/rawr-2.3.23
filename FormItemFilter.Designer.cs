using System.Windows.Forms;

namespace Rawr
{
    partial class FormItemFilter
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
            this.OKButton = new System.Windows.Forms.Button();
            this.cancelButton = new System.Windows.Forms.Button();
            this.bindingSourceItemFilter = new System.Windows.Forms.BindingSource(this.components);
            this.labelName = new System.Windows.Forms.Label();
            this.textBoxName = new System.Windows.Forms.TextBox();
            this.textBoxPattern = new System.Windows.Forms.TextBox();
            this.labelPattern = new System.Windows.Forms.Label();
            this.textBoxMinItemLevel = new System.Windows.Forms.TextBox();
            this.label1 = new System.Windows.Forms.Label();
            this.textBoxMaxItemLevel = new System.Windows.Forms.TextBox();
            this.label3 = new System.Windows.Forms.Label();
            this.comboBoxMinItemQuality = new System.Windows.Forms.ComboBox();
            this.comboBoxMaxItemQuality = new System.Windows.Forms.ComboBox();
            this.label4 = new System.Windows.Forms.Label();
            this.label5 = new System.Windows.Forms.Label();
            this.labelBind = new System.Windows.Forms.Label();
            this.checkBoA = new System.Windows.Forms.CheckBox();
            this.checkBoE = new System.Windows.Forms.CheckBox();
            this.checkBoP = new System.Windows.Forms.CheckBox();
            this.checkBoU = new System.Windows.Forms.CheckBox();
            this.checkBoN = new System.Windows.Forms.CheckBox();
            this.checkBoxAdditiveFilter = new System.Windows.Forms.CheckBox();
            this.checkBoxAppliesToItems = new System.Windows.Forms.CheckBox();
            this.label6 = new System.Windows.Forms.Label();
            this.checkBoxAppliesToGems = new System.Windows.Forms.CheckBox();
            this.label7 = new System.Windows.Forms.Label();
            this.buttonNewFilter = new System.Windows.Forms.Button();
            this.buttonNewSubfilter = new System.Windows.Forms.Button();
            this.buttonUp = new System.Windows.Forms.Button();
            this.buttonDown = new System.Windows.Forms.Button();
            this.buttonDelete = new System.Windows.Forms.Button();
            this.LayoutPanel = new System.Windows.Forms.TableLayoutPanel();
            this.label2 = new System.Windows.Forms.Label();
            this.itemFilterTreeView = new Rawr.ItemFilterTreeView();
            this.panel1 = new System.Windows.Forms.Panel();
            ((System.ComponentModel.ISupportInitialize)(this.bindingSourceItemFilter)).BeginInit();
            this.LayoutPanel.SuspendLayout();
            this.panel1.SuspendLayout();
            this.SuspendLayout();
            // 
            // OKButton
            // 
            this.OKButton.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.OKButton.DialogResult = System.Windows.Forms.DialogResult.OK;
            this.OKButton.Location = new System.Drawing.Point(213, 484);
            this.OKButton.Name = "OKButton";
            this.OKButton.Size = new System.Drawing.Size(75, 25);
            this.OKButton.TabIndex = 0;
            this.OKButton.Text = "OK";
            this.OKButton.UseVisualStyleBackColor = true;
            // 
            // cancelButton
            // 
            this.cancelButton.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.cancelButton.DialogResult = System.Windows.Forms.DialogResult.Cancel;
            this.cancelButton.Location = new System.Drawing.Point(132, 484);
            this.cancelButton.Name = "cancelButton";
            this.cancelButton.Size = new System.Drawing.Size(75, 25);
            this.cancelButton.TabIndex = 1;
            this.cancelButton.Text = "Cancel";
            this.cancelButton.UseVisualStyleBackColor = true;
            // 
            // bindingSourceItemFilter
            // 
            this.bindingSourceItemFilter.DataSource = typeof(Rawr.ItemFilterRegex);
            this.bindingSourceItemFilter.CurrentItemChanged += new System.EventHandler(this.bindingSourceItemFilter_CurrentItemChanged);
            // 
            // labelName
            // 
            this.labelName.AutoSize = true;
            this.labelName.Dock = System.Windows.Forms.DockStyle.Fill;
            this.labelName.Location = new System.Drawing.Point(3, 183);
            this.labelName.Name = "labelName";
            this.labelName.Size = new System.Drawing.Size(59, 24);
            this.labelName.TabIndex = 2;
            this.labelName.Text = "Name:";
            this.labelName.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            // 
            // textBoxName
            // 
            this.LayoutPanel.SetColumnSpan(this.textBoxName, 3);
            this.textBoxName.DataBindings.Add(new System.Windows.Forms.Binding("Text", this.bindingSourceItemFilter, "Name", true));
            this.textBoxName.Dock = System.Windows.Forms.DockStyle.Fill;
            this.textBoxName.Location = new System.Drawing.Point(66, 184);
            this.textBoxName.Margin = new System.Windows.Forms.Padding(1);
            this.textBoxName.Name = "textBoxName";
            this.textBoxName.Size = new System.Drawing.Size(208, 20);
            this.textBoxName.TabIndex = 3;
            // 
            // textBoxPattern
            // 
            this.LayoutPanel.SetColumnSpan(this.textBoxPattern, 3);
            this.textBoxPattern.DataBindings.Add(new System.Windows.Forms.Binding("Text", this.bindingSourceItemFilter, "Pattern", true));
            this.textBoxPattern.Dock = System.Windows.Forms.DockStyle.Fill;
            this.textBoxPattern.Location = new System.Drawing.Point(66, 208);
            this.textBoxPattern.Margin = new System.Windows.Forms.Padding(1);
            this.textBoxPattern.Multiline = true;
            this.textBoxPattern.Name = "textBoxPattern";
            this.textBoxPattern.Size = new System.Drawing.Size(208, 60);
            this.textBoxPattern.TabIndex = 5;
            // 
            // labelPattern
            // 
            this.labelPattern.AutoSize = true;
            this.labelPattern.Dock = System.Windows.Forms.DockStyle.Fill;
            this.labelPattern.Location = new System.Drawing.Point(3, 207);
            this.labelPattern.Name = "labelPattern";
            this.labelPattern.Size = new System.Drawing.Size(59, 62);
            this.labelPattern.TabIndex = 4;
            this.labelPattern.Text = "Pattern:";
            this.labelPattern.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            // 
            // textBoxMinItemLevel
            // 
            this.textBoxMinItemLevel.DataBindings.Add(new System.Windows.Forms.Binding("Text", this.bindingSourceItemFilter, "MinItemLevel", true));
            this.textBoxMinItemLevel.Dock = System.Windows.Forms.DockStyle.Fill;
            this.textBoxMinItemLevel.Location = new System.Drawing.Point(66, 270);
            this.textBoxMinItemLevel.Margin = new System.Windows.Forms.Padding(1);
            this.textBoxMinItemLevel.Name = "textBoxMinItemLevel";
            this.textBoxMinItemLevel.Size = new System.Drawing.Size(63, 20);
            this.textBoxMinItemLevel.TabIndex = 7;
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.label1.Location = new System.Drawing.Point(1, 270);
            this.label1.Margin = new System.Windows.Forms.Padding(1);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(63, 24);
            this.label1.TabIndex = 6;
            this.label1.Text = "Item Level:";
            this.label1.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            // 
            // textBoxMaxItemLevel
            // 
            this.textBoxMaxItemLevel.DataBindings.Add(new System.Windows.Forms.Binding("Text", this.bindingSourceItemFilter, "MaxItemLevel", true));
            this.textBoxMaxItemLevel.Dock = System.Windows.Forms.DockStyle.Fill;
            this.textBoxMaxItemLevel.Location = new System.Drawing.Point(196, 270);
            this.textBoxMaxItemLevel.Margin = new System.Windows.Forms.Padding(1);
            this.textBoxMaxItemLevel.Name = "textBoxMaxItemLevel";
            this.textBoxMaxItemLevel.Size = new System.Drawing.Size(78, 20);
            this.textBoxMaxItemLevel.TabIndex = 9;
            // 
            // label3
            // 
            this.label3.AutoSize = true;
            this.LayoutPanel.SetColumnSpan(this.label3, 2);
            this.label3.Dock = System.Windows.Forms.DockStyle.Fill;
            this.label3.Location = new System.Drawing.Point(1, 296);
            this.label3.Margin = new System.Windows.Forms.Padding(1);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(128, 24);
            this.label3.TabIndex = 10;
            this.label3.Text = "Min Item Quality:";
            this.label3.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            // 
            // comboBoxMinItemQuality
            // 
            this.LayoutPanel.SetColumnSpan(this.comboBoxMinItemQuality, 2);
            this.comboBoxMinItemQuality.DataBindings.Add(new System.Windows.Forms.Binding("SelectedItem", this.bindingSourceItemFilter, "MinItemQuality", true));
            this.comboBoxMinItemQuality.Dock = System.Windows.Forms.DockStyle.Fill;
            this.comboBoxMinItemQuality.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.comboBoxMinItemQuality.FormattingEnabled = true;
            this.comboBoxMinItemQuality.Location = new System.Drawing.Point(131, 296);
            this.comboBoxMinItemQuality.Margin = new System.Windows.Forms.Padding(1);
            this.comboBoxMinItemQuality.Name = "comboBoxMinItemQuality";
            this.comboBoxMinItemQuality.Size = new System.Drawing.Size(143, 21);
            this.comboBoxMinItemQuality.TabIndex = 11;
            // 
            // comboBoxMaxItemQuality
            // 
            this.LayoutPanel.SetColumnSpan(this.comboBoxMaxItemQuality, 2);
            this.comboBoxMaxItemQuality.DataBindings.Add(new System.Windows.Forms.Binding("SelectedItem", this.bindingSourceItemFilter, "MaxItemQuality", true));
            this.comboBoxMaxItemQuality.Dock = System.Windows.Forms.DockStyle.Fill;
            this.comboBoxMaxItemQuality.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.comboBoxMaxItemQuality.FormattingEnabled = true;
            this.comboBoxMaxItemQuality.Location = new System.Drawing.Point(131, 322);
            this.comboBoxMaxItemQuality.Margin = new System.Windows.Forms.Padding(1);
            this.comboBoxMaxItemQuality.Name = "comboBoxMaxItemQuality";
            this.comboBoxMaxItemQuality.Size = new System.Drawing.Size(143, 21);
            this.comboBoxMaxItemQuality.TabIndex = 13;
            // 
            // label4
            // 
            this.label4.AutoSize = true;
            this.LayoutPanel.SetColumnSpan(this.label4, 2);
            this.label4.Dock = System.Windows.Forms.DockStyle.Fill;
            this.label4.Location = new System.Drawing.Point(1, 322);
            this.label4.Margin = new System.Windows.Forms.Padding(1);
            this.label4.Name = "label4";
            this.label4.Size = new System.Drawing.Size(128, 24);
            this.label4.TabIndex = 12;
            this.label4.Text = "Max Item Quality:";
            this.label4.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            // 
            // label5
            // 
            this.label5.AutoSize = true;
            this.LayoutPanel.SetColumnSpan(this.label5, 2);
            this.label5.Dock = System.Windows.Forms.DockStyle.Fill;
            this.label5.Location = new System.Drawing.Point(1, 400);
            this.label5.Margin = new System.Windows.Forms.Padding(1);
            this.label5.Name = "label5";
            this.label5.Size = new System.Drawing.Size(128, 24);
            this.label5.TabIndex = 20;
            this.label5.Text = "Additive Filter:";
            this.label5.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            // 
            // labelBind
            // 
            this.labelBind.AutoSize = true;
            this.LayoutPanel.SetColumnSpan(this.labelBind, 2);
            this.labelBind.Dock = System.Windows.Forms.DockStyle.Fill;
            this.labelBind.Location = new System.Drawing.Point(1, 348);
            this.labelBind.Margin = new System.Windows.Forms.Padding(1);
            this.labelBind.Name = "labelBind";
            this.labelBind.Size = new System.Drawing.Size(128, 24);
            this.labelBind.TabIndex = 14;
            this.labelBind.Text = "Item bind type:";
            this.labelBind.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            // 
            // checkBoA
            // 
            this.checkBoA.AutoSize = true;
            this.checkBoA.DataBindings.Add(new System.Windows.Forms.Binding("Checked", this.bindingSourceItemFilter, "BoA", true));
            this.checkBoA.Location = new System.Drawing.Point(198, 376);
            this.checkBoA.Name = "checkBoA";
            this.checkBoA.Size = new System.Drawing.Size(66, 17);
            this.checkBoA.TabIndex = 19;
            this.checkBoA.Text = "Account";
            // 
            // checkBoE
            // 
            this.checkBoE.AutoSize = true;
            this.checkBoE.DataBindings.Add(new System.Windows.Forms.Binding("Checked", this.bindingSourceItemFilter, "BoE", true));
            this.checkBoE.Location = new System.Drawing.Point(133, 350);
            this.checkBoE.Name = "checkBoE";
            this.checkBoE.Size = new System.Drawing.Size(53, 17);
            this.checkBoE.TabIndex = 16;
            this.checkBoE.Text = "Equip";
            // 
            // checkBoP
            // 
            this.checkBoP.AutoSize = true;
            this.checkBoP.DataBindings.Add(new System.Windows.Forms.Binding("Checked", this.bindingSourceItemFilter, "BoP", true));
            this.checkBoP.Location = new System.Drawing.Point(198, 350);
            this.checkBoP.Name = "checkBoP";
            this.checkBoP.Size = new System.Drawing.Size(47, 17);
            this.checkBoP.TabIndex = 17;
            this.checkBoP.Text = "Pick";
            // 
            // checkBoU
            // 
            this.checkBoU.AutoSize = true;
            this.checkBoU.DataBindings.Add(new System.Windows.Forms.Binding("Checked", this.bindingSourceItemFilter, "BoU", true));
            this.checkBoU.Location = new System.Drawing.Point(133, 376);
            this.checkBoU.Name = "checkBoU";
            this.checkBoU.Size = new System.Drawing.Size(45, 17);
            this.checkBoU.TabIndex = 18;
            this.checkBoU.Text = "Use";
            // 
            // checkBoN
            // 
            this.checkBoN.AutoSize = true;
            this.checkBoN.DataBindings.Add(new System.Windows.Forms.Binding("Checked", this.bindingSourceItemFilter, "BoN", true));
            this.checkBoN.Location = new System.Drawing.Point(68, 376);
            this.checkBoN.Name = "checkBoN";
            this.checkBoN.Size = new System.Drawing.Size(46, 17);
            this.checkBoN.TabIndex = 15;
            this.checkBoN.Text = "N/A";
            // 
            // checkBoxAdditiveFilter
            // 
            this.checkBoxAdditiveFilter.AutoSize = true;
            this.checkBoxAdditiveFilter.DataBindings.Add(new System.Windows.Forms.Binding("Checked", this.bindingSourceItemFilter, "AdditiveFilter", true));
            this.checkBoxAdditiveFilter.Dock = System.Windows.Forms.DockStyle.Fill;
            this.checkBoxAdditiveFilter.Location = new System.Drawing.Point(131, 400);
            this.checkBoxAdditiveFilter.Margin = new System.Windows.Forms.Padding(1);
            this.checkBoxAdditiveFilter.Name = "checkBoxAdditiveFilter";
            this.checkBoxAdditiveFilter.Size = new System.Drawing.Size(63, 24);
            this.checkBoxAdditiveFilter.TabIndex = 21;
            this.checkBoxAdditiveFilter.UseVisualStyleBackColor = true;
            // 
            // checkBoxAppliesToItems
            // 
            this.checkBoxAppliesToItems.AutoSize = true;
            this.checkBoxAppliesToItems.DataBindings.Add(new System.Windows.Forms.Binding("Checked", this.bindingSourceItemFilter, "AppliesToItems", true));
            this.checkBoxAppliesToItems.Dock = System.Windows.Forms.DockStyle.Fill;
            this.checkBoxAppliesToItems.Location = new System.Drawing.Point(131, 426);
            this.checkBoxAppliesToItems.Margin = new System.Windows.Forms.Padding(1);
            this.checkBoxAppliesToItems.Name = "checkBoxAppliesToItems";
            this.checkBoxAppliesToItems.Size = new System.Drawing.Size(63, 18);
            this.checkBoxAppliesToItems.TabIndex = 23;
            this.checkBoxAppliesToItems.UseVisualStyleBackColor = true;
            // 
            // label6
            // 
            this.label6.AutoSize = true;
            this.LayoutPanel.SetColumnSpan(this.label6, 2);
            this.label6.Dock = System.Windows.Forms.DockStyle.Fill;
            this.label6.Location = new System.Drawing.Point(1, 426);
            this.label6.Margin = new System.Windows.Forms.Padding(1);
            this.label6.Name = "label6";
            this.label6.Size = new System.Drawing.Size(128, 18);
            this.label6.TabIndex = 22;
            this.label6.Text = "Applies to Items:";
            this.label6.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            // 
            // checkBoxAppliesToGems
            // 
            this.checkBoxAppliesToGems.AutoSize = true;
            this.checkBoxAppliesToGems.DataBindings.Add(new System.Windows.Forms.Binding("Checked", this.bindingSourceItemFilter, "AppliesToGems", true));
            this.checkBoxAppliesToGems.Dock = System.Windows.Forms.DockStyle.Fill;
            this.checkBoxAppliesToGems.Location = new System.Drawing.Point(131, 446);
            this.checkBoxAppliesToGems.Margin = new System.Windows.Forms.Padding(1);
            this.checkBoxAppliesToGems.Name = "checkBoxAppliesToGems";
            this.checkBoxAppliesToGems.Size = new System.Drawing.Size(63, 19);
            this.checkBoxAppliesToGems.TabIndex = 25;
            this.checkBoxAppliesToGems.UseVisualStyleBackColor = true;
            // 
            // label7
            // 
            this.label7.AutoSize = true;
            this.LayoutPanel.SetColumnSpan(this.label7, 2);
            this.label7.Dock = System.Windows.Forms.DockStyle.Fill;
            this.label7.Location = new System.Drawing.Point(1, 446);
            this.label7.Margin = new System.Windows.Forms.Padding(1);
            this.label7.Name = "label7";
            this.label7.Size = new System.Drawing.Size(128, 19);
            this.label7.TabIndex = 24;
            this.label7.Text = "Applies to Gems:";
            this.label7.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            // 
            // buttonNewFilter
            // 
            this.buttonNewFilter.Location = new System.Drawing.Point(2, 2);
            this.buttonNewFilter.Name = "buttonNewFilter";
            this.buttonNewFilter.Size = new System.Drawing.Size(63, 23);
            this.buttonNewFilter.TabIndex = 0;
            this.buttonNewFilter.Text = "New Filter";
            this.buttonNewFilter.UseVisualStyleBackColor = true;
            this.buttonNewFilter.Click += new System.EventHandler(this.buttonNewFilter_Click);
            // 
            // buttonNewSubfilter
            // 
            this.buttonNewSubfilter.Enabled = false;
            this.buttonNewSubfilter.Location = new System.Drawing.Point(66, 2);
            this.buttonNewSubfilter.Name = "buttonNewSubfilter";
            this.buttonNewSubfilter.Size = new System.Drawing.Size(80, 23);
            this.buttonNewSubfilter.TabIndex = 1;
            this.buttonNewSubfilter.Text = "New Subfilter";
            this.buttonNewSubfilter.UseVisualStyleBackColor = true;
            this.buttonNewSubfilter.Click += new System.EventHandler(this.buttonNewSubfilter_Click);
            // 
            // buttonUp
            // 
            this.buttonUp.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.buttonUp.Enabled = false;
            this.buttonUp.Font = new System.Drawing.Font("Wingdings", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(2)));
            this.buttonUp.Location = new System.Drawing.Point(202, 2);
            this.buttonUp.Name = "buttonUp";
            this.buttonUp.Size = new System.Drawing.Size(23, 23);
            this.buttonUp.TabIndex = 2;
            this.buttonUp.Text = "á";
            this.buttonUp.UseVisualStyleBackColor = true;
            this.buttonUp.Click += new System.EventHandler(this.buttonUp_Click);
            // 
            // buttonDown
            // 
            this.buttonDown.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.buttonDown.Enabled = false;
            this.buttonDown.Font = new System.Drawing.Font("Wingdings", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(2)));
            this.buttonDown.Location = new System.Drawing.Point(225, 2);
            this.buttonDown.Name = "buttonDown";
            this.buttonDown.Size = new System.Drawing.Size(23, 23);
            this.buttonDown.TabIndex = 3;
            this.buttonDown.Text = "â";
            this.buttonDown.UseVisualStyleBackColor = true;
            this.buttonDown.Click += new System.EventHandler(this.buttonDown_Click);
            // 
            // buttonDelete
            // 
            this.buttonDelete.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.buttonDelete.Enabled = false;
            this.buttonDelete.Location = new System.Drawing.Point(248, 2);
            this.buttonDelete.Name = "buttonDelete";
            this.buttonDelete.Size = new System.Drawing.Size(23, 23);
            this.buttonDelete.TabIndex = 4;
            this.buttonDelete.Text = "X";
            this.buttonDelete.UseVisualStyleBackColor = true;
            this.buttonDelete.Click += new System.EventHandler(this.buttonDelete_Click);
            // 
            // LayoutPanel
            // 
            this.LayoutPanel.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom)
                        | System.Windows.Forms.AnchorStyles.Left)
                        | System.Windows.Forms.AnchorStyles.Right)));
            this.LayoutPanel.ColumnCount = 4;
            this.LayoutPanel.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Absolute, 65F));
            this.LayoutPanel.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Absolute, 65F));
            this.LayoutPanel.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Absolute, 65F));
            this.LayoutPanel.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Absolute, 80F));
            this.LayoutPanel.Controls.Add(this.label2, 2, 4);
            this.LayoutPanel.Controls.Add(this.itemFilterTreeView, 0, 1);
            this.LayoutPanel.Controls.Add(this.textBoxName, 1, 2);
            this.LayoutPanel.Controls.Add(this.labelName, 0, 2);
            this.LayoutPanel.Controls.Add(this.labelPattern, 0, 3);
            this.LayoutPanel.Controls.Add(this.textBoxPattern, 1, 3);
            this.LayoutPanel.Controls.Add(this.label1, 0, 4);
            this.LayoutPanel.Controls.Add(this.label7, 0, 11);
            this.LayoutPanel.Controls.Add(this.label6, 0, 10);
            this.LayoutPanel.Controls.Add(this.label3, 0, 5);
            this.LayoutPanel.Controls.Add(this.label5, 0, 9);
            this.LayoutPanel.Controls.Add(this.label4, 0, 6);
            this.LayoutPanel.Controls.Add(this.labelBind, 0, 7);
            this.LayoutPanel.Controls.Add(this.checkBoE, 2, 7);
            this.LayoutPanel.Controls.Add(this.checkBoP, 3, 7);
            this.LayoutPanel.Controls.Add(this.checkBoN, 1, 8);
            this.LayoutPanel.Controls.Add(this.checkBoU, 2, 8);
            this.LayoutPanel.Controls.Add(this.checkBoA, 3, 8);
            this.LayoutPanel.Controls.Add(this.textBoxMinItemLevel, 1, 4);
            this.LayoutPanel.Controls.Add(this.comboBoxMinItemQuality, 2, 5);
            this.LayoutPanel.Controls.Add(this.comboBoxMaxItemQuality, 2, 6);
            this.LayoutPanel.Controls.Add(this.checkBoxAdditiveFilter, 2, 9);
            this.LayoutPanel.Controls.Add(this.checkBoxAppliesToItems, 2, 10);
            this.LayoutPanel.Controls.Add(this.checkBoxAppliesToGems, 2, 11);
            this.LayoutPanel.Controls.Add(this.panel1, 0, 0);
            this.LayoutPanel.Controls.Add(this.textBoxMaxItemLevel, 3, 4);
            this.LayoutPanel.Location = new System.Drawing.Point(12, 12);
            this.LayoutPanel.Name = "LayoutPanel";
            this.LayoutPanel.RowCount = 10;
            this.LayoutPanel.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 26F));
            this.LayoutPanel.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 71.42857F));
            this.LayoutPanel.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 24F));
            this.LayoutPanel.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 28.57143F));
            this.LayoutPanel.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 26F));
            this.LayoutPanel.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 26F));
            this.LayoutPanel.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 26F));
            this.LayoutPanel.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 26F));
            this.LayoutPanel.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 26F));
            this.LayoutPanel.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 26F));
            this.LayoutPanel.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 20F));
            this.LayoutPanel.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 20F));
            this.LayoutPanel.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 20F));
            this.LayoutPanel.Size = new System.Drawing.Size(275, 466);
            this.LayoutPanel.TabIndex = 0;
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Dock = System.Windows.Forms.DockStyle.Fill;
            this.label2.Location = new System.Drawing.Point(131, 270);
            this.label2.Margin = new System.Windows.Forms.Padding(1);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(63, 24);
            this.label2.TabIndex = 8;
            this.label2.Text = "to";
            this.label2.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // itemFilterTreeView
            // 
            this.LayoutPanel.SetColumnSpan(this.itemFilterTreeView, 4);
            this.itemFilterTreeView.Dock = System.Windows.Forms.DockStyle.Fill;
            this.itemFilterTreeView.EditMode = true;
            this.itemFilterTreeView.HideSelection = false;
            this.itemFilterTreeView.Location = new System.Drawing.Point(1, 27);
            this.itemFilterTreeView.Margin = new System.Windows.Forms.Padding(1);
            this.itemFilterTreeView.Name = "itemFilterTreeView";
            this.itemFilterTreeView.Size = new System.Drawing.Size(273, 155);
            this.itemFilterTreeView.TabIndex = 1;
            this.itemFilterTreeView.AfterSelect += new System.Windows.Forms.TreeViewEventHandler(this.itemFilterTreeView_AfterSelect);
            // 
            // panel1
            // 
            this.LayoutPanel.SetColumnSpan(this.panel1, 4);
            this.panel1.Controls.Add(this.buttonNewFilter);
            this.panel1.Controls.Add(this.buttonDelete);
            this.panel1.Controls.Add(this.buttonNewSubfilter);
            this.panel1.Controls.Add(this.buttonDown);
            this.panel1.Controls.Add(this.buttonUp);
            this.panel1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.panel1.Location = new System.Drawing.Point(0, 0);
            this.panel1.Margin = new System.Windows.Forms.Padding(0);
            this.panel1.Name = "panel1";
            this.panel1.Size = new System.Drawing.Size(275, 26);
            this.panel1.TabIndex = 0;
            // 
            // FormItemFilter
            // 
            this.AcceptButton = this.OKButton;
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.CancelButton = this.cancelButton;
            this.ClientSize = new System.Drawing.Size(308, 531);
            this.Controls.Add(this.LayoutPanel);
            this.Controls.Add(this.OKButton);
            this.Controls.Add(this.cancelButton);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.SizableToolWindow;
            this.MinimumSize = new System.Drawing.Size(316, 555);
            this.Name = "FormItemFilter";
            this.ShowIcon = false;
            this.ShowInTaskbar = false;
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterParent;
            this.Text = "Item Filter Editor...";
            ((System.ComponentModel.ISupportInitialize)(this.bindingSourceItemFilter)).EndInit();
            this.LayoutPanel.ResumeLayout(false);
            this.LayoutPanel.PerformLayout();
            this.panel1.ResumeLayout(false);
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.Button OKButton;
        private System.Windows.Forms.Button cancelButton;
        private System.Windows.Forms.Label labelName;
        private System.Windows.Forms.TextBox textBoxName;
        private System.Windows.Forms.TextBox textBoxPattern;
        private System.Windows.Forms.Label labelPattern;
        public System.Windows.Forms.BindingSource bindingSourceItemFilter;
        private System.Windows.Forms.TextBox textBoxMinItemLevel;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.TextBox textBoxMaxItemLevel;
        private System.Windows.Forms.Label label3;
        private System.Windows.Forms.ComboBox comboBoxMinItemQuality;
        private System.Windows.Forms.ComboBox comboBoxMaxItemQuality;
        private System.Windows.Forms.Label label4;
        private System.Windows.Forms.Label label5;
        private System.Windows.Forms.Label labelBind;
        private System.Windows.Forms.CheckBox checkBoA;
        private System.Windows.Forms.CheckBox checkBoE;
        private System.Windows.Forms.CheckBox checkBoP;
        private System.Windows.Forms.CheckBox checkBoU;
        private System.Windows.Forms.CheckBox checkBoN;
        private System.Windows.Forms.CheckBox checkBoxAdditiveFilter;
        private System.Windows.Forms.CheckBox checkBoxAppliesToItems;
        private System.Windows.Forms.Label label6;
        private System.Windows.Forms.CheckBox checkBoxAppliesToGems;
        private System.Windows.Forms.Label label7;
        internal ItemFilterTreeView itemFilterTreeView;
        private System.Windows.Forms.Button buttonNewFilter;
        private System.Windows.Forms.Button buttonNewSubfilter;
        private System.Windows.Forms.Button buttonUp;
        private System.Windows.Forms.Button buttonDown;
        private System.Windows.Forms.Button buttonDelete;
        private System.Windows.Forms.TableLayoutPanel LayoutPanel;
        private System.Windows.Forms.Panel panel1;
        private System.Windows.Forms.Label label2;
    }
}