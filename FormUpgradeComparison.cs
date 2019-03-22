using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;
using Rawr.Optimizer;
using System.IO;
using System.Xml.Serialization;
using System.Drawing.Imaging;

namespace Rawr
{
    public partial class FormUpgradeComparison : Form
    {
        private FormUpgradeComparison()
        {
            InitializeComponent();
			if (Type.GetType("Mono.Runtime") != null) 
				copyDataToClipboardToolStripMenuItem.Text += " (Doesn't work under Mono)";
        }

        private static FormUpgradeComparison _instance;
        public static FormUpgradeComparison Instance
        {
            get
            {
                if (_instance == null)
                {
                    _instance = new FormUpgradeComparison();
                }
                return _instance;
            }
        }


        private string[] customSubpoints;

        private void SetCustomSubpoints(string[] customSubpoints)
        {
            this.customSubpoints = customSubpoints;
            if (customSubpoints != null)
            {
                comparisonGraph1.DisplayMode = ComparisonGraph.GraphDisplayMode.CustomSubpoints;
                toolStripDropDownButtonSort.DropDownItems.Clear();
                toolStripDropDownButtonSort.DropDownItems.Add(overallToolStripMenuItem);
                toolStripDropDownButtonSort.DropDownItems.Add(alphabeticalToolStripMenuItem);
                foreach (string name in customSubpoints)
                {
                    ToolStripMenuItem toolStripMenuItemSubPoint = new ToolStripMenuItem(name);
                    toolStripMenuItemSubPoint.Tag = toolStripDropDownButtonSort.DropDownItems.Count - 2;
                    toolStripMenuItemSubPoint.Click += new System.EventHandler(this.sortToolStripMenuItem_Click);
                    toolStripDropDownButtonSort.DropDownItems.Add(toolStripMenuItemSubPoint);
                }
            }
            else
            {
                comparisonGraph1.DisplayMode = ComparisonGraph.GraphDisplayMode.Overall;
                toolStripDropDownButtonSort.DropDownItems.Clear();
                toolStripDropDownButtonSort.DropDownItems.Add(overallToolStripMenuItem);
                toolStripDropDownButtonSort.DropDownItems.Add(alphabeticalToolStripMenuItem);
            }
        }

        private Dictionary<string, ComparisonCalculationUpgrades[]> _itemCalculations;

		public void RemoveItem(ItemInstance item)
		{
			string[] keys = new string[_itemCalculations.Keys.Count];
			_itemCalculations.Keys.CopyTo(keys, 0);
			foreach (string slot in keys)
			{
				List<ComparisonCalculationUpgrades> slotItems = new List<ComparisonCalculationUpgrades>(
					_itemCalculations[slot]);
				ComparisonCalculationUpgrades existing = slotItems.Find(ccu => ccu.ItemInstance.GemmedId == item.GemmedId);
				if (existing != null)
				{
					slotItems.Remove(existing);
				}
				_itemCalculations[slot] = slotItems.ToArray();
			}
			comparisonGraph1.ItemCalculations = _itemCalculations[_currentSlot];
		}

        [Serializable]
        public class SerializationData
        {
            public string[] CustomSubpoints;
            public List<string> Keys;
            public List<ComparisonCalculationUpgrades[]> ItemCalculations;
        }

        public bool LoadFile(string file)
        {
            try
            {
                if (File.Exists(file))
                {
                    SerializationData data = new SerializationData();
                    using (StreamReader reader = new StreamReader(file, Encoding.UTF8))
                    {
                        XmlSerializer serializer = new XmlSerializer(typeof(SerializationData));
                        data = (SerializationData)serializer.Deserialize(reader);
                        reader.Close();
                    }
                    comparisonGraph1.EquipSlot = CharacterSlot.AutoSelect;
                    comparisonGraph1.Character = FormMain.Instance.Character;
                    SetCustomSubpoints(data.CustomSubpoints);

                    _itemCalculations = new Dictionary<string, ComparisonCalculationUpgrades[]>();
                    for (int i = 0; i < data.Keys.Count; i++)
                    {
                        _itemCalculations[data.Keys[i]] = data.ItemCalculations[i];
                    }
                    slotToolStripMenuItem_Click(allToolStripMenuItem, null);
					return true;
                }
            }
            catch
            {
				MessageBox.Show("The chosen file is not a Rawr Upgrade List.", "Unable to Load Upgrade List", MessageBoxButtons.OK, MessageBoxIcon.Warning);
            }
			return false;
        }

        private void SaveFile(string file)
        {
            SerializationData data = new SerializationData();
            data.CustomSubpoints = customSubpoints;
            data.Keys = new List<string>();
            data.ItemCalculations = new List<ComparisonCalculationUpgrades[]>();
            foreach (KeyValuePair<string, ComparisonCalculationUpgrades[]> kvp in _itemCalculations)
            {
                data.Keys.Add(kvp.Key);
                data.ItemCalculations.Add(kvp.Value);
            }

            try
            {
                using (StreamWriter writer = new StreamWriter(file, false, Encoding.UTF8))
                {
                    XmlSerializer serializer = new XmlSerializer(typeof(SerializationData));
                    serializer.Serialize(writer, data);
                    writer.Close();
				}
			}
			catch (Exception ex)
			{
				MessageBox.Show(ex.Message);
			}
        }

        public void LoadData(Dictionary<CharacterSlot, List<ComparisonCalculationUpgrades>> calculations, string[] customSubpoints)
        {
            comparisonGraph1.EquipSlot = CharacterSlot.AutoSelect;
            comparisonGraph1.Character = FormMain.Instance.Character;
            SetCustomSubpoints(customSubpoints);

            _itemCalculations = new Dictionary<string, ComparisonCalculationUpgrades[]>();
            List<ComparisonCalculationUpgrades> all = new List<ComparisonCalculationUpgrades>();
            foreach (KeyValuePair<CharacterSlot, List<ComparisonCalculationUpgrades>> kvp in calculations)
            {
                all.AddRange(kvp.Value);
                _itemCalculations["Gear." + kvp.Key.ToString()] = kvp.Value.ToArray();
            }
            _itemCalculations["Gear.All"] = all.ToArray();
            slotToolStripMenuItem_Click(allToolStripMenuItem, null);
        }

		private string _currentSlot = "Gear.All";
        private void slotToolStripMenuItem_Click(object sender, EventArgs e)
        {
            this.Cursor = Cursors.WaitCursor;
            foreach (ToolStripItem item in toolStripDropDownButtonSlot.DropDownItems)
            {
                if (item is ToolStripMenuItem)
                {
                    (item as ToolStripMenuItem).Checked = item == sender;
                    if ((item as ToolStripMenuItem).Checked)
                    {
                        string[] tag = item.Tag.ToString().Split('.');
                        toolStripDropDownButtonSlot.Text = "Chart: " + tag[0];
                        if (tag.Length > 1) toolStripDropDownButtonSlot.Text += " > " + item.Text;
                        comparisonGraph1.RoundValues = true;
						_currentSlot = (string)item.Tag;
                        comparisonGraph1.ItemCalculations = _itemCalculations[_currentSlot];
                    }
                }
            }
            this.Cursor = Cursors.Default;
        }

        private void sortToolStripMenuItem_Click(object sender, EventArgs e)
        {
            this.Cursor = Cursors.WaitCursor;
            ComparisonGraph.ComparisonSort sort = ComparisonGraph.ComparisonSort.Overall;
            foreach (ToolStripItem item in toolStripDropDownButtonSort.DropDownItems)
            {
                if (item is ToolStripMenuItem)
                {
                    (item as ToolStripMenuItem).Checked = item == sender;
                    if ((item as ToolStripMenuItem).Checked)
                    {
                        toolStripDropDownButtonSort.Text = "Sort: " + item.Text;
                        sort = (ComparisonGraph.ComparisonSort)((int)item.Tag);
                    }
                }
            }
            comparisonGraph1.Sort = sort;
            this.Cursor = Cursors.Default;
        }

        private void FormUpgradeComparison_FormClosing(object sender, FormClosingEventArgs e)
        {
            if (e.CloseReason == CloseReason.UserClosing)
            {
                e.Cancel = true;
                Hide();
            }
        }

        private void openToolStripMenuItem_Click(object sender, EventArgs e)
        {
            OpenFileDialog dialog = new OpenFileDialog();
            dialog.DefaultExt = ".xml";
            dialog.Filter = "Rawr Upgrade List Files|*.xml";
            dialog.Multiselect = false;
            if (dialog.ShowDialog(this) == DialogResult.OK)
            {
                LoadFile(dialog.FileName);
            }
            dialog.Dispose();
        }

        private void saveAsToolStripMenuItem_Click(object sender, EventArgs e)
        {
            SaveFileDialog dialog = new SaveFileDialog();
            dialog.DefaultExt = ".xml";
            dialog.Filter = "Rawr Upgrade List Files|*.xml";
            if (dialog.ShowDialog(this) == DialogResult.OK)
            {
                SaveFile(dialog.FileName);
            }
            dialog.Dispose();
        }

		private string GetChartDataCSV()
		{
			StringBuilder sb = new StringBuilder("Item Name,Equipped,Slot,Gem1,Gem2,Gem3,Enchant,Source,ItemLevel,ItemId,GemmedId,Overall Upgrade");
			sb.AppendLine();
			foreach (ComparisonCalculationUpgrades upgrade in comparisonGraph1.ItemCalculations)
			{
				ItemInstance item = upgrade.ItemInstance;
				if (item != null)
				{
					sb.AppendFormat("{0},{1},{2},{3},{4},{5},{6},{7},{8},{9},{10},{11}",
						item.Item.Name.Replace(',',';'),
						upgrade.Equipped,
						item.Slot,
						item.Gem1 != null ? item.Gem1.Name : null,
						item.Gem2 != null ? item.Gem2.Name : null,
						item.Gem3 != null ? item.Gem3.Name : null,
						item.Enchant.Name,
                        item.Item.LocationInfo[0].Description.Split(',')[0] + (item.Item.LocationInfo[1] != null ? "|" + item.Item.LocationInfo[1].Description.Split(',')[0] : ""),
                        item.Item.ItemLevel,
						item.Id,
						item.GemmedId,
						upgrade.OverallPoints);
					sb.AppendLine();
				}
			}

			return sb.ToString();
		}

        private void copyDataToClipboardToolStripMenuItem_Click(object sender, EventArgs e)
        {
			try
			{
				Clipboard.SetText(GetChartDataCSV(), TextDataFormat.Text);
			}
			catch { }
        }

		private void exportToImageToolStripMenuItem_Click(object sender, EventArgs e)
		{
			SaveFileDialog dialog = new SaveFileDialog();
			dialog.DefaultExt = ".png";
			dialog.Filter = "PNG|*.png|GIF|*.gif|JPG|*.jpg|BMP|*.bmp";
			if (dialog.ShowDialog(this) == DialogResult.OK)
			{
				try
				{
					ImageFormat imgFormat = ImageFormat.Bmp;
					if (dialog.FileName.EndsWith(".png")) imgFormat = ImageFormat.Png;
					else if (dialog.FileName.EndsWith(".gif")) imgFormat = ImageFormat.Gif;
					else if (dialog.FileName.EndsWith(".jpg") || dialog.FileName.EndsWith(".jpeg")) imgFormat = ImageFormat.Jpeg;
					comparisonGraph1.PrerenderedGraph.Save(dialog.FileName, imgFormat);
				}
				catch (Exception ex)
				{
					MessageBox.Show(ex.Message);
				}
			}
			dialog.Dispose();
		}

		private void exportToCSVToolStripMenuItem_Click(object sender, EventArgs e)
		{
			SaveFileDialog dialog = new SaveFileDialog();
			dialog.DefaultExt = ".csv";
			dialog.Filter = "Comma Separated Values | *.csv";
			if (dialog.ShowDialog(this) == DialogResult.OK)
			{
				try
				{
					using (StreamWriter writer = System.IO.File.CreateText(dialog.FileName))
					{
						writer.Write(GetChartDataCSV());
						writer.Flush();
						writer.Close();
						writer.Dispose();
					}
				}
				catch (Exception ex)
				{
					MessageBox.Show(ex.Message);
				}
			}
			dialog.Dispose();
		}

        private void exportToLootPlanToolStripMenuItem_Click(object sender, EventArgs e)
        {
            try
            {
                StringBuilder sb = new StringBuilder();
                foreach (ComparisonCalculationUpgrades upgrade in comparisonGraph1.ItemCalculations)
                {
                    ItemInstance item = upgrade.ItemInstance;
                    if (item != null)
                    {
                        sb.AppendFormat("{0}:{1} Upgrade Score;",
                            item.Id,
                            upgrade.OverallPoints);
                    }
                }

                Clipboard.SetText(sb.ToString(), TextDataFormat.Text);
            }
            catch { }
        }
    }
}