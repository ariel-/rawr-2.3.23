using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;
using System.Xml.Serialization;
using System.IO;
using Rawr.Optimizer;

namespace Rawr
{
    public partial class FormBatchTools : Form
    {
        internal BatchTools batchTools;
        private bool _unsavedChanges;
        private string _filePath;

        FormMain formMain;

        public FormBatchTools(FormMain formMain)
        {
            InitializeComponent();

            Rectangle bounds = ConfigBounds;
            if (bounds.Width >= this.MinimumSize.Width && bounds.Height >= this.MinimumSize.Height)
            {
                this.StartPosition = FormStartPosition.Manual;
                this.Bounds = bounds;
            }

            batchTools = new BatchTools();
            batchTools.OverrideRegem = Properties.Optimizer.Default.OverrideRegem;
            batchTools.OverrideReenchant = Properties.Optimizer.Default.OverrideReenchant;
            batchTools.Thoroughness = Properties.Optimizer.Default.Thoroughness;
            batchTools.GreedyOptimizationMethod = Properties.Optimizer.Default.GreedyOptimizationMethod;
            batchTools.OptimizationMethod = Properties.Optimizer.Default.OptimizationMethod;
            batchTools.TemplateGemsEnabled = Properties.Optimizer.Default.TemplateGemsEnabled;

            batchTools.OperationCompleted += new EventHandler(batchTools_OperationCompleted);
            batchTools.StatusUpdated += new EventHandler(batchTools_StatusUpdated);
            batchTools.UpgradeListCompleted += new EventHandler(batchTools_UpgradeListCompleted);

            this.formMain = formMain;
            batchToolsBindingSource.DataSource = batchTools;
            batchCharacterListBindingSource.DataSource = batchTools.BatchCharacterList;

            TableLayoutSettings layout = (TableLayoutSettings)statusStrip1.LayoutSettings;
            layout.RowCount = 1;
            layout.ColumnCount = 2;
            layout.ColumnStyles.Clear();
            layout.ColumnStyles.Add(new ColumnStyle(SizeType.AutoSize));
            layout.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 100f));
            layout.RowStyles.Clear();
            layout.RowStyles.Add(new RowStyle(SizeType.Absolute, statusStrip1.Height));
            statusProgressBar.Dock = DockStyle.Fill;
        }

        public Rectangle ConfigBounds
        {
            get
            {
                return new Rectangle(Properties.Recent.Default.BatchToolsWindowLocation,
                    Properties.Recent.Default.BatchToolsWindowSize);
            }
            set
            {
                Properties.Recent.Default.BatchToolsWindowLocation = value.Location;
                Properties.Recent.Default.BatchToolsWindowSize = value.Size;
            }
        }

        void batchTools_UpgradeListCompleted(object sender, EventArgs e)
        {
            FormUpgradeComparison.Instance.LoadData(batchTools.Upgrades, batchTools.CustomSubpoints);
            FormUpgradeComparison.Instance.Show();
            FormUpgradeComparison.Instance.BringToFront();
        }

        void batchTools_StatusUpdated(object sender, EventArgs e)
        {
            if (InvokeRequired)
            {
                Invoke((MethodInvoker)delegate { batchTools_StatusUpdated(sender, e); });
            }
            else
            {
                statusLabel.Text = batchTools.Status;
                statusProgressBar.Value = batchTools.Progress;
            }
        }

        void batchTools_OperationCompleted(object sender, EventArgs e)
        {
            buttonCancel.Enabled = false;
        }

        private void FormBatchTools_FormClosing(object sender, FormClosingEventArgs e)
        {
            e.Cancel = batchTools.IsBusy || !PromptToSaveBeforeClosing();
            ConfigBounds = this.Bounds;
            Properties.Recent.Default.Save();
        }

        private void FormBatchTools_FormClosed(object sender, FormClosedEventArgs e)
        {
            formMain.UnloadBatchCharacter();

            Properties.Optimizer.Default.OverrideRegem = checkBoxOverrideRegem.Checked;
            Properties.Optimizer.Default.OverrideReenchant = checkBoxOverrideReenchant.Checked;
            Properties.Optimizer.Default.Thoroughness = trackBarThoroughness.Value;
            Properties.Optimizer.Default.Save();
        }

        private bool PromptToSaveBeforeClosing()
        {
            if (_unsavedChanges)
            {
                DialogResult result = MessageBox.Show("Would you like to save the current batch list before closing it?", "Rawr - Save?", MessageBoxButtons.YesNoCancel);
                switch (result)
                {
                    case DialogResult.Yes:
                        saveToolStripMenuItem_Click(null, null);
                        return !string.IsNullOrEmpty(_filePath);
                    case DialogResult.No:
                        return true;
                    default:
                        return false;
                }
            }
            else
                return true;
        }

        private void newToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (PromptToSaveBeforeClosing())
            {
                _unsavedChanges = false;
                _filePath = null;
                batchTools.BatchCharacterList = new BatchCharacterList();
                batchCharacterListBindingSource.DataSource = batchTools.BatchCharacterList;
            }
        }

        private void importToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (PromptToSaveBeforeClosing())
            {
                OpenFileDialog dialog = new OpenFileDialog();
                dialog.DefaultExt = ".xml";
                dialog.Filter = "Rawr Xml Character Files | *.xml";
                dialog.Multiselect = true;
                if (dialog.ShowDialog(this) == DialogResult.OK)
                {
                    _unsavedChanges = true;
                    _filePath = null;
                    BatchCharacterList list = new BatchCharacterList();
                    foreach (string filename in dialog.FileNames)
                    {
                        list.Add(new BatchCharacter() { RelativePath = RelativePath(filename, AppDomain.CurrentDomain.BaseDirectory) });
                    }
                    batchTools.BatchCharacterList = list;
                    batchCharacterListBindingSource.DataSource = batchTools.BatchCharacterList;
                }
                dialog.Dispose();
            }
        }

        private void openToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (PromptToSaveBeforeClosing())
            {
                OpenFileDialog dialog = new OpenFileDialog();
                dialog.DefaultExt = ".xml";
                dialog.Filter = "Rawr Batch Files | *.xml";
                dialog.Multiselect = false;
                if (dialog.ShowDialog(this) == DialogResult.OK)
                {
                    _filePath = dialog.FileName;
                    batchTools.BatchCharacterList = BatchCharacterList.Load(_filePath);
                    batchCharacterListBindingSource.DataSource = batchTools.BatchCharacterList;
                    FormMain.Instance.AddRecentCharacter(_filePath);
                }
                dialog.Dispose();
            }
        }

        private void saveToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (!string.IsNullOrEmpty(_filePath))
            {
                ((BatchCharacterList)batchCharacterListBindingSource.DataSource).Save(_filePath);
                FormMain.Instance.AddRecentCharacter(_filePath);
                _unsavedChanges = false;
            }
            else
            {
                saveAsToolStripMenuItem_Click(null, null);
            }
        }

        private void saveAsToolStripMenuItem_Click(object sender, EventArgs e)
        {
            SaveFileDialog dialog = new SaveFileDialog();
            dialog.DefaultExt = ".xml";
            dialog.Filter = "Rawr Batch Files | *.xml";
            if (dialog.ShowDialog(this) == DialogResult.OK)
            {
                _filePath = dialog.FileName;
                ((BatchCharacterList)batchCharacterListBindingSource.DataSource).Save(_filePath);
                FormMain.Instance.AddRecentCharacter(_filePath);
                _unsavedChanges = false;
            }
            dialog.Dispose();
        }

        private void exitToolStripMenuItem_Click(object sender, EventArgs e)
        {
            Close();
        }

        #region Custom Controls
        private class MyStatusStrip : StatusStrip
        {
            // disable spring behavior to manually control the grid
            protected override void OnSpringTableLayoutCore()
            {
            }
        }

        private class MyDataGridViewButtonColumn : DataGridViewButtonColumn
        {
            public MyDataGridViewButtonColumn()
            {
                CellTemplate = new MyDataGridViewButtonCell();
                NewRowButtonVisible = false;
            }

            public override object Clone()
            {
                MyDataGridViewButtonColumn column = (MyDataGridViewButtonColumn)base.Clone();
                column.NewRowButtonVisible = this.NewRowButtonVisible;
                return column;
            }

            [DefaultValue(false)]
            public bool NewRowButtonVisible { get; set; }
        }

        private class MyDataGridViewButtonCell : DataGridViewButtonCell
        {
            public MyDataGridViewButtonCell()
            {
                ButtonVisible = true;
            }

            public bool ButtonVisible { get; set; }

            public override object Clone()
            {
                MyDataGridViewButtonCell cell = (MyDataGridViewButtonCell)base.Clone();
                cell.ButtonVisible = this.ButtonVisible;
                return cell;
            }

            protected override void Paint(Graphics graphics, Rectangle clipBounds, Rectangle cellBounds, int rowIndex, DataGridViewElementStates elementState, object value, object formattedValue, string errorText, DataGridViewCellStyle cellStyle, DataGridViewAdvancedBorderStyle advancedBorderStyle, DataGridViewPaintParts paintParts)
            {
                MyDataGridViewButtonColumn owner = (MyDataGridViewButtonColumn)OwningColumn;
                if (ButtonVisible && (owner.NewRowButtonVisible || rowIndex != DataGridView.NewRowIndex))
                {
                    if (UseColumnTextForButtonValue)
                    {
                        base.Paint(graphics, clipBounds, cellBounds, rowIndex, elementState, value, owner.Text, errorText, cellStyle, advancedBorderStyle, paintParts);
                    }
                    else
                    {
                        base.Paint(graphics, clipBounds, cellBounds, rowIndex, elementState, value, formattedValue, errorText, cellStyle, advancedBorderStyle, paintParts);
                    }
                }
                else
                {
                    if ((paintParts & DataGridViewPaintParts.Background) == DataGridViewPaintParts.Background)
                    {
                        SolidBrush brush = new SolidBrush(((paintParts & DataGridViewPaintParts.SelectionBackground) == DataGridViewPaintParts.SelectionBackground && (elementState & DataGridViewElementStates.Selected) == DataGridViewElementStates.Selected) ? cellStyle.SelectionBackColor : cellStyle.BackColor);
                        graphics.FillRectangle(brush, cellBounds);
                        brush.Dispose();
                    }

                    if ((paintParts & DataGridViewPaintParts.Border) == DataGridViewPaintParts.Border)
                    {
                        PaintBorder(graphics, clipBounds, cellBounds, cellStyle, advancedBorderStyle);
                    }
                }
            }
        }
        #endregion

        private void dataGridView_CellClick(object sender, DataGridViewCellEventArgs e)
        {
            if (e.RowIndex < 0) return;

            if (e.ColumnIndex == loadBatchCharacterColumn.Index)
            {
                OpenFileDialog dialog = new OpenFileDialog();
                dialog.DefaultExt = ".xml";
                dialog.Filter = "Rawr Xml Character Files | *.xml";
                dialog.Multiselect = false;
                if (dialog.ShowDialog(this) == DialogResult.OK)
                {
                    dataGridView[e.ColumnIndex, e.RowIndex].Value = RelativePath(dialog.FileName, AppDomain.CurrentDomain.BaseDirectory);
                    dataGridView.NotifyCurrentCellDirty(true); // force DataGridView to add new row
                    dataGridView.NotifyCurrentCellDirty(false);
                }
                dialog.Dispose();
            }
            else if (e.ColumnIndex == showBatchCharacterColumn.Index && e.RowIndex != dataGridView.NewRowIndex)
            {
                formMain.LoadBatchCharacter(dataGridView.Rows[e.RowIndex].DataBoundItem as BatchCharacter);
            }
            else if (e.ColumnIndex == diffBatchCharacterColumn.Index && e.RowIndex != dataGridView.NewRowIndex)
            {
                BatchCharacter character = (BatchCharacter)dataGridView.Rows[e.RowIndex].DataBoundItem;
                Character before = Character.Load(character.AbsolutePath); // load clean version for comparison
                Character after = character.Character;
                FormOptimizeResult result = new FormOptimizeResult(before, after);
                result.SetOptimizerScores(character.Score, character.NewScore.GetValueOrDefault(character.Score));
                if (result.ShowDialog(this) == DialogResult.No)
                {
                    // we don't want the new character, reload the old one
                    Character _character = character.Character;
                    _character.IsLoading = true;
                    _character.SetItems(before, false, false);
                    _character.ActiveBuffs = before.ActiveBuffs;
                    //_character.CurrentTalents = before.CurrentTalents; // let's not play with talents for now
                    _character.IsLoading = false;
                    _character.OnCalculationsInvalidated();
                    character.UnsavedChanges = false; // reset the dirty flag and update ui
                }
            }
        }

        // based on http://mrpmorris.blogspot.com/2007/05/convert-absolute-path-to-relative-path.html
        private string RelativePath(string absolutePath, string relativeTo)
        {
            string[] relativeDirectories = absolutePath.Split(Path.DirectorySeparatorChar);
            string[] absoluteDirectories = relativeTo.Split(Path.DirectorySeparatorChar);
            //Get the shortest of the two paths
            int length = absoluteDirectories.Length < relativeDirectories.Length ? absoluteDirectories.Length : relativeDirectories.Length;
            //Use to determine where in the loop we exited
            int lastCommonRoot = -1;
            int index;
            //Find common root
            for (index = 0; index < length; index++)
                if (absoluteDirectories[index] == relativeDirectories[index])
                    lastCommonRoot = index;
                else
                    break;
            //If we didn't find a common prefix then throw
            if (lastCommonRoot == -1)
                return absolutePath;
            //Build up the relative path
            StringBuilder relativePath = new StringBuilder();
            //Add on the ..
            for (index = lastCommonRoot + 1; index < absoluteDirectories.Length; index++)
                if (absoluteDirectories[index].Length > 0)
                    relativePath.Append(".." + Path.DirectorySeparatorChar);
            //Add on the folders
            for (index = lastCommonRoot + 1; index < relativeDirectories.Length - 1; index++)
                relativePath.Append(relativeDirectories[index] + Path.DirectorySeparatorChar);
            relativePath.Append(relativeDirectories[relativeDirectories.Length - 1]);
            return relativePath.ToString();
        }

        private void setAvailableGearToolStripMenuItem_Click(object sender, EventArgs e)
        {
            batchTools.SetAvailableGear(formMain.Character.AvailableItems);
        }

        private void replaceUnavailableToolStripMenuItem_Click(object sender, EventArgs e)
        {
            batchTools.ReplaceUnavailableGear();
        }

        private void saveCharactersToolStripMenuItem_Click(object sender, EventArgs e)
        {
            BatchCharacterList list = (BatchCharacterList)batchCharacterListBindingSource.DataSource;

            foreach (BatchCharacter character in list)
            {
                if (character.Character != null && character.UnsavedChanges)
                {
                    character.Character.Save(character.AbsolutePath);
                    character.UnsavedChanges = false;
                    FormMain.Instance.BatchCharacterSaved(character);
                }
            }
        }

        private void saveCharactersAsCopyToolStripMenuItem_Click(object sender, EventArgs e)
        {
            DateTime now = DateTime.Now;
            BatchCharacterList list = (BatchCharacterList)batchCharacterListBindingSource.DataSource;

            foreach (BatchCharacter character in list)
            {
                if (character.Character != null && character.UnsavedChanges)
                {
                    string copyPath = Path.ChangeExtension(character.AbsolutePath, null) + " " + now.ToString("yyyy-M-d H-m") + ".xml";
                    character.Character.Save(copyPath);
                }
            }
        }

        private void optimizeToolStripMenuItem_Click(object sender, EventArgs e)
        {
            buttonCancel.Enabled = true;
            batchTools.Optimize();
        }

        private void buttonCancel_Click(object sender, EventArgs e)
        {
            batchTools.Cancel();
        }

        private void batchCharacterListBindingSource_ListChanged(object sender, ListChangedEventArgs e)
        {
            //if (e.ListChangedType == ListChangedType.ItemAdded || e.ListChangedType == ListChangedType.ItemDeleted) _unsavedChanges = true;
            if (batchTools != null)
            {
                batchTools.UpdateStatusLabel();
            }
        }

        private void buildUpgradeListToolStripMenuItem_Click(object sender, EventArgs e)
        {
            batchTools.SingleItemUpgrade = batchTools.GetSingleItemUpgrade(textBoxSingleItemUpgrade.Text);
            buttonCancel.Enabled = true;
            batchTools.BuildUpgradeList();
        }

        private void bathcOptimizeToolStripMenuItem_Click(object sender, EventArgs e)
        {
            buttonCancel.Enabled = true;
            batchTools.BatchOptimize();
        }

        private void buildBatchUpgradeListToolStripMenuItem_Click(object sender, EventArgs e)
        {
            batchTools.SingleItemUpgrade = batchTools.GetSingleItemUpgrade(textBoxSingleItemUpgrade.Text);
            buttonCancel.Enabled = true;
            batchTools.BuildBatchUpgradeList();
        }

        private void progressiveOptimizeToolStripMenuItem_Click(object sender, EventArgs e)
        {
            buttonCancel.Enabled = true;
            batchTools.ProgressiveOptimize();
        }

        private void buildProgressiveUpgradeListToolStripMenuItem_Click(object sender, EventArgs e)
        {
            batchTools.SingleItemUpgrade = batchTools.GetSingleItemUpgrade(textBoxSingleItemUpgrade.Text);
            buttonCancel.Enabled = true;
            batchTools.BuildProgressiveUpgradeList();
        }

        private void dataGridView_SelectionChanged(object sender, EventArgs e)
        {
            if (dataGridView.SelectedRows.Count == 1)
            {
                buttonUp.Enabled = dataGridView.SelectedRows[0].Index > 0;
                buttonDown.Enabled = dataGridView.SelectedRows[0].Index < batchTools.BatchCharacterList.Count - 1;
            }
            else
            {
                buttonUp.Enabled = false;
                buttonDown.Enabled = false;
            }
        }

        private void buttonUp_Click(object sender, EventArgs e)
        {
            if (dataGridView.SelectedRows.Count == 1)
            {
                int index = dataGridView.SelectedRows[0].Index;
                if (index > 0)
                {
                    BatchCharacter b = batchTools.BatchCharacterList[index - 1];
                    batchTools.BatchCharacterList.RemoveAt(index - 1);
                    batchTools.BatchCharacterList.Insert(index, b);
                    dataGridView.Rows[index].Selected = false;
                    dataGridView.Rows[index - 1].Selected = true;
                    dataGridView_SelectionChanged(null, EventArgs.Empty);
                }
            }
        }

        private void buttonDown_Click(object sender, EventArgs e)
        {
            if (dataGridView.SelectedRows.Count == 1)
            {
                int index = dataGridView.SelectedRows[0].Index;
                if (index < batchTools.BatchCharacterList.Count - 1)
                {
                    BatchCharacter b = batchTools.BatchCharacterList[index];
                    batchTools.BatchCharacterList.RemoveAt(index);
                    dataGridView.Rows[index].Selected = false;
                    batchTools.BatchCharacterList.Insert(index + 1, b);
                    dataGridView.Rows[index + 1].Selected = true;
                    dataGridView_SelectionChanged(null, EventArgs.Empty);
                }
            }
        }
    }
}
